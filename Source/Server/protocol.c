#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "server.h"
#include "message.h"
#include "packet.h"
#include "log.h"
#include "performance.h"

#if _MSC_VER
#define snprintf _snprintf
#endif

enum {
    UPDATE_INTERVAL = 30,
    RETRANSMIT_INTERVAL = 2*UPDATE_INTERVAL,
};

static void protocol_send_full(Address *adr, size_t seqno);
static void protocol_send_gamestate(Client *c);
static void protocol_timed_out(Client *c);

static struct {
    size_t nsend,nresend,nrecv;
} stats;

typedef struct QueuedMessage QueuedMessage;
typedef struct PerClient     PerClient;

struct PerClient {
    size_t seqno;
    size_t tries;
    Clock last_tx_time;
};

struct QueuedMessage {
    List _l;
    unsigned int dest;
    /* sequence numbers for each client */
    PerClient c[MAX_CLIENTS];
    Message m;
};

static QueuedMessage _queue[MAX_QUEUE];

static void qm_ctor(size_t i, void *p) {
    QueuedMessage *qm = (QueuedMessage*)p;
    qm->dest = 0;
}

static void qm_dtor(size_t i, void *p) {}

static int qm_check_obsolete(size_t i, void *p) {
    QueuedMessage *qm = (QueuedMessage*)p;
    /* only keep qm for receiving clients */
    return (qm->dest & server->client_mask) == 0;
}

static int qm_check_dest(Client *c, QueuedMessage *qm) {
    size_t id = c->player.id.n;
    return (qm->dest & (1 << id));
}

static void qm_set_dest(Client *c, QueuedMessage *qm) {
    size_t id = c->player.id.n;
    qm->dest |= (1 << id);
}

static void qm_clear_dest(Client *c, QueuedMessage *qm) {
    size_t id = c->player.id.n;
    qm->dest &= ~(1 << id);
}

#define qm_seqno(c,qm) qm->c[c->player.id.n].seqno
#define qm_tries(c,qm) qm->c[c->player.id.n].tries
#define qm_last_tx_time(c,qm) qm->c[c->player.id.n].last_tx_time

static int qm_check_relevant(Client *c, QueuedMessage *qm) {
    
    /* message not for c */
    if(!qm_check_dest(c, qm)) {
        return 0;
    }

    if(   qm_tries(c,qm) > 0
       && qm_last_tx_time(c,qm) + RETRANSMIT_INTERVAL < server->cur_time)
    {
        return 0;
    }
    else {
        qm_last_tx_time(c,qm) = server->cur_time;
    }

    /* unreliable message for c, do not resend */
    if(!is_reliable(&qm->m)) {
        qm_clear_dest(c, qm);
        return 1;
    }

    /* reliable message for c, already acknowledged */
    if(qm_seqno(c, qm) <= c->last_in_ack) {
        qm_clear_dest(c, qm);
        return 0;
    }

    /* reliable, unacknowledged message for c */
    return 1;
}

static int m_check_seqno(Client *c, Message *m, size_t seqno) {
    if(c && is_reliable(m)) {
        if(seqno <= c->last_in_seqno) return 0;
        c->last_in_seqno = seqno;
    }
    return 1;
}

static QueuedMessage *qm_create() {
    QueuedMessage *qm = pool_new(&server->queue, QueuedMessage);
    assert(qm); /* TODO: handle allocation failure */
    return qm;
}

static void qm_enqueue(Client *c, QueuedMessage *qm) {
    qm_set_dest(c,qm);
    if(is_reliable(&qm->m)) {
        qm_seqno(c,qm) = (c->next_out_seqno ++);
        qm_tries(c,qm) = 0;
    }
}

static Message *message_unicast(Client *c, MessageType type) {
    QueuedMessage *qm = qm_create();
    Message *m = &qm->m;
    m->type = type;
    qm_enqueue(c,qm);
    return m;
}

static Message *message_broadcast(MessageType type) {
    QueuedMessage *qm = qm_create();
    Message *m = &qm->m;
    Client *c;
    m->type = type;
    clients_foreach(c)
        qm_enqueue(c,qm);
    return m;
}

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

static int misbehaves(Client *c, int test, const char *msg) {
    if(test) {
        c->misbehavior ++;
		log_debug("misbehavior of %d: %s", c->player.id.n, msg);
    }
    return test;
}

static int misbehaves_id(Client *c, Id id) {
    return misbehaves(c, !id_eq(c->player.id, id), "wrong player id");
}

static void message_handle(Client *c, Address *adr, Message *m, size_t time, size_t seqno) {
    Message *r;

    switch(m->type) {
    case MESSAGE_CONNECT:
        /* TODO: probably allow reconnects */
        if(misbehaves(c, c != 0, "reconnect")) return;
        c = client_create(adr);
        if(c) {
            m_check_seqno(c, m, seqno);
            r = message_broadcast(MESSAGE_JOIN);
            r->join.player_id = c->player.id;

            c->last_activity = server->cur_time;
            protocol_send_gamestate(c);
        } else {
            protocol_send_full(adr, seqno);
        }
        break;

    case MESSAGE_DISCONNECT:
        if(!c) return;
        c->hasleft = 1; /* do not broadcast leave message on timeout */
        r = message_broadcast(MESSAGE_LEAVE);
        r->leave.player_id = c->player.id;
        break;

    case MESSAGE_CHAT:
        if(!c) return;
        if(misbehaves_id(c, m->chat.player_id)) return;
        r = message_broadcast(MESSAGE_CHAT);
        r->chat.player_id = m->chat.player_id;
        r->chat.msg       = m->chat.msg;
        break;

    case MESSAGE_SELECTION:
        if(!c) return;
        if(misbehaves_id(c, m->selection.player_id)) return;
        player_select(&c->player, m->selection.ship_type, m->selection.weapon_type);
        r = message_broadcast(MESSAGE_SELECTION);
        r->selection.player_id   = m->selection.player_id;
        r->selection.ship_type   = m->selection.ship_type;
        r->selection.weapon_type = m->selection.weapon_type;
        break;

    case MESSAGE_NAME:
        if(!c) return;
        if(misbehaves_id(c, m->name.player_id)) return;
        player_rename(&c->player, m->name.nick);
        r = message_broadcast(MESSAGE_NAME);
        r->name.player_id = m->name.player_id;
        r->name.nick      = m->name.nick;
        break;

    case MESSAGE_INPUT:
        if(!c) return;
        if(misbehaves_id(c, m->input.player_id)) return;
        {
            if(m->input.frameno < c->last_in_frameno)
                break;
            uint8_t mask = ~(0xff << (m->input.frameno - c->last_in_frameno));
			c->last_in_frameno = m->input.frameno;

            player_input(&c->player,
                         mask & m->input.up,
                         mask & m->input.down,
                         mask & m->input.left,
                         mask & m->input.right,
                         mask & m->input.fire1,
                         mask & m->input.fire2,
                            rad(m->input.angle));
        }
        break;

    default:
        misbehaves(c, c != 0, "invalid message id");
    }
}

static void packet_scan(Packet *p) {
    Message m;
    size_t seqno;
    Client *c = client_lookup(&p->adr);
    if(c) {
        c->last_in_ack   = max(p->ack, c->last_in_ack);
        c->last_activity = max(server->cur_time, c->last_activity);
    }

    while(packet_get(p, &m, &seqno)) {
        if(m_check_seqno(c, &m, seqno)) {
            if(is_reliable(&m))
                message_debug(&m, src_fmt(c));
            message_handle(c, &p->adr, &m, p->time, seqno);
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
        if(c) protocol_timed_out(c);
    }

    timer_stop(TIMER_RECV);
    counter_set(COUNTER_RECV, stats.nrecv);
}

void protocol_notify_state(Entity *e) {
    Message *m;
    if(e->dead) {
        m = message_broadcast(MESSAGE_REMOVE);
        m->remove.entity_id = e->id;
    } else {
        m = message_broadcast(MESSAGE_ADD);
        m->add.entity_id = e->id;
        m->add.player_id = e->player->id;
        m->add.type_id   = e->type->id;
    }
}

void protocol_notify_collision(Entity *e0, Entity *e1, Vec v) {
    Message *m = message_broadcast(MESSAGE_COLLISION);
    m->collision.entity_id[0] = e0->id;
    m->collision.entity_id[1] = e1->id;
    m->collision.x = v.x;
    m->collision.y = v.y;
}

static void protocol_send_gamestate(Client *cn) {
    Message *r;
    Client *c;
    clients_foreach(c) {
		if(c->dead) continue; // TODO: Hack
        if(c == cn) continue;

        r = message_unicast(cn, MESSAGE_JOIN);
        r->join.player_id = c->player.id;

        r = message_unicast(cn, MESSAGE_NAME);
        r->name.player_id = c->player.id;
        r->name.nick      = c->player.name;
    }

    Entity *e;
    entities_foreach(e) {
		if(e->dead) continue; // TODO: Hack
        r = message_unicast(cn, MESSAGE_ADD);
        r->add.entity_id  = e->id;
        r->add.player_id  = e->player->id;
        r->add.type_id    = e->type->id;
    }

    r = message_unicast(cn, MESSAGE_SYNCED);
}

static void packet_init_header(Client *c, Packet *p) {
    packet_init(p, &c->adr, c->last_in_seqno, server->cur_time);
}

static int packet_fmt(Client *c, Packet *p, QueuedMessage *qm) {
    if(!qm_check_relevant(c, qm)) return 1;
    if(qm_tries(c,qm) == 0 /* && is_reliable(&qm->m) */)
        message_debug(&qm->m, dest_fmt(c));

    if(packet_put(p, &qm->m, qm_seqno(c, qm))) {
        if(qm_tries(c,qm) > 0)
            stats.nresend ++;
        qm_tries(c,qm) ++;
        return 1;
    }
    return 0;
}

static int packet_send_init(Client *c, Packet *p) {
    if(packet_hasdata(p)) {
        //packet_debug(p);
        packet_send(p);
        stats.nsend ++;
        if(p->io_failed) return 0;
        packet_init_header(c, p);
    }
    return 1;
}

static int packet_fmt_update_header(Packet *p, size_t n) {
    Message m;
    m.type = MESSAGE_UPDATE;
    m.update.n = n;
    return packet_put(p, &m, 0);
}

static int packet_fmt_update(Packet *p, Entity *e) {
    Update u;
    u.entity_id = e->id;
    u.x   = e->x.x;
    u.y   = e->x.y;
    u.vx  = e->v.x;
    u.vy  = e->v.y;
    u.angle  = deg(e->phi);
    u.health = e->health * 100 / e->type->max_health;
    return packet_put_u(p, &u);
}

static int protocol_send_client(Client *c) {
    Packet p;
    Entity *e;
    QueuedMessage *qm;

    packet_init_header(c, &p);

    queue_foreach(qm) {
    again_qm:
        if(!packet_fmt(c, &p, qm)) { /* did not fit any more */
            int ok = packet_send_init(c, &p);
            if(!ok) return 0;
            goto again_qm;
        }
    }

    size_t n = 0;
    size_t k = pool_nused(&server->entities);
    entities_foreach(e) {
    again_e:
        if(!n) {
            n = min(k, packet_update_n(&p));
            if(n) {
                assert(packet_fmt_update_header(&p, n));
            } else {
                int ok = packet_send_init(c, &p);
                if(!ok) return 0;
            }
            goto again_e;
        } else {
            assert(packet_fmt_update(&p, e));
            n --;
            k --;
        }
    }
    assert(k == 0);
    
    return packet_send_init(c, &p);
}

/* If a client times out, its dead flag is set,
 * so that the client can be cleaned up later.
 * If the client has previously sent a disconnect message,
 * then hasleft is set and the other players do not need further notice.
 * client_remove also disables c as message recepient.
 */
static void protocol_timed_out(Client *c) {
    Message *r;
    if(!c->hasleft) {
        r = message_broadcast(MESSAGE_LEAVE);
        r->leave.player_id = c->player.id;
    }
    client_remove(c);
}

/* Client has sent too many confusing messages,
 */
static void protocol_kick(Client *c) {
    Message *r;
    static char kick_msg[] = "kicked for (protocol) misbehavior";
    Str kick_msg_s = { strlen(kick_msg), kick_msg };
    r = message_unicast(c, MESSAGE_CHAT);
    r->chat.player_id = server->self->player.id;
    r->chat.msg = kick_msg_s;
}

/* (re)send queued messages */
void protocol_send(int force) {
    if(!force && !clock_periodic(&server->update_periodic, UPDATE_INTERVAL))
        return;

    timer_start(TIMER_SEND);
    stats.nsend   = 0;
    stats.nresend = 0;

    Client *c;
    clients_foreach(c) {
        if(client_isbot(c)) {
            continue;
        } else if(c->last_activity + TIMEOUT_INTERVAL < server->cur_time) {
            protocol_timed_out(c);
        } else if (c->misbehavior > MISBEHAVIOR_LIMIT) {
            protocol_kick(c);
            protocol_timed_out(c);
        }
        else {
            int ok = protocol_send_client(c);
            /* !ok indicates a socket error */
            if(!ok) protocol_timed_out(c);
        }
    }

    timer_stop(TIMER_SEND);
    counter_set(COUNTER_SEND,   stats.nsend);
    counter_set(COUNTER_RESEND, stats.nresend);
}

static void protocol_send_full(Address *adr, size_t seqno) {
    Packet p;
    Message m;
    m.type = MESSAGE_FULL;
    packet_init(&p, adr, seqno, 0);
    packet_put(&p, &m, 0);
    packet_send(&p);
    stats.nsend ++;
}

void protocol_init() {
    pool_static(&server->queue, _queue, qm_ctor, qm_dtor);
}

void protocol_cleanup() {
    pool_free_pred(&server->queue,   qm_check_obsolete);
}
