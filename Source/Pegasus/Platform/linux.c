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
	
	x11.wmState = XInternAtom(x11.display, "_NET_WM_STATE", False);
	x11.wmStateFullscreen = XInternAtom(x11.display, "_NET_WM_STATE_FULLSCREEN", False);
	x11.wmStateMaximizedVert = XInternAtom(x11.display, "_NET_WM_STATE_MAXIMIZED_VERT", False);
	x11.wmStateMaximizedHorz = XInternAtom(x11.display, "_NET_WM_STATE_MAXIMIZED_HORZ", False);
	x11.wmStateHidden = XInternAtom(x11.display, "_NET_WM_STATE_HIDDEN", False);
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

pgVoid pgShowMessageBox(pgString caption, pgString message)
{
	PG_UNUSED(caption);
	PG_UNUSED(message);

	PG_DIE("Message boxes are not supported by Linux.");
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgVoid pgInitializeCore()
{
}

pgVoid pgShutdownCore()
{
}

#endif
