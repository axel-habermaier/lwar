#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void gravity(Entity *e0);
static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1);

static const Pos gravity_factor = 10000; // 0.04;
static EntityType _planet = {
    ENTITY_TYPE_PLANET,     /* type id */
    gravity,                /* activation callback */
    hit,                    /* collision callback  */
    128,                    /* radius  */
    10000,                  /* mass    */
    {0,0},                  /* acceleration */
    {0,0},                  /* brake   */
    0,                      /* turn speed   */
    1000,                   /* max health   */
};

EntityType *type_planet = &_planet;

static void gravity(Entity *e0) {
    Entity *e1;
    Pos m0 = e0->type->mass;

    entities_foreach(e1) {
        if(e0->type != e1->type) {
            Pos m1 = e1->type->mass;
            if(m1 == 0) continue;

            Vec dx = sub(e0->x, e1->x);
            Pos l  = len(dx);
            Vec r  = normalize(dx);

            /* force is quadratic wrt proximity,
             * and wrt to inverse of mass of e1
             */
            Vec a  = scale(r, gravity_factor * (m0 + m1) / m1 / (l*l)); 
            entity_push(e1, a);
        }
    }
}

static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1) {
    /* e0->health -= len(v0);  */
    /* e0->v = add(e0->v, v0); */
}
