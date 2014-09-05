#include "prelude.h"

#ifdef PG_GRAPHICS_OPENGL3

//====================================================================================================================
// State
//====================================================================================================================

static pgRenderTarget* boundRenderTarget = NULL;

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgValidateFramebufferCompleteness();
static pgVoid pgRebindRenderTarget();
static pgVoid pgBindRenderTargetGL(pgRenderTarget* renderTarget);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgInt32 i;
	static GLenum buffers[] = 
	{
		GL_COLOR_ATTACHMENT0,
		GL_COLOR_ATTACHMENT1,
		GL_COLOR_ATTACHMENT2,
		GL_COLOR_ATTACHMENT3,
	};
	
	PG_ASSERT(PG_MAX_COLOR_ATTACHMENTS == sizeof(buffers) / sizeof(GLenum), "Attachment count mismatch.");
	PG_GL_ALLOC("Framebuffer", glGenFramebuffers, renderTarget->id);
	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->id);

	if (renderTarget->depthStencil != NULL)
	{
		glFramebufferTexture2D( GL_DRAW_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, renderTarget->depthStencil->glType, 
			renderTarget->depthStencil->id, 0);
	}

	for (i = 0; i < renderTarget->count; ++i)
	{
		glFramebufferTexture2D( GL_DRAW_FRAMEBUFFER, GL_COLOR_ATTACHMENT0 + i, renderTarget->colorBuffers[i]->glType, 
			renderTarget->colorBuffers[i]->id, 0);
	}

	pgValidateFramebufferCompleteness();
	PG_ASSERT_NO_GL_ERRORS();

	glDrawBuffers(renderTarget->count, buffers);
	PG_ASSERT_NO_GL_ERRORS();

	pgRebindRenderTarget();
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	PG_GL_FREE(glDeleteFramebuffers, renderTarget->id);

	if (boundRenderTarget == renderTarget)
		boundRenderTarget = NULL;
}

pgVoid pgClearColorCore(pgRenderTarget* renderTarget, pgColor color)
{
	GLboolean scissorEnabled = renderTarget->device->scissorEnabled;

	pgBindRenderTarget(renderTarget);
	pgEnableScissor(renderTarget->device, PG_FALSE);

    pgSetClearColor(renderTarget->device, color);
    glClear(GL_COLOR_BUFFER_BIT);

	pgEnableScissor(renderTarget->device, scissorEnabled);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgClearDepthStencilCore(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil)
{
	pgInt32 glTargets = 0;
	GLboolean scissorEnabled = renderTarget->device->scissorEnabled;
	GLboolean depthWritesEnabled = renderTarget->device->depthWritesEnabled;

	pgBindRenderTarget(renderTarget);
	pgEnableScissor(renderTarget->device, PG_FALSE);
	pgEnableDepthWrites(renderTarget->device, PG_TRUE);

	PG_ASSERT(renderTarget->swapChain != NULL || renderTarget->depthStencil != NULL, 
		"Cannot clear depth stencil of a render target without a depth stencil buffer.");

    if (clearDepth)
        glTargets |= GL_DEPTH_BUFFER_BIT;
    if (clearStencil)
        glTargets |= GL_STENCIL_BUFFER_BIT;

    pgSetClearDepth(renderTarget->device, depth);
    pgSetClearStencil(renderTarget->device, stencil);
    glClear(glTargets);

	pgEnableScissor(renderTarget->device, scissorEnabled);
	pgEnableDepthWrites(renderTarget->device, depthWritesEnabled);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgGraphicsDevice* device = renderTarget->device;
	pgRectangle viewport = renderTarget->device->viewport;
	pgRectangle scissorArea = renderTarget->device->scissorArea;

	boundRenderTarget = renderTarget;
	pgBindRenderTargetGL(renderTarget);

	// We have to update the viewport and scissor rectangle as the the new render target might have a different size 
	// than the old one; viewports and scissor rectangles depend on the size of the currently bound render target
	// as the Y coordinate has to be inverted.
	// Without the following two lines, code that sets the viewport/scissor rectangle before binding the render
	// target would not work correctly
	pgSetViewportCore(device, viewport);
	pgSetScissorAreaCore(device, scissorArea);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgValidateFramebufferCompleteness()
{
	GLenum status = glCheckFramebufferStatus(GL_DRAW_FRAMEBUFFER);
	PG_ASSERT_NO_GL_ERRORS();

	switch (status)
	{
	case GL_FRAMEBUFFER_COMPLETE:
		return;
	case GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT.");
	case GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT.");
	case GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER.");
	case GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER.");
	case GL_FRAMEBUFFER_UNSUPPORTED:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_UNSUPPORTED.");
	case GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE.");
	case GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS:
		PG_DIE("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS.");
	default:
		PG_DIE("The frame buffer is incomplete for an unknown reason.");
	}
}

static pgVoid pgRebindRenderTarget()
{
	if (boundRenderTarget != NULL)
		pgBindRenderTargetGL(boundRenderTarget);
}

static pgVoid pgBindRenderTargetGL(pgRenderTarget* renderTarget)
{
	PG_ASSERT_NOT_NULL(renderTarget);

	if (renderTarget->swapChain != NULL)
	{
		pgMakeCurrent(&renderTarget->swapChain->context);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
	}
	else
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->id);

	PG_ASSERT_NO_GL_ERRORS();
}

#endif