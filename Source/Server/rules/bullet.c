#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void decay(Entity *e);
static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1);

static EntityType _bullet = {
    ENTITY_TYPE_BULLET,     /* type id */
    decay,                  /* activation callback */
    hit,                    /* collision callback  */
    8,                      /* radius  */
    .1,                     /* mass    */
    {0,500},                /* acceleration */
    {0,0},                  /* brake */
    0,                      /* turn speed   */
    100,                    /* max health   */
};

EntityType *type_bullet = &_bullet;

static void decay(Entity *e) {
    //if(e->age > 5000)
        //e->health = 0;
}

static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1) {
    e0->health -= 0.05*physics_impact(e0->v, v0);
    e0->v = v0; // add(e0->v, v0);
}
