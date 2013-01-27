#include <math.h>
#include <stdint.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "server.h"
#include "message.h"

typedef struct QueuedMessage QueuedMessage;
struct QueuedMessage {
    Message m;
    List l;
};

void mqueue_init(List *l) {
    INIT_LIST_HEAD(l);
}

void mqueue_destroy(List *l) {
    mqueue_ack(l, ~0);
}

/* expects l to be sorted wrt. sequence numbers in ascending order */
void mqueue_ack(List *l, uint32_t ack) {
    while(!list_empty(l)) {
        List *a = l->next;
        QueuedMessage *qm = list_entry(a, QueuedMessage, l);
        if(qm->m.seqno > ack) return;
        free(qm);
        list_del(l);
    }
}

static void message_enqueue(Client *c, QueuedMessage *qm) {
    list_add_tail(&c->queue, &qm->l);
    qm->m.seqno = (c->next_seqno ++);
}

static void message_enqueue_all(QueuedMessage *qm) {
    Client *c;
    for_each_client(server, c)
        if(c->connected)
            message_enqueue(c, qm);
}

/* if c == 0 then broadcast */
Message *message_reliable(Client *c, Message *m, uint8_t new_type) {
    QueuedMessage *qm = (QueuedMessage*)malloc(sizeof(QueuedMessage));
    if(m) memcpy(&qm->m, m, sizeof(Message));
    m = &qm->m;
    m->type = new_type;
    if(c) message_enqueue(c, qm);
    else  message_enqueue_all(qm);
    return m;
}

void message_update(Entity *e, Message *m) {
    m->type = MESSAGE_UPDATE;
    m->time = server->cur_time; /* TODO: subtract some base_time */

    m->update.entity = e->id;

    m->update.x   = e->x.x;
    m->update.y   = e->x.y;

    m->update.vx  = e->v.x;
    m->update.vy  = e->v.y;

    m->update.rot = e->rot;
    m->update.health = e->health;
}

static void message_handle(Address *a, Message *m) {
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
            /* TODO: if we need to send back to c, then keep c around with deleted flag */
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
            /* TODO: rotation? */
            player_input(&c->player, m->input.up, m->input.down, m->input.left, m->input.right, m->input.shooting);
            uint32_t ack = m->input.ack;
            mqueue_ack(&c->queue, ack);
            c->last_ack = ack;
        }
        break;
    }
}

size_t packet_scan(char *p, size_t n, Address a) {
    size_t i=0,k;
    Message m;
    while(i < n) {
        k = message_unpack(p+i, &m, n-i);

        if(k > 0) message_handle(&a, &m);
        else break;

        i += k;
    }
    return i;
}

size_t packet_fmt_queue(Client *c, char *p, size_t n, Address *a, void **cookie) {
    size_t i=0,k;
    QueuedMessage *qm;
    Message *m;

    list_for_each_entry_cont(*cookie, qm, QueuedMessage, &c->queue, l) {
        m = &qm->m;
        k = message_pack(p+i, m, n-i);
        if(k > 0) {
            i += k;
        } else { /* does not fit any more */
            *cookie = qm;
            *a      = c->adr;
            return i;
        }
    }
    return 0;
}

size_t packet_fmt_update(Entity *e, char *p, size_t n) {
    Message m;
    message_update(e,&m);
    return message_pack(p, &m, n);
}
