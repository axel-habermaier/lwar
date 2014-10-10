#include "types.h"

#include "message.h"

#include "uint.h"
#include "debug.h"
#include "server.h"
#include "physics.h"
#include "client.h"
#include "entity.h"
#include "log.h"

#include <stdlib.h>

bool is_reliable(Message *m) {
    return m->type < 100;
}

bool is_update(Message *m) {
    return    m->type >= MESSAGE_UPDATE
           && m->type <= MESSAGE_UPDATE_SHIP;
}

void message_join(Message *m, Client *c) {
    m->type = MESSAGE_JOIN;
    m->join.player_id = c->player.id;
    m->join.nick = c->player.name;
}

void message_leave(Message *m, Client *c, LeaveReason reason) {
    m->type = MESSAGE_LEAVE;
    m->leave.player_id = c->player.id;
    m->leave.reason = reason;
}

void message_collision(Message *m, Collision *c) {
    m->type = MESSAGE_COLLISION;
    m->collision.entity_id[0] = c->e[0]->id;
    m->collision.entity_id[1] = c->e[1]->id;
    m->collision.x = c->x.x;
    m->collision.y = c->x.y;
}

void message_add(Message *m, Entity *e) {
    assert(!e->dead);
    m->type = MESSAGE_ADD;
    m->add.entity_id = e->id;
    m->add.player_id = e->player->id;
    m->add.type_id   = e->type->id;
}

void message_remove(Message *m, Entity *e) {
    m->type = MESSAGE_REMOVE;
    m->remove.entity_id = e->id;
}

void message_kill(Message *m, Player *k, Player *v) {
    m->type = MESSAGE_KILL;
    m->kill.killer_id = k->id;
    m->kill.victim_id = v->id;
}

void message_stats(Message *m) {
	Client* c;

    m->type = MESSAGE_STATS;
	m->stats.n = 0;

	clients_foreach(c) {
		if (c->player.id.n == 0)
			continue;
	
		m->stats.info[m->stats.n].player_id = c->player.id;
		m->stats.info[m->stats.n].kills = c->player.kills;
		m->stats.info[m->stats.n].deaths = c->player.deaths;
		m->stats.info[m->stats.n].ping = c->ping;
		m->stats.n ++;
	}
}

void message_synced(Message *m) {
    m->type = MESSAGE_SYNCED;
}

void message_reject(Message *m, RejectReason reason) {
    m->type = MESSAGE_REJECT;
	m->reject.reason = reason;
}

void message_update(Message *m, Format *f) {
    m->type = f->type;
    m->update.n = f->n;
    m->update.f = f;
}
