#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void active(Entity *ray);

EntityType type_ray = {
    ENTITY_TYPE_RAY,        /* type id */
    active,                 /* activation callback */
    0,                      /* collision callback  */
    0,                      /* interval */
    0,                      /* energy */
    1,                      /* health */
    0,                      /* length */
    0,                      /* mass */
    512,                    /* radius */ /* TODO: hack for visualization */
    {0,0},                  /* acceleration */
    {0,0},                  /* brake */
    0,                      /* rotation */
};

static void active(Entity *ray) {
    Entity *phaser = ray->parent;
    assert(phaser);
    if(!phaser->active) {
        entity_remove(ray);
        return;
    }

    Vec u = unit(ray->phi);

    Real    best_t;
    Entity *best_e = 0;

    Entity *e;
    entities_foreach(e) {
        if(e == ray) continue;
        if(e == ray->parent) continue;
        if(ray->parent->parent && e == ray->parent->parent) continue;

        Real r = e->radius;
        Vec dx = sub(ray->x, e->x);

        Real t,t0,t1;

        Real a =   dot_sq(u);
        Real b = 2*dot(dx,u);
        Real c =   dot_sq(dx) - r*r;

        int  n =   roots(a,b,c, &t0,&t1);
        if(n) {
                 if(0 < t0 && (t0 < t1 || t1 < 0)) t = t0;
            else if(0 < t1 && (t1 < t0 || t0 < 0)) t = t1;
            else continue;

            if(t > ray->radius)
                continue;

            if(!best_e || t < best_t) {
                best_t = t;
                best_e = e;
            }
        }
    }

    if(best_e) {
        ray->len = best_t;
    } else {
        ray->len = ray->radius;
    }
}
