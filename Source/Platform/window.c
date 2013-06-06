#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgWindowKeyUp(pgWindow* window, pgKey key, pgInt32 scanCode);
static pgVoid pgWindowKeyDown(pgWindow* window, pgKey key, pgInt32 scanCode);
static pgVoid pgWindowButtonUp(pgWindow* window, pgMouseButton button, pgInt32 x, pgInt32 y);
static pgVoid pgWindowButtonDown(pgWindow* window, pgMouseButton button, pgInt32 x, pgInt32 y);
static pgVoid pgWindowLostFocus(pgWindow* window, pgInt32 x, pgInt32 y);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgWindow* pgOpenWindow(pgString title, pgWindowPlacement placement, pgWindowCallbacks callbacks)
{
	pgWindow* window;

	PG_ASSERT_NOT_NULL(title);
	PG_ASSERT_NOT_NULL(callbacks.characterEntered);
	PG_ASSERT_NOT_NULL(callbacks.keyPressed);
	PG_ASSERT_NOT_NULL(callbacks.keyReleased);
	PG_ASSERT_NOT_NULL(callbacks.mouseWheel);
	PG_ASSERT_NOT_NULL(callbacks.mousePressed);
	PG_ASSERT_NOT_NULL(callbacks.mouseReleased);
	PG_ASSERT_NOT_NULL(callbacks.mouseMoved);
	PG_ASSERT_NOT_NULL(callbacks.mouseEntered);
	PG_ASSERT_NOT_NULL(callbacks.mouseLeft);
	pgConstrainWindowPlacement(&placement);

	PG_ALLOC(pgWindow, window);
	window->callbacks = callbacks;
	window->placement = placement;

	pgOpenWindowCore(window, title);
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
		case PG_MESSAGE_GAINED_FOCUS:
			window->focused = PG_TRUE;
			if (window->swapChain != NULL)
				pgActivateSwapChain(window->swapChain, PG_TRUE);
			break;
		case PG_MESSAGE_LOST_FOCUS:
			{
				pgInt32 i, x, y;
				window->focused = PG_FALSE;

				if (window->swapChain != NULL)
					pgActivateSwapChain(window->swapChain, PG_FALSE);

				// Make sure that all mouse buttons and keys are released (we might miss some events, for instance,
				// when the user uses Alt+Tab to switch to another window)
				for (i = 0; i < PG_KEY_COUNT - 1; ++i)
				{
					if (window->keyState[i])
					{
						window->keyState[i] = PG_FALSE;
						window->callbacks.keyReleased((pgKey)(i + 1), window->scanCode[i]);
					}
				}

				pgGetMousePosition(window, &x, &y);
				for (i = 0; i < PG_BUTTON_COUNT - 1; ++i)
				{
					if (window->buttonState[i])
					{
						window->buttonState[i] = PG_FALSE;
						window->callbacks.mouseReleased((pgMouseButton)(i + 1), message.x, message.y);
					}
				}
			}
			break;
		case PG_MESSAGE_CHARACTER_ENTERED:
			window->callbacks.characterEntered(message.character, message.scanCode);
			break;
		case PG_MESSAGE_KEY_UP:
			window->keyState[message.key - 1] = PG_FALSE;
			window->callbacks.keyReleased(message.key, message.scanCode);
			break;
		case PG_MESSAGE_KEY_DOWN:
			window->keyState[message.key - 1] = PG_TRUE;
			window->scanCode[message.key - 1] = message.scanCode;
			window->callbacks.keyPressed(message.key, message.scanCode);
			break;
		case PG_MESSAGE_MOUSE_WHEEL:
			window->callbacks.mouseWheel(message.delta);
			break;
		case PG_MESSAGE_MOUSE_DOWN:
			window->buttonState[message.button - 1] = PG_TRUE;
			window->callbacks.mousePressed(message.button, message.doubleClick, message.x, message.y);
			break;
		case PG_MESSAGE_MOUSE_UP:
			window->buttonState[message.button - 1] = PG_FALSE;
			window->callbacks.mouseReleased(message.button, message.x, message.y);
			break;
		case PG_MESSAGE_MOUSE_MOVED:
			window->callbacks.mouseMoved(message.x, message.y);
			break;
		case PG_MESSAGE_MOUSE_ENTERED:
			window->callbacks.mouseEntered();
			break;
		case PG_MESSAGE_MOUSE_LEFT:
			window->callbacks.mouseLeft();
			break;
		}

		memset(&message, 0, sizeof(message));
	}

	// Update the window placement and check whether the size has changed
	width = window->placement.width;
	height = window->placement.height;
	pgGetWindowPlacement(window, &window->placement);

	if (window->swapChain != NULL && (width != window->placement.width || height != window->placement.height))
		pgResizeSwapChain(window->swapChain, window->placement.width, window->placement.height);
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

	pgGetWindowPlacementCore(window);
	*placement = window->placement;
}

pgVoid pgSetWindowSize(pgWindow* window, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(window);

	if (window->fullscreen)
		return;

	window->placement.width = width;
	window->placement.height = height;

	pgConstrainWindowPlacement(&window->placement);
	pgSetWindowMode(window, PG_WINDOW_NORMAL);
	pgSetWindowSizeCore(window);

	if (window->swapChain != NULL)
		pgResizeSwapChain(window->swapChain, window->placement.width, window->placement.height);
}

pgVoid pgSetWindowPosition(pgWindow* window, pgInt32 x, pgInt32 y)
{
	PG_ASSERT_NOT_NULL(window);

	if (window->fullscreen)
		return;

	window->placement.x = x;
	window->placement.y = y;

	pgConstrainWindowPlacement(&window->placement);
	pgSetWindowMode(window, PG_WINDOW_NORMAL);
	pgSetWindowPositionCore(window);
}

pgVoid pgSetWindowMode(pgWindow* window, pgWindowMode state)
{
	PG_ASSERT_NOT_NULL(window);

	if (window->fullscreen)
		return;

	window->placement.state = state;

	pgConstrainWindowPlacement(&window->placement);
	pgSetWindowModeCore(window);
}

pgVoid pgSetWindowTitle(pgWindow* window, pgString title)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(title);

	pgSetWindowTitleCore(window, title);
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