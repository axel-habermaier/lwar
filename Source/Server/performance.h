enum {
    TIMER_RECV,
    TIMER_SEND,
    TIMER_ENTITIES,
    TIMER_PHYSICS,
    TIMER_TOTAL,

    COUNTER_RECV,
    COUNTER_SEND,
};

void timer_start(unsigned int timer);
void timer_stop(unsigned int timer);
void counter_set(unsigned int counter, unsigned int value);

