#ifndef VECTOR_H
#define VECTOR_H

#include "real.h"

#include <math.h>

typedef struct Vec Vec;

struct Vec {
    Real x,y;
};

static const Vec _0 = {0,0};

static inline Vec add(Vec v0, Vec v1) {
    Vec v = { v0.x+v1.x, v0.y+v1.y };
    return v;
}

static inline Vec sub(Vec v0, Vec v1) {
    Vec v = { v0.x-v1.x, v0.y-v1.y };
    return v;
}

static inline Vec scale(Vec v0, Real s) {
    Vec v = { v0.x*s, v0.y*s };
    return v;
}

static inline Real dot(Vec v0, Vec v1) {
    return v0.x*v1.x + v0.y*v1.y;
}

static inline Real dot_sq(Vec v) {
    return dot(v,v);
}

static inline Real len(Vec v) {
    return sqrt(dot_sq(v));
}

static inline Real dist(Vec v0, Vec v1) {
    return len(sub(v0,v1));
}

static inline Real dist_sq(Vec v0, Vec v1) {
    return dot_sq(sub(v0,v1));
}

static inline Vec normalize(Vec v) {
    Real s = len(v);
    Vec r = { v.x/s, v.y/s };
    return r;
}

/* ( cos phi   - sin phi )  ( x )
 * ( sin phi     cos phi )  ( y )
 */
static inline Vec rotate(Vec v, Real phi) {
    Real _cos = cos(phi);
    Real _sin = sin(phi);
    Vec r = { _cos * v.x - _sin * v.y,
              _sin * v.x + _cos * v.y };
    return r;
}

static inline Vec unit(Real phi) {
    Vec v = { cos(phi), sin(phi) };
    return v;
}

/* unit for phi + pi/2 */
static inline Vec ortho(Real phi) {
    Vec v = { -sin(phi), cos(phi) };
    return v;
}

static inline Real arctan(Vec v) {
    return atan2(v.y,v.x);
}

/* compute the projection and restriction of v onto b
 * b must be normalized
 * such that v = p + r and p || b
 */
static inline void project(Vec v, Vec b, Vec *p, Vec *r) {
    Real s = dot(v, b);
    *p = scale(b, s);
    *r = sub(v,*p);
}

#endif
