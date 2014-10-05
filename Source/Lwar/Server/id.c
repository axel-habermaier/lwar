#include "id.h"

#include "uint.h"

bool id_eq(Id id0, Id id1) {
    return    id0.n   == id1.n
           && id0.gen == id1.gen;
}

