#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void shoot(Entity *gun);

EntityType type_gun = {
    ENTITY_TYPE_GUN,        /* type id */
    shoot,                  /* activation callback */
    0,                      /* collision callback  */
    300,                    /* interval */
    1000,                   /* energy */
    1,                      /* health */
    0,                      /* length */
    0,                      /* mass */
    0,                      /* radius */
    {0,0},                  /* acceleration */
    {0,0},                  /* brake */
    0,                      /* rotation */
};

static void shoot(Entity *gun) {
    if(gun->energy <= 0)
        return;

    gun->energy --;

    Vec f = unit(gun->phi);
    Vec x = add(gun->x, scale(f, gun->radius + type_bullet.init_radius*2));
    Vec v = add(gun->v, scale(f, type_bullet.max_a.y)); /* initial speed */

    entity_create(&type_bullet,gun->player,x,v);
}

