/*
 * d(t)  = | f(t) |
 *         where f(t) = x1(t) - x2(t)
 *
 * d'(t) = .5 (h(t) ^ -.5) h'(t)
 *         where h(t) = f^2(t)
 * 
 *      d'(t)  = 0
 * iff  h'(t)  = 0
 *      h'(t)  = 2 f(t) g(t)
 *         where g(t) = v1(t) - v2(t)
 * iff  f(t) g(t) = 0
 */

/* analytically compute the extrema of the distance between two moving objects */
static int dist_extrema(Vec x0, Vec v0, Vec a0, Vec x1, Vec v1, Vec a1, float *d0, float *d1) {
    float a =       dot_sq(a0)
              +     dot_sq(a1)
              - 2 * dot(a0,a1);

    float b =   2 * dot(v0,a0)
              + 2 * dot(v1,a1)
              - 2 * dot(v0,a1)
              - 2 * dot(v1,a0)
              +     dot(x0,a0)
              +     dot(x1,a1)
              -     dot(x0,a1)
              -     dot(x1,a0);

    float c =       dot_sq(v0)
              +     dot_sq(v1)
              -     dot(v0,v1)
              +     dot(x0,v0)
              +     dot(x1,v1)
              -     dot(x0,v1)
              -     dot(x1,v0);

    return roots(a,b,c,d0,d1);
}

static int dist_minimum(Vec x0, Vec v0, Vec a0, Vec x1, Vec v1, Vec a1, float *d) {
    float d0=0,d1=0;
    int n = dist_extrema(x0,v0,a0,x1,v1,a1,&d0,&d1);
    if(d) *d = min(d0,d1);
    return n;
}

/* compute possible collision point v at time t in the future
 * return 1 if a collision occurs, 0 otherwise,
 * assuming that ei is at pos ei->x with speed ei->v at t=0 */
int physics_collide(Entity *e0, Entity *e1, Vec *v, clock_t *t) {
    Time d = {0}; /* initial guess */
    size_t  i;
    Vec x0 = e0->x, x1 = e1->x;
    Vec v0 = e0->v, v1 = e1->v;
    Vec a0 = e0->a, a1 = e1->a;

    /* first:  are e0 and e1 approaching each other?
     *         imprecise, since acceleration is not taken into account */
    int approaching = (dist2(add(x0,v0),add(x1,v1)) < dist2(x0,x1));
    if(!approaching) return 0;

    /* second: do boundaries neglecting the direction of movement intersect?
               assumes an upper bound of dt */
    Vec y0 = physics_predict(x0,v0,a0, MAX_DT);
    Vec y1 = physics_predict(x1,v1,a1, MAX_DT);

    Pos r = entity_radius(e0) + entity_radius(e1);

    /* radius of boundings squared */
    int b0 = dist2(y0,x0);
    int b1 = dist2(y1,x1);
    int intersecting = dist2(x0,x1) < b0 + b1 + r*r;
    if(!intersecting) return 0;

    /* third:  use Newton's method to approximate collision time */
    for(i=0; i<MAX_ITER; i++) {
        d.t = d.t - physics_q(x0,v0,a0,x1,v1,a1,d);
    }

    /* positions at collision time */
    Vec z0 = physics_predict(x0,v0,a0, d);
    Vec z1 = physics_predict(x1,v1,a1, d);

    int colliding = dist2(z0,z1) < r*r;
    
    return colliding;
}

/* dist(x0,x1,dt) - r / dist'(x0,x1,dt) */
static Pos physics_q(Vec x0, Vec v0, Vec a0, Vec x1, Vec v1, Vec a1, Time d) {
    Pos f = dot_sq(sub(physics_predict(x0,v0,a0,d),
                       physics_predict(x1,v1,a1,d)));
    return -2.0 * f;
}

