#ifndef LOG_H
#define LOG_H

#include "noreturn.h"

NORETURN void log_die(const char* message, ...);
void log_error(const char* message, ...);
void log_warn(const char* message, ...);
void log_info(const char* message, ...);
void log_debug(const char* message, ...);

#endif
