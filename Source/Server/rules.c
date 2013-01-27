#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>
#include <math.h>

#include "server.h"
#include "vector.h"
#include "log.h"

static const Pos gravity_factor = 0.04;

static void shoot(Entity *e) {
    /* Vec f = unit(rad(e->rot)); */
    Vec f = unit(rad(rand()%360));
    Vec x = add(e->x, scale(f, 2*entity_radius(e)));
    Vec v = add(e->v, scale(f, 2)); /* 1 = initial speed */
    EntityType *bullet = entity_type(1);
    Entity *b = entity_create(bullet,x,v);
}

static void gravity(Entity *e0) {
    Entity *e1;
    Pos m0 = entity_mass(e0);

    for_each_allocated_entity(server, e1) {
        if(e0->type != e1->type) {
            Pos m1 = entity_mass(e1);

            Vec dx = sub(e0->x, e1->x);
            Pos l  = len(dx);
            Vec r  = normalized(dx);

            /* force is quadratic wrt proximity */
            Vec a  = scale(r, gravity_factor * (m0 + m1) / (l*l)); 
            physics_acc(e1, a);
        }
    }
}

static EntityType types[] = {
    { 1, 1, {0.1,0.1},200, shoot   }, /* small player ship */
    {.5,.2, {0,0},      0, 0       }, /* bullet */
    { 5,50, {0,0},      0, gravity }, /* planet */
};

#define NTYPES (sizeof(types)/sizeof(EntityType))

void rules_init() {
}

EntityType *entity_type(size_t id) {
    if(id < NTYPES)
        return &types[id];
    else return 0;
}
