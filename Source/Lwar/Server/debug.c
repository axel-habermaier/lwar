#include "debug.h"

#include "pool.h"
#include "log.h"

jmp_buf assert_handler;
FailedAssertion failed_assertion;

static bool memchk(const void *p, char c, size_t n) {
    const char *s = (const char*)p;
    size_t i;
    for(i=0; i<n; i++) {
        if(s[i] != c)
            return false;
    }
    return true;
}

void debug_assert(bool test, const char *what, const char *file, size_t line) {
    if(test)
        return;

    failed_assertion.what = what;
    failed_assertion.file = file;
    failed_assertion.line = line;

    longjmp(assert_handler,1);
}

void debug_check_pool(Pool *pool) {
    List *l;

    size_t i=0;
    list_for_each(l,&pool->allocated) {
        i ++;
    }

    assert(pool->i == i);

    size_t n=i;
    list_for_each(l,&pool->free) {
        n ++;
        memchk(l+1, (char)0xFF, pool->size - sizeof(List));
    }

    assert(pool->n == n);
}

void debug_dump_pool(Pool *pool) {
    log_debug("%s pool %p\n", pool->dynamic ? "dynamic" : "static", pool->mem);
    log_debug("  allocated:  %zu (%lu%%)\n", pool->i, pool->i*100 / pool->n);
    log_debug("  capacity:   %zu", pool->n);
    log_debug("  item size:  %zu", pool->size);
}
