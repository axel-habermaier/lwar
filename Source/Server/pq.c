#include <string.h>
#include <stdint.h>

#include "pq.h"

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

/* borrowed from http://de.wikipedia.org/wiki/Bin%C3%A4rer_Heap */

static void heapify(char *pq, size_t i, size_t n, size_t s,
                    int (*compar)(const void *, const void *))
{
    for(;;) {
        int j = i;

        if(   left(i) < n
           && compar((char*)pq + left(i)*s, (char*)pq + j*s) < 0)
        {
            j = left(i);
        }

        if(   right(i) < n
           && compar((char*)pq + right(i)*s, (char*)pq + j*s) < 0)
        {
            j = right(i);
        }

        if(i == j) break;

        exch(pq,s,i,j);
        i = j;
    }
}

void pq_add(void *a,
            void *pq, size_t n, size_t s,
            int (*compar)(const void *, const void *))
{
    size_t i=n;
    memcpy((char*)pq + i*s,a,s);
    while(i>0 && compar((char*)pq + i*s, (char*)pq + parent(i)*s) < 0)
    {
        exch((char*)pq,s,i,parent(i));
        i = parent(i);
    }
}

void pq_del(void *a, size_t i,
            void *pq, size_t n, size_t s,
            int (*compar)(const void *, const void *))
{
    size_t l=n-1;
    memcpy(a,(char*)pq + i*s,s);
    exch((char*)pq,s,l,i);
    heapify((char*)pq,i,n,s,compar);
}

void pq_min(void *a,
            void *pq, size_t n, size_t s,
            int (*compar)(const void *, const void *))
{
    pq_del(a,0,pq,n,s,compar);
}

void pq_peek(void *a, void *pq, size_t s)
{
    memcpy(a,pq,s);
}
