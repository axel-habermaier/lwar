#include <stdarg.h>
#include <stdio.h>
#include <assert.h>

#include "log.h"
#include "server_export.h"

static LogCallbacks logCallbacks;

void server_callbacks(LogCallbacks callbacks)
{
	logCallbacks = callbacks;
}

static const char* format(const char* message, va_list vl)
{
	static char buffer[4096];

	if (vsnprintf((char*)buffer, sizeof(buffer), (char*)message, vl) < 0)
		log_die("Error while generating log message.");
	else
		return buffer;
}

void log_die(const char* message, ...)
{
	assert(logCallbacks.die != NULL);

	va_list vl;
	va_start(vl, message);
	logCallbacks.die(format(message, vl));
	va_end(vl);
}

void log_error(const char* message, ...)
{
	if (logCallbacks.error == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	logCallbacks.error(format(message, vl));
	va_end(vl);
}

void log_warn(const char* message, ...)
{
	if (logCallbacks.warning == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	logCallbacks.warning(format(message, vl));
	va_end(vl);
}

void log_info(const char* message, ...)
{
	if (logCallbacks.info == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	logCallbacks.info(format(message, vl));
	va_end(vl);
}

void log_debug(const char* message, ...)
{
	if (logCallbacks.debug == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	logCallbacks.debug(format(message, vl));
	va_end(vl);
}
