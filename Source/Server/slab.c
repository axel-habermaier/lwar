#include <stddef.h>
#include <assert.h>

#include "list.h"
#include "slab.h"

#include "log.h"

static size_t slab_i(Slab *s, void *p) {
    assert(s->mem <= (char*)p);
    assert((char*)p < s->mem + s->n * s->size);
    assert(((char*)p -  s->mem) % s->size == 0);
    return (((char *)p) - s->mem) / s->size;
}

void slab_init(Slab *s, void *p, size_t n, size_t size,
               void (*ctor)(size_t, void *),
               void (*dtor)(size_t, void *))
{
    assert(p);
    assert(size != 0);

    s->mem  = (char*)p;
    s->n    = n;
    s->size = size;
    s->ctor = ctor;
    s->dtor = dtor;
    INIT_LIST_HEAD(&s->free);
    INIT_LIST_HEAD(&s->allocated);

    log_info("Initialized Slab at %x, %d * #%d", p, n, size);

    size_t i;
    for(i = 0; i < s->n; i++) {
        List *l = (List *)(s->mem + s->size*i);
        INIT_LIST_HEAD(l);
        list_add_tail(l, &s->free);
    }
}

void *slab_alloc(Slab *s) {
    if(list_empty(&s->free))
        return 0;
    List *l = s->free.next;
    list_move_tail(l, &s->allocated);
    if(s->ctor)
        s->ctor(slab_i(s,l), l);
    return l;
}

void slab_free_pred(Slab *s, int (*pred)(size_t , void *)) {
    List *l, *n;
    list_for_each_safe(l,n,&s->allocated) {
        if(pred(slab_i(s,l), l))
            slab_free(s, l);
    }
}

void slab_free(Slab *s, void *p) {
    List *l = (List *)p;
    list_move_tail(l, &s->free);
    if(s->dtor)
        s->dtor(slab_i(s,l), l);
}
