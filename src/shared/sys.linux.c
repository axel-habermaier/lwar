#include "prelude.h"

#ifdef LINUX

static void log_console(const LogData* data)
{
	sys_printf(logtype_to_string(data->type));
	sys_printf(": ");
	sys_printf(data->msg);
	sys_printf("\n");
}

static Logger console_logger = LOGGER_INIT(log_console);

void sys_init()
{
	log_register(&console_logger);
}

#endif