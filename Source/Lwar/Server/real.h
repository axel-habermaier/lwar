#ifndef REAL_H
#define REAL_H

typedef float Real;

#ifndef max
#define max(n,m) ((n) < (m) ? (m) : (n))
#endif

#ifndef sgn
#define sgn(n) ((n) == 0 ? 0 : ((n) < 0 ? -1 : 1))
#endif

#ifndef min
#define min(n,m) ((n) > (m) ? (m) : (n))
#endif

Real rad(Real a);   /* radians of a */
unsigned short deg(Real a); /* degrees of a */
unsigned short deg100(Real a);

int roots(Real a, Real b, Real c, Real *x0, Real *x1);

#endif
