#include <time.h>
#include <string.h>

#include "server_export.h"
#include "visualization.h"
#include "log.h"

typedef unsigned long long Clock;
static int visual;

static Clock clock_get() {
    struct timespec tp;
    clock_gettime(CLOCK_MONOTONIC, &tp);
    time_t sec = tp.tv_sec;
    long  msec = tp.tv_nsec / 1000000;

    return sec * 1000.0 + msec;
}

int main(int argc, char *argv[]) {
    if(argc > 1 && !strcmp(argv[1], "-visual"))
        visual = 1;

    if(!server_init())        return 1;

    if(visual) {
        if(!visualization_init()) return 1;
    }

    for(;;) {
        Clock t = clock_get();
        if(!server_update(t,0))     break;
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
