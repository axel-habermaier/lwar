#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

void decay(Entity *self);
void gun_shoot(Entity *self);
void phaser_shoot(Entity *self);
void gravity(Entity *self);
void ray_act(Entity *self);
void aim(Entity *self);

EntityType type_bullet =
{
   ENTITY_TYPE_BULLET, 
   decay, 
   NULL, 
   0, 
   0, 
   100, 
   0, 
   0.1, 
   8, 
   { 0, 500 }, 
   { 0, 0 }, 
   0, 
};

EntityType type_gun =
{
   ENTITY_TYPE_GUN, 
   gun_shoot, 
   NULL, 
   300, 
   1000, 
   1, 
   0, 
   0, 
   0, 
   { 0, 0 }, 
   { 0, 0 }, 
   0, 
};

EntityType type_phaser =
{
   ENTITY_TYPE_PHASER, 
   phaser_shoot, 
   NULL, 
   0, 
   1000, 
   1, 
   0, 
   0, 
   0, 
   { 0, 0 }, 
   { 0, 0 }, 
   0, 
};

EntityType type_planet =
{
   ENTITY_TYPE_PLANET, 
   gravity, 
   NULL, 
   0, 
   0, 
   1, 
   0, 
   10000, 
   128, 
   { 0, 0 }, 
   { 0, 0 }, 
   0, 
};

EntityType type_ray =
{
   ENTITY_TYPE_RAY, 
   ray_act, 
   NULL, 
   0, 
   0, 
   1, 
   0, 
   0, 
   512, 
   { 0, 0 }, 
   { 0, 0 }, 
   0, 
};

EntityType type_rocket =
{
   ENTITY_TYPE_ROCKET, 
   aim, 
   NULL, 
   0, 
   1000, 
   1, 
   0, 
   1, 
   16, 
   { 500, 20 }, 
   { 20, 20 }, 
   1, 
};

EntityType type_ship =
{
   ENTITY_TYPE_SHIP, 
   NULL, 
   NULL, 
   0, 
   1000, 
   200, 
   0, 
   1, 
   32, 
   { 200, 200 }, 
   { 200, 200 }, 
   3, 
};

