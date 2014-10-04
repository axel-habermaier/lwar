#ifndef STATE_H
#define STATE_H

#include "array.h"
#include "bitset.h"
#include "client.h"
#include "clock.h"
#include "connection.h"
#include "list.h"
#include "pool.h"
#include "pq.h"

typedef struct Server Server;

extern Server *server;

struct Connection {
	char _[8];
};

struct Server {
    bool       running;
    Client    *self;

    Pool       clients;
    BitSet     connected;
			   
    Pool       entities;
    Pool       queue;
    Array      types;
    List       formats;
    PrioQueue  collisions;
    Pool       strings;
			   
    Clock      cur_clock;
    Clock      prev_clock;
    Clock      update_periodic;
	Clock      discovery_periodic;

	Connection conn_clients;
	Connection conn_discovery;
};

#define clients_foreach(c)       pool_foreach(&server->clients, c, Client)
#define entities_foreach(e)      pool_foreach(&server->entities, e, Entity)
#define queue_foreach(qm)        pool_foreach(&server->queue, qm, QueuedMessage)
#define children_foreach(e0,e1)  list_for_each_entry(e1, Entity, &e0->children, siblings)
#define collisions_foreach(c)    pq_foreach(&server->collisions, c, Collision)
#define formats_foreach(f)       list_for_each_entry(f, Format, &server->formats, _l)
#define updates_foreach(t,e)     list_for_each_entry(e, Entity, &t->all, _u)
#define slots_foreach(p,s,st)    for(s = p->weapons, st = (p->ship.entity ? p->ship.entity->type->slots : 0); s<p->weapons+NUM_SLOTS; s++, st = (p->ship.entity ? st + 1 : 0))

#endif