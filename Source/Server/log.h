#ifdef _MSC_VER
	#define NORETURN __declspec(noreturn)
#else
	#define NORETURN __attribute__ ((noreturn))
#endif

NORETURN void log_die(const char* message, ...);
void log_error(const char* message, ...);
void log_warn(const char* message, ...);
void log_info(const char* message, ...);
void log_debug(const char* message, ...);