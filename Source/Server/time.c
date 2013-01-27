#include <stddef.h>
#include <stdint.h>

#include "server.h"

static const Pos TIME_SCALE = 1000;

Clock to_clock(Time d) {
    return d.t * TIME_SCALE;
}

Time to_time(Clock t) {
    Time d = {t / TIME_SCALE};
    return d;
}

int time_cmp(Time d0, Time d1) {
    if(d0.t == d1.t) return  0;
    if(d0.t <  d1.t) return -1;
    else             return  1;
}

Time time_add(Time d0, Time d1) {
    Time d = {d0.t + d1.t};
    return d;
}

Time time_sub(Time d0, Time d1) {
    Time d = {d0.t - d1.t};
    return d;
}

void time_update(Clock t) {
    server->prev_time = server->cur_time;
    server->cur_time  = t;
}

Clock clock_delta() {
    return server->cur_time - server->prev_time;
}

/* process periodic clock that ticks with interval i in ms
 * i should be larger than frame times
 * return true i time has passed since last tick
 * and resets the clock
 */
int clock_periodic(Clock *t, Clock i) {
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
