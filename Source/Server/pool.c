#include <stddef.h>
#include <assert.h>

#include "list.h"
#include "pool.h"

#include "log.h"

static void check_i(Pool *s, void *p) {
    assert(s->mem <= (char*)p);
    assert((char*)p < s->mem + s->n * s->size);
    assert(((char*)p -  s->mem) % s->size == 0);
}

static size_t get_i(Pool *s, void *p) {
    return (((char *)p) - s->mem) / s->size;
}

void pool_init(Pool *s, void *p, size_t n, size_t size,
               void (*ctor)(size_t, void *),
               void (*dtor)(size_t, void *))
{
    assert(p);
    assert(size != 0);

    s->mem  = (char*)p;
    s->n    = n;
    s->i    = 0;
    s->size = size;
    s->ctor = ctor;
    s->dtor = dtor;
    INIT_LIST_HEAD(&s->free);
    INIT_LIST_HEAD(&s->allocated);

    log_debug("Initialized Pool at %x, %d * #%d", p, n, size);

    size_t i;
    for(i = 0; i < s->n; i++) {
        List *l = (List *)(s->mem + s->size*i);
        INIT_LIST_HEAD(l);
        list_add_tail(l, &s->free);
    }
}

void pool_add(Pool *s, size_t i) {
    List *l = (List *)s->mem + s->size * i;
    check_i(s, l);
    INIT_LIST_HEAD(l);
    list_add_tail(l, &s->free);
}

void *pool_get(Pool *s, size_t i) {
    List *l = (List *)s->mem + s->size * i;
    check_i(s, l);
    return l;
}

void *pool_remove(Pool *s, size_t i) {
    List *l = (List *)s->mem + s->size * i;
    check_i(s, l);
    list_del(l);
    return l;
}

void *pool_alloc(Pool *s) {
    if(list_empty(&s->free))
        return 0;
    List *l = s->free.next;

    check_i(s, l);
    list_move_tail(l, &s->allocated);
    if(s->ctor)
        s->ctor(get_i(s,l), l);
    s->i ++;

    return l;
}

void pool_free_pred(Pool *s, int (*pred)(size_t , void *)) {
    List *l, *n;
    list_for_each_safe(l,n,&s->allocated) {
        check_i(s, l);
        if(pred(get_i(s,l), l))
            pool_free(s, l);
    }
}

void pool_free(Pool *s, void *p) {
    List *l = (List *)p;
    check_i(s, l);
    list_move_tail(l, &s->free);
    if(s->dtor)
        s->dtor(get_i(s,l), l);
    s->i --;
}
