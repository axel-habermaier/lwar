#include "array.h"

#include "debug.h"

#include <stdlib.h> /* malloc */

void *array_at_check(Array *a, size_t i) {
    assert(i < a->n);
    return a->mem + a->size * i;
}

void array_init(Array *a, void *p, size_t n, size_t size) {
    if(p) a->mem = (char*)p;
    else  a->mem = (char*)malloc(n * size);
    a->dynamic = !!p;
    a->n    = n;
    a->size = size;
}

void array_shutdown(Array *a) {
    if(a->dynamic) {
        free(a->mem);
    }
}
