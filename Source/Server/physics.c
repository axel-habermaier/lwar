#define _USE_MATH_DEFINES // required for M_PI on VS2012
#include <math.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"

#include "vector.h"
#include "log.h"
#include "performance.h"
#include "pq.h"

static size_t mod(long a,long b) {
    return (a % b + b) % b;
}

Pos rad(Pos a) {
    return a * M_PI / 180.0;
}

size_t deg(Pos a) {
    Pos d = a * 180.0 / M_PI;
    return mod(d, 360);
}

typedef struct Collision Collision;
struct Collision {
    Time d;
    Entity *e0,*e1;
};

static size_t   ncollisions;
static Collision collisions[MAX_COLLISIONS];

/* solve a x^2 + bx + c = 0 for x
 * and store solutions in x0,x1.
 * return the number of solutions */
static int roots(Pos a, Pos b, Pos c, Pos *x0, Pos *x1) {
    Pos d = b*b - 4*a*c;
    if(d<0) return 0;
    if(x0) *x0 = (-b + sqrt(d)) / (2*a);
    if(x1) *x1 = (-b - sqrt(d)) / (2*a);
    return (d==0) ? 1 : 2;
}

/* predict velocity at time d, given current acceleration */
Vec physics_v(Vec v, Vec a, Time d) {
    return add(scale(a, d.t), v);
}

/* predict position at time d, given velocity at time d */
Vec physics_x(Vec x, Vec v, Time d) {
    return add(scale(v, d.t), x);
}

/* predict orientation at time d, given rotation */
Pos physics_phi(Pos phi, Pos r, Time d) {
    return phi + r * d.t;
}

/* compute impact from old and new veloctiy */
Pos physics_impact(Vec v0, Vec v1) {
    return len(sub(v0,v1));
}

/* update position and velocity of e to time d */
static void move(Entity *e, Time d) {
    e->v   = physics_v(e->v, e->a, d);
    e->x   = physics_x(e->x, e->v, d);
    e->phi = physics_phi(e->phi, e->r, d);
    e->remaining = time_sub(e->remaining, d);
}

static void move_remaining(Entity *e) {
    if(e->remaining.t > 0) {
        e->v   = physics_v(e->v, e->a, e->remaining);
        e->x   = physics_x(e->x, e->v, e->remaining);
        e->phi = physics_phi(e->phi, e->r, e->remaining);
    }
    e->remaining.t = 0;
}

/* compute possible collision point at time d in the future
 * return 1 if a collision occurs, 0 otherwise,
 * assuming that ei is at pos ei->x with speed ei->v at t=0 */
static int collide(Entity *e0, Entity *e1, Time *d) {
    Vec x0 = e0->x, x1 = e1->x;
    Vec v0 = e0->v, v1 = e1->v;
    Pos r = e0->type->radius + e1->type->radius;
    
    /* ignore acceleration,
       TODO: test whether this has an impact */
    Vec x = sub(x0, x1);

    /* no collision if the entities intersect */
    if(dot_sq(x) < r*r)
        return 0;

    Vec v = sub(v0, v1);

    Pos d0,d1;

    /* dist(t) = | x0(t) - x1(t) |
               = (vt + x)^2 */
    Pos a =   dot_sq(v);
    Pos b = 2*dot(v,x);
    Pos c =   dot_sq(x) - r*r;
    int n =   roots(a,b,c, &d0,&d1);
    if(n && d) {
        /* prevent negative d (collisions in the past) */
        if(0 < d0 && (d0 < d1 || d1 < 0)) { d->t = d0; return 1; }
        if(0 < d1 && (d1 < d0 || d0 < 0)) { d->t = d1; return 1; }
    }
    return 0;
}

/* compute new velocities after a collision with respect to masses */
static void bounce(Entity *e0, Entity *e1, Vec *v0, Vec *v1) {
    /* masses */
    Pos m0 = e0->type->mass;
    Pos m1 = e1->type->mass;

    /* collision axis */
    Vec dx = normalize(sub(e0->x, e1->x));

    Vec p0,p1;

    /* see http://en.wikipedia.org/wiki/Momentum#Application_to_collisions */
    project(e0->v, dx, &p0, v0);
    project(e1->v, dx, &p1, v1);

    *v0 = add(*v0, scale(p0,(m0-m1)/(m0+m1)));
    *v0 = add(*v0, scale(p1, (2*m1)/(m0+m1)));

    *v1 = add(*v1, scale(p1,(m1-m0)/(m0+m1)));
    *v1 = add(*v1, scale(p0, (2*m0)/(m0+m1)));
}

/* compute whether e0 and e1 intersect at the moment */
static int intersect(Entity *e0, Entity *e1) {
    Pos r = e0->type->radius + e1->type->radius;
    return dist2(e0->x, e1->x) < r*r;
}

static int collisions_cmp(const void *v0, const void *v1) {
    const Collision *c0 = (const Collision*)v0,
                    *c1 = (const Collision*)v1;
    return time_cmp(c0->d, c1->d);
}

/* add new collision, or replace some late one */
static void collision_insert(Entity *e0, Entity *e1, Time d) {
    Collision c = {d,e0,e1};

    pq_add(&c,collisions,ncollisions,sizeof(Collision),collisions_cmp);

    if(ncollisions < MAX_COLLISIONS)
        ncollisions++;
}

static void find_collisions(Time d0) {
    Entity *e0,*e1;
    ncollisions = 0;

    entities_foreach(e0) {
        entities_foreach(e1) {
            Time d1;
            if(   e0->id.n < e1->id.n    /* prevent to compare two entities twice  */
               && collide(e0,e1,&d1)     /* check for collision, d1 yields the time */
               && time_cmp(d1,d0) <= 0)  /* only consider if in current frame      */
            {
                collision_insert(e0,e1,d1);
            }
        }
    }
}

static void handle_collisions(Time d) {
    size_t i;
    for(i=0; i<ncollisions; i++) {
        Collision *c = &collisions[i];
        Vec v0,v1;

        Time d = c->d;
        Entity *e0 = c->e0;
        Entity *e1 = c->e1;
        /* log_debug("collision of %d and %d at Δt %.3f", e0->id.n, e1->id.n, c->d.t); */

        /* move to collision point */
        move(e0, d);
        move(e1, d);

        /* compute force vectors */
        bounce(e0, e1, &v0, &v1);
        entities_notify_collision(e0, e1, v0, v1);

        /* compute collision point */
        Pos r0 = e0->type->radius;
        Pos r1 = e1->type->radius;
        Vec v = add(scale(e0->x, r0/(r0+r1)),
                    scale(e1->x, r1/(r0+r1)));
        protocol_notify_collision(e0, e1, v);

        /* remaining time of the entities will be spent in physics_move */
    }
}

static void physics_move(Time d) {
    Entity *e;
    entities_foreach(e) {
        move_remaining(e);
        e->a = _0; /* reset acceleration */
        e->r =  0; /* and rotation       */

       /* if(e->x.x < 0) e->x.x += 1000;
        if(e->x.x > 1000) e->x.x -= 1000;
        if(e->x.y < 0) e->x.y += 800;
        if(e->x.y > 800) e->x.y -= 800;*/

    }
}

void physics_update() {
    timer_start(TIMER_PHYSICS);

    Time d = to_time(clock_delta());
    Entity *e;
    entities_foreach(e)
        e->remaining = d;
    find_collisions(d);
    handle_collisions(d);
    physics_move(d);

    timer_stop(TIMER_PHYSICS);
}
