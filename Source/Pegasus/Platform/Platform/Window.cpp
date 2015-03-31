#include "Prelude.hpp"

PG_API_EXPORT WindowInterface* CreateWindowInterface(WindowCallbacks* callbacks)
{
	PG_ASSERT_NOT_NULL(callbacks);
	PG_ASSERT_NOT_NULL(callbacks->TextEntered);
	PG_ASSERT_NOT_NULL(callbacks->KeyPressed);
	PG_ASSERT_NOT_NULL(callbacks->KeyReleased);
	PG_ASSERT_NOT_NULL(callbacks->MouseWheel);
	PG_ASSERT_NOT_NULL(callbacks->MousePressed);
	PG_ASSERT_NOT_NULL(callbacks->MouseReleased);
	PG_ASSERT_NOT_NULL(callbacks->MouseMoved);

	auto windowInterface = PG_NEW(WindowInterface);
	PG_NEW(NativeWindow, windowInterface, callbacks);

	return windowInterface;
}

PG_API_EXPORT void FreeWindowInterface(WindowInterface* windowInterface)
{
	if (windowInterface == nullptr)
		return;

	PG_DELETE(static_cast<NativeWindow*>(windowInterface->_this));
	PG_DELETE(windowInterface);
}

PG_API_EXPORT void GetSupportedWindowDimensions(int32* minWidth, int32* minHeight, int32* maxWidth, int32* maxHeight)
{
	*minWidth = NativeWindow::MinimumWidth;
	*minHeight = NativeWindow::MinimumHeight;
	*maxWidth = NativeWindow::MaximumWidth;
	*maxHeight = NativeWindow::MaximumHeight;
}

PG_API_EXPORT void ShowMessageBox(const char* caption, const char* message)
{
#ifdef PG_SYSTEM_WINDOWS
	// The native Windows message box looks much better...
	MessageBox(nullptr, message, caption, MB_ICONERROR | MB_OK);
#else
	if (SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, caption, message, nullptr) != 0)
		PG_ERROR("Failed to show message box: %s.", SDL_GetError());
#endif
}

PG_API_EXPORT void HandleEventMessages()
{
	auto getWindow = [](uint32 id)
	{
		return static_cast<NativeWindow*>(SDL_GetWindowData(SDL_GetWindowFromID(id), NativeWindow::WndPtr));
	};

	SDL_Event e;
	while (SDL_PollEvent(&e))
	{
		switch (e.type)
		{
		case SDL_WINDOWEVENT:
		{
			auto window = getWindow(e.window.windowID);
			if (window == nullptr)
				return;

			switch (e.window.event)
			{
			case SDL_WINDOWEVENT_SHOWN:
			case SDL_WINDOWEVENT_HIDDEN:
			case SDL_WINDOWEVENT_EXPOSED:
			case SDL_WINDOWEVENT_MINIMIZED:
			case SDL_WINDOWEVENT_MAXIMIZED:
			case SDL_WINDOWEVENT_RESTORED:
			case SDL_WINDOWEVENT_MOVED:
			case SDL_WINDOWEVENT_RESIZED:
			case SDL_WINDOWEVENT_ENTER:
			case SDL_WINDOWEVENT_LEAVE:
			case SDL_WINDOWEVENT_SIZE_CHANGED:
				// Don't care
				break;
			case SDL_WINDOWEVENT_FOCUS_GAINED:
				window->Focused = true;
				break;
			case SDL_WINDOWEVENT_FOCUS_LOST:
				window->Focused = false;
				break;
			case SDL_WINDOWEVENT_CLOSE:
				window->ShouldClose = true;
				break;
			default:
				PG_NO_SWITCH_DEFAULT;
			}

			break;
		}
		case SDL_KEYDOWN:
		{
			auto window = getWindow(e.key.windowID);
			if (window == nullptr)
				return;

			window->Callbacks.KeyPressed(e.key.keysym.sym, e.key.keysym.scancode);
			break;
		}
		case SDL_KEYUP:
		{
			auto window = getWindow(e.key.windowID);
			if (window == nullptr)
				return;

			window->Callbacks.KeyReleased(e.key.keysym.sym, e.key.keysym.scancode);
			break;
		}
		case SDL_TEXTINPUT:
		{
			auto window = getWindow(e.text.windowID);
			if (window == nullptr || !window->TextInputEnabled)
				return;

			window->Callbacks.TextEntered(reinterpret_cast<byte*>(e.text.text));
			break;
		}
		case SDL_MOUSEMOTION:
		{
			auto window = getWindow(e.motion.windowID);
			if (window == nullptr)
				return;

			if (SDL_GetRelativeMouseMode())
				window->Callbacks.MouseMoved(e.motion.xrel, e.motion.yrel);
			else
				window->Callbacks.MouseMoved(e.motion.x, e.motion.y);
			break;
		}
		case SDL_MOUSEBUTTONDOWN:
		{
			auto window = getWindow(e.button.windowID);
			if (window == nullptr)
				return;

			window->Callbacks.MousePressed(e.button.button, e.button.clicks == 2, e.button.x, e.button.y);
			break;
		}
		case SDL_MOUSEBUTTONUP:
		{
			auto window = getWindow(e.button.windowID);
			if (window == nullptr)
				return;

			window->Callbacks.MouseReleased(e.button.button, e.button.x, e.button.y);
			break;
		}
		case SDL_MOUSEWHEEL:
		{
			auto window = getWindow(e.wheel.windowID);
			if (window == nullptr)
				return;

			window->Callbacks.MouseWheel(e.wheel.y);
			break;
		}
		case SDL_TEXTEDITING:
		case SDL_QUIT:
			// Don't care
			break;
		default:
			PG_DEBUG("Unsupported SDL event.");
			break;
		}
	}
}
