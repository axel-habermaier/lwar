#include "Prelude.hpp"

PG_API_EXPORT void EnableRelativeMouseMode(bool enable)
{
	if (enable && !SDL_GetRelativeMouseMode())
	{
		if (SDL_SetRelativeMouseMode(SDL_TRUE) != 0)
			PG_DIE("Failed to enable relative mouse mode: %s.", SDL_GetError());
	}
	else if (!enable && SDL_GetRelativeMouseMode())
	{
		if (SDL_SetRelativeMouseMode(SDL_FALSE) != 0)
			PG_DIE("Failed to disable relative mouse mode: %s.", SDL_GetError());

		auto window = SDL_GetMouseFocus();
		if (window == nullptr)
			return;

		int32 width, height;
		SDL_GetWindowSize(window, &width, &height);
		SDL_WarpMouseInWindow(window, width / 2, height / 2);
	}
}

PG_API_EXPORT bool IsRelativeMouseModeEnabled()
{
	return SDL_GetRelativeMouseMode() ? true : false;
}

PG_API_EXPORT void GetMousePosition(void* window, int32* x, int32* y)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(x);
	PG_ASSERT_NOT_NULL(y);

	*x = std::numeric_limits<int32>::max();
	*y = std::numeric_limits<int32>::max();

	auto sdlWindow = SDL_GetMouseFocus();
	if (sdlWindow == nullptr)
		return;

	if (SDL_GetWindowData(sdlWindow, NativeWindow::WndPtr) != window)
		return;

	SDL_GetMouseState(x, y);
}

PG_API_EXPORT void* CreateHardwareCursor(Surface* surface, int32 hotSpotX, int32 hotSpotY)
{
	PG_ASSERT_NOT_NULL(surface);
	PG_ASSERT_NOT_NULL(surface->Data);

	auto sdlSurface = SDL_CreateRGBSurfaceFrom(surface->Data, surface->Width, surface->Height, 32, surface->Stride,
											   0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

	if (sdlSurface == nullptr)
		PG_DIE("Failed to create surface for hardware cursor: %s.", SDL_GetError());

	auto cursor = SDL_CreateColorCursor(sdlSurface, hotSpotX, hotSpotY);
	if (cursor == nullptr)
		PG_DIE("Failed to create hardware cursor: %s.", SDL_GetError());

	SDL_FreeSurface(sdlSurface);
	return cursor;
}

PG_API_EXPORT void SetHardwareCursor(void* cursor)
{
	PG_ASSERT_NOT_NULL(cursor);
	SDL_SetCursor(static_cast<SDL_Cursor*>(cursor));
}

PG_API_EXPORT void ShowHardwareCursor(bool show)
{
	SDL_ShowCursor(show);
}

PG_API_EXPORT void FreeHardwareCursor(void* cursor)
{
	SDL_FreeCursor(static_cast<SDL_Cursor*>(cursor));
}