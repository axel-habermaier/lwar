#ifndef TIME_H
#define TIME_H

#include "real.h"

#include <stdbool.h>

/* clock_t on unix */
/* measures discrete time steps in milliseconds */
typedef unsigned long long Clock;

/* measures time intervals,
 * used for example by the physics engine
 */
typedef Real Time;

Clock clock_delta();
bool  clock_periodic(Clock *t, Clock i);
bool  clock_periodic_active(Clock *t, Clock i, bool active);
Clock to_clock(Time t);
Time  to_time(Clock t);
Time  time_delta();
int   time_cmp(Time t0, Time t1);
void  time_update(Clock t);

#endif
