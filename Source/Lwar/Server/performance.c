#include "performance.h"

#include "server_export.h"

static PerformanceCallbacks _callbacks;

void server_performance_callbacks(PerformanceCallbacks callbacks) {
    _callbacks = callbacks;
}

void timer_start(unsigned int timer) {
    if(_callbacks.start)
        _callbacks.start(timer);
}

void timer_stop(unsigned int timer) {
    if(_callbacks.stop)
        _callbacks.stop(timer);
}

void counter_set(unsigned int counter, unsigned int value) {
    if(_callbacks.counted)
        _callbacks.counted(counter, value);
}
