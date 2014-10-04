#ifndef POOL_H
#define POOL_H

#include <stdbool.h>
#include <stddef.h>

#include "list.h"

typedef struct Pool Pool;

struct Pool {
    char  *mem;
    bool   dynamic;
    size_t n,i;
    size_t size;
    void (*ctor)(size_t i, void *);
    void (*dtor)(size_t i, void *);
    List   allocated;
    List   free;
};

void  pool_init(Pool *pool, void *p, size_t n, size_t size,
                void (*ctor)(size_t, void *),
                void (*dtor)(size_t, void *));
void  pool_shutdown(Pool *pool);
void *pool_alloc(Pool *pool);
void *pool_alloc_check(Pool *pool, size_t size);
void  pool_free(Pool *pool, void *p);
void  pool_free_pred(Pool *pool, bool (*pred)(size_t, void *));

/* add/get/remove specific entries */
void  pool_add(Pool *pool, size_t i);
void *pool_get(Pool *pool, size_t i);
void *pool_remove(Pool *pool, size_t i);

#define pool_static(pool,p,c,d)    pool_init(pool, p, sizeof(p)/sizeof(*p), sizeof(*p), c, d);
#define pool_dynamic(pool,t,n,c,d) pool_init(pool, 0, n, sizeof(t), c, d);
#define pool_new(pool,t)           ((t*)pool_alloc_check(pool,sizeof(t)))
#define pool_at(pool,t,i)          ((i) < (pool)->n ? (t*)((pool)->mem + (pool)->size * (i)) : 0)

#define pool_nused(pool) ((pool)->i)

#define pool_foreach(pool,p,t) \
    for (p  = (t*)((pool)->allocated.next); \
         p != (t*)&(pool)->allocated; \
         p  = (t*)(((List *)p)->next))

#define pool_foreach_cont(pool,p,t) \
    for (p  = ((p) ? (p) : (t*)((pool)->allocated.next)); \
         p != (t*)&(pool)->allocated; \
         p  = (t*)(((List *)p)->next))

#endif
