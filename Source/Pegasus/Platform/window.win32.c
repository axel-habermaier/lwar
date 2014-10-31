#include "prelude.h"

#ifdef PG_SYSTEM_WINDOWS

//====================================================================================================================
// Helper functions, defines, and state
//====================================================================================================================

#define WndClassName "PegasusWindowClass"

static struct pgWindowsState
{
	pgInt32		openWindows;
	pgMessage	message;
} state;

static pgVoid Initialize();
static pgVoid Shutdown();
static pgVoid RegisterWindowClass(pgString className, WNDPROC wndProc);
static pgVoid HandleWindowMessages(HWND hwnd);
static LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
static LRESULT HandleKeyboardInput(LPARAM lParam);
static pgVoid CenterCursor(pgWindow* window);
static pgKey TranslateKey(UINT virtualKey);

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgOpenWindowCore(pgWindow* window, pgString title)
{
	RECT rect;
	LONG width, height;
	UINT style = WS_VISIBLE | WS_CAPTION | WS_MINIMIZEBOX | WS_THICKFRAME | WS_MAXIMIZEBOX | WS_POPUP | WS_SYSMENU;

	if (state.openWindows++ == 0)
		Initialize();

	// Set the client size to the given width and height
	rect.left = 0;
	rect.top = 0;
	rect.right = window->placement.width; 
	rect.bottom = window->placement.height;

	if (!AdjustWindowRect(&rect, style, PG_FALSE))
		pgWin32Die("Failed to adjust window rectangle.");

	width = rect.right - rect.left;
	height = rect.bottom - rect.top;

	// Create the window
	CreateWindowEx(0, WndClassName, title, style, window->placement.x, window->placement.y, 
		width, height, NULL, NULL, GetModuleHandle(NULL), window);

	if (window->hwnd == NULL)
		pgWin32Die("Failed to open window.");

	if (window->placement.mode == PG_WINDOW_MAXIMIZED)
		ShowWindow(window->hwnd, SW_SHOWMAXIMIZED);
	else if (window->placement.mode == PG_WINDOW_MINIMIZED)
		ShowWindow(window->hwnd, SW_SHOWMINIMIZED);
	else
		ShowWindow(window->hwnd, SW_SHOW);
}

pgVoid pgCloseWindowCore(pgWindow* window)
{
	PG_ASSERT(state.openWindows > 0, "There are no open windows.");

	ShowCursor(PG_TRUE);

	if (window->hwnd != NULL && !DestroyWindow(window->hwnd))
		pgWin32Die("Failed to destroy window.");

	if (--state.openWindows == 0)
		Shutdown();

	window->hwnd = NULL;
}

pgBool pgProcessWindowEvent(pgWindow* window, pgMessage* message)
{
	HandleWindowMessages(window->hwnd);
	if (state.message.type != PG_MESSAGE_INVALID)
	{
		*message = state.message;
		return PG_TRUE;
	}

	return PG_FALSE;
}

pgVoid pgGetWindowPlacementCore(pgWindow* window)
{
	RECT rect;

	if (window->placement.mode != PG_WINDOW_FULLSCREEN)
	{
		if (IsZoomed(window->hwnd))
			window->placement.mode = PG_WINDOW_MAXIMIZED;
		else if (IsIconic(window->hwnd))
			window->placement.mode = PG_WINDOW_MINIMIZED;
		else
			window->placement.mode = PG_WINDOW_NORMAL;
	}

	// Don't update the position and size when the window is minimized, as that only results in invalid values
	if (IsIconic(window->hwnd))
		return;

	if (!GetClientRect(window->hwnd, &rect))
		pgWin32Die("Failed to get window size.");

    window->placement.width = rect.right - rect.left;
	window->placement.height = rect.bottom - rect.top;

	// Don't update the position when the window is maximized, as that only results in invalid values
	if (IsIconic(window->hwnd))
		return;

	if (!GetWindowRect(window->hwnd, &rect))
		pgWin32Die("Failed to get window position.");
	
	window->placement.x = rect.left;
	window->placement.y = rect.top;
}

pgBool pgIsWindowFocusedCore(pgWindow* window)
{
	return GetForegroundWindow() == window->hwnd;
}

pgVoid pgChangeToFullscreenModeCore(pgWindow* window)
{
	LONG_PTR style = GetWindowLongPtr(window->hwnd, GWL_STYLE);
	style &= ~(WS_CAPTION | WS_THICKFRAME | WS_SYSMENU | WS_OVERLAPPEDWINDOW);

	if (!SetWindowLongPtr(window->hwnd, GWL_STYLE, style))
		pgWin32Die("Failed to set fullscreen window style.");

	if (window->placement.mode == PG_WINDOW_MAXIMIZED)
	{
		HMONITOR monitor = MonitorFromWindow(window->hwnd, MONITOR_DEFAULTTONEAREST);
		MONITORINFO monitorInfo = { sizeof(MONITORINFO) };

		if (!GetMonitorInfo(monitor, &monitorInfo)) 
			pgWin32Die("Failed to get monitor info.");

		LONG x = monitorInfo.rcMonitor.left;
		LONG y = monitorInfo.rcMonitor.top;
		LONG width = monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left;
		LONG height = monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top;

		if (!SetWindowPos(window->hwnd, NULL, x, y, width, height, SWP_FRAMECHANGED | SWP_SHOWWINDOW))
			pgWin32Die("Failed to change fullscreen window style.");
	}
	else if (!ShowWindow(window->hwnd, SW_SHOWMAXIMIZED))
		pgWin32Die("Failed to maximize fullscreen window.");
}

pgVoid pgChangeToWindowedModeCore(pgWindow* window)
{ 
	LONG_PTR style = GetWindowLongPtr(window->hwnd, GWL_STYLE);
	style |= WS_CAPTION | WS_THICKFRAME | WS_SYSMENU | WS_OVERLAPPEDWINDOW;

	if (!SetWindowLongPtr(window->hwnd, GWL_STYLE, style))
		pgWin32Die("Failed to set new window style.");

	if (!ShowWindow(window->hwnd, SW_RESTORE))
		pgWin32Die("Failed to get window into normal mode.");
}

pgVoid pgSetWindowTitleCore(pgWindow* window, pgString title)
{
	if (!SetWindowText(window->hwnd, title))
		pgWin32Die("Failed to set window title.");
}

pgVoid pgCaptureMouseCore(pgWindow* window)
{
	CenterCursor(window);
}

pgVoid pgReleaseMouseCore(pgWindow* window)
{
	PG_UNUSED(window);
}

pgVoid pgClipCursor(pgWindow* window, pgBool clip)
{
	if (clip)
	{
		RECT rect;
		if (!GetWindowRect(window->hwnd, &rect))
			pgWin32Die("Failed to get window position.");
		
		if (!ClipCursor(&rect))
			pgWin32Die("Failed to clip cursor to window bounds.");
	}
	else
	{
		if (!ClipCursor(NULL))
			pgWin32Die("Failed to release cursor.");
	}
}

pgVoid pgGetMousePositionCore(pgWindow* window, pgInt32* x, pgInt32* y)
{
	POINT point;

	GetCursorPos(&point);
	ScreenToClient(window->hwnd, &point);

	*x = point.x;
	*y = point.y;
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgRectangle pgGetDesktopArea()
{
	RECT rect;
	pgRectangle rectangle;

	if (!GetClientRect(GetDesktopWindow(), &rect))
		pgWin32Die("Failed to get desktop size.");

	rectangle.left = rect.left;
	rectangle.top = rect.top;
	rectangle.width = rect.right - rect.left;
	rectangle.height = rect.bottom - rect.top;

	return rectangle;
}

pgVoid pgCancelDeadCharacter()
{
	BYTE keyState[256];
	WCHAR buffer[8];
	if (!GetKeyboardState(keyState))
		pgWin32Die("Failed to get keyboard state.");

	// Clear the internal keyboard buffer so that the next WM_CHAR message is not influenced by the dead key
	ToUnicode(VK_SPACE, 39, keyState, buffer, sizeof(buffer) / sizeof(WCHAR), 0);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Initialize()
{
	RAWINPUTDEVICE device;

	RegisterWindowClass(WndClassName, WndProc);
	
	// Register the keyboard raw input device.
	device.usUsagePage = 0x01; // keyboard
	device.usUsage = 0x06; // keyboard
	device.dwFlags = 0;
	device.hwndTarget = NULL;

	if (!RegisterRawInputDevices(&device, 1, sizeof(RAWINPUTDEVICE)))
		pgWin32Die("Failed to register keyboard raw input device.");
}

static pgVoid Shutdown()
{
	PG_ASSERT(state.openWindows == 0, "There should be no open windows left.");

	if (!UnregisterClass(WndClassName, GetModuleHandle(NULL)))
		pgWin32Die("Unable to unregister window class.");
}

static pgVoid RegisterWindowClass(pgString className, WNDPROC wndProc)
{
	WNDCLASS wndClass;

	memset(&wndClass, 0, sizeof(WNDCLASS));
	wndClass.style = CS_DBLCLKS;
	wndClass.lpfnWndProc = wndProc;
	wndClass.cbClsExtra = 0;
	wndClass.cbWndExtra = 0;
	wndClass.hInstance = GetModuleHandle(NULL);
	wndClass.hIcon = NULL;
	wndClass.hCursor = NULL;
	wndClass.hbrBackground = (HBRUSH)GetStockObject (BLACK_BRUSH);
	wndClass.lpszMenuName = NULL;
	wndClass.lpszClassName = (LPCSTR)className;

	if (RegisterClass(&wndClass) == 0)
		pgWin32Die("Unable to register window class.");
}

static pgVoid HandleWindowMessages(HWND hwnd)
{
	MSG message;

	state.message.type = PG_MESSAGE_INVALID;
	while (state.message.type == PG_MESSAGE_INVALID && PeekMessage(&message, hwnd, 0, 0, PM_REMOVE))
	{
		memset(&state.message, 0, sizeof(state.message));
		TranslateMessage(&message);
		DispatchMessage(&message);
	}
}

static LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	pgMessage* message = &state.message;
	pgWindow* window;

	if (msg == WM_CREATE)
	{
		// Get the pgWindow instance that was passed as the last argument of CreateWindow
		window = (pgWindow*)((CREATESTRUCT*)lParam)->lpCreateParams;

		// Set the "user data" parameter of the window and set the window's hwnd
		SetWindowLongPtr(hwnd, GWLP_USERDATA, (LONG_PTR)window);
		window->hwnd = hwnd;
	}

	window = (pgWindow*)GetWindowLongPtr(hwnd, GWLP_USERDATA);

	if (window == NULL)
		return DefWindowProc(hwnd, msg, wParam, lParam);
	
	switch (msg)
	{
	case WM_SETCURSOR:
		// The mouse has moved, so if the cursor is in our window we must refresh the cursor
		if (LOWORD(lParam) == HTCLIENT)
			SetCursor(NULL);
		break;

	case WM_CLOSE:
		// Do not forward the message to the default wnd proc, as we only want the window to be 
		// closed explicitly by the client application
		message->type = PG_MESSAGE_CLOSING;
		return 0;

	case WM_GETMINMAXINFO:
		{
			MINMAXINFO* info = (MINMAXINFO*)lParam;
			RECT minRect = { 0, 0, PG_WINDOW_MIN_WIDTH, PG_WINDOW_MIN_HEIGHT };
			RECT maxRect = { 0, 0, PG_WINDOW_MAX_WIDTH, PG_WINDOW_MAX_HEIGHT };

			if (!AdjustWindowRect(&minRect, (DWORD)GetWindowLongPtr(window->hwnd, GWL_STYLE), PG_FALSE))
				pgWin32Die("Unable to get minimum window size.");

			if (!AdjustWindowRect(&maxRect, (DWORD)GetWindowLongPtr(window->hwnd, GWL_STYLE), PG_FALSE))
				pgWin32Die("Unable to get maximum window size.");

			// Set the minimum and maximum allowed window sizes
			info->ptMaxTrackSize.x = maxRect.right - maxRect.left;
			info->ptMaxTrackSize.y = maxRect.bottom - maxRect.top;
			info->ptMinTrackSize.x = minRect.right - minRect.left;
			info->ptMinTrackSize.y = minRect.bottom - minRect.top;
		}
		break;

	// Check WM_ACTIVATE, WM_NCACTIVATE, WM_ACTIVATEAPP in order to ensure that we do not miss an activation or deactivation
	case WM_ACTIVATE:
		if (LOWORD(wParam) == WA_INACTIVE)
			message->type = PG_MESSAGE_LOST_FOCUS;
		else
			message->type = PG_MESSAGE_GAINED_FOCUS;
		break;

	case WM_NCACTIVATE:
	case WM_ACTIVATEAPP:
		if (!wParam)
			message->type = PG_MESSAGE_LOST_FOCUS;
		else
			message->type = PG_MESSAGE_GAINED_FOCUS;
		break;

	case WM_MOUSEMOVE:
		if (window->mouseCaptured)
			CenterCursor(window);

		// If the cursor is entering the window, raise the mouse entered event and tell Windows to inform
		// us when the cursor leaves the window.
		if (!window->cursorInside)
		{
			TRACKMOUSEEVENT mouseEvent;
			mouseEvent.cbSize = sizeof(TRACKMOUSEEVENT);
			mouseEvent.hwndTrack = window->hwnd;
			mouseEvent.dwFlags = TME_LEAVE;
			TrackMouseEvent(&mouseEvent);

			window->cursorInside = PG_TRUE;
			message->type = PG_MESSAGE_MOUSE_ENTERED;
			break;
		}

		message->type = PG_MESSAGE_MOUSE_MOVED;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_MOUSELEAVE:
		window->cursorInside = PG_FALSE;
		message->type = PG_MESSAGE_MOUSE_LEFT;
		break;

	case WM_LBUTTONDBLCLK:
		message->doubleClick = PG_TRUE;
		/* fall-through */

	case WM_LBUTTONDOWN:
		message->type = PG_MESSAGE_MOUSE_DOWN;
		message->button = PG_MOUSE_LEFT;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_LBUTTONUP:
		message->type = PG_MESSAGE_MOUSE_UP;
		message->button = PG_MOUSE_LEFT;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_RBUTTONDBLCLK:
		message->doubleClick = PG_TRUE;
		/* fall-through */

	case WM_RBUTTONDOWN:
		message->type = PG_MESSAGE_MOUSE_DOWN;
		message->button = PG_MOUSE_RIGHT;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_RBUTTONUP:
		message->type = PG_MESSAGE_MOUSE_UP;
		message->button = PG_MOUSE_RIGHT;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_MBUTTONDBLCLK:
		message->doubleClick = PG_TRUE;
		/* fall-through */

	case WM_MBUTTONDOWN:
		message->type = PG_MESSAGE_MOUSE_DOWN;
		message->button = PG_MOUSE_MIDDLE;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_MBUTTONUP:
		message->type = PG_MESSAGE_MOUSE_UP;
		message->button = PG_MOUSE_MIDDLE;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_XBUTTONDBLCLK:
		message->doubleClick = PG_TRUE;
		/* fall-through */

	case WM_XBUTTONDOWN:
		message->type = PG_MESSAGE_MOUSE_DOWN;
		message->button = HIWORD(wParam) == XBUTTON1 ? PG_MOUSE_X_BUTTON1 : PG_MOUSE_X_BUTTON2;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_XBUTTONUP:
		message->type = PG_MESSAGE_MOUSE_UP;
		message->button = HIWORD(wParam) == XBUTTON1 ? PG_MOUSE_X_BUTTON1 : PG_MOUSE_X_BUTTON2;
		message->x = LOWORD(lParam);
		message->y = HIWORD(lParam);
		break;

	case WM_MOUSEWHEEL:
		message->type = PG_MESSAGE_MOUSE_WHEEL;
		message->delta = GET_WHEEL_DELTA_WPARAM(wParam) / 120;
		break;

	case WM_SYSCOMMAND:
		if (wParam == SC_KEYMENU)
			return 0;
		break;

	case WM_DEADCHAR:
		message->type = PG_MESSAGE_DEAD_CHARACTER_ENTERED;
		message->character = (pgUInt16)wParam;
		message->scanCode = (pgInt32)((pgByte*)&lParam)[2];
		break;

	case WM_CHAR:
		message->type = PG_MESSAGE_CHARACTER_ENTERED;
		message->character = (pgUInt16)wParam;
		message->scanCode = (pgInt32)((pgByte*)&lParam)[2];
		break;

	case WM_INPUT:
		HandleKeyboardInput(lParam);
	}

	return DefWindowProc(hwnd, msg, wParam, lParam);
}

static LRESULT HandleKeyboardInput(LPARAM lParam)
{
	pgMessage* message = &state.message;
	RAWINPUT input;
	UINT size;
	UINT outSize;

	size = sizeof(RAWINPUT);
	outSize = GetRawInputData((HRAWINPUT)lParam, RID_INPUT, &input, &size, sizeof(RAWINPUTHEADER));

	if (outSize == -1)
		pgWin32Die("Failed to read raw keyboard input.");

	// Extract keyboard raw input data; see also http://molecularmusings.wordpress.com/2011/09/05/properly-handling-keyboard-input/
	if (input.header.dwType == RIM_TYPEKEYBOARD)
	{
		UINT virtualKey = input.data.keyboard.VKey;
		UINT scanCode = input.data.keyboard.MakeCode;
		USHORT flags = input.data.keyboard.Flags;
		pgBool released, isE0, isE1;
		pgKey key;

		// A key can either produce a "make" or "break" scan code. this is used to differentiate between
		// down-presses and releases; see http://www.win.tue.nl/~aeb/linux/kbd/scancodes-1.html
		released = (flags & RI_KEY_BREAK) != 0;

		// Discard "fake keys" which are part of an escaped sequence.
		if (virtualKey == 255)
			return 0;

		// Correct left-hand / right-hand SHIFT.
		if (virtualKey == VK_SHIFT)
			virtualKey = MapVirtualKey(scanCode, MAPVK_VSC_TO_VK_EX);
			// Correct PAUSE/BREAK and NUM LOCK silliness, and set the extended bit
		else if (virtualKey == VK_NUMLOCK)
			scanCode = (MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC) | 0x100);

		// e0 and e1 are escape sequences used for certain special keys, such as PRINT and PAUSE/BREAK
		// See http://www.win.tue.nl/~aeb/linux/kbd/scancodes-1.html
		isE0 = (flags & RI_KEY_E0) != 0;
		isE1 = (flags & RI_KEY_E1) != 0;

		if (isE1)
		{
			// For escaped sequences, turn the virtual key into the correct scan code using the MapVirtualKey function.
			// However, MapVirtualKey is unable to map VK_PAUSE, hence we map that by hand
			if (virtualKey == VK_PAUSE)
				scanCode = 0x45;
			else
				scanCode = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC);
		}

		key = PG_KEY_UNKNOWN;
		switch (virtualKey)
		{
		// Right-hand CONTROL and ALT have their e0 bit set
		case VK_CONTROL:
			key = isE0 ? PG_KEY_RIGHT_CONTROL : PG_KEY_LEFT_CONTROL;
			break;
		case VK_MENU:
			key = isE0 ? PG_KEY_RIGHT_ALT : PG_KEY_LEFT_ALT;
			break;
		// NUMPAD ENTER has its e0 bit set
		case VK_RETURN:
			key = isE0 ? PG_KEY_NUMPAD_ENTER : PG_KEY_RETURN;
			break;
		// The standard INSERT, DELETE, HOME, END, PRIOR and NEXT keys will always have their e0 bit 
		// set, but the corresponding keys on the NUMPAD will not
		case VK_INSERT:
			key = !isE0 ? PG_KEY_NUMPAD0 : PG_KEY_INSERT;
			break;
		case VK_DELETE:
			key = !isE0 ? PG_KEY_NUMPAD_DECIMAL : PG_KEY_DELETE;
			break;
		case VK_HOME:
			key = !isE0 ? PG_KEY_NUMPAD7 : PG_KEY_HOME;
			break;
		case VK_END:
			key = !isE0 ? PG_KEY_NUMPAD1 : PG_KEY_END;
			break;
		case VK_PRIOR:
			key = !isE0 ? PG_KEY_NUMPAD9 : PG_KEY_PAGE_UP;
			break;
		case VK_NEXT:
			key = !isE0 ? PG_KEY_NUMPAD3 : PG_KEY_PAGE_DOWN;
			break;
		// The standard arrow keys will always have their e0 bit set, but the corresponding keys on 
		// the NUMPAD will not
		case VK_LEFT:
			key = !isE0 ? PG_KEY_NUMPAD4 : PG_KEY_LEFT;
			break;
		case VK_RIGHT:
			key = !isE0 ? PG_KEY_NUMPAD6 : PG_KEY_RIGHT;
			break;
		case VK_UP:
			key = !isE0 ? PG_KEY_NUMPAD8 : PG_KEY_UP;
			break;
		case VK_DOWN:
			key = !isE0 ? PG_KEY_NUMPAD2 : PG_KEY_DOWN;
			break;
		// NUMPAD 5 doesn't have its e0 bit set
		case VK_CLEAR:
			if (!isE0)
				key = PG_KEY_NUMPAD5;
			break;
		default:
			key = TranslateKey(virtualKey);
			break;
		}

		message->type = released ? PG_MESSAGE_KEY_UP : PG_MESSAGE_KEY_DOWN;
		message->key = key;
		message->scanCode = scanCode;
	}

	return 0;
}

static pgVoid CenterCursor(pgWindow* window)
{
	POINT point, current;
	
	point.x = window->placement.width / 2;
	point.y = window->placement.height / 2;

	GetCursorPos(&current);
	ScreenToClient(window->hwnd, &current);

	if (current.x == point.x && current.y == point.y)
		return;

	ClientToScreen(window->hwnd, &point);
	SetCursorPos(point.x, point.y);
}

static pgKey TranslateKey(UINT virtualKey)
{
	switch (virtualKey)
	{
	case VK_OEM_102:
		return PG_KEY_BACK_SLASH2;
	case VK_SCROLL:
		return PG_KEY_SCROLL;
	case VK_SNAPSHOT:
		return PG_KEY_PRINT;
	case VK_NUMLOCK:
		return PG_KEY_NUM_LOCK;
	case VK_DECIMAL:
		return PG_KEY_NUMPAD_DECIMAL;
	case VK_LSHIFT:
		return PG_KEY_LEFT_SHIFT;
	case VK_RSHIFT:
		return PG_KEY_RIGHT_SHIFT;
	case VK_LWIN:
		return PG_KEY_LEFT_SYSTEM;
	case VK_RWIN:
		return PG_KEY_RIGHT_SYSTEM;
	case VK_APPS:
		return PG_KEY_MENU;
	case VK_OEM_1:
		return PG_KEY_SEMICOLON;
	case VK_OEM_2:
		return PG_KEY_SLASH;
	case VK_OEM_PLUS:
		return PG_KEY_EQUAL;
	case VK_OEM_MINUS:
		return PG_KEY_DASH;
	case VK_OEM_4:
		return PG_KEY_LEFT_BRACKET;
	case VK_OEM_6:
		return PG_KEY_RIGHT_BRACKET;
	case VK_OEM_COMMA:
		return PG_KEY_COMMA;
	case VK_OEM_PERIOD:
		return PG_KEY_PERIOD;
	case VK_OEM_7:
		return PG_KEY_QUOTE;
	case VK_OEM_5:
		return PG_KEY_BACK_SLASH;
	case VK_OEM_3:
		return PG_KEY_GRAVE;
	case VK_ESCAPE:
		return PG_KEY_ESCAPE;
	case VK_SPACE:
		return PG_KEY_SPACE;
	case VK_RETURN:
		return PG_KEY_RETURN;
	case VK_BACK:
		return PG_KEY_BACK;
	case VK_TAB:
		return PG_KEY_TAB;
	case VK_PRIOR:
		return PG_KEY_PAGE_UP;
	case VK_NEXT:
		return PG_KEY_PAGE_DOWN;
	case VK_END:
		return PG_KEY_END;
	case VK_HOME:
		return PG_KEY_HOME;
	case VK_INSERT:
		return PG_KEY_INSERT;
	case VK_DELETE:
		return PG_KEY_DELETE;
	case VK_ADD:
		return PG_KEY_ADD;
	case VK_SUBTRACT:
		return PG_KEY_SUBTRACT;
	case VK_MULTIPLY:
		return PG_KEY_MULTIPLY;
	case VK_DIVIDE:
		return PG_KEY_DIVIDE;
	case VK_PAUSE:
		return PG_KEY_PAUSE;
	case VK_F1:
		return PG_KEY_F1;
	case VK_F2:
		return PG_KEY_F2;
	case VK_F3:
		return PG_KEY_F3;
	case VK_F4:
		return PG_KEY_F4;
	case VK_F5:
		return PG_KEY_F5;
	case VK_F6:
		return PG_KEY_F6;
	case VK_F7:
		return PG_KEY_F7;
	case VK_F8:
		return PG_KEY_F8;
	case VK_F9:
		return PG_KEY_F9;
	case VK_F10:
		return PG_KEY_F10;
	case VK_F11:
		return PG_KEY_F11;
	case VK_F12:
		return PG_KEY_F12;
	case VK_F13:
		return PG_KEY_F13;
	case VK_F14:
		return PG_KEY_F14;
	case VK_F15:
		return PG_KEY_F15;
	case VK_LEFT:
		return PG_KEY_LEFT;
	case VK_RIGHT:
		return PG_KEY_RIGHT;
	case VK_UP:
		return PG_KEY_UP;
	case VK_DOWN:
		return PG_KEY_DOWN;
	case VK_CAPITAL:
		return PG_KEY_CAPS_LOCK;
	case VK_NUMPAD0:
		return PG_KEY_NUMPAD0;
	case VK_NUMPAD1:
		return PG_KEY_NUMPAD1;
	case VK_NUMPAD2:
		return PG_KEY_NUMPAD2;
	case VK_NUMPAD3:
		return PG_KEY_NUMPAD3;
	case VK_NUMPAD4:
		return PG_KEY_NUMPAD4;
	case VK_NUMPAD5:
		return PG_KEY_NUMPAD5;
	case VK_NUMPAD6:
		return PG_KEY_NUMPAD6;
	case VK_NUMPAD7:
		return PG_KEY_NUMPAD7;
	case VK_NUMPAD8:
		return PG_KEY_NUMPAD8;
	case VK_NUMPAD9:
		return PG_KEY_NUMPAD9;
	case 'A':
		return PG_KEY_A;
	case 'B':
		return PG_KEY_B;
	case 'C':
		return PG_KEY_C;
	case 'D':
		return PG_KEY_D;
	case 'E':
		return PG_KEY_E;
	case 'F':
		return PG_KEY_F;
	case 'G':
		return PG_KEY_G;
	case 'H':
		return PG_KEY_H;
	case 'I':
		return PG_KEY_I;
	case 'J':
		return PG_KEY_J;
	case 'K':
		return PG_KEY_K;
	case 'L':
		return PG_KEY_L;
	case 'M':
		return PG_KEY_M;
	case 'N':
		return PG_KEY_N;
	case 'O':
		return PG_KEY_O;
	case 'P':
		return PG_KEY_P;
	case 'Q':
		return PG_KEY_Q;
	case 'R':
		return PG_KEY_R;
	case 'S':
		return PG_KEY_S;
	case 'T':
		return PG_KEY_T;
	case 'U':
		return PG_KEY_U;
	case 'V':
		return PG_KEY_V;
	case 'W':
		return PG_KEY_W;
	case 'X':
		return PG_KEY_X;
	case 'Y':
		return PG_KEY_Y;
	case 'Z':
		return PG_KEY_Z;
	case '0':
		return PG_KEY_NUM0;
	case '1':
		return PG_KEY_NUM1;
	case '2':
		return PG_KEY_NUM2;
	case '3':
		return PG_KEY_NUM3;
	case '4':
		return PG_KEY_NUM4;
	case '5':
		return PG_KEY_NUM5;
	case '6':
		return PG_KEY_NUM6;
	case '7':
		return PG_KEY_NUM7;
	case '8':
		return PG_KEY_NUM8;
	case '9':
		return PG_KEY_NUM9;
	}

	PG_DEBUG("An unknown key was pressed. Virtual key code: '%d'.", virtualKey);
	return PG_KEY_UNKNOWN;
}

#endif
