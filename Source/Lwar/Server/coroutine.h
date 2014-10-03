#ifndef COROUTINE_H
#define COROUTINE_H

/* credits to: http://www.chiark.greenend.org.uk/~sgtatham/coroutines.html */

typedef struct cr_t cr_t;

struct cr_t {
    unsigned int state;
};

#define cr_restart(s)   do { s->state  = 0; } while(0);
#define cr_running(s)   (s->state != 0)
#define cr_begin(s)     switch(s->state) { case 0:
#define cr_pause(s)     do { s->state = __LINE__; case __LINE__:; } while(0);
#define cr_yield(s,x)   do { s->state = __LINE__; return x; case __LINE__:; } while(0);
#define cr_end(s)       s->state = 0; }
#define cr_return(s,x)  s->state = 0; } return x;

#endif
