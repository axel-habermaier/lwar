#include <assert.h>
#include <math.h>
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

/* rotate e by r in [-1..1] */
void entity_rotate(Entity *e, Real r) {
    e->rot += r * e->type->max_rot;
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

void entity_hit(Entity *e, Real damage, Player *k) {
    Player *v = e->player;

    /* prevent multiple kills of the same entity */
    if(   e->health > 0 && e->health <= damage
       && e == v->ship.entity)
    {
        /* TODO: move somewhere else? */
        k->kills ++;
        v->deaths ++;

        protocol_notify_kill(k, v);
    }

    e->health -= damage;
}

static void notify(Entity *e) {
    player_notify_entity(e);
    protocol_notify_entity(e);
}

static void act(Entity *e) {
    e->age += clock_delta();

    if(clock_periodic_active(&e->periodic, e->interval, e->active)) {
        if(e->health > 0 && e->type->act)
            e->type->act(e);
    }

    if(e->health <= 0)
        entity_remove(e);
}

void entities_notify_collision(Collision *c) {
    Entity *e0 = c->e[0];
    Entity *e1 = c->e[1];
    Real i0 = c->i[0];
    Real i1 = c->i[1];
    EntityType *t0 = e0->type;
    EntityType *t1 = e1->type;
    if(t0->collide) t0->collide(e0, e1, i0);
    if(t1->collide) t1->collide(e1, e0, i1);
}

void entities_update() {
    timer_start(TIMER_ENTITIES);

    Entity *e;
    entities_foreach(e) {
        act(e);
    }

    timer_stop(TIMER_ENTITIES);
}

static void entity_ctor(size_t i, void *p) {
    Entity *e = (Entity*)p;
    e->id.n = i;
    e->dead = 0;
    e->age  = 0;
    e->parent = 0;
    INIT_LIST_HEAD(&e->_u);
    INIT_LIST_HEAD(&e->children);
    INIT_LIST_HEAD(&e->siblings);
}

static void entity_dtor(size_t i, void *p) {
    Entity *e = (Entity*)p;

    list_del(&e->siblings);
    e->id.gen ++;
}

static bool entity_check_obsolete(size_t i, void *p) {
    Entity *e = (Entity*)p;
    return e->dead;
}

static void entity_set_type(Entity *e, EntityType *t) {
    e->type = t;
    Format *f = t->format;
    if(f) {
        list_add_tail(&e->_u, &f->all);
        f->n ++;
    }
}

static void entity_unset_type(Entity *e) {
    Format *f = e->type->format;
    if(f) {
        list_del(&e->_u);
        f->n --;
    }
}

Entity *entity_create(EntityType *t, Player *p, Vec x, Vec v) {
    assert(t);
    assert(p);
    Entity *e = pool_new(&server->entities, Entity);
    assert(e);

    entity_set_type(e, t);

    e->player = p;
    e->x      = x;
    e->v      = v;
    e->phi    = 0;
    e->rot    = 0;
    e->active = 0;
    e->periodic = 0;
    e->interval = t->init_interval;
    e->energy = t->init_energy;
    e->health = t->init_health;
    e->len    = t->init_len;
    e->mass   = t->init_mass;
    e->radius = t->init_radius;
    e->collides = (e->radius > 0);   /* TODO: this is a hacky-heuristics */
    e->bounces  = (e->mass < 1000);
    notify(e);
    log_debug("+ entity %d (%s), pos = (%.1f,%.1f) v = (%.1f,%.1f)", e->id.n, e->type->name, e->x.x, e->x.y, e->v.x, e->v.y);
    return e;
}

void entity_remove(Entity *e) {
    Entity *c;

    if(e) {
        assert(!e->dead);
        e->dead = 1;
        notify(e);
        log_debug("- entity %d (%s)", e->id.n, e->type->name);
        children_foreach(e,c)
            entity_remove(c);
            
        entity_unset_type(e);
    }
}

void entity_attach(Entity *e, Entity *c, Vec dx, Real dphi) {
    assert(list_empty(&c->siblings));
    list_add_tail(&c->siblings, &e->children);
    c->collides = 0;
    c->bounces  = 0;
    c->parent   = e;
    c->dx       = dx;
    c->dphi     = dphi;
}

void entities_init() {
    pool_static(&server->entities, _entities, entity_ctor, entity_dtor);
}

void entities_cleanup() {
    pool_free_pred(&server->entities, entity_check_obsolete);
}
