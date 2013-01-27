/* credits to: http://www.chiark.greenend.org.uk/~sgtatham/coroutines.html */
#define cr_begin()    static int __state = 0; switch(__state) { case 0:
#define cr_yield(x)   do { __state = __LINE__; return x; case __LINE__:; } while(0);
#define cr_end(x)     __state = 0; }
#define cr_return(x)  __state = 0; } return x;
