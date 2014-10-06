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

	qm_tries(c,qm) = 0;
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


void queue_timeout(Client *c) {
    Message *r;
    assert(c->remote);
    if(!c->hasleft) {
        r = message_broadcast(MESSAGE_LEAVE);
        r->leave.player_id = c->player.id;
		r->leave.reason = LEAVE_DROPPED;
    }
}

/* Note: already enqueued add messages won't be duplicated,
 *       since these are not marked for client cn in qm->dest
 */
void queue_gamestate_for(Client *cn) {
    Message *r;
    Client *c;
    clients_foreach(c) {
		if(c->dead) continue;
        if(c == cn) continue;

        r = message_unicast(cn, MESSAGE_JOIN);
        r->join.player_id = c->player.id;
        r->join.nick      = c->player.name;
    }

    Entity *e;
    entities_foreach(e) {
		if(e->dead) continue;
        if(!e->type->format) continue;

        r = message_unicast(cn, MESSAGE_ADD);
        r->add.entity_id  = e->id;
        r->add.player_id  = e->player->id;
        r->add.type_id    = e->type->id;
    }

    r = message_unicast(cn, MESSAGE_SYNCED);
}

void queue_forward(Message *m) {
    Message *r;
    r = message_broadcast(m->type);
    *r = *m;
}

void queue_join(Client *c) {
    Message *r;
    r = message_broadcast(MESSAGE_JOIN);
    r->join.player_id = c->player.id;
	r->join.nick = c->player.name;
}

void queue_leave(Client *c) {
    Message *r;
    r = message_broadcast(MESSAGE_LEAVE);
    r->leave.player_id = c->player.id;
	r->leave.reason = LEAVE_QUIT;
}

void queue_collision(Collision *c) {
    Message *r;
    r = message_broadcast(MESSAGE_COLLISION);
    r->collision.entity_id[0] = c->e[0]->id;
    r->collision.entity_id[1] = c->e[1]->id;
    r->collision.x = c->x.x;
    r->collision.y = c->x.y;
}

void queue_add(Entity *e) {
    assert(!e->dead);
    Message *r;
    r = message_broadcast(MESSAGE_ADD);
    r->add.entity_id = e->id;
    r->add.player_id = e->player->id;
    r->add.type_id   = e->type->id;
}

void queue_remove(Entity *e) {
    assert(e->dead);
    Message *r;
    r = message_broadcast(MESSAGE_REMOVE);
    r->remove.entity_id = e->id;
}

void queue_kill(Player *k, Player *v) {
    Message *r;
    r = message_broadcast(MESSAGE_KILL);
    r->kill.killer_id = k->id;
    r->kill.victim_id = v->id;
}

void queue_stats()
{
	Message *r;
	Client* c;

	r = message_broadcast(MESSAGE_STATS);
	r->stats.n = 0;

	clients_foreach(c) {
		if (c->player.id.n == 0)
			continue;
	
		r->stats.info[r->stats.n].player_id = c->player.id;
		r->stats.info[r->stats.n].kills = c->player.kills;
		r->stats.info[r->stats.n].deaths = c->player.deaths;
		r->stats.info[r->stats.n].ping = c->ping;
		++r->stats.n;
	}
}

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
