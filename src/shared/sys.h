// Aborts the execution of the application immediately
LWAR_NORETURN void sys_abort();

// Prints the format string, retuning the number of characters that have been written or a negative number
// on failure
int32_t sys_printf(const char* const fmt, ...);

// Writes the formatted string into dest, retuning the number of characters that have been written or a negative number
// on failure
int32_t sys_sprintf(char* const dest, size_t length, const char* const fmt, ...);
int32_t sys_vsprintf(char* const dest, size_t length, const char* const fmt, va_list vl);

// Initializes the platform-specific data structures
void sys_init();

// Copies the given number of bytes from src to dest, checking that the memory regions do not overlap
void mem_copy(void* const dest, const void* const src, size_t bytes);

// Moves the given number of bytes from src to dest; the copied memory regions are allowed to overlap
void mem_move(void* const dest, const void* const src, size_t bytes);