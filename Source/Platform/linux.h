#ifndef linux_h__
#define linux_h__

//====================================================================================================================
// X11 includes and initialization state
//====================================================================================================================

#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <X11/keysym.h>

typedef struct
{
	Display*	display;
	int			screen;
	int			refCount;
} X11State;

extern X11State x11State;

pgVoid pgInitializeX11();
pgVoid pgShutdownX11();

//====================================================================================================================
// Linux-specific data types
//====================================================================================================================

#define PG_WINDOW_PLATFORM 		\
	Window 		handle;			\
	Atom		closeAtom;		\
	XIM			inputMethod;	\
	XIC			inputContext;

#endif
