#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "server.h"
#include "message.h"
#include "packet.h"

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
    return qm->dest == 0;
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

QueuedMessage *qm_create() {
    return slab_new(&server->queue, QueuedMessage);
}

void qm_remove(QueuedMessage *qm) {
    slab_free(&server->queue, qm);
}

static void qm_enqueue(Client *c, QueuedMessage *qm) {
    qm_set_dest(c,qm);
    if(is_reliable(&qm->m))
        qm_seqno(c,qm) = (c->next_out_seqno ++);
}

static Message *message_unicast(Client *c, size_t type) {
    QueuedMessage *qm = qm_create();
    Message *m = &qm->m;
    m->type = type;
    qm_enqueue(c,qm);
    return m;
}

static Message *message_broadcast(size_t type) {
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
    m->time = server->cur_time; /* TODO: subtract some base_time *

    m->update.entity = e->id;

    m->update.x   = e->x.x;     /* TODO: some scaling *
    m->update.y   = e->x.y;

    m->update.vx  = e->v.x;
    m->update.vy  = e->v.y;

    m->update.rot = e->rot;
    m->update.health = e->health;
    return m;
}
*/

/* TODO: must update seqno for clients */
static int message_handle(Address *a, Message *m, size_t seqno) {
    /*
    Client *c;

    message_print(m);

    switch(m->type) {
    case MESSAGE_CONNECT:
        {
            c = client_create();
            m = message_reliable(0, m, MESSAGE_JOIN);
            m->join.player = c->player.id;
            c->adr = *a;
        }
        break;
    case MESSAGE_LEAVE:
        {
            /* TODO: if we need to send back to c, then keep c around with deleted flag *
            c = client_get(m->leave.player, a);
            if(!c) return;
            client_remove(c);
            m = message_reliable(0, m, MESSAGE_LEAVE);
        }
        break;
    case MESSAGE_CHAT:
        {
            c = client_get(m->chat.player, a);
            if(!c) return;
            m = message_reliable(0, m, MESSAGE_CHAT);
        }
        break;
    case MESSAGE_INPUT:
        {
            c = client_get(m->input.player, a);
            if(!c) return;
            /* TODO: rotation? *
            player_input(&c->player, m->input.up, m->input.down, m->input.left, m->input.right, m->input.shooting);
            uint32_t ack = m->input.ack;
            mqueue_ack(&c->queue, ack);
            c->last_ack = ack;
        }
        break;
    }
    */
	return 0;
}

static void packet_scan(Packet *p) {
    Message m;
    size_t seqno;
	// TODO: Must parse packet header first
    while(packet_get(p, &m, &seqno)) {
        message_handle(&p->adr, &m, seqno);
    }
}

/* handle all pending incoming messages */
void protocol_recv() {
    Packet p;
    while(packet_recv(&p)) {
        packet_scan(&p);
    }
}

static void packet_init_header(Client *c, Packet *p) {
    packet_init(p);
	// TODO: What is this? VS no like.
    Message m ;//= { MESSAGE_HEADER, .header = { APP_ID, c->last_in_seqno, server->cur_time } };
    packet_put(p, &m, 0); /* just dummy, header shouldn't be marked reliable */
}

static int packet_fmt(Client *c, Packet *p, QueuedMessage *qm) {
    /* first, check whether the message is relevant for c */
    if(!qm_check_relevant(c, qm)) return 1;
    return packet_put(p, &qm->m, qm_seqno(c, qm));
}

static void protocol_send_client(Client *c) {
    Packet p;
    QueuedMessage *qm;

    packet_init_header(c, &p);

    queue_foreach(qm) {
    again:
        if(!packet_fmt(c, &p, qm)) { /* did not fit any more */
            packet_send(&p);
            packet_init_header(c, &p);
            goto again;
        }
    }
}

/* (re)send queued messages */
void protocol_send() {
    Client *c;
    clients_foreach(c) {
        protocol_send_client(c);
    }
    /* deferred, because queue_foreach is not safe against deletion */
    slab_free_pred(&server->queue, qm_check_obsolete);
}

void protocol_init() {
    slab_static(&server->queue, _queue, qm_ctor, qm_dtor);
}
