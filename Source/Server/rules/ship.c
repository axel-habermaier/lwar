#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1);

static EntityType _ship = {
    ENTITY_TYPE_SHIP,   /* type id */
    0,                  /* activation callback */
    hit,                /* collision callback  */
    32,                 /* radius  */
    1,                  /* mass    */
    {200,200},          /* acceleration */
    {200,200},          /* brake   */
    3,                  /* turn speed   */
    200,                /* max health   */
};

EntityType *type_ship = &_ship;

static void hit(Entity *ship, Entity *e1, Vec v0, Vec v1) {
    ship->health -= 0.1*physics_impact(ship->v, v0);
    ship->v = v0; // add(ship->v, v0);
}
