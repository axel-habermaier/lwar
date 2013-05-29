#include "prelude.h"

#ifdef LINUX

#include <time.h>

//====================================================================================================================
// X11 state helper functions
//====================================================================================================================

X11State x11;

pgVoid pgInitializeX11()
{
	if (x11.refCount++ != 0)
		return;

	x11.display = XOpenDisplay(NULL);
	if (x11.display == NULL)
		PG_DIE("Unable to connect to X11.");

	x11.screen = XDefaultScreen(x11.display);
}

pgVoid pgShutdownX11()
{
	PG_ASSERT(x11.refCount > 0, "More shutdown requests than initialization requests.");

	if (--x11.refCount != 0)
		return;

	XCloseDisplay(x11.display);
	x11.display = NULL;
	x11.screen = 0;
}

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgFloat64 pgGetTime()
{
	struct timespec time;
    clock_gettime(CLOCK_MONOTONIC, &time);
    return (double)(((pgUint64)time.tv_sec) * 1e9 + time.tv_nsec) * 1e-9;
}

#endif
