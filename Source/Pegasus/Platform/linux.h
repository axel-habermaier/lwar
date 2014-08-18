#ifndef linux_h__
#define linux_h__

//====================================================================================================================
// X11 includes and initialization state
//====================================================================================================================

#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <X11/keysym.h>
#include <X11/Xatom.h>
#include <X11/extensions/Xrandr.h>

#include <arpa/inet.h>
#include <errno.h>
#include <fcntl.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>

typedef struct
{
	Display*	display;
	int			screen;
	int			refCount;
	Atom		wmState;
	Atom		wmStateFullscreen;
	Atom		wmStateMaximizedVert;
	Atom		wmStateMaximizedHorz;
	Atom		wmStateHidden;
	
} X11State;

extern X11State x11;

pgVoid pgInitializeX11();
pgVoid pgShutdownX11();

//====================================================================================================================
// Linux-specific data types
//====================================================================================================================

#define PG_WINDOW_PLATFORM 		\
	Window 		handle;			\
	Atom		closeAtom;		\
	XIM			inputMethod;	\
	XIC			inputContext;	\
	Cursor		cursor;

//====================================================================================================================
// Network types and defines
//====================================================================================================================

typedef int Socket;
#define socket_error(s)   ((s) < 0)
#define socket_invalid(s) ((s) < 0)
#define closesocket close

#endif
