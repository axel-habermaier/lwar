#ifndef DEBUG_H
#define DEBUG_H

#include <setjmp.h>
#include <stdbool.h>
#include <stddef.h>

#ifdef DEBUG
#define assert(test) \
    debug_assert((test), #test, __FILE__, __LINE__)
#else
#define assert(test)
#endif

void debug_assert(bool test, const char *what, const char *file, size_t line);

typedef struct FailedAssertion FailedAssertion;

struct FailedAssertion {
    const char *what;
    const char *file;
    size_t line;
};

extern jmp_buf assert_handler;
extern FailedAssertion failed_assertion;

#endif
