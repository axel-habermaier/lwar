#include <assert.h>
#include <stddef.h>

#include "array.h"

void *array_at_check(Array *a, size_t i) {
    assert(i < a->n);
    return a->mem + a->size * i;
}

void array_init(Array *a, void *p, size_t n, size_t size) {
    a->mem  = (char*)p;
    a->n    = n;
    a->size = size;
}
