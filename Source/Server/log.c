#include <stdarg.h>
#include <stdio.h>

#include "log.h"

#ifdef _MSC_VER
#include <windows.h>
#endif

int log_print(char *s, ...) {
	va_list ap;
	va_start(ap, s);
	int r = vprintf(s, ap);

#ifdef _MSC_VER
	static char buffer[2048];
	vsprintf_s(buffer, s, ap);
	OutputDebugString(buffer);
#endif

	va_end(ap);
	return r;
}

int log_printn(char *s, ...) {
    va_list ap;
    va_start(ap, s);
    int r = vprintf(s, ap);
	printf("\n");
   
#ifdef _MSC_VER
	static char buffer[2048];
	vsprintf_s(buffer, s, ap);
	OutputDebugString(buffer);
	OutputDebugString("\n");
#endif

	va_end(ap);
    return r;
}
