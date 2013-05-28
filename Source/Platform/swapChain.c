#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgSwapChain* pgCreateSwapChain(pgGraphicsDevice* device, pgWindow* window)
{
	pgSwapChain* swapChain;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(window);

	PG_ALLOC(pgSwapChain, swapChain);
	swapChain->device = device;
	swapChain->renderTarget.device = device;

	pgGetWindowSize(window, &swapChain->renderTarget.width, &swapChain->renderTarget.height);
	pgCreateSwapChainCore(swapChain, window);

	return swapChain;
}

pgVoid pgDestroySwapChain(pgSwapChain* swapChain)
{
	if (swapChain == NULL)
		return;

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
	PG_ASSERT_IN_RANGE(width, 0, 4096);
	PG_ASSERT_IN_RANGE(height, 0, 4096);

	pgResizeSwapChainCore(swapChain, width, height);
}

pgBool pgUpdateSwapChainState(pgSwapChain* swapChain, pgInt32 width, pgInt32 height, pgBool fullscreen)
{
	PG_ASSERT_NOT_NULL(swapChain);
	PG_ASSERT_IN_RANGE(width, 0, 4096);
	PG_ASSERT_IN_RANGE(height, 0, 4096);

	return pgUpdateSwapChainStateCore(swapChain, width, height, fullscreen);
}