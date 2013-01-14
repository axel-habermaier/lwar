#include "prelude.h"

#ifdef WINDOWS

// Exclude rarely used Windows functionality
#define WIN32_LEAN_AND_MEAN
#define WIN32_EXTRA_LEAN
#define VC_EXTRALEAN

// Prevent Windows from polluting the global namespace with too many things
#define NOGDICAPMASKS
#define NOMENUS
#define NOSYSCOMMANDS
#define NORASTEROPS
#define NOATOM
#define NODRAWTEXT
#define NOKERNEL
#define NOMEMMGR
#define NOMETAFILE
#define NOMINMAX
#define NOOPENFILE
#define NOSERVICE
#define NOSOUND
#define NOWINDOWSTATION
#define NOCOMM
#define NOKANJI
#define NOHELP
#define NOPROFILER
#define NODEFERWINDOWPOS
#define NOMCX

// Disable all warnings for the windows header
#pragma warning(push, 0)
#include <windows.h>
#pragma warning(pop)

HANDLE hConsole;

// ---------------------------------------------------------------------------------------------------------------------
// Console logger
static void log_console(const LogData* data)
{
	switch (data->type)
	{
	case LT_FATAL:
		SetConsoleTextAttribute(hConsole, FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_RED | BACKGROUND_RED);
		break;
	case LT_ERROR:
		SetConsoleTextAttribute(hConsole, FOREGROUND_RED | 0xc);
		break;
	case LT_WARNING:
		SetConsoleTextAttribute(hConsole, 4 | 0xe);
		break;
	case LT_INFO:
		SetConsoleTextAttribute(hConsole, FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_RED);
		break;
	case LT_DEBUG:
		SetConsoleTextAttribute(hConsole, 5 | 0xd);
		break;
	default:
		LWAR_NO_SWITCH_DEFAULT;
	}

	sys_printf(logtype_to_string(data->type));
	sys_printf(": ");
	sys_printf(data->msg);
	sys_printf("\n");
}

static Logger console_logger = LOGGER_INIT(log_console);

// ---------------------------------------------------------------------------------------------------------------------
// Visual Studio logger

static void log_vs(const LogData* data)
{
	static char buffer[4096];
	sys_sprintf(buffer, sizeof(buffer), "%s(%d): %s: %s", data->file, data->line, logtype_to_string(data->type), data->msg);
	OutputDebugString(buffer);
	OutputDebugString("\n");
}

static Logger vs_logger = LOGGER_INIT(log_vs);

void sys_init()
{
	hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

	log_register(&console_logger);
	log_register(&vs_logger);
}

#endif