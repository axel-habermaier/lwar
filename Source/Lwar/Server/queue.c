#include "types.h"

#include "queue.h"

#include <stdint.h>

#include "bitset.h"
#include "config.h"
#include "coroutine.h"
#include "debug.h"
#include "log.h"
#include "message.h"
#include "physics.h"
#include "server.h"

typedef struct QueuedMessage QueuedMessage;
typedef struct PerClient     PerClient;

struct PerClient {
    size_t seqno;
    size_t tries;
    Clock last_tx_time;
};

struct QueuedMessage {
    List _l;
    BitSet dest;
    /* sequence numbers for each client */
    PerClient perclient[MAX_CLIENTS];
    Message m;
};

static void qm_ctor(size_t i, void *p) {
    QueuedMessage *qm = (QueuedMessage*)p;
    qm->dest = set_empty;
}

static void qm_dtor(size_t i, void *p) {}

static bool qm_check_obsolete(size_t i, void *p) {
    QueuedMessage *qm = (QueuedMessage*)p;
    /* only keep qm for receiving clients */
    return set_disjoint(qm->dest, server->connected);
}

static bool qm_check_dest(Client *c, QueuedMessage *qm) {
    size_t id = c->player.id.n;
    return set_contains(qm->dest, id);
}

static void qm_set_dest(Client *c, QueuedMessage *qm) {
    size_t id = c->player.id.n;
    set_insert(qm->dest, id);
}

static void qm_clear_dest(Client *c, QueuedMessage *qm) {
    size_t id = c->player.id.n;
    set_remove(qm->dest, id);
}

#define qm_seqno(c,qm) qm->perclient[c->player.id.n].seqno
#define qm_tries(c,qm) qm->perclient[c->player.id.n].tries
#define qm_last_tx_time(c,qm) qm->perclient[c->player.id.n].last_tx_time

static bool qm_check_relevant(Client *c, QueuedMessage *qm) {
    /* message not for c */
    if(!qm_check_dest(c, qm)) {
        return false;
    }

	 /* unreliable message for c, do not resend */
    if(!is_reliable(&qm->m)) {
        qm_clear_dest(c, qm);
        return true;
    }

    if(   qm_tries(c,qm) > 0
       && qm_last_tx_time(c,qm) + RETRANSMIT_INTERVAL >= server->cur_clock)
    {
        return false;
    }
    else {
        qm_last_tx_time(c,qm) = server->cur_clock;
    }

    /* reliable message for c, already acknowledged */
    if(qm_seqno(c, qm) <= c->last_in_ack) {
        qm_clear_dest(c, qm);
        return false;
    }

    /* reliable, unacknowledged message for c */
    return true;
}

static QueuedMessage *qm_create() {
    QueuedMessage *qm = pool_new(&server->queue, QueuedMessage);
    /*
	if (!qm) {
		queue_foreach(qm) {
			log_debug("dest = %lx", qm->dest);
			debug_message(&qm->m, "");
		}
		assert(false);
	}
    */
    assert(qm); /* TODO: handle allocation failure */
    return qm;
}

static void qm_enqueue(Client *c, QueuedMessage *qm) {
    qm_set_dest(c,qm);

    if(is_reliable(&qm->m)) {
        qm_seqno(c,qm) = (c->next_out_reliable_seqno ++);
    }
	else {
		qm_seqno(c,qm) = (c->next_out_unreliable_seqno ++);
	}

    qm_last_tx_time(c,qm) = 0;
	qm_tries(c,qm) = 0;
}

void queue_unicast(Client *c, Message *m) {
    QueuedMessage *qm = qm_create();
    qm->m = *m;
    qm_enqueue(c,qm);
}

void queue_broadcast(Message *m) {
    QueuedMessage *qm = qm_create();
    qm->m = *m;

    Client *c;
    clients_foreach(c)
        qm_enqueue(c,qm);
}


/*
void queue_timeout(Client *c) {
    Message *r;
    assert(c->remote);
    if(!c->hasleft) {
        r = message_broadcast(MESSAGE_LEAVE);
        r->leave.player_id = c->player.id;
		r->leave.reason = LEAVE_DROPPED;
    }
}
*/

Message *queue_next(cr_t *state, Client *c, size_t *tries) {
    static QueuedMessage *qm;

    cr_begin(state);

    queue_foreach(qm) {
        if(!qm_check_relevant(c, qm))
            continue;

        if(tries)
            *tries = qm_tries(c,qm);

        qm->m.seqno = qm_seqno(c,qm);
        cr_yield(state,&qm->m);

        qm_tries(c,qm) ++;
    }

    cr_return(state,0);
}

/* TODO: these two functions do not really belong here */
void queue_init() {
    INIT_LIST_HEAD(&server->formats);
    pool_dynamic(&server->queue, QueuedMessage, MAX_QUEUE, qm_ctor, qm_dtor);
}

void queue_cleanup() {
    pool_free_pred(&server->queue, qm_check_obsolete);
}

void queue_shutdown() {
    pool_shutdown(&server->queue);
}
