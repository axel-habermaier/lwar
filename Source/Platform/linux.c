#include "prelude.h"

#ifdef LINUX

#include <time.h>

//====================================================================================================================
// X11 state helper functions
//====================================================================================================================

X11State x11State;

pgVoid pgInitializeX11()
{
	if (x11State.refCount++ != 0)
		return;

	x11State.display = XOpenDisplay(NULL);
	if (x11State.display == NULL)
		PG_DIE("Unable to connect to X11.");

	x11State.screen = XDefaultScreen(x11State.display);
}

pgVoid pgShutdownX11()
{
	PG_ASSERT(x11State.refCount > 0, "More shutdown requests than initialization requests.");

	if (--x11State.refCount != 0)
		return;

	XCloseDisplay(x11State.display);
	x11State.display = NULL;
	x11State.screen = 0;
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
