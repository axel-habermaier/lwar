#include <assert.h>
#include <math.h>
#include <setjmp.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "server.h"
#include "coroutine.h"
#include "message.h"
#include "packet.h"
#include "log.h"
#include "performance.h"

#if _MSC_VER
#define snprintf _snprintf
#endif

enum {
    UPDATE_INTERVAL = 30,
};

static void send_reject(Address *adr, size_t ack, RejectReason reason);
static void send_kick(Client *c);
static void protocol_timeout(Client *c);

void queue_forward(Message *m);
Message *queue_next(cr_t *state, Client *c, size_t *tries);

static struct {
    size_t nsend,nresend,nrecv;
} stats;

static jmp_buf io_error_handler;

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

static bool check_seqno(Client *c, Message *m, size_t seqno) {
	if (!c) return true;

    if(is_reliable(m)) {
        if(seqno != c->last_in_reliable_seqno + 1) return false;
        c->last_in_reliable_seqno = seqno;
    }
	else {
        if(seqno <= c->last_in_unreliable_seqno) return false;
        c->last_in_unreliable_seqno = seqno;
    }
    return true;
}

static void message_handle(Client *c, Address *adr, Message *m, size_t seqno) {
    switch(m->type) {
    case MESSAGE_CONNECT:
		if (m->connect.rev != NETWORK_REVISION) { 
			send_reject(adr, seqno, REJECT_VERSION_MISMATCH);
			break;
		}

        /* TODO: probably allow reconnects */
        if(check_behavior(c, c != 0, "reconnect")) return;
        c = client_create(adr);
        if(c) {
            check_seqno(c, m, seqno);
            c->last_activity = server->cur_clock;

			player_rename(&c->player, m->connect.nick);
            queue_join(c);
            queue_gamestate_for(c);
        } else {
            send_reject(adr, seqno, REJECT_FULL);
        }
        break;

    case MESSAGE_DISCONNECT:
        if(!c || c->hasleft) return;
        c->hasleft = 1; /* do not broadcast leave message on timeout,
                           but delay removal till then (TODO: check why?) */
        queue_leave(c);
        break;

    case MESSAGE_CHAT:
        if(!c) return;
        if(check_behavior_id(c, m->chat.player_id)) return;
        queue_forward(m);
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
        queue_forward(m);
        break;

    case MESSAGE_NAME:
        if(!c) return;
        if(check_behavior_id(c, m->name.player_id)) return;
        player_rename(&c->player, m->name.nick);
        queue_forward(m);
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

static void packet_scan(Packet *p) {
    Message m;
    Client *c = client_lookup(&p->adr);
    if(c) {
        c->last_in_ack   = max(p->ack, c->last_in_ack);
        c->last_activity = max(server->cur_clock, c->last_activity);
    }

    while(packet_get(p, message_unpack, &m)) {
        if(check_seqno(c, &m, m.seqno)) {
            if(is_reliable(&m))
                message_debug(&m, src_fmt(c));
            message_handle(c, &p->adr, &m, m.seqno);
        }
    }
}

/* handle all pending incoming messages */
void protocol_recv() {
    timer_start(TIMER_RECV);
    stats.nrecv = 0;

    Packet p;
    while(packet_recv(&p)) {
        stats.nrecv ++;
        packet_scan(&p);
    }
    if(p.io_failed) {
        Client *c = client_lookup(&p.adr);
        if(c) protocol_timeout(c);
    }

    timer_stop(TIMER_RECV);
    counter_set(COUNTER_RECV, stats.nrecv);
}

void protocol_notify_entity(Entity *e) {
    if(!e->type->format) return;

    if(e->dead) queue_remove(e);
    else        queue_add(e);
}

void protocol_notify_collision(Collision *c) {
    queue_collision(c);
}

void protocol_notify_kill(Player *k, Player *v) {
    queue_kill(k, v);
}

static void packet_init_header(Client *c, Packet *p) {
    packet_init_send(p, &c->adr, c->last_in_reliable_seqno, server->cur_clock);
}

static void send_kick(Client *c) {
    Packet  p;
    packet_init_header(c, &p);

    Message m;
    m.type = MESSAGE_LEAVE;
    m.leave.player_id = c->player.id;
	m.leave.reason = LEAVE_MISBEHAVED;
    packet_put(&p, message_pack, &m);

    packet_send(&p);
    stats.nsend ++;
}

static void send_reject(Address *adr, size_t ack, RejectReason reason) {
    Packet p;
    packet_init_send(&p, adr, ack, 0);

    Message m;
    m.type  = MESSAGE_REJECT;
	m.reject.reason = reason;
    m.seqno = 1;
    packet_put(&p, message_pack, &m);

    packet_send(&p);
    stats.nsend ++;
}

static void packet_send_to(Client *c, Packet *p) {
    if(packet_hasdata(p)) {
        //packet_debug(p);
        packet_send(p);
        stats.nsend ++;
        if(p->io_failed)
            longjmp(io_error_handler,1);
        /* packet_init_header(c, p); */
    }
}

static void packet_send_to_init(Client *c, Packet *p) {
    packet_send_to(c,p);
    packet_init_header(c,p);
}

static bool packet_put_update(Client *c, Packet *p, size_t type, size_t n) {
    Message m;
    m.type = (MessageType)type;
    m.seqno = c->next_out_unreliable_seqno ++;
    m.update.n = n;
    return packet_put(p, message_pack, &m);
}

static void send_queue_for(Client *c, Packet *p) {
    Message *m;
    size_t tries;
    cr_t state = {0};

    while((m = queue_next(&state, c, &tries))) {
        if(tries > 0)
            stats.nresend ++;
        if(tries == 0 && is_reliable(m))
            message_debug(m, dest_fmt(c));

    again_m:
        if(!packet_put(p, message_pack, m)) {
            packet_send_to_init(c, p);
            goto again_m;
        }
    }
}

static void send_updates_for(Client *c, Packet *p, Format *f) {
    Entity *e;
    size_t k = 0;
    size_t n = f->n;

    updates_foreach(f,e) {
		if(e->dead) { n --; continue; }
    again_e:
        if(!k) {
            k = min(n, packet_update_n(p,f->len));
            if(k) {
                assert(packet_put_update(c, p, f->id, k));
            } else {
                packet_send_to_init(c, p);
            }
            goto again_e;
        } else {
            assert(packet_put(p, f->pack, e));
            k --;
            n --;
        }
    }
    assert(k == 0);
    assert(n == 0);
}

static void send_messages_for(Client *c, Packet *p) {
    Format *f;

    packet_init_header(c, p);

    send_queue_for(c, p);
    formats_foreach(f)
        send_updates_for(c, p, f);

    packet_send_to(c, p);
}

static void protocol_timeout(Client *c) {
    queue_timeout(c);
    client_remove(c);
}

/* (re)send queued messages */
void protocol_send(bool force) {
    if(!force && !clock_periodic(&server->update_periodic, UPDATE_INTERVAL))
        return;

    timer_start(TIMER_SEND);
    stats.nsend   = 0;
    stats.nresend = 0;

    Client *c;
    clients_foreach(c) {
        if(!c->remote) {
            continue;
        }
        else if(c->last_activity + TIMEOUT_INTERVAL < server->cur_clock) {
            protocol_timeout(c);
        }
        else if (c->misbehavior > MISBEHAVIOR_LIMIT) {
            send_kick(c);
            protocol_timeout(c);
        }
        else if(setjmp(io_error_handler)) {
            /* if setjmp returns != 0
             * there was an error in send_messages_for
             * thrown in packet_send_init
             */
            protocol_timeout(c);
        }
        else {
            Packet p;
            send_messages_for(c,&p);
        }
    }

    timer_stop(TIMER_SEND);
    counter_set(COUNTER_SEND,   stats.nsend);
    counter_set(COUNTER_RESEND, stats.nresend);
}
