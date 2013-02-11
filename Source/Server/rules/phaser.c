#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void shoot(Entity *e, Item *phaser);

static ItemType _phaser = {
    ITEM_TYPE_PHASER,  /* type id */
    shoot,          /* activation callback */
    0,            /* activation interval */
    ~0,             /* initial ammunition */
};

ItemType *type_phaser = &_phaser;

static void shoot(Entity *e, Item *phaser) {
    if(phaser->ammunition <= 0)
        return;

    phaser->ammunition --;

    EntityType *ray = entity_type_get(ENTITY_TYPE_RAY);

    Vec f = unit(e->phi);
    Vec x = add(e->x, scale(f, e->type->radius + ray->radius*2));
    Vec v = add(e->v, scale(f, ray->max_a.y)); /* initial speed */

    entity_create(ray,e->player,x,v);
}


