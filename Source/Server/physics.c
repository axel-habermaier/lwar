#define _USE_MATH_DEFINES // required for M_PI on VS2012
#include <math.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"

#include "vector.h"
#include "log.h"
#include "pq.h"

/* TODO: shared with visualization */
float rad(float a) {
    return a * M_PI / 180.0;
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

/* accelerate e by a */
void physics_acc(Entity *e, Vec a) {
    e->a = add(e->a, a);
}

/* accelerate e by a relative to orientation */
void physics_acc_rel(Entity *e, Vec a) {
    /* forward */
    Vec f = unit(rad(e->rot));
    /* right */
    Vec r = { f.y, -f.x };
    physics_acc(e, add(scale(f, a.y), scale(r, a.x)));
}

/* update position and velocity of e to time d */
static void move(Entity *e, Time d) {
    e->v = physics_v(e->v, e->a, d);
    e->x = physics_x(e->x, e->v, d);
    e->remaining = time_sub(e->remaining, d);
}

static void move_remaining(Entity *e) {
    if(e->remaining.t > 0) {
        e->v = physics_v(e->v, e->a, e->remaining);
        e->x = physics_x(e->x, e->v, e->remaining);
    }
    e->remaining.t = 0;
}

/* compute possible collision point v at time t in the future
 * return 1 if a collision occurs, 0 otherwise,
 * assuming that ei is at pos ei->x with speed ei->v at t=0 */
static int collide(Entity *e0, Entity *e1, Time *d) {
    Vec x0 = e0->x, x1 = e1->x;
    Vec v0 = e0->v, v1 = e1->v;
    Pos r = entity_radius(e0) + entity_radius(e1);
    
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
    int   n = roots(a,b,c, &d0,&d1);
    if(n && d) {
        /* prevent negative d (collisions in the past) */
        if(0 < d0 && (d0 < d1 || d1 < 0)) { d->t = d0; return 1; }
        if(0 < d1 && (d1 < d0 || d0 < 0)) { d->t = d1; return 1; }
    }
    return 0;
}

/* compute new velocities after a collision with respect to masses */
static void bounce(Entity *e0, Entity *e1) {
    /* masses */
    Pos m0 = entity_mass(e0);
    Pos m1 = entity_mass(e1);
    
    /* normalized impulse, proportional to other mass */
    Pos i0 = m1 / (m0+m1);
    Pos i1 = m0 / (m0+m1);
    
    /* delta of position and velocity */
    Vec dx = sub(e0->x, e1->x);
    Vec dv = sub(e0->v, e1->v);
    
    /* weird projection magic follows */
    Pos k = 2.0 * dot(dx,dv)/dot_sq(dx);
    e0->v = sub(e0->v, scale(dx,i0 * k)); /* sub,add with respect to dx */
    e1->v = add(e1->v, scale(dx,i1 * k));
}

/* compute whether e0 and e1 intersect at the moment */
static int intersect(Entity *e0, Entity *e1) {
    Pos r = entity_radius(e0) + entity_radius(e1);
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

static void find_collisions(Time d) {
    Entity *e0,*e1;
    ncollisions = 0;

    entities_foreach(e0) {
        entities_foreach(e1) {
            Time e;
            if(   e0->id.n < e1->id.n  /* prevent to compare two entities twice  */
               && collide(e0,e1,&e)    /* check for collision, e yields the time */
               && time_cmp(e,d) <= 0)  /* only consider if in current frame      */
            {
                collision_insert(e0,e1,e);
            }
        }
    }
}

static void handle_collisions(Time d) {
    size_t i;
    for(i=0; i<ncollisions; i++) {
        Collision *c = &collisions[i];
        Entity *e0 = c->e0;
        Entity *e1 = c->e1;
        // log_info("collision of %d and %d at Δt %.3f", e0->id.n, e1->id.n, c->d.t);

        /* move to collision point */
        move(e0, c->d);
        move(e1, c->d);

        bounce(e0, e1);

        /* remaining time of the entities will be spent in physics_move */
    }
}

static void physics_move(Time d) {
    Entity *e;
    entities_foreach(e) {
        move_remaining(e);
        e->a = _0; /* reset acceleration */

        /* TODO: circular world hack */
        if(e->x.x < -12)
            e->x.x += 24;

        if(e->x.x >  12)
            e->x.x -= 24;

        if(e->x.y < -10)
            e->x.y += 20;

        if(e->x.y >  10)
            e->x.y -= 20;
    }
}

void physics_update() {
    Time d = to_time(clock_delta());
    Entity *e;
    entities_foreach(e)
        e->remaining = d;
    find_collisions(d);
    handle_collisions(d);
    physics_move(d);
}
