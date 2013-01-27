#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void gravity(Entity *e0);

static const Pos gravity_factor = 0.04;
static EntityType _planet = { 5,50, {0,0}, 0, gravity };
EntityType *type_planet = &_planet;

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

