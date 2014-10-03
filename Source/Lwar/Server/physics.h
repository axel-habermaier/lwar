#ifndef PHYSICS_H
#define PHYSICS_H

#include "time.h"
#include "vector.h"

struct Collision {
    Time t;
    Entity *e[2];
    Real    i[2];
    Vec x;
};

void physics_init();
void physics_cleanup();
void physics_update();

#endif
