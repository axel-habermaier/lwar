#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void gravity(Entity *e0);

static const Real gravity_factor = 10000; // 0.04;
EntityType type_planet = {
    ENTITY_TYPE_PLANET,     /* type id */
    gravity,                /* activation callback */
    0,                      /* collision callback */
    0,                      /* interval */
    0,                      /* energy */
    1,                      /* health */
    0,                      /* length */
    10000,                  /* mass */
    128,                    /* radius */
    {0,0},                  /* acceleration */
    {0,0},                  /* brake */
    0,                      /* rotation */
};

static void gravity(Entity *e0) {
    Entity *e1;
    Real m0 = e0->mass;

    entities_foreach(e1) {
        if(e0->type != e1->type) {
            Real m1 = e1->mass;
            if(m1 == 0) continue;

            Vec dx = sub(e0->x, e1->x);
            Real l  = len(dx);
            Vec r  = normalize(dx);

            /* force is quadratic wrt proximity,
             * and wrt to inverse of mass of e1
             */
            Vec a  = scale(r, gravity_factor * (m0 + m1) / m1 / (l*l)); 
            entity_push(e1, a);
        }
    }
}

