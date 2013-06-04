#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgWindow* pgOpenWindow(pgWindowParams* windowParams)
{
	pgWindow* window;

	PG_ASSERT_NOT_NULL(windowParams);
	PG_ASSERT_IN_RANGE(windowParams->width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(windowParams->height, 0, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT_NOT_NULL(windowParams->title);

	PG_ALLOC(pgWindow, window);
	window->params = *windowParams;
	window->params.title = NULL;
	
	pgOpenWindowCore(window);
	pgSetWindowTitle(window, windowParams->title);
	pgGetWindowPlacement(window, &window->placement);

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
	pgMessage message;
	PG_ASSERT_NOT_NULL(window);

	while (pgProcessWindowEvent(window, &message))
	{
		switch (message.type)
		{
		case PG_MESSAGE_CLOSING:
			window->params.closing();
			break;
		case PG_MESSAGE_CHARACTER_ENTERED:
			window->params.characterEntered(message.characterEntered.character, message.characterEntered.scanCode);
			break;
		case PG_MESSAGE_GAINED_FOCUS:
			window->params.gainedFocus();
			break;
		case PG_MESSAGE_LOST_FOCUS:
			window->params.lostFocus();
			break;
		case PG_MESSAGE_KEY_UP:
			window->params.keyReleased(message.key.key, message.key.scanCode);
			break;
		case PG_MESSAGE_KEY_DOWN:
			window->params.keyPressed(message.key.key, message.key.scanCode);
			break;
		case PG_MESSAGE_MOUSE_WHEEL:
			window->params.mouseWheel(message.wheel.delta);
			break;
		case PG_MESSAGE_MOUSE_DOWN:
			window->params.mousePressed(message.mouse.button, message.mouse.x, message.mouse.y);
			break;
		case PG_MESSAGE_MOUSE_UP:
			window->params.mouseReleased(message.mouse.button, message.mouse.x, message.mouse.y);
			break;
		case PG_MESSAGE_MOUSE_MOVED:
			window->params.mouseMoved(message.moved.x, message.moved.y);
			break;
		case PG_MESSAGE_MOUSE_ENTERED:
			window->params.mouseEntered();
			break;
		case PG_MESSAGE_MOUSE_LEFT:
			window->params.mouseLeft();
			break;
		}
	}

	pgGetWindowPlacement(window, &window->placement);
}

pgVoid pgGetWindowPlacement(pgWindow* window, pgWindowPlacement* placement)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_NOT_NULL(placement);

	if (!window->fullscreen)
		pgGetWindowPlacementCore(window);

	*placement = window->placement;
}

pgVoid pgSetWindowPlacement(pgWindow* window, pgWindowPlacement placement)
{
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_IN_RANGE(placement.x, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(placement.y, 0, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT_IN_RANGE(placement.width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(placement.height, 0, PG_WINDOW_MAX_HEIGHT);

	window->placement = placement;

	if (!window->fullscreen)
		pgSetWindowPlacementCore(window);
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

//pgVoid pgWindowKeyUp(pgWindow* window, pgKey key, pgInt32 scanCode)
//{
//	PG_ASSERT_NOT_NULL(window);
//
//	window->keyState[key - 1] = PG_FALSE;
//	if (window->params.keyReleased != NULL)
//		window->params.keyReleased(key, scanCode);
//}
//
//pgVoid pgWindowKeyDown(pgWindow* window, pgKey key, pgInt32 scanCode)
//{
//	PG_ASSERT_NOT_NULL(window);
//
//	window->keyState[key - 1] = PG_TRUE;
//	window->scanCode[key - 1] = scanCode;
//	if (window->params.keyPressed != NULL)
//		window->params.keyPressed(key, scanCode);
//}
//
//pgVoid pgWindowButtonUp(pgWindow* window, pgMouseButton button, pgInt32 x, pgInt32 y)
//{
//	PG_ASSERT_NOT_NULL(window);
//
//	window->buttonState[button - 1] = PG_FALSE;
//	if (window->params.mouseReleased != NULL)
//		window->params.mouseReleased(button, x, y);
//}
//
//pgVoid pgWindowButtonDown(pgWindow* window, pgMouseButton button, pgInt32 x, pgInt32 y)
//{
//	PG_ASSERT_NOT_NULL(window);
//
//	window->buttonState[button - 1] = PG_TRUE;
//	if (window->params.mousePressed != NULL)
//		window->params.mousePressed(button, x, y);
//}
//
//pgVoid pgWindowLostFocus(pgWindow* window, pgInt32 x, pgInt32 y)
//{
//	pgInt32 i;
//	PG_ASSERT_NOT_NULL(window);
//
//	// Make sure that all mouse buttons and keys are released (we might miss some events, for instance,
//	// when the user uses Alt+Tab to switch to another window)
//	for (i = 0; i < PG_KEY_COUNT - 1; ++i)
//	{
//		if (window->keyState[i])
//			pgWindowKeyUp(window, (pgKey)(i + 1), window->scanCode[i]);
//	}
//
//	for (i = 0; i < PG_BUTTON_COUNT - 1; ++i)
//	{
//		if (window->buttonState[i])
//			pgWindowButtonUp(window, (pgMouseButton)(i + 1), x, y);
//	}
//
//	if (window->params.lostFocus != NULL)
//		window->params.lostFocus();
//}