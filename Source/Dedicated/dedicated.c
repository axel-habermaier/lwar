#include <assert.h>
#include <time.h>
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#include "server_export.h"
#include "server.h"
#include "visualization.h"
#include "log.h"
#include "performance.h"

/* typedef unsigned long long Clock; */
static int visual,stats;
static Clock base,periodic;

static Clock clock_get();

enum {
    S  = 1000000,
    MS = 1000,
    MAX_MEASURES = 16,
    FRAME_MS       =  30,
    FRAME_INTERVAL =  FRAME_MS * MS,
    STAT_S         =  1,
    STAT_INTERVAL  =  STAT_S * S,
};

typedef struct Measure Measure;
struct Measure {
    Clock base, current;
};
static Measure measures[MAX_MEASURES];

/* time in microseconds since game start */
static Clock clock_get() {
    struct timespec tp;
    clock_gettime(CLOCK_MONOTONIC, &tp);
    Clock sec  = tp.tv_sec;
    Clock msec = tp.tv_nsec / 1000; /* microseconds */

    Clock now = sec * S + msec;
    if(!base) base = now;
    return now - base;
}

static void start(unsigned int timer)   { assert(timer   < MAX_MEASURES); measures[timer].base = clock_get(); }
static void stop(unsigned int timer)    { assert(timer   < MAX_MEASURES); measures[timer].current += (clock_get() - measures[timer].base); }
static void reset(unsigned int counter) { assert(counter < MAX_MEASURES); measures[counter].base = 0; measures[counter].current = 0; }
static void inc(unsigned int counter, unsigned int value) { assert(counter < MAX_MEASURES); measures[counter].current += value; }

static Clock get(unsigned int counter)  { assert(counter < MAX_MEASURES); return measures[counter].current; }

static void iputs(const char *msg) { fputs(msg,stdout); fputs("\n",stdout); fflush(stdout); }
static void eputs(const char *msg) { fputs(msg,stderr); fputs("\n",stderr); fflush(stderr); }
static void die  (const char *msg) { fputs(msg,stderr); fputs("\n",stderr); fflush(stderr); exit(1); };

static LogCallbacks log = { die, eputs, eputs, iputs, eputs, };
static PerformanceCallbacks perf = { start, stop, inc };

static void print_stats() {
    float tall  = 100.0 * (float)get(TIMER_TOTAL)    / STAT_INTERVAL;
    float trecv = 100.0 * (float)get(TIMER_RECV)     / STAT_INTERVAL;
    float tsend = 100.0 * (float)get(TIMER_SEND)     / STAT_INTERVAL;
    float tenty = 100.0 * (float)get(TIMER_ENTITIES) / STAT_INTERVAL;
    float tphys = 100.0 * (float)get(TIMER_PHYSICS)  / STAT_INTERVAL;

    unsigned int crecv = (unsigned int)get(COUNTER_RECV) / STAT_S;
    unsigned int csend = (unsigned int)get(COUNTER_SEND) / STAT_S;
    unsigned int crtx  = (unsigned int)get(COUNTER_RESEND) / STAT_S;

    printf("--- statistics ---\n");
    printf("cpu         %3.1f%%\n", tall);
    printf("  recv      %3.1f%%\n", trecv);
    printf("  send      %3.1f%%\n", tsend);
    printf("  phys      %3.1f%%\n", tphys);
    printf("  ai        %3.1f%%\n", tenty);
    printf("io (packets/s)\n");
    printf("  recv     %4d\n", crecv);
    printf("  send     %4d\n", csend);
    printf("  resend   %4d\n", crtx);
    printf("objects\n");
    printf("  client   %4ld\n", pool_nused(&server->clients));
    printf("  entities %4ld\n", pool_nused(&server->entities));
    printf("  items    %4ld\n", pool_nused(&server->items));
    printf("  queue    %4ld\n", pool_nused(&server->queue));
    printf("\n");
}

int main(int argc, char *argv[]) {
    int i;
    for(i=1; i<argc; i++) {
        if(!strcmp(argv[i], "-visual"))
            visual = 1;
        else if(!strcmp(argv[i], "-stats"))
            stats = 1;
    }

    server_log_callbacks(log);
    server_performance_callbacks(perf);

    if(!server_init()) return 1;

    if(visual) {
        if(!visualization_init()) return 1;
    }

    periodic = clock_get();

    for(;;) {
        Clock t0 = clock_get();

        start(TIMER_TOTAL);
        if(!server_update(t0/MS,0)) break;
        stop(TIMER_TOTAL);

        if(visual) {
            if(!visualization_update()) break;
        }
        Clock t1 = clock_get();

        if(t1 - t0 < FRAME_INTERVAL) {
            usleep(FRAME_INTERVAL - (t1 - t0));
        }

        if(stats && t1 - periodic > STAT_INTERVAL) {
            periodic = t1;
            print_stats();
            int i;
            for(i=0; i<MAX_MEASURES; i++)
                reset(i);
        }
    }

    if(visual) {
        visualization_shutdown();
    }
    server_shutdown();

    return 0;
}
