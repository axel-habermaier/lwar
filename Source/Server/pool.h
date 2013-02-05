typedef struct list_head List;
typedef struct Pool Pool;

struct Pool {
    char  *mem;
    size_t n,i;
    size_t size;
    void (*ctor)(size_t i, void *);
    void (*dtor)(size_t i, void *);
    List   allocated;
    List   free;
};

void  pool_init(Pool *s, void *p, size_t n, size_t size,
                void (*ctor)(size_t, void *),
                void (*dtor)(size_t, void *));
void *pool_alloc(Pool *s);
void  pool_free(Pool *s, void *p);
void  pool_free_pred(Pool *s, int (*pred)(size_t, void *));

/* add/get/remove specific entries */
void  pool_add(Pool *s, size_t i);
void *pool_get(Pool *s, size_t i);
void *pool_remove(Pool *s, size_t i);

#define pool_static(s,p,c,d) pool_init(s, p, sizeof(p)/sizeof(*p), sizeof(*p), c, d);
#define pool_new(s,t)        ((t*)pool_alloc(s))
#define pool_at(s,t,i)       ((i) < (s)->n ? (t*)((s)->mem + (s)->size * (i)) : 0)

#define pool_nused(s) ((s)->i)

#define pool_foreach(s,p,t) \
    for (p  = (t*)((s)->allocated.next); \
         p != (t*)&(s)->allocated; \
         p  = (t*)(((List *)p)->next))

#define pool_foreach_cont(s,p,t) \
    for (p  = ((p) ? (p) : (t*)((s)->allocated.next)); \
         p != (t*)&(s)->allocated; \
         p  = (t*)(((List *)p)->next))
