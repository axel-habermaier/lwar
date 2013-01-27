/* simple implementation of priority queues
 *
 * pq is a pointer to an array of length n (n+1 for insert)
 *                             with elements of size s
 * pq_add receives the new element in a
 * pq_del and pq_add copy the removed element into a
 *
 * compar should have the same semantics as in qsort(3)
 */

void pq_add(void *a,
            void *pq, size_t n, size_t s,
            int (*compar)(const void *, const void *));

void pq_del(void *a, size_t i,
            void *pq, size_t n, size_t s,
            int (*compar)(const void *, const void *));

void pq_min(void *a,
            void *pq, size_t n, size_t s,
            int (*compar)(const void *, const void *));

void pq_peek(void *a, void *pq, size_t s);
