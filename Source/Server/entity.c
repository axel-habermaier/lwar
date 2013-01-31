#include <assert.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"

static Entity _entities[MAX_ENTITIES];

Pos entity_radius(Entity *e) {
    return e->type->r;
}

Pos entity_mass(Entity *e) {
    return e->type->m;
}

Vec entity_acc(Entity *e) {
    return e->type->a;
}

static void entity_action(Entity *e) {
    if(e->active) {
        if(clock_periodic(&e->activation_periodic,
                           e->type->activation_interval))
        {
            if(e->type->action)
                e->type->action(e);
        }
    } else {
        e->activation_periodic = 0;
    }
}

void entity_actions() {
    Entity *e;
    entities_foreach(e) {
        entity_action(e);
    }
}

static void entity_ctor(size_t i, void *p) {
    Entity *e = (Entity*)p;
    e->id.n = i;
}

static void entity_dtor(size_t i, void *p) {
    Entity *e = (Entity*)p;
    e->id.gen ++;
}

static int entity_check_obsolete(size_t i, void *p) {
    Entity *e = (Entity*)p;
    return e->dead;
}

Entity *entity_create(EntityType *t, Player *p, Vec x, Vec v) {
    assert(t);
    Entity *e = slab_new(&server->entities, Entity);
    assert(e);
    e->player = p;
    e->type   = t;
    e->x      = x;
    e->v      = v;
    e->rot    = 0; /* TODO: use v */
    e->health = t->health;
    e->dead   = 0;
    /* TODO: notify */
    return e;
}

void entity_remove(Entity *e) {
    /* TODO: notify */
    if(e) e->dead = 1;
}

void entities_init() {
    slab_static(&server->entities, _entities, entity_ctor, entity_dtor);
}

void entities_cleanup() {
    slab_free_pred(&server->entities, entity_check_obsolete);
}

EntityType *entity_type_get(size_t id) {
    if(id < MAX_TYPES)
        return server->types[id];
    else return 0;
}

void entity_type_register(size_t id, EntityType *t) {
    if(id < MAX_TYPES)
        server->types[id] = t;
}
