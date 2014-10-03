#include "debug.h"

jmp_buf assert_handler;
FailedAssertion failed_assertion;

void debug_assert(bool test, const char *what, const char *file, size_t line) {
    failed_assertion.what = what;
    failed_assertion.file = file;
    failed_assertion.line = line;

    longjmp(assert_handler,1);
}
