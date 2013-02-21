#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void decay(Entity *e);

EntityType type_bullet = {
    ENTITY_TYPE_BULLET,     /* type id */
    decay,                  /* activation callback */
    0,                      /* collision callback  */
    0,                      /* interval */
    0,                      /* energy */
    100,                    /* health */
    0,                      /* length */
    .1,                     /* mass */
    8,                      /* radius */
    {0,500},                /* acceleration */
    {0,0},                  /* brake */
    0,                      /* rotation */
};

static void decay(Entity *e) {
    if(e->age > 5000)
        e->health = 0;
}
