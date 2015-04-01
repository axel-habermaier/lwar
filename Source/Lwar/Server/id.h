#ifndef ID_H
#define ID_H

#include <stdbool.h>
#include <stddef.h>
#include <stdint.h>

typedef struct Id Id;

struct Id {
    uint16_t gen;
    uint16_t n;
};

bool id_eq(Id id0, Id id1);

#define ID_ARG(id) (id).n, (id).gen

#endif
