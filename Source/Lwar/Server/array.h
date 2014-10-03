#ifndef ARRAY_H
#define ARRAY_H

#include <stdbool.h>
#include <stddef.h>

typedef struct Array Array;

struct Array {
    char  *mem;
    bool   dynamic;
    size_t n;
    size_t size;
};

void  array_init(Array *a, void *p, size_t n, size_t size);
void  array_shutdown(Array *a);
void *array_at_check(Array *a, size_t i);

#define array_static(a,p) array_init(a, p, sizeof(p)/sizeof(*p), sizeof(*p));
#define array_at(a,t,i)     ((t*)array_at_check(a,i))[0]

#define array_foreach(a,p,t) \
    for(p = (t*)(a->mem); \
        p < (a)->mem + (a)->size * (a)->n; \
        p ++)

#endif
