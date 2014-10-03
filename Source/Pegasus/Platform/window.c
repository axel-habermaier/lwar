#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgReleaseButtonsAndKeys(pgWindow* window);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgWindow* pgOpenWindow(pgString title, pgWindowPlacement placement, pgWindowCallbacks callbacks)
{
	pgWindow* window;

	PG_ASSERT_NOT_NULL(title);
	PG_ASSERT_NOT_NULL(callbacks.characterEntered);
	PG_ASSERT_NOT_NULL(callbacks.deadCharacterEntered);
	PG_ASSERT_NOT_NULL(callbacks.keyPressed);
	PG_ASSERT_NOT_NULL(callbacks.keyReleased);
	PG_ASSERT_NOT_NULL(callbacks.mouseWheel);
	PG_ASSERT_NOT_NULL(callbacks.mousePressed);
	PG_ASSERT_NOT_NULL(callbacks.mouseReleased);
	PG_ASSERT_NOT_NULL(callbacks.mouseMoved);
	PG_ASSERT_NOT_NULL(callbacks.mouseEntered);
	PG_ASSERT_NOT_NULL(callbacks.mouseLeft);
	pgConstrainWindowPlacement(&placement);

	if (placement.mode == PG_WINDOW_MINIMIZED)
		placement.mode = PG_WINDOW_NORMAL;

	PG_ALLOC(pgWindow, window);
	window->callbacks = callbacks;
	window->placement = placement;

	pgOpenWindowCore(window, title);

	if (window->placement.mode == PG_WINDOW_FULLSCREEN)
	{
		pgChangeToFullscreenModeCore(window);
		pgClipCursor(window, PG_TRUE);
	}

	pgGetWindowPlacement(window, &window->placement);
	return window;
}

pgVoid pgCloseWindow(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);

	pgCloseWindowCore(window);
	PG_FREE(window);
}

pgVoid pgProcessWindowEvents(pgWindow* window)
{
	pgInt32 width, height;
	pgMessage message;

	PG_ASSERT_NOT_NULL(window);

	memset(&message, 0, sizeof(message));
	window->closing = PG_FALSE;

	while (pgProcessWindowEvent(window, &message))
	{
		switch (message.type)
		{
		case PG_MESSAGE_CLOSING:
			window->closing = PG_TRUE;
			break;
		
		case PG_MESSAGE_LOST_FOCUS:
			window->focused = PG_FALSE;

			// If we've lost the focus, make sure that all pressed keys are unpressed, because we'll miss the up event
			// if we're not focused; of course, if a key is not released while the window is out of focus and focused again,
			// the key state might be wrong; but this seems to be more acceptable than the alternative
			pgReleaseButtonsAndKeys(window);
			pgClipCursor(window, PG_FALSE);
			break;

		case PG_MESSAGE_GAINED_FOCUS:
			window->focused = PG_TRUE;

			// We sometimes do not get the lost focus event, so make sure we're now releasing all pressed buttons and keys
			pgReleaseButtonsAndKeys(window);
			pgClipCursor(window, window->placement.mode == PG_WINDOW_FULLSCREEN);
			break;

		/* Activate the window on all input-related events to ensure that we haven't missed the focused event */
		case PG_MESSAGE_CHARACTER_ENTERED:
			window->callbacks.characterEntered(message.character, message.scanCode);
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_DEAD_CHARACTER_ENTERED:
		{
			pgBool cancel = PG_FALSE;
			window->callbacks.deadCharacterEntered(message.character, message.scanCode, &cancel);
			window->focused = PG_TRUE;

			if (cancel)
				pgCancelDeadCharacter();
			break;
		}

		case PG_MESSAGE_KEY_UP:
			window->keyState[message.key] = PG_FALSE;
			window->callbacks.keyReleased(message.key, message.scanCode);
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_KEY_DOWN:
			window->keyState[message.key] = PG_TRUE;
			window->scanCode[message.key] = message.scanCode;
			window->callbacks.keyPressed(message.key, message.scanCode);
			break;

		case PG_MESSAGE_MOUSE_WHEEL:
			window->callbacks.mouseWheel(message.delta);
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_MOUSE_DOWN:
			window->buttonState[message.button] = PG_TRUE;
			window->callbacks.mousePressed(message.button, message.doubleClick, message.x, message.y);
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_MOUSE_UP:
			window->buttonState[message.button] = PG_FALSE;
			window->callbacks.mouseReleased(message.button, message.x, message.y);
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_MOUSE_MOVED:
			window->callbacks.mouseMoved(message.x, message.y);
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_MOUSE_ENTERED:
			window->callbacks.mouseEntered();
			window->focused = PG_TRUE;
			break;

		case PG_MESSAGE_MOUSE_LEFT:
			window->callbacks.mouseLeft();
			window->focused = PG_TRUE;
			break;

		default:
			PG_NO_SWITCH_DEFAULT;
			break;
		}

		memset(&message, 0, sizeof(message));
	}

	// Update the window placement and check whether the size has changed
	width = window->placement.width;
	height = window->placement.height;
	pgGetWindowPlacement(window, &window->placement);

	if (width != window->placement.width || height != window->placement.height)
	{
		pgClipCursor(window, window->focused && window->placement.mode == PG_WINDOW_FULLSCREEN);
		if (window->swapChain != NULL)
			pgResizeSwapChain(window->swapChain, window->placement.width, window->placement.height);
	}
}

pgBool pgIsWindowFocused(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);
	return window->focused;
}

pgBool pgIsWindowClosing(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);
	return window->closing;
}

pgVoid pgGetWindowPlacement(pgWindow* window, pgWindowPlacement* placement)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(placement);

	pgBool isFullscreen = window->placement.mode == PG_WINDOW_FULLSCREEN;

	pgGetWindowPlacementCore(window);
	*placement = window->placement;

	if (isFullscreen)
		placement->mode = PG_WINDOW_FULLSCREEN;
}

pgVoid pgSetWindowTitle(pgWindow* window, pgString title)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(title);

	pgSetWindowTitleCore(window, title);
}

pgVoid pgChangeToFullscreenMode(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT(window->placement.mode != PG_WINDOW_FULLSCREEN, "Window is already in fullscreen mode.");

	pgChangeToFullscreenModeCore(window);
	pgClipCursor(window, PG_TRUE);

	window->placement.mode = PG_WINDOW_FULLSCREEN;
	pgGetWindowPlacement(window, &window->placement);

	if (window->swapChain != NULL)
		pgResizeSwapChain(window->swapChain, window->placement.width, window->placement.height);
}

pgVoid pgChangeToWindowedMode(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT(window->placement.mode == PG_WINDOW_FULLSCREEN, "Window is already in windowed mode.");

	pgChangeToWindowedModeCore(window);
	pgClipCursor(window, PG_FALSE);

	window->placement.mode = PG_WINDOW_NORMAL;
	pgGetWindowPlacement(window, &window->placement);

	if (window->swapChain != NULL)
		pgResizeSwapChain(window->swapChain, window->placement.width, window->placement.height);
}

pgVoid pgCaptureMouse(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT(!window->mouseCaptured, "Mouse has already been captured.");

	window->mouseCaptured = PG_TRUE;
	pgCaptureMouseCore(window);
}

pgVoid pgReleaseMouse(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT(window->mouseCaptured, "Mouse has not been captured.");

	window->mouseCaptured = PG_FALSE;
	pgReleaseMouseCore(window);
}

pgVoid pgGetMousePosition(pgWindow* window, pgInt32* x, pgInt32* y)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(x);
	PG_ASSERT_NOT_NULL(y);

	pgGetMousePositionCore(window, x, y);
}

pgVoid pgGetMonitorResolution(pgWindow* window, pgInt32* width, pgInt32* height)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(width);
	PG_ASSERT_NOT_NULL(height);

	pgGetMonitorResolutionCore(window, width, height);
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgVoid pgConstrainWindowPlacement(pgWindowPlacement* placement)
{
	pgRectangle rect;

	PG_ASSERT_IN_RANGE(placement->x, -PG_WINDOW_MAX_WIDTH, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(placement->y, -PG_WINDOW_MAX_HEIGHT, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT_IN_RANGE(placement->width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(placement->height, 0, PG_WINDOW_MAX_HEIGHT);

	rect = pgGetDesktopArea();

	placement->x = pgClamp(placement->x, rect.left - placement->width + PG_WINDOW_MIN_OVERLAP, rect.left + rect.width - PG_WINDOW_MIN_OVERLAP);
	placement->y = pgClamp(placement->y, rect.top - placement->height + PG_WINDOW_MIN_OVERLAP, rect.top + rect.height - PG_WINDOW_MIN_OVERLAP);
	placement->width = pgClamp(placement->width, PG_WINDOW_MIN_WIDTH, rect.width);
	placement->height = pgClamp(placement->height, PG_WINDOW_MIN_HEIGHT, rect.height);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgReleaseButtonsAndKeys(pgWindow* window)
{
	pgInt32 i, x, y;

	// Make sure that all mouse buttons and keys are released (we might miss some events, for instance,
	// when the user uses Alt+Tab to switch to another window)
	for (i = 1; i < PG_KEY_COUNT - 1; ++i)
	{
		if (window->keyState[i])
		{
			window->keyState[i] = PG_FALSE;
			window->callbacks.keyReleased((pgKey)i, window->scanCode[i]);
		}
	}

	pgGetMousePosition(window, &x, &y);
	for (i = 1; i < PG_BUTTON_COUNT - 1; ++i)
	{
		if (window->buttonState[i])
		{
			window->buttonState[i] = PG_FALSE;
			window->callbacks.mouseReleased((pgMouseButton)i, x, y);
		}
	}
}