#include <assert.h>
#ifndef _MSC_VER
#include <stdbool.h>
#endif
#include <stddef.h>
#include <stdlib.h>

#include "list.h"
#include "pool.h"

static void check_i(Pool *pool, void *p) {
    assert(pool->mem <= (char*)p);
    assert((char*)p < pool->mem + pool->n * pool->size);
    assert(((char*)p -  pool->mem) % pool->size == 0);
}

static size_t get_i(Pool *pool, void *p) {
    return (((char *)p) - pool->mem) / pool->size;
}

void pool_init(Pool *pool, void *p, size_t n, size_t size,
               void (*ctor)(size_t, void *),
               void (*dtor)(size_t, void *))
{
    assert(p);
    assert(size != 0);

    if(p) pool->mem = (char*)p;
    else  pool->mem = (char*)malloc(n * size);
    pool->dynamic = !!p;

    pool->n    = n;
    pool->i    = 0;
    pool->size = size;
    pool->ctor = ctor;
    pool->dtor = dtor;
    INIT_LIST_HEAD(&pool->free);
    INIT_LIST_HEAD(&pool->allocated);

    size_t i;
    for(i = 0; i < pool->n; i++) {
        List *l = (List *)(pool->mem + pool->size*i);
        INIT_LIST_HEAD(l);
        list_add_tail(l, &pool->free);
    }
}

void pool_shutdown(Pool *pool) {
    assert(pool->i == 0);

    if(pool->dynamic) {
        free(pool->mem);
    }
}

void pool_add(Pool *pool, size_t i) {
    List *l = (List *)pool->mem + pool->size * i;
    check_i(pool, l);
    INIT_LIST_HEAD(l);
    list_add_tail(l, &pool->free);
}

void *pool_get(Pool *pool, size_t i) {
    List *l = (List *)pool->mem + pool->size * i;
    check_i(pool, l);
    return l;
}

void *pool_remove(Pool *pool, size_t i) {
    List *l = (List *)pool->mem + pool->size * i;
    check_i(pool, l);
    list_del(l);
    return l;
}

void *pool_alloc_check(Pool *pool, size_t size) {
    assert(pool->size == size);
    return pool_alloc(pool);
}


void *pool_alloc(Pool *pool) {
    if(list_empty(&pool->free))
        return 0;
    List *l = pool->free.next;

    check_i(pool, l);
    list_move_tail(l, &pool->allocated);
    if(pool->ctor)
        pool->ctor(get_i(pool,l), l);
    pool->i ++;

    return l;
}

void pool_free_pred(Pool *pool, bool (*pred)(size_t , void *)) {
    List *l, *n;
    list_for_each_safe(l,n,&pool->allocated) {
        check_i(pool, l);
        if(pred(get_i(pool,l), l))
            pool_free(pool, l);
    }
}

void pool_free(Pool *pool, void *p) {
    List *l = (List *)p;
    check_i(pool, l);
    list_move_tail(l, &pool->free);
    if(pool->dtor)
        pool->dtor(get_i(pool,l), l);
    pool->i --;
}
