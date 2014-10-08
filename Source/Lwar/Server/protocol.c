#include "types.h"

#include "protocol.h"

#include "config.h"
#include "debug.h"
#include "log.h"
#include "message.h"
#include "pack.h"
#include "performance.h"
#include "queue.h"
#include "server.h"
#include "stream.h"
#include "unpack.h"

#include <math.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#if _MSC_VER
#define snprintf _snprintf
#endif

void queue_gamestate_for(Client *cn);
void debug_message(Message *m, const char *s);

static void send_reject(Address *adr, size_t ack, RejectReason reason);
static void send_kick(Client *c);

static jmp_buf io_error_handler;

static Discovery discovery = {
    MESSAGE_DISCOVERY,
    APP_ID,
    NETWORK_REVISION,
    SERVER_PORT,
};

static const char *src_fmt(Client *c) {
    static char s[16];
    if(c) { snprintf(s,sizeof(s),"%d> ",c->player.id.n);
            return s; }
    else    return "?> ";
}

static const char *dest_fmt(Client *c) {
    static char s[16];
    if(c) { snprintf(s,sizeof(s),"<%d ",c->player.id.n);
            return s; }
    else    return "<? ";
}

static bool check_behavior(Client *c, bool test, const char *msg) {
    if(test) {
        c->misbehavior ++;
		log_debug("misbehavior of %d: %s", c->player.id.n, msg);
    }
    return test;
}

static bool check_behavior_id(Client *c, Id id) {
    return check_behavior(c, !id_eq(c->player.id, id), "wrong player id");
}

static bool check_seqno(Client *c, Message *m) {
	if (!c) return true;

    if(is_reliable(m)) {
        if(m->seqno != c->last_in_reliable_seqno + 1) return false;
        c->last_in_reliable_seqno = m->seqno;
    }
	else {
        if(m->seqno <= c->last_in_unreliable_seqno) return false;
        c->last_in_unreliable_seqno = m->seqno;
    }
    return true;
}

void message_handle(Client *c, Address *adr, Message *m) {
    Message r;

    switch(m->type) {
    case MESSAGE_CONNECT:
		if (m->connect.rev != NETWORK_REVISION) { 
			send_reject(adr, m->seqno, REJECT_VERSION_MISMATCH);
			break;
		}

        /* TODO: probably allow reconnects */
        if(check_behavior(c, c != 0, "reconnect")) return;
        c = client_create(adr);
        if(c) {
            check_seqno(c, m);
            c->last_activity = server->cur_clock;

			player_rename(&c->player, m->connect.nick);
            message_join(&r, c);
            queue_broadcast(&r);
            queue_gamestate_for(c);
        } else {
            send_reject(adr, m->seqno, REJECT_FULL);
        }
        break;

    case MESSAGE_DISCONNECT:
        if(!c) return;
        message_leave(&r, c, LEAVE_QUIT);
        queue_broadcast(&r);
        break;

    case MESSAGE_CHAT:
        if(!c) return;
        if(check_behavior_id(c, m->chat.player_id)) return;
        queue_broadcast(m);
        break;

    case MESSAGE_SELECTION:
        if(!c) return;
        if(check_behavior_id(c, m->selection.player_id)) return;
        player_select(&c->player,
                      m->selection.ship_type,
                      m->selection.weapon_type1,
                      m->selection.weapon_type2,
                      m->selection.weapon_type3,
                      m->selection.weapon_type4);
        queue_broadcast(m);
        break;

    case MESSAGE_NAME:
        if(!c) return;
        if(check_behavior_id(c, m->name.player_id)) return;
        player_rename(&c->player, m->name.nick);
        queue_broadcast(m);
        break;

    case MESSAGE_INPUT:
        if(!c) return;
        if(check_behavior_id(c, m->input.player_id)) return;
        {
            if(m->input.frameno < c->last_in_frameno)
                break;
            uint8_t mask = ~(0xff << (m->input.frameno - c->last_in_frameno));
			c->last_in_frameno = m->input.frameno;
            player_input(&c->player,
                         mask & m->input.forwards,
                         mask & m->input.backwards,
                         mask & m->input.turn_left,
                         mask & m->input.turn_right,
                         mask & m->input.strafe_left,
                         mask & m->input.strafe_right,
                         mask & m->input.fire1,
                         mask & m->input.fire2,
                         mask & m->input.fire3,
                         mask & m->input.fire4,
                                m->input.aim_x,
                                m->input.aim_y
                         );
        }
        break;

    default:
        check_behavior(c, c != 0, "invalid message id");
    }
}

void protocol_notify_entity(Entity *e) {
    if(!e->type->format) return;

    Message m;
    if(e->dead) message_remove(&m, e);
    else        message_add(&m, e);

    queue_broadcast(&m);
}

void protocol_notify_collision(Collision *c) {
    Message m;
    message_collision(&m, c);
    queue_broadcast(&m);
}

void protocol_notify_kill(Player *k, Player *v) {
    Message m;
    message_kill(&m, k, v);
    queue_broadcast(&m);
}

static void header_for(Header *h, Client *c) {
    h->app_id = APP_ID,
    h->ack = c->last_in_reliable_seqno;
    h->time = server->cur_clock;
    h->adr = c->adr;
}

static void header_for_unconnected(Header *h, Address *adr, size_t ack) {
    h->app_id = APP_ID,
    h->ack = ack;
    h->time = server->cur_clock;
    h->adr = *adr;
}

static void send_kick(Client *c) {
    Header h;
    Message m;
    header_for(&h, c);
    message_leave(&m, c, LEAVE_MISBEHAVED);
    m.seqno = 1; // correct value but ugly.
    stream_send_flush(&h, &m);
    // stats.nsend ++;
}

static void send_reject(Address *adr, size_t ack, RejectReason reason) {
    Header h;
    Message m;
    header_for_unconnected(&h, adr, ack);
    message_reject(&m, reason);
    m.seqno = 1; // correct value but ugly.
    stream_send_flush(&h, &m);
    // stats.nsend ++;
}

static void send_timeout(Client *c) {
    Message m;
    message_leave(&m, c, LEAVE_DROPPED);
    queue_broadcast(&m);
}


static void queue_stats() {
    Message m;
    message_stats(&m);
    queue_broadcast(&m);
}

static void queue_updates() {
    Message m;
    Format *f;
    formats_foreach(f) {
        message_update(&m, f);
        queue_broadcast(&m);
    }
}

/* Note: already enqueued add messages won't be duplicated,
 *       since these are not marked for client cn in qm->dest
 */
void queue_gamestate_for(Client *cn) {
    Message m;

    Client *c;
    clients_foreach(c) {
		if(c->dead) continue;
        if(c == cn) continue;

        message_join(&m, c);
        queue_unicast(cn, &m);
    }

    Entity *e;
    entities_foreach(e) {
		if(e->dead) continue;
        if(!e->type->format) continue;

        message_add(&m, e);
        queue_unicast(cn, &m);
    }

    message_synced(&m);
    queue_unicast(cn, &m);
}

static void send_queue_for(Client *c) {
    size_t tries;
    cr_t qs = {0};
    cr_t ss = {0};

    Header h;
    header_for(&h, c);

    Message *m;
    while((m = queue_next(&qs, c, &tries))) {
        // if(tries > 0)
            // stats.nresend ++;
        if(tries == 0 && is_reliable(m))
            debug_message(m, dest_fmt(c));

        if(!stream_send(&ss, &h, m))
            longjmp(io_error_handler,1);
    }

    stream_flush(&ss);
}

void protocol_recv() {
    cr_t ss = {0};

    Header h;
    Message m;

    while(stream_recv(&ss, &h, &m)) {
        Client *c = client_lookup(&h.adr);
        if(c) {
            c->last_in_ack   = max(h.ack, c->last_in_ack);
            c->last_activity = max(server->cur_clock, c->last_activity);
        }

        if(check_seqno(c, &m)) {
            if(is_reliable(&m))
                debug_message(&m, src_fmt(c));
            message_handle(c, &h.adr, &m);
        }
    }
}

/* (re)send queued messages */
void protocol_send(bool force) {
    if(!force && !clock_periodic(&server->update_periodic, UPDATE_INTERVAL))
        return;

    if(clock_periodic(&server->discovery_periodic, DISCOVERY_INTERVAL))
        stream_send_discovery(&discovery);

    // timer_start(TIMER_SEND);
    // stats.nsend   = 0;
    // stats.nresend = 0;

    queue_stats();
    queue_updates();

    Client *c;
    clients_foreach(c) {
        /* TODO: refactor into separate function */
        if(!c->remote) {
            continue;
        }
        else if(c->last_activity + TIMEOUT_INTERVAL < server->cur_clock) {
            send_timeout(c);
            client_remove(c);
        }
        else if (c->misbehavior > MISBEHAVIOR_LIMIT) {
            send_kick(c);
            send_timeout(c);
            client_remove(c);
        }
        else if(setjmp(io_error_handler)) {
            /* if setjmp returns != 0
             * there was an error in send_messages_for
             * thrown in packet_send_init
             */
            send_timeout(c);
            client_remove(c);
        }
        else {
            send_queue_for(c);
        }
    }

    // timer_stop(TIMER_SEND);
    // counter_set(COUNTER_SEND,   stats.nsend);
    // counter_set(COUNTER_RESEND, stats.nresend);
}
