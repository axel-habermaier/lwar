#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"
#include "vector.h"

EntityType type_ship = {
    ENTITY_TYPE_SHIP,       /* type id */
    0,                      /* activation callback */
    0,                      /* collision callback */
    0,                      /* interval */
    1000,                   /* energy */
    200,                    /* health */
    0,                      /* length */
    1,                      /* mass */
    32,                     /* radius */
    {200,200},              /* acceleration */
    {200,200},              /* brake */
    3,                      /* rotation */
};
