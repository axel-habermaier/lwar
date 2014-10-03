#include "event.h"

static void notify(Entity *e) {
    player_notify_entity(e);
    protocol_notify_entity(e);
}

void entities_notify_collision(Collision *c) {
    Entity *e0 = c->e[0];
    Entity *e1 = c->e[1];
    Real i0 = c->i[0];
    Real i1 = c->i[1];
    EntityType *t0 = e0->type;
    EntityType *t1 = e1->type;
    if(t0->collide) t0->collide(e0, e1, i0);
    if(t1->collide) t1->collide(e1, e0, i1);
}

static void notify(Collision *c) {
    entities_notify_collision(c);
    protocol_notify_collision(c);
}

