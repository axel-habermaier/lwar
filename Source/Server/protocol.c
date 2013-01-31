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

static void protocol_send_full(Address *adr, size_t seqno);
static void protocol_send_gamestate(Client *c);

typedef struct QueuedMessage QueuedMessage;
typedef struct PerClient     PerClient;

struct PerClient {
    size_t seqno;
    /* TODO: timeout for this message */
};

struct QueuedMessage {
    List l;
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

static int qm_check_relevant(Client *c, QueuedMessage *qm) {
    
    /* message not for c */
    if(!qm_check_dest(c, qm)) {
        return 0;
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
    return slab_new(&server->queue, QueuedMessage);
}

static void qm_enqueue(Client *c, QueuedMessage *qm) {
    qm_set_dest(c,qm);
    if(is_reliable(&qm->m))
        qm_seqno(c,qm) = (c->next_out_seqno ++);
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

/* return pointer to static location,
 * process messages strictly sequentially */
/*
static Message *message_update(Entity *e) {
    static Message _m;
    Message *m = &_m;
    m->type = MESSAGE_UPDATE;
    m->time = server->cur_time; TODO: subtract some base_time *

    m->update.entity = e->id;

    m->update.x   = e->x.x;     TODO: some scaling *
    m->update.y   = e->x.y;

    m->update.vx  = e->v.x;
    m->update.vy  = e->v.y;

    m->update.rot = e->rot;
    m->update.health = e->health;
    return m;
}
*/

static int misbehaves(Client *c, int test) {
    if(test) c->misbehavior ++;
    return test;
}

static int misbehaves_id(Client *c, Id id) {
    return misbehaves(c, !id_eq(c->player.id, id));
}

static void message_handle(Client *c, Address *adr, Message *m, size_t time, size_t seqno) {
    Message *r;

    switch(m->type) {
    case MESSAGE_CONNECT:
        if(misbehaves(c, c != 0)) return;
        c = client_create(adr);
        if(c) {
            m_check_seqno(c, m, seqno);
            r = message_broadcast(MESSAGE_JOIN);
            r->join.player_id = c->player.id;

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

            player_input(&c->player,
                         mask & m->input.up,
                         mask & m->input.down,
                         mask & m->input.left,
                         mask & m->input.right,
                         mask & m->input.shooting);
        }
        break;

    default:
        misbehaves(c, c != 0);
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
    
    if(c) {
        log_debug("%d> ack: %d, time: %d", c->player.id.n, p->ack, p->time);
    } else {
        log_debug("?> ack: %d, time: %d", p->ack, p->time);
    }

    while(packet_get(p, &m, &seqno)) {
        if(m_check_seqno(c, &m, seqno))
            log_debug("  > seqno: %d, type: %d", seqno, m.type);
            message_handle(c, &p->adr, &m, p->time, seqno);
    }
}

/* handle all pending incoming messages */
void protocol_recv() {
    Packet p;
    int ok;
    while((ok = packet_recv(&p))) {
        packet_scan(&p);
    }
    if(ok<0) {
        /* TODO: connection is dead */
    }
}

static void protocol_send_gamestate(Client *cn) {
    Message *r;
    Client *c;
    clients_foreach(c) {
        if(c == cn) continue;

        r = message_unicast(cn, MESSAGE_JOIN);
        r->join.player_id = c->player.id;

        r = message_unicast(cn, MESSAGE_NAME);
        r->name.player_id = c->player.id;
        r->name.nick      = c->player.name;
    }

    Entity *e;
    entities_foreach(e) {
        r = message_unicast(cn, MESSAGE_ADD);
        r->add.entity_id  = e->id;
        r->add.player_id  = (e->player ? e->player->id : server_id);
        r->add.type_id    = e->type->id;
    }

    r = message_unicast(cn, MESSAGE_SYNCED);
}

static void packet_init_header(Client *c, Packet *p) {
    packet_init(p, &c->adr, c->last_in_seqno, server->cur_time);
}

static int packet_fmt(Client *c, Packet *p, QueuedMessage *qm) {
    if(!qm_check_relevant(c, qm)) return 1;
    log_debug("  < seqno: %d, type: %d", qm_seqno(c, qm), qm->m.type);
    return packet_put(p, &qm->m, qm_seqno(c, qm));
}

static void packet_send_init(Client *c, Packet *p) {
    if(packet_hasdata(p)) {
        log_debug("<%d ack: %d, time: %d", c->player.id.n, p->ack, p->time);
        packet_send(p);
        packet_init_header(c, p);
    }
}

static void protocol_send_client(Client *c) {
    Packet p;
    QueuedMessage *qm;

    packet_init_header(c, &p);

    queue_foreach(qm) {
    again:
        if(!packet_fmt(c, &p, qm)) { /* did not fit any more */
            packet_send_init(c, &p);
            goto again;
        }
    }
    
    packet_send_init(c, &p);
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
 * and will be disconnected (by timeout later).
 * just there to use unicast and server_id :)
 */
static void protocol_kick(Client *c) {
    Message *r;
    static char kick_msg[] = "kicked for (protocol) misbehavior";
    Str kick_msg_s = { strlen(kick_msg), kick_msg };
    r = message_unicast(c, MESSAGE_CHAT);
    r->chat.player_id = server_id;
    r->chat.msg = kick_msg_s;

    r = message_broadcast(MESSAGE_DISCONNECT);
    message_handle(c, &c->adr, r, server->cur_time, 0);
}

/* (re)send queued messages */
void protocol_send() {
    Client *c;
    clients_foreach(c) {
        if(c->last_activity + TIMEOUT_INTERVAL < server->cur_time)
            protocol_timed_out(c);
        else if (c->misbehavior > MISBEHAVIOR_LIMIT)
            protocol_kick(c);
        else
            protocol_send_client(c);
    }
}

static void protocol_send_full(Address *adr, size_t seqno) {
    Packet p;
    Message m;
    m.type = MESSAGE_FULL;
    packet_init(&p, adr, seqno, 0);
    packet_put(&p, &m, 0);
    packet_send(&p);
}

void protocol_init() {
    slab_static(&server->queue, _queue, qm_ctor, qm_dtor);
}

void protocol_cleanup() {
    slab_free_pred(&server->queue,   qm_check_obsolete);
}
