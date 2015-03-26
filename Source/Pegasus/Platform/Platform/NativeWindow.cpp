#include "Prelude.hpp"

PG_DECLARE_WINDOWINTERFACE_API(NativeWindow, SDL)

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Helper functions
//-------------------------------------------------------------------------------------------------------------------------------------------------------

static void GetDesktopArea(int32* left, int32* top, int32* right, int32* bottom)
{
	*left = std::numeric_limits<int32>::max();
	*top = std::numeric_limits<int32>::max();
	*right = std::numeric_limits<int32>::min();
	*bottom = std::numeric_limits<int32>::min();

	auto num = SDL_GetNumVideoDisplays();

	if (num <= 0)
		PG_DIE("Failed to determine the number of displays: %s.", SDL_GetError());

	for (auto i = 0; i < num; ++i)
	{
		SDL_Rect bounds;
		if (SDL_GetDisplayBounds(i, &bounds) != 0)
			PG_DIE("Failed to retrieve display bounds of display %d: %s.", i, SDL_GetError());

		*left = bounds.x < *left ? bounds.x : *left;
		*right = bounds.x + bounds.w > *right ? bounds.x + bounds.w : *right;
		*top = bounds.y + *top >= bounds.y ? bounds.y : *top;
		*bottom = bounds.y + bounds.h > *bottom ? bounds.y + bounds.h : *bottom;
	}
}

static int32 Clamp(int32 value, int32 min, int32 max)
{
	if (value < min)
		return min;

	if (value > max)
		return max;

	return value;
}

static void ConstrainWindowPlacement(int32* x, int32* y, int32* width, int32* height)
{
	PG_ASSERT_IN_RANGE(*x, -NativeWindow::MaximumWidth, NativeWindow::MaximumWidth);
	PG_ASSERT_IN_RANGE(*y, -NativeWindow::MaximumHeight, NativeWindow::MaximumHeight);
	PG_ASSERT_LESS(*width, NativeWindow::MaximumWidth);
	PG_ASSERT_LESS(*height, NativeWindow::MaximumHeight);

	int32 desktopLeft, desktopTop, desktopRight, desktopBottom;
	GetDesktopArea(&desktopLeft, &desktopTop, &desktopRight, &desktopBottom);

	int32 desktopWidth = desktopRight - desktopLeft;
	int32 desktopHeight = desktopBottom - desktopTop;

	// The window's size must not exceed the size of the desktop
	*width = Clamp(*width, NativeWindow::MinimumWidth, desktopWidth);
	*height = Clamp(*height, NativeWindow::MinimumHeight, desktopHeight);

	// Move the window's desired position such that at least MinOverlap pixels of the window are visible 
	// both vertically and horizontally
	*x = Clamp(*x, desktopLeft - *width + NativeWindow::MinimumOverlap, desktopRight - NativeWindow::MinimumOverlap);
	*y = Clamp(*y, desktopTop - *height + NativeWindow::MinimumOverlap, desktopBottom - NativeWindow::MinimumOverlap);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Static initializers
//-------------------------------------------------------------------------------------------------------------------------------------------------------

const char* NativeWindow::WndPtr = "WndPtr";

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Window
//-------------------------------------------------------------------------------------------------------------------------------------------------------

NativeWindow::NativeWindow(WindowInterface* windowInterface, WindowCallbacks* callbacks)
	: Callbacks(*callbacks)
{
	PG_ASSERT_NOT_NULL(windowInterface);
	PG_ASSERT_NOT_NULL(callbacks);

	windowInterface->_this = this;
	PG_INITIALIZE_WINDOWINTERFACE(NativeWindow, SDL, windowInterface);
}

NativeWindow::~NativeWindow()
{
	SDL_DestroyWindow(_window);
}

void NativeWindow::Open(const char* title, int32 x, int32 y, int32 width, int32 height, WindowMode mode)
{
	ConstrainWindowPlacement(&x, &y, &width, &height);

	uint32 flags = SDL_WINDOW_RESIZABLE | SDL_WINDOW_OPENGL;
	switch (mode)
	{
	case WindowMode::Fullscreen:
		flags |= SDL_WINDOW_FULLSCREEN_DESKTOP | SDL_WINDOW_INPUT_GRABBED;
		break;
	case WindowMode::Maximized:
		flags |= SDL_WINDOW_MAXIMIZED;
		break;
	case WindowMode::Normal:
	case WindowMode::Minimized:
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	_window = SDL_CreateWindow(title, x, y, width, height, flags);
	if (_window == nullptr)
		PG_DIE("Failed to create window: %s.", SDL_GetError());

	SDL_SetWindowMinimumSize(_window, MinimumWidth, MinimumHeight);
	SDL_SetWindowMaximumSize(_window, MaximumWidth, MaximumHeight);
	SDL_SetWindowData(_window, WndPtr, this);
}

void NativeWindow::GetPosition(int32* x, int32* y)
{
	SDL_GetWindowPosition(_window, x, y);
}

void NativeWindow::GetSize(int32* width, int32* height)
{
	SDL_GetWindowSize(_window, width, height);
}

WindowMode NativeWindow::GetMode()
{
	auto flags = static_cast<SDL_WindowFlags>(SDL_GetWindowFlags(_window));

	if (Enum::HasFlag(flags, SDL_WINDOW_FULLSCREEN_DESKTOP))
		return WindowMode::Fullscreen;

	if (Enum::HasFlag(flags, SDL_WINDOW_MAXIMIZED))
		return WindowMode::Maximized;

	if (Enum::HasFlag(flags, SDL_WINDOW_MINIMIZED))
		return WindowMode::Minimized;

	return WindowMode::Normal;
}

#ifdef PG_SYSTEM_WINDOWS
HWND NativeWindow::GetHWND() const
{
	SDL_SysWMinfo info;
	SDL_VERSION(&info.version);

	if (!SDL_GetWindowWMInfo(_window, &info))
		PG_DIE("Failed to get the native window handle: %s.", SDL_GetError());

	PG_ASSERT(info.subsystem == SDL_SYSWM_WINDOWS, "Expected a HWND.");
	return info.info.win.window;
}
#endif

bool NativeWindow::HasFocus()
{
	return Focused;
}

bool NativeWindow::IsClosing()
{
	auto isClosing = ShouldClose;
	ShouldClose = false;
	return isClosing;
}

void NativeWindow::ChangeToFullscreenMode()
{
	PG_ASSERT(!Enum::HasFlag(static_cast<SDL_WindowFlags>(SDL_GetWindowFlags(_window)), SDL_WINDOW_FULLSCREEN_DESKTOP),
			  "Window is already in fullscreen mode.");

	if (SDL_SetWindowFullscreen(_window, SDL_WINDOW_FULLSCREEN_DESKTOP) != 0)
		PG_DIE("Failed to switch to fullscreen mode: %s.", SDL_GetError());

	SDL_SetWindowGrab(_window, SDL_TRUE);
}

void NativeWindow::ChangeToWindowedMode()
{
	PG_ASSERT(Enum::HasFlag(static_cast<SDL_WindowFlags>(SDL_GetWindowFlags(_window)), SDL_WINDOW_FULLSCREEN_DESKTOP),
			  "Window is already in windowed mode.");

	if (SDL_SetWindowFullscreen(_window, 0) != 0)
		PG_DIE("Failed to switch to windowed mode: %s.", SDL_GetError());

	SDL_SetWindowGrab(_window, SDL_FALSE);;
}

SDL_Window* NativeWindow::GetSDLWindow()
{
	return _window;
}
