#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgSwapChain* pgCreateSwapChain(pgGraphicsDevice* device, pgWindow* window)
{
	pgSwapChain* swapChain;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT(window->swapChain == NULL, "There is already another swap chain for this window.");

	PG_ALLOC(pgSwapChain, swapChain);
	swapChain->device = device;
	swapChain->renderTarget.device = device;
	swapChain->window = window;

	window->swapChain = swapChain;

	swapChain->renderTarget.width = window->placement.width;
	swapChain->renderTarget.height = window->placement.height;

	/*pgGetWindowSize(window, &swapChain->renderTarget.width, &swapChain->renderTarget.height);
	pgGetWindowSize(window, &swapChain->fullscreenWidth, &swapChain->fullscreenHeight);
	pgGetWindowSize(window, &swapChain->windowedWidth, &swapChain->windowedHeight);*/

	pgCreateSwapChainCore(swapChain, window);

	return swapChain;
}

pgVoid pgDestroySwapChain(pgSwapChain* swapChain)
{
	if (swapChain == NULL)
		return;

	swapChain->window->swapChain = NULL;

	pgDestroySwapChainCore(swapChain);
	PG_FREE(swapChain);
}

pgVoid pgPresent(pgSwapChain* swapChain)
{
	PG_ASSERT_NOT_NULL(swapChain);
	pgPresentCore(swapChain);
}

pgRenderTarget* pgGetBackBuffer(pgSwapChain* swapChain)
{
	PG_ASSERT_NOT_NULL(swapChain);
	return &swapChain->renderTarget;
}

pgVoid pgResizeSwapChain(pgSwapChain* swapChain, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(swapChain);
	PG_ASSERT_IN_RANGE(width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(height, 0, PG_WINDOW_MAX_HEIGHT);

	//swapChain->width = width;
	//swapChain->height = height;
	pgResizeSwapChainCore(swapChain, width, height);
}

pgBool pgUpdateSwapChainState(pgSwapChain* swapChain, pgInt32 width, pgInt32 height, pgBool fullscreen)
{
	PG_ASSERT_NOT_NULL(swapChain);
	PG_ASSERT_IN_RANGE(width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(height, 0, PG_WINDOW_MAX_HEIGHT);

	if (pgUpdateSwapChainStateCore(swapChain, width, height, fullscreen))
	{
		if (fullscreen)
		{
			swapChain->fullscreenWidth = width;
			swapChain->fullscreenHeight = height;
		}
		else
		{
			swapChain->windowedWidth = width;
			swapChain->windowedHeight = height;
		}
		
		swapChain->fullscreen = fullscreen;
		return PG_TRUE;
	}

	return PG_FALSE;
}