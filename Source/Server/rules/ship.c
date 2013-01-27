#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void shoot(Entity *e);

static EntityType _ship = { 1, 1, {0,0}, 200, shoot };
EntityType *type_ship = &_ship;

static void shoot(Entity *e) {
    /* Vec f = unit(rad(e->rot)); */
    Vec f = unit(rad(rand()%360));
    Vec x = add(e->x, scale(f, 2*entity_radius(e)));
    Vec v = add(e->v, scale(f, 4)); /* 1 = initial speed */
    EntityType *bullet = entity_type_get(ENTITY_TYPE_BULLET);
    Entity *b = entity_create(bullet,x,v);
}
