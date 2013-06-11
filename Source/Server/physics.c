#include <assert.h>
#define _USE_MATH_DEFINES // required for M_PI on VS2012
#include <math.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"

#include "vector.h"
#include "log.h"
#include "performance.h"

static size_t mod(long a,long b) {
    return (a % b + b) % b;
}

Real rad(Real a) {
    return a * M_PI / 180.0;
}

size_t deg(Real a) {
    Real d = a * 180.0 / M_PI;
    return mod(d, 360);
}

static Collision _collisions[MAX_COLLISIONS];

/* solve a x^2 + bx + c = 0 for x
 * and store solutions in x0,x1.
 * return the number of solutions */
int roots(Real a, Real b, Real c, Real *x0, Real *x1) {
    Real d = b*b - 4*a*c;
    if(d<0) return 0;
    if(x0) *x0 = (-b + sqrt(d)) / (2*a);
    if(x1) *x1 = (-b - sqrt(d)) / (2*a);
    return (d==0) ? 1 : 2;
}

/* predict velocity at time t, given current acceleration */
Vec physics_v(Vec v, Vec a, Time t) {
    return add(scale(a, t), v);
}

/* predict position at time t, given velocity at time t */
Vec physics_x(Vec x, Vec v, Time t) {
    return add(scale(v, t), x);
}

/* predict orientation at time t, given rotation */
Real physics_phi(Real phi, Real r, Time t) {
    return phi + r * t;
}

/* compute impact from old and new veloctiy */
Real impact(Vec v0, Vec v1) {
    return len(sub(v0,v1));
}

static void accelerate(Entity *e, Time t) {
    e->v = physics_v(e->v, e->a, t);
}

static void move(Entity *e, Time t) {
    assert(!e->parent);
    e->x   = physics_x(e->x, e->v, t);
    e->phi = physics_phi(e->phi, e->rot, t);
    e->remaining -= t;
}

static void move_remaining(Entity *e) {
    if(e->parent) {
        e->x   = add(e->parent->x, rotate(e->dx, e->parent->phi));
        e->v   = e->parent->v;
        e->phi = e->parent->phi + e->dphi;
    } else {
        move(e, e->remaining);
    }
}

static void notify(Collision *c) {
    entities_notify_collision(c);
    protocol_notify_collision(c);
}

/* compute possible collision point at time t in the future
 * return 1 if a collision occurs, 0 otherwise,
 * assuming that ei is at pos ei->x with speed ei->v at t=0 */
static bool collide(Entity *e0, Entity *e1, Time *t) {
    Real r = e0->radius + e1->radius;
    
    /* ignore acceleration,
       TODO: test whether this has an impact */
    Vec dx = sub(e0->x, e1->x);

    /* no collision if the entities intersect */
    if(dot_sq(dx) < r*r)
        return false;

    Vec dv = sub(e0->v, e1->v);

    Real t0,t1;

    /* dist(t) = | x0(t) - x1(t) |
               = (vt + x)^2 */
    Real a =   dot_sq(dv);
    Real b = 2*dot(dv,dx);
    Real c =   dot_sq(dx) - r*r;
    int  n =   roots(a,b,c, &t0,&t1);
    if(n && t) {
        /* prevent negative t (collisions in the past) */
        if(0 < t0 && (t0 < t1 || t1 < 0)) { *t = t0; return 1; }
        if(0 < t1 && (t1 < t0 || t0 < 0)) { *t = t1; return 1; }
    }
    return 0;
}

/* compute new velocities after a collision with respect to masses */
static void bounce(Entity *e0, Entity *e1) {
    /* masses */
    Real m0 = e0->mass;
    Real m1 = e1->mass;

    /* collision axis: assume that e0,e1 are already at the point of impact */
    Vec dx = normalize(sub(e0->x, e1->x));

    Vec v0,v1;
    Vec p0,p1;

    /* see http://en.wikipedia.org/wiki/Momentum#Application_to_collisions */
    project(e0->v, dx, &p0, &v0);
    project(e1->v, dx, &p1, &v1);

    if(!e1->bounces) {
        v0 = add(v0, scale(p0, -1));
        v0 = add(v0, scale(p1,  2));
    } else {
        v0 = add(v0, scale(p0,(m0-m1)/(m0+m1)));
        v0 = add(v0, scale(p1, (2*m1)/(m0+m1)));
    }

    if(!e0->bounces) {
        v1 = add(v1, scale(p1, -1));
        v1 = add(v1, scale(p0,  2));
    } else {
        v1 = add(v1, scale(p1,(m1-m0)/(m0+m1)));
        v1 = add(v1, scale(p0, (2*m0)/(m0+m1)));
    }

    if(e0->bounces) e0->v = v0;
    if(e1->bounces) e1->v = v1;
}

/* compute whether e0 and e1 intersect at the moment */
/*
static bool intersect(Entity *e0, Entity *e1) {
    Real r = e0->radius + e1->radius;
    return dist2(e0->x, e1->x) < r*r;
}
*/

static int collisions_cmp(const void *v0, const void *v1) {
    const Collision *c0 = (const Collision*)v0,
                    *c1 = (const Collision*)v1;
    return time_cmp(c0->t, c1->t);
}

static void find_collisions_for(Entity *e0, Time t0) {
    Entity *e1;

    entities_foreach(e1) {
        Time t1;
        if(!e1->collides) continue;

        if(   e0->id.n < e1->id.n    /* prevent to compare two entities twice  */
           && collide(e0,e1,&t1)     /* check for collision, d1 yields the time */
           && time_cmp(t1,t0) <= 0)  /* only consider if in current frame      */
        {
            Collision *c;
            c = pq_new(&server->collisions,Collision);
            c->t    = t1;
            c->e[0] = e0;
            c->e[1] = e1;
            pq_decreased(&server->collisions,c);
        }
    }
}

static void find_collisions(Time t0) {
    Entity *e0;

    entities_foreach(e0) {
        if(!e0->collides) continue;

        find_collisions_for(e0, t0);
    }
}

static void handle_collisions(Time t) {
    /* TODO: compute collisions by proximity,
     *       then recollide after each bounce,
             with close entities
     */

    Collision *c;
    /* TODO: remove redundancy by factoring out statements for one entity */

    collisions_foreach(c) {
        Time t = c->t;
        Entity *e0 = c->e[0];
        Entity *e1 = c->e[1];
        /* log_debug("collision of %d and %d at Δt %.3f", e0->id.n, e1->id.n, c->d.t); */

        /* move to collision point */
        move(e0, t);
        move(e1, t);

        Vec v0 = e0->v;
        Vec v1 = e1->v;

        /* compute new velocities */
        bounce(e0, e1);

        /* compute collision point */
        Real r0 = e0->radius;
        Real r1 = e1->radius;
        c->x = add(scale(e0->x, r0/(r0+r1)),
                   scale(e1->x, r1/(r0+r1)));

        /* compute impact as difference between new and old velocity */
        c->i[0] = impact(e0->v, v0);
        c->i[1] = impact(e1->v, v1);

        notify(c);

        /* remaining time of the entities will be spent in physics_move */
    }
}

void physics_update() {
    timer_start(TIMER_PHYSICS);

    Time t = time_delta();
    Entity *e;

    entities_foreach(e) {
        e->remaining = t;
        accelerate(e, t);
    }

    find_collisions(t);
    handle_collisions(t);

    entities_foreach(e) {
        move_remaining(e);
        e->a   = _0; /* reset acceleration */
        e->rot =  0; /* and rotation       */
    }

    timer_stop(TIMER_PHYSICS);
}

void physics_init() {
    pq_static(&server->collisions, _collisions, collisions_cmp);
}

void physics_cleanup() {
    pq_free_all(&server->collisions);
}
