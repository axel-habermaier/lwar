#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgSwapChain* pgCreateSwapChain(pgGraphicsDevice* device, pgWindow* window, pgInt32 width, pgInt32 height)
{
	pgSwapChain* swapChain;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(window);
	PG_ASSERT_IN_RANGE(width, PG_WINDOW_MIN_WIDTH, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(height, PG_WINDOW_MIN_HEIGHT, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT(window->swapChain == NULL, "There is already another swap chain for this window.");

	PG_ALLOC(pgSwapChain, swapChain);
	swapChain->device = device;
	swapChain->renderTarget.device = device;
	swapChain->window = window;
	swapChain->width = width;
	swapChain->height = height;

	window->swapChain = swapChain;

	swapChain->renderTarget.width = window->placement.width;
	swapChain->renderTarget.height = window->placement.height;

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

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgVoid pgResizeSwapChain(pgSwapChain* swapChain, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(swapChain);
	PG_ASSERT_IN_RANGE(width, PG_WINDOW_MIN_WIDTH, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(height, PG_WINDOW_MIN_HEIGHT, PG_WINDOW_MAX_HEIGHT);

	pgResizeSwapChainCore(swapChain, width, height);
}