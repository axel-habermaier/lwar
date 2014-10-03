#include "real.h"

#define _USE_MATH_DEFINES // required for M_PI on VS2012
#include <math.h>

static long mod(long a, long b) {
    return (a % b + b) % b;
}

Real rad(Real a) {
    return a * M_PI / 180.0;
}

unsigned short deg(Real a) {
    Real d = a * 180.0 / M_PI;
    return mod(d, 360);
}

unsigned short deg100(Real a){
	Real d = a * 180.0 / M_PI;
	return mod(d * 100, 360 * 100);
}

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

