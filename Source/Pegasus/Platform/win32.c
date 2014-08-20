#include "prelude.h"
#include <stdio.h>

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

pgString pgGetWin32ErrorMessage(DWORD error)
{
	static char buffer[2048];
	DWORD success;

	success = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, error,
							MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&buffer, sizeof(buffer), NULL);

	if (!success)
		sprintf(buffer, "(error code: 0x%X)", error);

	return buffer;
}

pgVoid pgWin32Error(pgString message) 
{ 
	pgDieWin32Error(message, GetLastError());
}

pgVoid pgDieWin32Error(pgString message, DWORD error)
{
	PG_DIE("%s %s", message, pgGetWin32ErrorMessage(error));
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgVoid pgInitializeCore()
{
	WSADATA wsaData;

	if (WSAStartup(MAKEWORD(1, 1), &wsaData) != 0)
		pgWin32Error("Winsock initialization failed.");
}

pgVoid pgShutdownCore()
{
	if (WSACleanup() != 0)
		pgWin32Error("Failed to shut down Winsock.");
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