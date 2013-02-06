#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"
#include "log.h"

static void aim(Entity *e);
static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1);

static Clock debug_clock;

static EntityType _rocket = {
    ENTITY_TYPE_ROCKET,     /* type id */
    aim,                    /* activation callback */
    hit,                    /* collision callback  */
    16,                     /* radius  */
    1,                      /* mass    */
    {500,20},               /* acceleration */
    {20,20},                /* brake   */
    1,                      /* turn speed   */
    100,                    /* max health   */
};

EntityType *type_rocket = &_rocket;

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
        Pos acc   = 1 - fabs(best_v.y);
        Pos speed = len(rocket->type->max_a) * acc * acc;
        Vec v = scale(best_v, speed);
        entity_accelerate_to(rocket, v);
        entity_rotate(rocket, best_v.y);
    } else {
        entity_accelerate_to(rocket, _0);
    }
}

static void hit(Entity *rocket, Entity *e1, Vec v0, Vec v1) {
    rocket->health -= 0.1*physics_impact(rocket->v, v0);
    rocket->v = v0;
}
