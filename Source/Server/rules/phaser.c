#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

static void shoot(Entity *phaser);

EntityType type_phaser = {
    ENTITY_TYPE_PHASER,     /* type id */
    shoot,                  /* activation callback */
    0,                      /* collision callback  */
    0,                      /* interval */
    1000,                   /* energy */
    1,                      /* health */
    0,                      /* length */
    0,                      /* mass */
    0,                      /* radius */
    {0,0},                  /* acceleration */
    {0,0},                  /* brake */
    0,                      /* rotation */
};

static void shoot(Entity *phaser) {
    /*
    if(phaser->energy <= 0)
        return;

    phaser->energy --;
    */

    if(!list_empty(&phaser->children))
        return;

    Vec f = unit(phaser->phi);
    Vec x = add(phaser->x, scale(f, phaser->radius));
    Vec v = _0;

    Entity *ray = entity_create(&type_ray,phaser->player,x,v);
    entity_attach(phaser, ray);
    ray->active = 1;
}

