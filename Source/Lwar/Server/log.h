#ifndef LOG_H
#define LOG_H

#include "attributes.h"

#ifdef _MSC_VER
    NORETURN void log_die(_Printf_format_string_ const char* message, ...);
    void log_error(_Printf_format_string_ const char* message, ...);
    void log_warn(_Printf_format_string_ const char* message, ...);
    void log_info(_Printf_format_string_ const char* message, ...);
    void log_debug(_Printf_format_string_ const char* message, ...);
#else
	#define FORMAT   __attribute__ ((format(printf,1,2)))
    FORMAT NORETURN void log_die(const char* message, ...);
    FORMAT void log_error(const char* message, ...);
    FORMAT void log_warn(const char* message, ...);
    FORMAT void log_info(const char* message, ...);
    FORMAT void log_debug(const char* message, ...);
#endif

#endif
