#include "prelude.h"

#ifdef WINDOWS

//====================================================================================================================
// Helper functions, defines, and state
//====================================================================================================================

#define WndClassName "PegasusWindowClass"
#define InputWndClassName "PegasusInputWindowClass"

static struct WindowState
{
	pgInt32		openWindows;
	HWND		inputWindow;
	pgWindow*	activeWindow;
} windowState;

static pgVoid Initialize();
static pgVoid Shutdown();
static pgVoid RegisterWindowClass(pgString className, WNDPROC wndProc);
static pgVoid HandleWindowMessages(HWND hwnd);
static pgVoid CenterCursor(pgWindow* window);
static LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
static LRESULT CALLBACK InputWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
static pgKey TranslateKey(UINT virtualKey);

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgOpenWindowCore(pgWindow* window)
{
	RECT clientRect;
	LONG width, height;
	pgUint32 style = WS_VISIBLE | WS_CAPTION | WS_MINIMIZEBOX | WS_THICKFRAME | WS_MAXIMIZEBOX | WS_SYSMENU | WS_POPUP;

	if (windowState.openWindows++ == 0)
		Initialize();

	// Set the client size to the given width and height
	clientRect.left = 0;
	clientRect.top = 0;
	clientRect.right = window->params.width;
	clientRect.bottom = window->params.height;

	if (!AdjustWindowRect(&clientRect, style, PG_FALSE))
		pgWin32Error("Failed to adjust window rectangle.");

	width = clientRect.right - clientRect.left;
	height = clientRect.bottom - clientRect.top;

	// Create the window
	window->hwnd = CreateWindowEx(0, WndClassName, window->params.title, style, CW_USEDEFAULT, 
		CW_USEDEFAULT, width, height, NULL, NULL, GetModuleHandle(NULL), window);

	if (window->hwnd == NULL)
		pgWin32Error("Failed to open window.");

	window->cursor = LoadCursor(NULL, IDC_ARROW);
	if (window->cursor == NULL)
		pgWin32Error("Failed to initialize the mouse cursor.");
}

pgVoid pgCloseWindowCore(pgWindow* window)
{
	PG_ASSERT(windowState.openWindows > 0, "There are no open windows.");

	ShowCursor(PG_TRUE);

	if (window->hwnd != NULL && !DestroyWindow(window->hwnd))
		pgWin32Error("Failed to destroy window.");

	if (--windowState.openWindows == 0)
		Shutdown();

	if (windowState.activeWindow == window)
		windowState.activeWindow = NULL;

	window->hwnd = NULL;
}

pgVoid pgProcessWindowEventsCore(pgWindow* window)
{
	HandleWindowMessages(windowState.inputWindow);
	HandleWindowMessages(window->hwnd);
}

pgVoid pgGetWindowSizeCore(pgWindow* window, pgInt32* width, pgInt32* height)
{
	RECT rect;
	GetClientRect(window->hwnd, &rect);
    *width = rect.right - rect.left;
	*height = rect.bottom - rect.top;
}

pgVoid pgSetWindowSizeCore(pgWindow* window, pgInt32 width, pgInt32 height)
{
	// SetWindowPos wants the total size of the window (including title bar and borders),
	// so we have to compute it
	RECT rect;

	rect.left = 0;
	rect.top = 0;
	rect.right = width;
	rect.bottom = height;

	if (!AdjustWindowRect(&rect, GetWindowLong(window->hwnd, GWL_STYLE), PG_FALSE))
		pgWin32Error("Failed to calculate new window size.");

	width = rect.right - rect.left;
	height = rect.bottom - rect.top;

	if (!SetWindowPos(window->hwnd, NULL, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER))
		pgWin32Error("Failed to resize window.");
}

pgVoid pgSetWindowTitleCore(pgWindow* window, pgString title)
{
	if (!SetWindowText(window->hwnd, title))
		pgWin32Error("Failed to set window title.");
}

pgVoid pgCaptureMouseCore(pgWindow* window)
{
	CenterCursor(window);

	window->cursor = NULL;
	SetCursor(window->cursor);
}

pgVoid pgReleaseMouseCore(pgWindow* window)
{
	window->cursor = LoadCursor(NULL, IDC_ARROW);
	SetCursor(window->cursor);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Initialize()
{
	RAWINPUTDEVICE device;

	RegisterWindowClass(InputWndClassName, InputWndProc);
	RegisterWindowClass(WndClassName, WndProc);
	
	// Open the hidden input window
	windowState.inputWindow = CreateWindowEx(0, InputWndClassName, "", WS_POPUP | WS_DISABLED, 0, 0, 1, 1,
		NULL, NULL, GetModuleHandle(NULL), NULL);

    if (windowState.inputWindow == NULL)
        pgWin32Error("Failed to initialize the input initialization window.");

    ShowWindow(windowState.inputWindow, SW_HIDE);

	// Register the keyboard raw input device.
	device.usUsagePage = 0x01; // keyboard
	device.usUsage = 0x06; // keyboard
	device.dwFlags = 0;
	device.hwndTarget = windowState.inputWindow;

	if (!RegisterRawInputDevices(&device, 1, sizeof(RAWINPUTDEVICE)))
		pgWin32Error("Failed to register keyboard raw input device.");
}

static pgVoid Shutdown()
{
	PG_ASSERT(windowState.openWindows == 0, "There should be no open windows left.");

	if (windowState.inputWindow != NULL && !DestroyWindow(windowState.inputWindow))
		pgWin32Error("Failed to destroy input window.");

	if (!UnregisterClass(WndClassName, GetModuleHandle(NULL)))
		pgWin32Error("Unable to unregister window class.");

	if (!UnregisterClass(InputWndClassName, GetModuleHandle(NULL)))
		pgWin32Error("Unable to unregister input window class.");
}

static pgVoid RegisterWindowClass(pgString className, WNDPROC wndProc)
{
	WNDCLASS wndClass;

	memset(&wndClass, 0, sizeof(WNDCLASS));
	wndClass.style = 0;
	wndClass.lpfnWndProc = wndProc;
	wndClass.cbClsExtra = 0;
	wndClass.cbWndExtra = 0;
	wndClass.hInstance = GetModuleHandle(NULL);
	wndClass.hIcon = NULL;
	wndClass.hCursor = NULL;
	wndClass.hbrBackground = NULL;
	wndClass.lpszMenuName = NULL;
	wndClass.lpszClassName = (LPCSTR)className;

	if (RegisterClass(&wndClass) == 0)
		pgWin32Error("Unable to register window class.");
}

static pgVoid HandleWindowMessages(HWND hwnd)
{
	MSG message;
	while (PeekMessage(&message, hwnd, 0, 0, PM_REMOVE))
	{
		TranslateMessage(&message);
		DispatchMessage(&message);
	}
}

static pgVoid CenterCursor(pgWindow* window)
{
	POINT point, current;
	pgInt32 width, height;

	pgGetWindowSize(window, &width, &height);
	point.x = width / 2;
	point.y = height / 2;

	GetCursorPos(&current);
	ScreenToClient(window->hwnd, &current);

	if (current.x == point.x && current.y == point.y)
		return;

	ClientToScreen(window->hwnd, &point);
	SetCursorPos(point.x, point.y);
}

static LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	pgWindow* window;
	pgWindowParams* params;

	if (msg == WM_CREATE)
	{
		// Get pgWindow instance that was passed as the last argument of CreateWindow
        LONG_PTR window = (LONG_PTR)((CREATESTRUCT*)lParam)->lpCreateParams;

        // Set as the "user data" parameter of the window
        SetWindowLongPtr(hwnd, GWLP_USERDATA, window);
	}

	window = (pgWindow*)GetWindowLongPtr(hwnd, GWLP_USERDATA);
	params = &window->params;

	if (window == NULL)
		return DefWindowProc(hwnd, msg, wParam, lParam);

	switch (msg)
	{
	case WM_SETCURSOR:
		// The mouse has moved, so if the cursor is in our window we must refresh the cursor
		if (LOWORD(lParam) == HTCLIENT)
			SetCursor(window->cursor);
		break;
	case WM_CLOSE:
		if (params->closing != NULL)
			params->closing();

		// Ignore WM_CLOSE messages, as we only want the window to be closed explicitly 
		return 0;
	case WM_SIZE:
		// Ignore size events triggered by a minimize (size == 0 in that case)
		if (params->resized != NULL && wParam != SIZE_MINIMIZED)
		{
			// Get the new size
			RECT rectangle;
			GetClientRect(window->hwnd, &rectangle);
			params->resized(rectangle.right - rectangle.left, rectangle.bottom - rectangle.top);
		}
		break;
	case WM_SETFOCUS:
		windowState.activeWindow = window;
		if (params->gainedFocus != NULL)
			params->gainedFocus();
		break;
	case WM_KILLFOCUS:
		if (params->lostFocus != NULL)
			params->lostFocus();
		break;
	case WM_MOUSEMOVE:
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
			if (params->mouseEntered != NULL)
				params->mouseEntered();
		}

		if (params->mouseMoved != NULL)
			params->mouseMoved(LOWORD(lParam), HIWORD(lParam));

		if (window->mouseCaptured)
			CenterCursor(window);

		break;
	case WM_MOUSELEAVE:
		window->cursorInside = PG_FALSE;
		if (params->mouseLeft != NULL)
			params->mouseLeft();
		break;
	case WM_LBUTTONDOWN:
		if (params->mousePressed != NULL)
			params->mousePressed(PG_MOUSE_LEFT, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_LBUTTONUP:
		if (params->mouseReleased != NULL)
			params->mouseReleased(PG_MOUSE_LEFT, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_RBUTTONDOWN:
		if (params->mousePressed != NULL)
			params->mousePressed(PG_MOUSE_RIGHT, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_RBUTTONUP:
		if (params->mouseReleased != NULL)
			params->mouseReleased(PG_MOUSE_RIGHT, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_MBUTTONDOWN:
		if (params->mousePressed != NULL)
			params->mousePressed(PG_MOUSE_MIDDLE, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_MBUTTONUP:
		if (params->mouseReleased != NULL)
			params->mouseReleased(PG_MOUSE_MIDDLE, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_XBUTTONDOWN:
		if (params->mousePressed != NULL)
			params->mousePressed(HIWORD(wParam) == XBUTTON1? PG_MOUSE_XBUTTON1 : PG_MOUSE_XBUTTON2, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_XBUTTONUP:
		if (params->mouseReleased != NULL)
			params->mouseReleased(HIWORD(wParam) == XBUTTON1? PG_MOUSE_XBUTTON1 : PG_MOUSE_XBUTTON2, LOWORD(lParam), HIWORD(lParam));
		break;
	case WM_MOUSEWHEEL:
		if (params->mouseWheel != NULL)
			params->mouseWheel(GET_WHEEL_DELTA_WPARAM(wParam) / 120);
		break;
	case WM_CHAR:
		// Ignore all non-printable characters below 32 (= single white-space). Otherwise, character entered 
		// events would also be raised for the enter and back space keys, for instance. The character that is
		// passed to the callback is UTF-16 encoded
		if ((pgUint16)wParam > 31 && params->characterEntered != NULL)
			params->characterEntered((pgUint16)wParam);
		break;
	}

	return DefWindowProc(hwnd, msg, wParam, lParam);
}

static LRESULT CALLBACK InputWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	pgWindowParams* params = &windowState.activeWindow->params;
	RAWINPUT input;
	UINT size;
	pgInt32 outSize;

	if (msg != WM_INPUT)
		return DefWindowProc(hwnd, msg, wParam, lParam);

	size = sizeof(RAWINPUT);
	outSize = GetRawInputData((HRAWINPUT)lParam, RID_INPUT, &input, &size, sizeof(RAWINPUTHEADER));

	if (outSize == -1)
		pgWin32Error("Failed to read raw keyboard input.");

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
			key = isE0 ? PG_KEY_RIGHTCONTROL : PG_KEY_LEFTCONTROL;
			break;
		case VK_MENU:
			key = isE0 ? PG_KEY_RIGHTALT : PG_KEY_LEFTALT;
			break;
			// NUMPAD ENTER has its e0 bit set
		case VK_RETURN:
			key = isE0 ? PG_KEY_NUMPADENTER : PG_KEY_RETURN;
			break;
			// The standard INSERT, DELETE, HOME, END, PRIOR and NEXT keys will always have their e0 bit 
			// set, but the corresponding keys on the NUMPAD will not
		case VK_INSERT:
			key = !isE0 ? PG_KEY_NUMPAD0 : PG_KEY_INSERT;
			break;
		case VK_DELETE:
			key = !isE0 ? PG_KEY_NUMPADDECIMAL : PG_KEY_DELETE;
			break;
		case VK_HOME:
			key = !isE0 ? PG_KEY_NUMPAD7 : PG_KEY_HOME;
			break;
		case VK_END:
			key = !isE0 ? PG_KEY_NUMPAD1 : PG_KEY_END;
			break;
		case VK_PRIOR:
			key = !isE0 ? PG_KEY_NUMPAD9 : PG_KEY_PAGEUP;
			break;
		case VK_NEXT:
			key = !isE0 ? PG_KEY_NUMPAD3 : PG_KEY_PAGEDOWN;
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

		// Raise the key event
		if (released && params->keyReleased != NULL)
			params->keyReleased(key, scanCode);
		else if (!released && params->keyPressed != NULL)
			params->keyPressed(key, scanCode);
	}

	return 0;
}

static pgKey TranslateKey(UINT virtualKey)
{
	switch (virtualKey)
	{
	case VK_OEM_102:
		return PG_KEY_BACKSLASH2;
	case VK_SCROLL:
		return PG_KEY_SCROLL;
	case VK_SNAPSHOT:
		return PG_KEY_PRINT;
	case VK_NUMLOCK:
		return PG_KEY_NUMLOCK;
	case VK_DECIMAL:
		return PG_KEY_NUMPADDECIMAL;
	case VK_LSHIFT:
		return PG_KEY_LEFTSHIFT;
	case VK_RSHIFT:
		return PG_KEY_RIGHTSHIFT;
	case VK_LWIN:
		return PG_KEY_LEFTSYSTEM;
	case VK_RWIN:
		return PG_KEY_RIGHTSYSTEM;
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
		return PG_KEY_LEFTBRACKET;
	case VK_OEM_6:
		return PG_KEY_RIGHTBRACKET;
	case VK_OEM_COMMA:
		return PG_KEY_COMMA;
	case VK_OEM_PERIOD:
		return PG_KEY_PERIOD;
	case VK_OEM_7:
		return PG_KEY_QUOTE;
	case VK_OEM_5:
		return PG_KEY_BACKSLASH;
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
		return PG_KEY_PAGEUP;
	case VK_NEXT:
		return PG_KEY_PAGEDOWN;
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
		return PG_KEY_CAPSLOCK;
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

	pgDebugInfo("An unknown key was pressed. Virtual key code: '%d'.", virtualKey);
	return PG_KEY_UNKNOWN;
}

#endif
