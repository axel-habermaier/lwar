#ifndef pgWindow_h__
#define pgWindow_h__

#include "pg.h"

//====================================================================================================================
// Window types
//====================================================================================================================

/// <summary>
///     Represents a platform-specific window.
/// </summary>
typedef struct pgWindow pgWindow;

/// <summary>
///     The minimum supported window width.
/// </summary>
#define PG_WINDOW_MIN_WIDTH 800

/// <summary>
///     The minimum supported window height.
/// </summary>
#define PG_WINDOW_MIN_HEIGHT 600

/// <summary>
///     The maximum supported window width.
/// </summary>
#define PG_WINDOW_MAX_WIDTH 4096 

/// <summary>
///     The maximum supported window height.
/// </summary>
#define PG_WINDOW_MAX_HEIGHT 2160

/// <summary>
///     Identifies a mouse button.
/// </summary>
typedef enum
{
	/// <summary>
	///     Represents an unknown mouse button.
	/// </summary>
	PG_MOUSE_UNKNOWN		= 0,

	/// <summary>
	///     Identifies the left mouse button.
	/// </summary>
	PG_MOUSE_LEFT			= 1,

	/// <summary>
	///     Identifies the right mouse button.
	/// </summary>
	PG_MOUSE_RIGHT			= 2,

	/// <summary>
	///     Identifies the middle mouse button.
	/// </summary>
	PG_MOUSE_MIDDLE			= 3,

	/// <summary>
	///     Identifies the first extra mouse button.
	/// </summary>
	PG_MOUSE_X_BUTTON1		= 4,

	/// <summary>
	///     Identifies the second extra mouse button.
	/// </summary>
	PG_MOUSE_X_BUTTON2		= 5,
} pgMouseButton;

/// <summary>
///     Identifies a keyboard key.
/// </summary>
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
	PG_KEY_LEFT_CONTROL		= 38,
	PG_KEY_LEFT_SHIFT		= 39,
	PG_KEY_LEFT_ALT			= 40,
	PG_KEY_LEFT_SYSTEM		= 41,
	PG_KEY_RIGHT_CONTROL	= 42,
	PG_KEY_RIGHT_SHIFT		= 43,
	PG_KEY_RIGHT_ALT		= 44,
	PG_KEY_RIGHT_SYSTEM		= 45,
	PG_KEY_MENU				= 46,
	PG_KEY_LEFT_BRACKET		= 47,
	PG_KEY_RIGHT_BRACKET	= 48,
	PG_KEY_SEMICOLON		= 49,
	PG_KEY_COMMA			= 50,
	PG_KEY_PERIOD			= 51,
	PG_KEY_QUOTE			= 52,
	PG_KEY_SLASH			= 53,
	PG_KEY_BACK_SLASH		= 54,
	PG_KEY_GRAVE			= 55,
	PG_KEY_EQUAL			= 56,
	PG_KEY_DASH				= 57,
	PG_KEY_SPACE			= 58,
	PG_KEY_RETURN			= 59,
	PG_KEY_BACK				= 60,
	PG_KEY_TAB				= 61,
	PG_KEY_PAGE_UP			= 62,
	PG_KEY_PAGE_DOWN		= 63,
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
	PG_KEY_NUMPAD_ENTER		= 102,
	PG_KEY_NUMPAD_DECIMAL	= 103,
	PG_KEY_NUM_LOCK			= 104,
	PG_KEY_SCROLL			= 105,
	PG_KEY_PRINT			= 106,
	PG_KEY_CAPS_LOCK		= 107,
	PG_KEY_BACK_SLASH2		= 108
} pgKey;

/// <summary>
///     The type of the callback that is invoked when a character has been entered.
/// </summary>
/// <param name="character">The character that has been entered.</param>
/// <param name="scanCode">The scan code of the character that has been entered.</param>
typedef pgVoid (*pgCharacterEnteredCallback)(pgUInt16 character, pgInt32 scanCode);

/// <summary>
///     The type of the callback that is invoked when a dead character has been entered.
/// </summary>
/// <param name="character">The dead character that has been entered.</param>
/// <param name="scanCode">The scan code of the dead character that has been entered.</param>
/// <param name="cancel">Indicates whether the dead character input should be canceled, if possible.</param>
typedef pgVoid (*pgDeadCharacterEnteredCallback)(pgUInt16 character, pgInt32 scanCode, out pgBool* cancel);

/// <summary>
///     The type of the callback that is invoked when a key has been pressed.
/// </summary>
/// <param name="character">The key that has been pressed.</param>
/// <param name="scanCode">The scan code of the character that has been pressed.</param>
typedef pgVoid (*pgKeyPressedCallback)(pgKey key, pgInt32 scanCode);

/// <summary>
///     The type of the callback that is invoked when a key has been released.
/// </summary>
/// <param name="character">The key that has been released.</param>
/// <param name="scanCode">The scan code of the character that has been released.</param>
typedef pgVoid (*pgKeyReleasedCallback)(pgKey key, pgInt32 scanCode);

/// <summary>
///     The type of the callback that is invoked when the mouse wheel has been used.
/// </summary>
/// <param name="delta">The delta of the mouse wheel position.</param>
typedef pgVoid (*pgMouseWheelCallback)(pgInt32 delta);

/// <summary>
///     The type of the callback that is invoked when a mouse button has been pressed.
/// </summary>
/// <param name="button">The mouse button that has been pressed.</param>
/// <param name="doubleClick">Indicates whether the button has been double-clicked.</param>
/// <param name="x">The X component of the position where the mouse button has been pressed.</param>
/// <param name="y">The Y component of the position where the mouse button has been pressed.</param>
typedef pgVoid (*pgMousePressedCallback)(pgMouseButton button, pgBool doubleClick, pgInt32 x, pgInt32 y);

/// <summary>
///     The type of the callback that is invoked when a mouse button has been released.
/// </summary>
/// <param name="button">The mouse button that has been released.</param>
/// <param name="x">The X component of the position where the mouse button has been released.</param>
/// <param name="y">The Y component of the position where the mouse button has been released.</param>
typedef pgVoid (*pgMouseReleasedCallback)(pgMouseButton button, pgInt32 x, pgInt32 y);

/// <summary>
///     The type of the callback that is invoked when the mouse has been moved.
/// </summary>
/// <param name="x">The X component of the new mouse position.</param>
/// <param name="y">The Y component of the new mouse position.</param>
typedef pgVoid (*pgMouseMovedCallback)(pgInt32 x, pgInt32 y);

/// <summary>
///     The type of the callback that is invoked when the mouse has entered the window's client area.
/// </summary>
typedef pgVoid (*pgMouseEnteredCallback)();

/// <summary>
///     The type of the callback that is invoked when the mouse has left the window's client area.
/// </summary>
typedef pgVoid (*pgMouseLeftCallback)();

/// <summary>
///     Represents the set of event callbacks supported by a window.
/// </summary>
typedef struct
{
	/// <summary>
	///     Invoked when a character has been entered.
	/// </summary>
	pgCharacterEnteredCallback characterEntered;

	/// <summary>
	///     Invoked when a dead character has been entered.
	/// </summary>
	pgDeadCharacterEnteredCallback deadCharacterEntered;

	/// <summary>
	///     Invoked when a key has been pressed.
	/// </summary>
	pgKeyPressedCallback keyPressed;

	/// <summary>
	///     Invoked when a key has been released.
	/// </summary>
	pgKeyReleasedCallback keyReleased;

	/// <summary>
	///     Invoked when the mouse wheel has been used.
	/// </summary>
	pgMouseWheelCallback mouseWheel;

	/// <summary>
	///     Invoked when a mouse button has been pressed.
	/// </summary>
	pgMousePressedCallback mousePressed;

	/// <summary>
	///     Invoked when a mouse button has been released.
	/// </summary>
	pgMouseReleasedCallback mouseReleased;

	/// <summary>
	///     Invoked when the mouse has been moved.
	/// </summary>
	pgMouseMovedCallback mouseMoved;

	/// <summary>
	///     Invoked when the mouse has entered the window's client area.
	/// </summary>
	pgMouseEnteredCallback mouseEntered;

	/// <summary>
	///     Invoked when the mouse has left the window's client area.
	/// </summary>
	pgMouseLeftCallback mouseLeft;
} pgWindowCallbacks;

/// <summary>
///     Indicates the whether a window is minimized, maximized, in fullscreen mode, or neither of which.
/// </summary>
typedef enum
{
	/// <summary>
	///     Indicates that the window is neither minimized nor maximized nor in fullscreen mode.
	/// </summary>
	PG_WINDOW_NORMAL = 1,

	/// <summary>
	///     Indicates that the window is maximized, filling the entire screen.
	/// </summary>
	PG_WINDOW_MAXIMIZED = 2,

	/// <summary>
	///     Indicates that the window is minimized and invisible.
	/// </summary>
	PG_WINDOW_MINIMIZED = 3,

	/// <summary>
	///     Indicates that the window is in border-less fullscreen mode, filling the entire screen.
	/// </summary>
	PG_WINDOW_FULLSCREEN = 4
} pgWindowMode;

/// <summary>
///     Describes the placement, size, and mode of a window.
/// </summary>
typedef struct
{
	/// <summary>
	///     The mode of the window.
	/// </summary>
	pgWindowMode mode;

	/// <summary>
	///     The X component of the window's position.
	/// </summary>
	pgInt32 x;

	/// <summary>
	///     The Y component of the window's position.
	/// </summary>
	pgInt32 y;

	/// <summary>
	///     The width of the window.
	/// </summary>
	pgInt32 width;

	/// <summary>
	///     The size of the window.
	/// </summary>
	pgInt32 height;
} pgWindowPlacement;

//====================================================================================================================
// Window functions
//====================================================================================================================

/// <summary>
///     Opens a new window.
/// </summary>
/// <param name="title">The title that should be shown in the window's caption.</param>
/// <param name="placement">The initial placement of the window.</param>
/// <param name="callbacks">The callbacks that should be used to report window events.</param>
PG_API_EXPORT pgWindow* pgOpenWindow(pgString title, pgWindowPlacement placement, pgWindowCallbacks callbacks);

/// <summary>
///     Closes the given window.
/// </summary>
/// <param name="window">The window that should be closed.</param>
PG_API_EXPORT pgVoid pgCloseWindow(pgWindow* window);

/// <summary>
///     Processes all pending events of the given window.
/// </summary>
/// <param name="window">The window whose pending events should be processed.</param>
PG_API_EXPORT pgVoid pgProcessWindowEvents(pgWindow* window);

/// <summary>
///     Gets a value indicating whether the given window currently has the user focus.
/// </summary>
/// <param name="window">The window that should be checked.</param>
PG_API_EXPORT pgBool pgIsWindowFocused(pgWindow* window);

/// <summary>
///     Gets a value indicating whether the user requested the given window to be closed. If true, the value is reset the
///		next time pgProcessWindowEvents is called.
/// </summary>
/// <param name="window">The window that should be checked.</param>
PG_API_EXPORT pgBool pgIsWindowClosing(pgWindow* window);

/// <summary>
///     Gets information about the current placement of the given window.
/// </summary>
/// <param name="window">The window the placement should be returned for.</param>
/// <param name="placement">Returns the current placement of the window.</param>
PG_API_EXPORT pgVoid pgGetWindowPlacement(pgWindow* window, out pgWindowPlacement* placement);

/// <summary>
///     Changes the title of the given window.
/// </summary>
/// <param name="window">The window whose title should be changed.</param>
/// <param name="title">The new title of the window.</param>
PG_API_EXPORT pgVoid pgSetWindowTitle(pgWindow* window, pgString title);

/// <summary>
///     Changes the given window to fullscreen mode.
/// </summary>
/// <param name="window">The window whose mode should be changed.</param>
PG_API_EXPORT pgVoid pgChangeToFullscreenMode(pgWindow* window);

/// <summary>
///     Changes the given window to windowed mode.
/// </summary>
/// <param name="window">The window whose mode should be changed.</param>
PG_API_EXPORT pgVoid pgChangeToWindowedMode(pgWindow* window);

/// <summary>
///     Captures the mouse within the given window, preventing it from leaving the window's client area.
/// </summary>
/// <param name="window">The window that should capture the mouse.</param>
PG_API_EXPORT pgVoid pgCaptureMouse(pgWindow* window);

/// <summary>
///     Releases the mouse from the given window, no longer preventing it from leaving the window's client area.
/// </summary>
/// <param name="window">The window that should release the mouse.</param>
PG_API_EXPORT pgVoid pgReleaseMouse(pgWindow* window);

/// <summary>
///     Gets the position of the mouse relative to the given window.
/// </summary>
/// <param name="window">The window the mouse position should be relative to.</param>
/// <param name="x">Returns the X component of the mouse position.</param>
/// <param name="y">Returns the Y component of the mouse position.</param>
PG_API_EXPORT pgVoid pgGetMousePosition(pgWindow* window, out pgInt32* x, out pgInt32* y);

/// <summary>
///     Shows a message box with the given caption and message.
/// </summary>
/// <param name="caption">The caption of the message box.</param>
/// <param name="message">The message that the message box should display.</param>
PG_API_EXPORT pgVoid pgShowMessageBox(pgString caption, pgString message);

#endif