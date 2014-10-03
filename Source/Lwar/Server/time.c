#include <stddef.h>
#include <stdint.h>

#include "server.h"
#include "server_export.h"

static const Real TIME_SCALE = 1000;

Clock to_clock(Time t) {
    return t * TIME_SCALE;
}

Time to_time(Clock t) {
    return t / TIME_SCALE;
}

int time_cmp(Time t0, Time t1) {
    if(t0 == t1) return  0;
    if(t0 <  t1) return -1;
    else         return  1;
}

void time_update(Clock t) {
    server->prev_clock = server->cur_clock;
    server->cur_clock  = t;
}

Clock clock_delta() {
    return server->cur_clock - server->prev_clock;
}

Time time_delta() {
    return to_time(clock_delta());
}

/* process periodic clock that ticks with interval i in ms
 * i should be larger than frame times
 * return true i time has passed since last tick
 * and resets the clock
 */
bool clock_periodic(Clock *t, Clock i) {
    Clock d = clock_delta();
    if(*t < d) {
        if(*t + i < d) *t = 0; /* prevent underflow */
        else           *t = *t + i - d;
        return 1;
    } else {
        *t -= d;
        return 0;
    }
}

/* process events that are activated by flag active,
 * if *t != 0 then continue running the timer * until it fires next,
 * to guarantuee that at least i time has passed,
 * then reset the timer to zero to allow immediate firing the next time the timer becomes active
 */
bool clock_periodic_active(Clock *t, Clock i, bool active) {
    if(*t || active) {
        if(clock_periodic(t, i)) {
            if(!active) *t = 0;
            return active;
        }
    }
    return 0;
}
