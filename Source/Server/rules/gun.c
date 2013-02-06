#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void shoot(Entity *e, Item *gun);

static ItemType _gun = {
    ITEM_TYPE_GUN,  /* type id */
    shoot,          /* activation callback */
    300,            /* activation interval */
    ~0,             /* initial ammunition */
};

ItemType *type_gun = &_gun;

static void shoot(Entity *e, Item *gun) {
    if(gun->ammunition <= 0)
        return;

    gun->ammunition --;

    EntityType *bullet = entity_type_get(ENTITY_TYPE_BULLET);

    Vec f = unit(e->phi);
    Vec x = add(e->x, scale(f, e->type->radius + bullet->radius*2));
    Vec v = add(e->v, scale(f, bullet->max_a.y)); /* initial speed */

    entity_create(bullet,e->player,x,v);
}

