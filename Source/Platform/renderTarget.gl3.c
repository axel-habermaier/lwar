#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget, pgTexture* texture)
{
	// TODO: Implement
	renderTarget->id = 0;
	renderTarget->swapChain = NULL;
	PG_UNUSED(texture);
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	// TODO: Implement
	PG_UNUSED(renderTarget);
}

pgVoid pgClearCore(pgRenderTarget* renderTarget, pgClearTargets targets, pgColor color, pgFloat32 depth, pgUint8 stencil)
{
	pgInt32 glTargets = 0;
	pgRectangle viewport, scissor;

    if ((targets & PG_CLEAR_COLOR) == PG_CLEAR_COLOR)
        glTargets |= GL_COLOR_BUFFER_BIT;
    if ((targets & PG_CLEAR_DEPTH) == PG_CLEAR_DEPTH)
        glTargets |= GL_DEPTH_BUFFER_BIT;
    if ((targets & PG_CLEAR_STENCIL) == PG_CLEAR_STENCIL)
        glTargets |= GL_STENCIL_BUFFER_BIT;

	viewport = renderTarget->device->state.viewport;
	scissor = renderTarget->device->state.scissorRectangle;
	pgSetViewport(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
	pgSetScissorRect(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);

	pgBindRenderTarget(renderTarget);
    glClearColor(color.red, color.green, color.blue, color.alpha);
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