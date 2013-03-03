#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget)
{
	// TODO: Implement
	renderTarget->id = 0;
	renderTarget->swapChain = NULL;
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	// TODO: Implement
	PG_UNUSED(renderTarget);
}

pgVoid pgClearColorCore(pgRenderTarget* renderTarget, pgColor color)
{
	pgRectangle viewport, scissor;

	viewport = renderTarget->device->viewport;
	scissor = renderTarget->device->scissorRectangle;
	pgSetViewport(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
	pgSetScissorRect(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);

    glClearColor(color.red, color.green, color.blue, color.alpha);
    glClear(GL_COLOR_BUFFER_BIT);

	pgSetViewport(renderTarget->device, viewport.left, viewport.top, viewport.width, viewport.height);
	pgSetScissorRect(renderTarget->device, scissor.left, scissor.top, scissor.width, scissor.height);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgClearDepthStencilCore(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil)
{
	pgInt32 glTargets = 0;
	pgRectangle viewport, scissor;

    if (clearDepth)
        glTargets |= GL_DEPTH_BUFFER_BIT;
    if (clearStencil)
        glTargets |= GL_STENCIL_BUFFER_BIT;

	viewport = renderTarget->device->viewport;
	scissor = renderTarget->device->scissorRectangle;
	pgSetViewport(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
	pgSetScissorRect(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);

    glClearDepth(depth);
    glClearStencil(stencil);
    glClear(glTargets);

	pgSetViewport(renderTarget->device, viewport.left, viewport.top, viewport.width, viewport.height);
	pgSetScissorRect(renderTarget->device, scissor.left, scissor.top, scissor.width, scissor.height);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget)
{
	// We have to emulate the D3D11 behavior here
	if (renderTarget->swapChain != NULL)
		pgMakeCurrent(&renderTarget->swapChain->context);

	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->id);
	PG_ASSERT_NO_GL_ERRORS();
}

#endif