#include "pq.h"
#include "debug.h"

// #include <string.h>
#include <stdlib.h> /* malloc */

static void check_i(PrioQueue *pq, void *p) {
    assert(pq->mem <= (char*)p);
    assert((char*)p < pq->mem + pq->n * pq->size);
    assert(((char*)p -  pq->mem) % pq->size == 0);
}

static size_t get_i(PrioQueue *pq, void *p) {
    return (((char *)p) - pq->mem) / pq->size;
}

static size_t parent(size_t i) {
    return i/2;
}

static size_t left(size_t i) {
    return i*2;
}

static size_t right(size_t i) {
    return i*2+1;
}

/* borrowed from dietlibc lib/qsort.c, https://www.fefe.de */
static void exch(char* base,size_t size,size_t a,size_t b) {
  char* x=base+a*size;
  char* y=base+b*size;
  while (size) {
    char z=*x;
    *x=*y;
    *y=z;
    --size; ++x; ++y;
  }
}

/* borrowed from http://de.wikipedia.org/wiki/Binärer_Heap */

static void down(char *mem, size_t i, size_t n, size_t s,
                 int (*cmp)(const void *, const void *))
{
    for(;;) {
        int j = i;

        if(   left(i) < n
           && cmp((char*)mem + left(i)*s, (char*)mem + j*s) < 0)
        {
            j = left(i);
        }

        if(   right(i) < n
           && cmp((char*)mem + right(i)*s, (char*)mem + j*s) < 0)
        {
            j = right(i);
        }

        if(i == j) break;

        exch(mem,s,i,j);
        i = j;
    }
}

static void up(char *mem, size_t i, size_t n, size_t s,
               int (*cmp)(const void *, const void *))
{
    while(i>0 && cmp((char*)mem + i*s, (char*)mem + parent(i)*s) < 0)
    {
        exch((char*)mem,s,i,parent(i));
        i = parent(i);
    }
}

void pq_init(PrioQueue *pq, void *p, size_t n, size_t size,
             int (*cmp)(const void *, const void *))
{
    assert(cmp);
    assert(size != 0);

    if(p) pq->mem = (char*)p;
    else  pq->mem = (char*)calloc(n, size);
    pq->dynamic = !!p;

    pq->n    = n;
    pq->i    = 0;
    pq->size = size;
    pq->cmp  = cmp;
}

void pq_shutdown(PrioQueue *pq) {
    assert(pq->i == 0);

    if(pq->dynamic) {
        free(pq->mem);
    }
}

void *pq_alloc(PrioQueue *pq) {
    if(pq->i == pq->n)
        return 0;
    return pq->mem + pq->size * (pq->i++);
}

void *pq_alloc_check(PrioQueue *pq, size_t size) {
    assert(pq->size == size);
    return pq_alloc(pq);
}

void pq_free_min(PrioQueue *pq) {
    assert(pq->i != 0);
    /* swap first and last element */
    exch(pq->mem,pq->size,pq->i-1,0);
    /* push down first element */
    down(pq->mem,0,pq->i,pq->size,pq->cmp);
    pq->i --;
}

void  pq_free_all(PrioQueue *pq) {
    pq->i = 0;
}

void pq_decreased(PrioQueue *pq, void *p) {
    check_i(pq, p);
    size_t i = get_i(pq, p);
    /* push up the element */
    up(pq->mem,i,pq->i,pq->size,pq->cmp);
}
