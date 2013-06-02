#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgWindow* pgOpenWindow(pgWindowParams* windowParams)
{
	pgWindow* window;
	size_t titleLength;

	PG_ASSERT_NOT_NULL(windowParams);
	PG_ASSERT_IN_RANGE(windowParams->width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(windowParams->height, 0, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT_NOT_NULL(windowParams->title);

	PG_ALLOC(pgWindow, window);
	window->params = *windowParams;
	
	// Copy of the title
	titleLength = strlen(windowParams->title);
	PG_ALLOC_ARRAY(pgChar, titleLength + 1, window->params.title);
	memcpy((void*)window->params.title, windowParams->title, titleLength + 1);

	pgOpenWindowCore(window);

	return window;
}

pgVoid pgCloseWindow(pgWindow* window)
{
	PG_ASSERT_NOT_NULL(window);

	pgCloseWindowCore(window);

	if (window->params.closed != NULL)
		window->params.closed();

	PG_FREE(window->params.title);
	PG_FREE(window);
}

pgVoid pgProcessWindowEvents(pgWindow* window)
{
	// There might be multiple resize messages queued; only raise the resize event for the last message for
	// performance reasons
	pgInt32 width = window->width, height = window->height;
	PG_ASSERT_NOT_NULL(window);

	pgProcessWindowEventsCore(window);

	if (window->params.resized != NULL && (width != window->width || height != window->height))
		window->params.resized(window->width, window->height);
}

pgVoid pgGetWindowSize(pgWindow* window, pgInt32* width, pgInt32* height)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(width);
	PG_ASSERT_NOT_NULL(height);

	pgGetWindowSizeCore(window, width, height);
}

pgVoid pgSetWindowSize(pgWindow* window, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_IN_RANGE(width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(height, 0, PG_WINDOW_MAX_HEIGHT);

	pgSetWindowSizeCore(window, width, height);
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

pgVoid pgWindowKeyUp(pgWindow* window, pgKey key, pgInt32 scanCode)
{
	PG_ASSERT_NOT_NULL(window);

	window->keyState[key - 1] = PG_FALSE;
	if (window->params.keyReleased != NULL)
		window->params.keyReleased(key, scanCode);
}

pgVoid pgWindowKeyDown(pgWindow* window, pgKey key, pgInt32 scanCode)
{
	PG_ASSERT_NOT_NULL(window);

	window->keyState[key - 1] = PG_TRUE;
	window->scanCode[key - 1] = scanCode;
	if (window->params.keyPressed != NULL)
		window->params.keyPressed(key, scanCode);
}

pgVoid pgWindowButtonUp(pgWindow* window, pgMouseButton button, pgInt32 x, pgInt32 y)
{
	PG_ASSERT_NOT_NULL(window);

	window->buttonState[button - 1] = PG_FALSE;
	if (window->params.mouseReleased != NULL)
		window->params.mouseReleased(button, x, y);
}

pgVoid pgWindowButtonDown(pgWindow* window, pgMouseButton button, pgInt32 x, pgInt32 y)
{
	PG_ASSERT_NOT_NULL(window);

	window->buttonState[button - 1] = PG_TRUE;
	if (window->params.mousePressed != NULL)
		window->params.mousePressed(button, x, y);
}

pgVoid pgWindowLostFocus(pgWindow* window, pgInt32 x, pgInt32 y)
{
	pgInt32 i;
	PG_ASSERT_NOT_NULL(window);

	// Make sure that all mouse buttons and keys are released (we might miss some events, for instance,
	// when the user uses Alt+Tab to switch to another window)
	for (i = 0; i < PG_KEY_COUNT - 1; ++i)
	{
		if (window->keyState[i])
			pgWindowKeyUp(window, (pgKey)(i + 1), window->scanCode[i]);
	}

	for (i = 0; i < PG_BUTTON_COUNT - 1; ++i)
	{
		if (window->buttonState[i])
			pgWindowButtonUp(window, (pgMouseButton)(i + 1), x, y);
	}

	if (window->params.lostFocus != NULL)
		window->params.lostFocus();
}