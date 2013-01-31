#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "rules.h"

static EntityType _bullet = { .1, .1, {0,0}, 10, 0, 0 };
EntityType *type_bullet = &_bullet;
