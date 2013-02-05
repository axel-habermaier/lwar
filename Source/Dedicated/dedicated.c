#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "server_export.h"
#include "visualization.h"
#include "log.h"

typedef unsigned long long Clock;
static int visual;
static Clock base;

static void iputs(const char *msg) { fputs(msg,stdout); fputs("\n",stdout); fflush(stdout); }
static void eputs(const char *msg) { fputs(msg,stderr); fputs("\n",stderr); fflush(stderr); }
static void die  (const char *msg) { fputs(msg,stderr); fputs("\n",stderr); fflush(stderr); exit(1); };

static LogCallbacks callbacks = { die, eputs, eputs, iputs, eputs, };

static Clock clock_get() {
    struct timespec tp;
    clock_gettime(CLOCK_MONOTONIC, &tp);
    time_t sec = tp.tv_sec;
    long  msec = tp.tv_nsec / 1000000;

    Clock now = sec * 1000.0 + msec;
    if(!base) base = now;
    return now - base;
}

int main(int argc, char *argv[]) {
    if(argc > 1 && !strcmp(argv[1], "-visual"))
        visual = 1;

    server_callbacks(callbacks);

    if(!server_init()) return 1;

    if(visual) {
        if(!visualization_init()) return 1;
    }

    for(;;) {
        Clock t = clock_get();
        if(!server_update(t,0)) break;
        if(visual) {
            if(!visualization_update()) break;
        }
    }

    if(visual) {
        visualization_shutdown();
    }
    server_shutdown();

    return 0;
}
