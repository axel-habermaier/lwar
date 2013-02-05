#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void shoot(Entity *e);
static void hit(Entity *e0, Entity *e1, Vec v0, Vec v1);

static EntityType _ship = {
    ENTITY_TYPE_SHIP,   /* type id */
    32,                 /* radius  */
    1,                  /* mass    */
    {200,200},          /* acceleration */
    {200,200},          /* brake   */
    3,                  /* turn speed   */
    200,                /* max health   */
    300,                /* activation I */
    shoot,              /* activation callback */
    hit,                /* collision callback  */
};

EntityType *type_ship = &_ship;

static void shoot(Entity *ship) {
    EntityType *bullet = entity_type_get(ENTITY_TYPE_BULLET);

    Vec f = unit(ship->phi);
    Vec x = add(ship->x, scale(f, ship->type->radius+bullet->radius*2));
    Vec v = add(ship->v, scale(f, bullet->max_a.y)); /* initial speed */
    Entity *b = entity_create(bullet,ship->player,x,v);
    b->active = 1;
}

static void hit(Entity *ship, Entity *e1, Vec v0, Vec v1) {
    ship->health -= 0.1*physics_impact(ship->v, v0);
    ship->v = v0; // add(ship->v, v0);
}
