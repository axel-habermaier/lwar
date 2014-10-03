#ifndef ID_H
#define ID_H

#include <stdint.h>

typedef struct Id Id;

struct Id {
    uint16_t gen;
    uint16_t n;
};

bool id_eq(Id id0, Id id1);

size_t id_pack(char *out, Id id);
size_t id_unpack(const char *out, Id *id);


#endif
