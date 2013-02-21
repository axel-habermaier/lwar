#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"
#include "log.h"

static void aim(Entity *e);

EntityType type_rocket = {
    ENTITY_TYPE_ROCKET,     /* type id */
    aim,                    /* activation callback */
    0,                      /* collision callback */
    0,                      /* interval */
    1000,                   /* energy */
    1,                      /* health */
    0,                      /* length */
    1,                      /* mass */
    16,                     /* radius */
    {500,20},               /* acceleration */
    {20,20},                /* brake */
    1,                      /* rotation */
};

static void aim(Entity *rocket) {
    Entity *e;

    Vec     best_v;
    Entity *target = 0;

    entities_foreach(e) {
        if(rocket->player == e->player)
            continue;

        Vec dx = sub(e->x, rocket->x);

        /* desired direction of velocity */
        Vec v = normalize(rotate(dx, -rocket->phi));

        if(v.x < 0) continue; /* target is behind rocket */

        if(!target || fabs(v.y) < fabs(best_v.y)) {
            best_v = v;
            target = e;
        }
    }

    if(target) {
        Real acc   = 1 - fabs(best_v.y);
        Real speed = len(rocket->type->max_a) * acc * acc;
        Vec v = scale(best_v, speed);
        entity_accelerate_to(rocket, v);
        entity_rotate(rocket, best_v.y);
    } else {
        entity_accelerate_to(rocket, _0);
    }
}
