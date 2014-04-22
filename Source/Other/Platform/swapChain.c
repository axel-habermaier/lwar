#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgSwapChain* pgCreateSwapChain(pgGraphicsDevice* device, pgWindow* window, pgBool fullscreen, pgInt32 width, pgInt32 height)
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
	swapChain->fullscreenWidth = width;
	swapChain->fullscreenHeight = height;

	window->swapChain = swapChain;

	swapChain->renderTarget.width = window->placement.width;
	swapChain->renderTarget.height = window->placement.height;

	pgCreateSwapChainCore(swapChain, window);
	if (fullscreen)
		pgSwapChainFullscreen(swapChain, width, height);

	return swapChain;
}

pgVoid pgDestroySwapChain(pgSwapChain* swapChain)
{
	if (swapChain == NULL)
		return;

	pgSwapChainWindowed(swapChain);
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

pgBool pgSwapChainFullscreen(pgSwapChain* swapChain, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(swapChain);
	PG_ASSERT_IN_RANGE(width, PG_WINDOW_MIN_WIDTH, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(height, PG_WINDOW_MIN_HEIGHT, PG_WINDOW_MAX_HEIGHT);

	if (swapChain->fullscreen && swapChain->fullscreenWidth == width && swapChain->fullscreenHeight == height)
		return PG_TRUE;
	
	if (!swapChain->fullscreen)
	{
		swapChain->windowedWidth = swapChain->window->placement.width;
		swapChain->windowedHeight = swapChain->window->placement.height;
	}

	if (pgSwapChainFullscreenCore(swapChain, width, height))
	{
		swapChain->fullscreenWidth = width;
		swapChain->fullscreenHeight = height;
		
		swapChain->fullscreen = PG_TRUE;
		swapChain->window->fullscreen = PG_TRUE;
		return PG_TRUE;
	}

	return PG_FALSE;
}

pgVoid pgSwapChainWindowed(pgSwapChain* swapChain)
{
	PG_ASSERT_NOT_NULL(swapChain);

	if (!swapChain->fullscreen)
		return;

	swapChain->fullscreen = PG_FALSE;
	swapChain->window->fullscreen = PG_FALSE;
	pgSwapChainWindowedCore(swapChain);
}

pgBool pgSwapChainIsFullscreen(pgSwapChain* swapChain)
{
	PG_ASSERT_NOT_NULL(swapChain);
	return swapChain->fullscreen;
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