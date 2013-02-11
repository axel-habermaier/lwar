#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgWindow* pgOpenWindow(pgWindowParams* windowParams)
{
	pgWindow* window;
	size_t titleLength;

	PG_ASSERT_NOT_NULL(windowParams);
	PG_ASSERT_IN_RANGE(windowParams->width, 0, 4096);
	PG_ASSERT_IN_RANGE(windowParams->height, 0, 4096);
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
	PG_ASSERT_NOT_NULL(window);
	pgProcessWindowEventsCore(window);
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
	PG_ASSERT_IN_RANGE(width, 0, 4096);
	PG_ASSERT_IN_RANGE(height, 0, 4096);

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