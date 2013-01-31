static inline Vec add(Vec v0, Vec v1) {
    Vec v = { v0.x+v1.x, v0.y+v1.y };
    return v;
}

static inline Vec sub(Vec v0, Vec v1) {
    Vec v = { v0.x-v1.x, v0.y-v1.y };
    return v;
}

static inline Vec scale(Vec v0, Pos s) {
    Vec v = { v0.x*s, v0.y*s };
    return v;
}

static inline Pos dot(Vec v0, Vec v1) {
    return v0.x*v1.x + v0.y*v1.y;
}

static inline Pos dot_sq(Vec v) {
    return dot(v,v);
}

static inline Pos len(Vec v) {
    return sqrt(dot_sq(v));
}

static inline Pos dist(Vec v0, Vec v1) {
    return len(sub(v0,v1));
}

static inline Pos dist2(Vec v0, Vec v1) {
    return dot_sq(sub(v0,v1));
}

static inline Vec normalized(Vec v) {
    Pos s = len(v);
    Vec r = { v.x/s, v.y/s };
    return r;
}

static inline Vec unit(Pos phi) {
    Vec v = { cos(phi), sin(phi) };
    return v;
}

/* computes the projection and restriction of v onto b
 * b must be normalized
 * such that v = p + r and p || b
 */
static inline void project(Vec v, Vec b, Vec *p, Vec *r) {
    Pos s = dot(v, b);
    *p = scale(b, s);
    *r = sub(v,*p);
}
