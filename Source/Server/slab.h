typedef struct list_head List;
typedef struct Slab Slab;

struct Slab {
    char  *mem;
    size_t n;
    size_t size;
    void (*ctor)(size_t i, void *);
    void (*dtor)(size_t i, void *);
    List   allocated;
    List   free;
};

void  slab_init(Slab *s, void *p, size_t n, size_t size,
                void (*ctor)(size_t, void *),
                void (*dtor)(size_t, void *));
void *slab_alloc(Slab *s);
void  slab_free(Slab *s, void *p);
void  slab_free_pred(Slab *s, int (*pred)(size_t, void *));

#define slab_static(s,p,c,d) slab_init(s, p, sizeof(p)/sizeof(*p), sizeof(*p), c, d);
#define slab_new(s,t)        ((t*)slab_alloc(s))
#define slab_at(s,t,i)       ((i) < (s)->n ? (t*)((s)->mem + (s)->size * (i)) : 0)

#define slab_foreach(s,p,t) \
    for (p  = (t*)((s)->allocated.next); \
         p != (t*)&(s)->allocated; \
         p  = (t*)(((List *)p)->next))

#define slab_foreach_cont(s,p,t) \
    for (p  = ((p) ? (p) : (t*)((s)->allocated.next)); \
         p != (t*)&(s)->allocated; \
         p  = (t*)(((List *)p)->next))
