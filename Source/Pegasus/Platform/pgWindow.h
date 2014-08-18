#ifndef pgWindow_h__
#define pgWindow_h__

#include "pg.h"

//====================================================================================================================
// Window types
//====================================================================================================================

typedef struct pgWindow pgWindow;

#define PG_WINDOW_MIN_WIDTH 800
#define PG_WINDOW_MIN_HEIGHT 600
#define PG_WINDOW_MAX_WIDTH 4096 
#define PG_WINDOW_MAX_HEIGHT 2160

typedef enum
{
	PG_MOUSE_UNKNOWN		= 0,
	PG_MOUSE_LEFT			= 1,
	PG_MOUSE_RIGHT			= 2,
	PG_MOUSE_MIDDLE			= 3,
	PG_MOUSE_XBUTTON1		= 4,
	PG_MOUSE_XBUTTON2		= 5,
} pgMouseButton;

typedef enum
{
	PG_KEY_UNKNOWN			= 0,
	PG_KEY_A				= 1,
	PG_KEY_B				= 2,
	PG_KEY_C				= 3,
	PG_KEY_D				= 4,
	PG_KEY_E				= 5,
	PG_KEY_F				= 6,
	PG_KEY_G				= 7,
	PG_KEY_H				= 8,
	PG_KEY_I				= 9,
	PG_KEY_J				= 10,
	PG_KEY_K				= 11,
	PG_KEY_L				= 12,
	PG_KEY_M				= 13,
	PG_KEY_N				= 14,
	PG_KEY_O				= 15,
	PG_KEY_P				= 16,
	PG_KEY_Q				= 17,
	PG_KEY_R				= 18,
	PG_KEY_S				= 19,
	PG_KEY_T				= 20,
	PG_KEY_U				= 21,
	PG_KEY_V				= 22,
	PG_KEY_W				= 23,
	PG_KEY_X				= 24,
	PG_KEY_Y				= 25,
	PG_KEY_Z				= 26,
	PG_KEY_NUM0				= 27,
	PG_KEY_NUM1				= 28,
	PG_KEY_NUM2				= 29,
	PG_KEY_NUM3				= 30,
	PG_KEY_NUM4				= 31,
	PG_KEY_NUM5				= 32,
	PG_KEY_NUM6				= 33,
	PG_KEY_NUM7				= 34,
	PG_KEY_NUM8				= 35,
	PG_KEY_NUM9				= 36,
	PG_KEY_ESCAPE			= 37,
	PG_KEY_LEFTCONTROL		= 38,
	PG_KEY_LEFTSHIFT		= 39,
	PG_KEY_LEFTALT			= 40,
	PG_KEY_LEFTSYSTEM		= 41,
	PG_KEY_RIGHTCONTROL		= 42,
	PG_KEY_RIGHTSHIFT		= 43,
	PG_KEY_RIGHTALT			= 44,
	PG_KEY_RIGHTSYSTEM		= 45,
	PG_KEY_MENU				= 46,
	PG_KEY_LEFTBRACKET		= 47,
	PG_KEY_RIGHTBRACKET		= 48,
	PG_KEY_SEMICOLON		= 49,
	PG_KEY_COMMA			= 50,
	PG_KEY_PERIOD			= 51,
	PG_KEY_QUOTE			= 52,
	PG_KEY_SLASH			= 53,
	PG_KEY_BACKSLASH		= 54,
	PG_KEY_GRAVE			= 55,
	PG_KEY_EQUAL			= 56,
	PG_KEY_DASH				= 57,
	PG_KEY_SPACE			= 58,
	PG_KEY_RETURN			= 59,
	PG_KEY_BACK				= 60,
	PG_KEY_TAB				= 61,
	PG_KEY_PAGEUP			= 62,
	PG_KEY_PAGEDOWN			= 63,
	PG_KEY_END				= 64,
	PG_KEY_HOME				= 65,
	PG_KEY_INSERT			= 66,
	PG_KEY_DELETE			= 67,
	PG_KEY_ADD				= 68,
	PG_KEY_SUBTRACT			= 69,
	PG_KEY_MULTIPLY			= 70,
	PG_KEY_DIVIDE			= 71,
	PG_KEY_LEFT				= 72,
	PG_KEY_RIGHT			= 73,
	PG_KEY_UP				= 74,
	PG_KEY_DOWN				= 75,
	PG_KEY_NUMPAD0			= 76,
	PG_KEY_NUMPAD1			= 77,
	PG_KEY_NUMPAD2			= 78,
	PG_KEY_NUMPAD3			= 79,
	PG_KEY_NUMPAD4			= 80,
	PG_KEY_NUMPAD5			= 81,
	PG_KEY_NUMPAD6			= 82,
	PG_KEY_NUMPAD7			= 83,
	PG_KEY_NUMPAD8			= 84,
	PG_KEY_NUMPAD9			= 85,
	PG_KEY_F1				= 86,
	PG_KEY_F2				= 87,
	PG_KEY_F3				= 88,
	PG_KEY_F4				= 89,
	PG_KEY_F5				= 90,
	PG_KEY_F6				= 91,
	PG_KEY_F7				= 92,
	PG_KEY_F8				= 93,
	PG_KEY_F9				= 94,
	PG_KEY_F10				= 95,
	PG_KEY_F11				= 96,
	PG_KEY_F12				= 97,
	PG_KEY_F13				= 98,
	PG_KEY_F14				= 99,
	PG_KEY_F15				= 100,
	PG_KEY_PAUSE			= 101,
	PG_KEY_NUMPADENTER		= 102,
	PG_KEY_NUMPADDECIMAL	= 103,
	PG_KEY_NUMLOCK			= 104,
	PG_KEY_SCROLL			= 105,
	PG_KEY_PRINT			= 106,
	PG_KEY_CAPSLOCK			= 107,
	PG_KEY_BACKSLASH2		= 108
} pgKey;

typedef pgVoid (*pgCharacterEnteredCallback)(pgUint16 character, pgInt32 scanCode);
typedef pgVoid (*pgDeadCharacterEnteredCallback)(pgUint16 character, pgInt32 scanCode, pgBool* cancel);
typedef pgVoid (*pgKeyPressedCallback)(pgKey key, pgInt32 scanCode);
typedef pgVoid (*pgKeyReleasedCallback)(pgKey key, pgInt32 scanCode);
typedef pgVoid (*pgMouseWheelCallback)(pgInt32 delta);
typedef pgVoid (*pgMousePressedCallback)(pgMouseButton button, pgBool doubleClick, pgInt32 x, pgInt32 y);
typedef pgVoid (*pgMouseReleasedCallback)(pgMouseButton button, pgInt32 x, pgInt32 y);
typedef pgVoid (*pgMouseMovedCallback)(pgInt32 x, pgInt32 y);
typedef pgVoid (*pgMouseEnteredCallback)();
typedef pgVoid (*pgMouseLeftCallback)();

typedef struct
{
	pgCharacterEnteredCallback		characterEntered;
	pgDeadCharacterEnteredCallback	deadCharacterEntered;
	pgKeyPressedCallback			keyPressed;
	pgKeyReleasedCallback			keyReleased;
	pgMouseWheelCallback			mouseWheel;
	pgMousePressedCallback			mousePressed;
	pgMouseReleasedCallback			mouseReleased;
	pgMouseMovedCallback			mouseMoved;
	pgMouseEnteredCallback			mouseEntered;
	pgMouseLeftCallback				mouseLeft;
} pgWindowCallbacks;

typedef enum
{
	PG_WINDOW_NORMAL = 1,
	PG_WINDOW_MAXIMIZED = 2,
	PG_WINDOW_MINIMIZED = 3,
	PG_WINDOW_FULLSCREEN = 4
} pgWindowMode;

typedef struct
{
	pgWindowMode	mode;
	pgInt32			x;
	pgInt32			y;
	pgInt32			width;
	pgInt32			height;
} pgWindowPlacement;

//====================================================================================================================
// Window functions
//====================================================================================================================

PG_API_EXPORT pgWindow* pgOpenWindow(pgString title, pgWindowPlacement placement, pgWindowCallbacks callbacks);
PG_API_EXPORT pgVoid pgCloseWindow(pgWindow* window);

PG_API_EXPORT pgVoid pgProcessWindowEvents(pgWindow* window);

PG_API_EXPORT pgBool pgIsWindowFocused(pgWindow* window);
PG_API_EXPORT pgBool pgIsWindowClosing(pgWindow* window);
PG_API_EXPORT pgVoid pgGetWindowPlacement(pgWindow* window, pgWindowPlacement* placement);

PG_API_EXPORT pgVoid pgSetWindowTitle(pgWindow* window, pgString title);
PG_API_EXPORT pgVoid pgChangeToFullscreenMode(pgWindow* window);
PG_API_EXPORT pgVoid pgChangeToWindowedMode(pgWindow* window);

PG_API_EXPORT pgVoid pgCaptureMouse(pgWindow* window);
PG_API_EXPORT pgVoid pgReleaseMouse(pgWindow* window);
PG_API_EXPORT pgVoid pgGetMousePosition(pgWindow* window, pgInt32* x, pgInt32* y);

PG_API_EXPORT pgVoid pgShowMessageBox(pgString caption, pgString message);

#endif