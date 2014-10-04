#ifndef LOG_H
#define LOG_H

#include "attributes.h"

FORMAT NORETURN void log_die(const char* message, ...);
FORMAT void log_error(const char* message, ...);
FORMAT void log_warn(const char* message, ...);
FORMAT void log_info(const char* message, ...);
FORMAT void log_debug(const char* message, ...);

#endif
