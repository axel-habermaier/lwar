#pragma once

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Represents a window that displays the application.
//-------------------------------------------------------------------------------------------------------------------------------------------------------
class NativeWindow
{
public:
	NativeWindow(WindowInterface* windowInterface, WindowCallbacks* callbacks);
	~NativeWindow();

	PG_DECLARE_WINDOWINTERFACE_METHODS

	static const int32 MinimumWidth = 800;
	static const int32 MinimumHeight = 600;
	static const int32 MaximumWidth = 4096;
	static const int32 MaximumHeight = 2160;
	static const uint16 MinimumOverlap = 100;
	static const char* WndPtr;

	bool TextInputEnabled = false;
	bool ShouldClose = false;
	bool Focused = false;

	int32 LastMouseX = 0;
	int32 LastMouseY = 0;

	WindowCallbacks Callbacks;

	SDL_Window* GetSDLWindow();
	PG_WINDOWS_ONLY(HWND GetHWND() const);

private:
	SDL_Window* _window = nullptr;
};
