#include "prelude.h"

#ifdef WINDOWS

#include "win32.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgFloat64 pgGetFequency();

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgFloat64 pgGetTime()
{
	LARGE_INTEGER time;
    QueryPerformanceCounter(&time);

    // Return the current time in seconds
	return time.QuadPart / pgGetFequency();
}

pgVoid pgShowMessageBox(pgString caption, pgString message)
{
	PG_ASSERT_NOT_NULL(caption);
	PG_ASSERT_NOT_NULL(message);

	MessageBox(NULL, message, caption, MB_ICONERROR | MB_OK);
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

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgFloat64 pgGetFequency()
{
	static pgFloat64 frequency;

	if (frequency == 0)
	{
		LARGE_INTEGER f;
		QueryPerformanceFrequency(&f);

		frequency = (pgFloat64)f.QuadPart;
	}

	return frequency;
}

#endif