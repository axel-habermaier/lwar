#include "prelude.h"

#ifdef WINDOWS

#include "win32.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgFloat64 pgGetTime()
{
	LARGE_INTEGER frequency, time;

	QueryPerformanceFrequency(&frequency);
    QueryPerformanceCounter(&time);

    // Return the current time in seconds
    return time.QuadPart / (pgFloat64)frequency.QuadPart;
}

//====================================================================================================================
// Windows-specific functions
//====================================================================================================================

pgVoid pgWin32Error(pgString message) 
{ 
	pgDieWin32Error(message, GetLastError());
}

pgVoid pgDieWin32Error(pgString message, DWORD error)
{
	static char buffer[2048];
	DWORD success;

	success = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,	NULL, error,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&buffer,	sizeof(buffer), NULL);

	if (success)
		PG_DIE("%s %s", message, buffer);
	else
		PG_DIE("%s (error code: 0x%X)", message, error);
}

#endif