#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

void hi(Entity *self);
void decay(Entity *self);
void gnah(Entity *self, Entity *other);

EntityType type_a =
{
   ENTITY_TYPE_SHIP, 
   hi, 
   gnah, 
   3, 
   1.04, 
   3.14, 
   1, 
   10, 
   15, 
   { 1, 2 }, 
   { 2, 3.43 }, 
   17, 
};

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

