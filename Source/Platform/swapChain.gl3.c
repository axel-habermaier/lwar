#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateSwapChainCore(pgSwapChain* swapChain, pgWindow* window)
{
	pgBindContext(&swapChain->context, swapChain->device, window);
	pgSetPixelFormat(&swapChain->context);
	swapChain->renderTarget.swapChain = swapChain;
	swapChain->renderTarget.count = 1;

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroySwapChainCore(pgSwapChain* swapChain)
{
	pgDestroyBoundContext(&swapChain->context);

	if (swapChain->device->renderTarget == &swapChain->renderTarget)
		swapChain->device->renderTarget = NULL;

	pgMakeCurrent(&swapChain->device->context);
}

pgVoid pgPresentCore(pgSwapChain* swapChain)
{
	pgMakeCurrent(&swapChain->context);
	pgSwapBuffers(&swapChain->context);
}

pgVoid pgResizeSwapChainCore(pgSwapChain* swapChain, pgInt32 width, pgInt32 height)
{
	swapChain->renderTarget.width = width;
	swapChain->renderTarget.height = height;
}

pgVoid pgUpdateSwapChainStateCore(pgSwapChain* swapChain, pgInt32 width, pgInt32 height, pgBool fullscreen)
{
	pgMakeCurrent(&swapChain->context);
	pgUpdateContextState(&swapChain->context, width, height, fullscreen);
}

#endif