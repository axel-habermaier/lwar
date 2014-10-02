#define assert(test) \
    debug_assert((test), #test, __FILE__, __LINE__)

typedef struct FailedAssertion FailedAssertion;

struct FailedAssertion {
    const char *what;
    const char *file;
    size_t line;
};

extern FailedAssertion failed_assertion;

void debug_assert(bool test, const char *what, const char *file, size_t line);
