#include <stdarg.h>
#include <stdio.h>
#include <assert.h>

#include "log.h"
#include "server_export.h"

static LogCallbacks _callbacks;

void server_log_callbacks(LogCallbacks callbacks)
{
	_callbacks = callbacks;
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
	assert(_callbacks.die != NULL);

	va_list vl;
	va_start(vl, message);
	_callbacks.die(format(message, vl));
	va_end(vl);

#ifndef _MSC_VER
    for(;;);
#endif
}

void log_error(const char* message, ...)
{
	if (_callbacks.error == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	_callbacks.error(format(message, vl));
	va_end(vl);
}

void log_warn(const char* message, ...)
{
	if (_callbacks.warning == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	_callbacks.warning(format(message, vl));
	va_end(vl);
}

void log_info(const char* message, ...)
{
	if (_callbacks.info == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	_callbacks.info(format(message, vl));
	va_end(vl);
}

void log_debug(const char* message, ...)
{
	if (_callbacks.debug == NULL)
		return;

	va_list vl;
	va_start(vl, message);
	_callbacks.debug(format(message, vl));
	va_end(vl);
}
