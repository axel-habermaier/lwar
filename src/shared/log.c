#include "prelude.h"
#include <stdarg.h>

static List loggers;

void log_init()
{
	LWAR_ASSERT_NULL(loggers.next);
	list_init_head(&loggers);
}

void log_register(Logger* logger)
{
	LWAR_ASSERT_NOT_NULL(logger);
	LWAR_ASSERT_NOT_NULL(loggers.next);
	LWAR_ASSERT_NULL(logger->list.next);
	LWAR_ASSERT_NULL(logger->list.prev);
	LWAR_ASSERT_NOT_NULL(logger->callback);

	list_add(&logger->list, &loggers);
}

void log_write(const char* msg, LogType type, const char* file, size_t line, ...)
{
	LWAR_ASSERT_NOT_NULL(msg);
	LWAR_ASSERT_NOT_NULL(file);

	LogData data;
	data.file = file;
	data.line = line;
	data.type = type;

	static char buffer[4096];
	va_list vl;
	va_start(vl, line);
	if (sys_vsprintf(buffer, sizeof(buffer), msg, vl) < 0)
		data.msg = "Error while generation log message.";
	else
		data.msg = buffer;

	va_end(vl);

	Logger* logger;
	List* node;
	list_foreach(node, &loggers)
	{
		logger = list_element(node, Logger, list);
		logger->callback(&data);
	}
}

const char* logtype_to_string(LogType type)
{
	switch (type)
	{
	case LT_FATAL:		return "Fatal";
	case LT_ERROR:		return "Error";
	case LT_WARNING:	return "Warning";
	case LT_INFO:		return "Info";
	case LT_DEBUG:		return "Debug";
	default:
		LWAR_NO_SWITCH_DEFAULT;
	}
}