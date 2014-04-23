#ifndef win32_h__
#define win32_h__

//====================================================================================================================
// Windows include
//====================================================================================================================

// Exclude rarely used Windows functionality
#define WIN32_LEAN_AND_MEAN
#define WIN32_EXTRA_LEAN
#define VC_EXTRALEAN

// Prevent Windows from polluting the global namespace with too many things
#define NOGDICAPMASKS
#define NOMENUS
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

#include <winsock2.h>
#include <ws2tcpip.h>

//====================================================================================================================
// Windows functions
//====================================================================================================================

pgString pgGetWin32ErrorMessage(DWORD error);
pgVoid pgWin32Error(pgString message);
pgVoid pgDieWin32Error(pgString message, DWORD error);

//====================================================================================================================
// Window-specific data types
//====================================================================================================================

#define PG_WINDOW_PLATFORM	\
	HWND	hwnd;			\
	HCURSOR cursor;			\
	pgBool  cursorInside;

//====================================================================================================================
// Network types and defines
//====================================================================================================================

typedef SOCKET Socket;
#define socket_error(s)   ((s) == SOCKET_ERROR)
#define socket_invalid(s) ((s) == INVALID_SOCKET)

#endif