#include <stdint.h>
#include <stddef.h>

#include "server.h"

/* TODO: use entitie's type */
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
    for_each_allocated_entity(server, e) {
        entity_action(e);
    }
}

Entity *entity_create(EntityType *t, Vec x, Vec v) {
    if(list_empty(&server->free))
        return 0;
    
    List *l = server->free.next;
    list_del(l);
    list_add_tail(l, &server->created);

    Entity *e = list_entry(l, Entity, l);
    e->x      = x;
    e->v      = v;
    e->rot    = 0;
    e->health = 100;
    e->type   = t;
    return e;
}


void entity_remove(Entity *e) {
    List *l = &e->l;
    e->id.gen ++;
    list_del(l);
    list_add_tail(l, &server->free);
}

void entities_init() {
    size_t i;
    for(i=0; i<MAX_ENTITIES; i++) {
        Entity *e = &server->entities[i];
        e->id.n = i;

        INIT_LIST_HEAD(&e->l);
        list_add_tail(&e->l, &server->free);
    }
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
