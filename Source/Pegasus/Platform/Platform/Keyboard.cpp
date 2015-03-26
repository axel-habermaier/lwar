#include "Prelude.hpp"

PG_API_EXPORT void EnableTextInput(bool enable)
{
	auto sdlWindow = SDL_GetKeyboardFocus();
	if (sdlWindow == nullptr)
		return;

	auto window = static_cast<NativeWindow*>(SDL_GetWindowData(sdlWindow, NativeWindow::WndPtr));
	if (window == nullptr)
		return;

	window->TextInputEnabled = enable;

	if (enable)
		SDL_StartTextInput();
	else
		SDL_StopTextInput();
}

PG_API_EXPORT bool IsTextInputEnabled()
{
	return SDL_IsTextInputActive() ? true : false;
}

PG_API_EXPORT void SetTextInputArea(int32 left, int32 top, int32 width, int32 height)
{
	SDL_Rect rect;
	rect.x = left;
	rect.y = top;
	rect.w = width;
	rect.h = height;

	SDL_SetTextInputRect(&rect);
}

PG_API_EXPORT uint32 GetScanCodeCount()
{
	return SDL_NUM_SCANCODES;
}

PG_API_EXPORT int32 KeyToScanCode(int32 key)
{
	return SDL_GetScancodeFromKey(key);
}