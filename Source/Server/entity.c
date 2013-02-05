#include <math.h>
#include <assert.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"
#include "vector.h"
#include "log.h"
#include "performance.h"

static Entity _entities[MAX_ENTITIES];

/* accelerate e by absolute a */
void entity_push(Entity *e, Vec a) {
    e->a = add(e->a, a);
}

Vec apply_acc(Vec v, Vec a, Vec b) {
    v.x *= (v.x>0 ? a.x : b.x);
    v.y *= (v.y>0 ? a.y : b.y);
    return v;
}

/* accelerate forward by a.y, and to the right by a.x
 * using the entity's orientation
 */
void entity_accelerate(Entity *e, Vec a) {
    entity_push(e, rotate(apply_acc(a, e->type->max_a, e->type->max_b), e->phi));
}

/* try to reach exactly velocity v (relative to e)
 */
void entity_accelerate_to(Entity *e, Vec v) {
    /* rotate actual speed to the entity's orientation */
    Vec w = rotate(e->v, -e->phi);
    Vec dv = sub(v, w);
    // entity_push(e, rotate(dv, e->phi));
    Vec a  = { sgn(dv.x), sgn(dv.y) };
    entity_accelerate(e, a);
}

/* rotate e by r */
void entity_rotate(Entity *e, Pos r) {
    e->r += r * e->type->max_r;
}

static void notify(Entity *e) {
    player_notify_state(e);
    protocol_notify_state(e);
}

static void entity_action(Entity *e) {
    if(e->health < 0)
        entity_remove(e);

    if(!e->type->action)
        return;

    if(e->active) {
        if(clock_periodic(&e->activation_periodic,
                           e->type->activation_interval))
        {
            e->type->action(e);
        }
    } else {
        e->activation_periodic = 0;
    }
}

void entities_notify_collision(Entity *e0, Entity *e1, Vec v0, Vec v1) {
    EntityType *t0 = e0->type;
    EntityType *t1 = e1->type;
    if(t0->collision) t0->collision(e0, e1, v0, v1);
    if(t1->collision) t1->collision(e1, e0, v1, v0);
}

void entities_update() {
    timer_start(TIMER_ENTITIES);

    Entity *e;
    entities_foreach(e) {
        entity_action(e);
    }

    timer_stop(TIMER_ENTITIES);
}

static void entity_ctor(size_t i, void *p) {
    Entity *e = (Entity*)p;
    e->id.n = i;
}

static void entity_dtor(size_t i, void *p) {
    Entity *e = (Entity*)p;
    e->id.gen ++;
}

static int entity_check_obsolete(size_t i, void *p) {
    Entity *e = (Entity*)p;
    return e->dead;
}

Entity *entity_create(EntityType *t, Player *p, Vec x, Vec v) {
    assert(t);
    assert(p);
    Entity *e = pool_new(&server->entities, Entity);
    assert(e);
    e->player = p;
    e->type   = t;
    e->x      = x;
    e->v      = v;
    e->phi    = 0;
    e->r      = 0;
    e->health = t->max_health;
    e->dead   = 0;
    notify(e);
    log_debug("+ entity %d (%.1f,%.1f)", e->id.n, e->x.x, e->x.y);
    return e;
}

void entity_remove(Entity *e) {
    if(e) {
        e->dead = 1;
        notify(e);
        log_debug("- entity %d", e->id.n);
    }
}

void entities_init() {
    pool_static(&server->entities, _entities, entity_ctor, entity_dtor);
}

void entities_cleanup() {
    pool_free_pred(&server->entities, entity_check_obsolete);
}

EntityType *entity_type_get(size_t id) {
    if(id < MAX_TYPES)
        return server->types[id];
    else return 0;
}

/* entity type 0 is invalid, should map to NULL */
void entity_type_register(size_t id, EntityType *t) {
    if(0 < id && id < MAX_TYPES)
        server->types[id] = t;
}
