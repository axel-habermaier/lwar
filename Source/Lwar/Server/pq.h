#ifndef PQ_H
#define PQ_H

#include <stdbool.h>
#include <stddef.h>

typedef struct PrioQueue PrioQueue;

struct PrioQueue {
    char  *mem;
    bool dynamic;
    size_t n,i;
    size_t size;
    int  (*cmp)(const void *, const void *);
};

void pq_init(PrioQueue *pq, void *p, size_t n, size_t size,
             int (*cmp)(const void *, const void *));
void pq_shutdown(PrioQueue *pq);

void *pq_alloc(PrioQueue *pq);
void *pq_alloc_check(PrioQueue *pq, size_t size);

/* invalidate all iterators */
void  pq_free_min(PrioQueue *pq);
void  pq_free_all(PrioQueue *pq);
void  pq_decreased(PrioQueue *pq, void *p);

#define pq_static(pq,p,c)   pq_init(pq, p, sizeof(p)/sizeof(*p), sizeof(*p), c);
#define pq_new(pq,t)        ((t*)pq_alloc_check(pq,sizeof(t)))
#define pq_min(pq,t)        ((t*)(pq)->mem)
#define pq_empty(pq)        ((pq)->i == 0)

#define pq_foreach(pq,p,t) \
    for (p = pq_min(pq,t); \
         !pq_empty(pq); \
         pq_free_min(pq)) /* note: no need to update p each iteration */

#endif
