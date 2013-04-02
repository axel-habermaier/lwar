#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgValidateFramebufferCompleteness();

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgInt32 i;
	GLint boundFramebuffer;

	PG_GL_ALLOC("Framebuffer", glGenFramebuffers, renderTarget->id);
	glGetIntegerv(GL_DRAW_FRAMEBUFFER_BINDING, &boundFramebuffer);
	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->id);

	if (renderTarget->depthStencil != NULL)
	{
		glFramebufferTexture2D(GL_DRAW_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, renderTarget->depthStencil->glType, 
			renderTarget->depthStencil->id, 0);
	}

	for (i = 0; i < renderTarget->count; ++i)
	{
		glFramebufferTexture2D(GL_DRAW_FRAMEBUFFER, GL_COLOR_ATTACHMENT0 + i, renderTarget->colorBuffers[i]->glType, 
			renderTarget->colorBuffers[i]->id, 0);
	}

	pgValidateFramebufferCompleteness();
	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, boundFramebuffer);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	PG_GL_FREE(glDeleteFramebuffers, renderTarget->id);
}

pgVoid pgClearColorCore(pgRenderTarget* renderTarget, pgColor color)
{
	GLboolean scissorEnabled;
	glGetBooleanv(GL_SCISSOR_TEST, &scissorEnabled);
	glDisable(GL_SCISSOR_TEST);

	PG_UNUSED(renderTarget);

    glClearColor(color.red, color.green, color.blue, color.alpha);
    glClear(GL_COLOR_BUFFER_BIT);

	if (scissorEnabled)
		glEnable(GL_SCISSOR_TEST);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgClearDepthStencilCore(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil)
{
	pgInt32 glTargets = 0;
	GLboolean scissorEnabled;
	glGetBooleanv(GL_SCISSOR_TEST, &scissorEnabled);
	glDisable(GL_SCISSOR_TEST);

	PG_ASSERT(renderTarget->swapChain != NULL || renderTarget->depthStencil != NULL, 
		"Cannot clear depth stencil of a render target without a depth stencil buffer.");

    if (clearDepth)
        glTargets |= GL_DEPTH_BUFFER_BIT;
    if (clearStencil)
        glTargets |= GL_STENCIL_BUFFER_BIT;

    glClearDepth(depth);
    glClearStencil(stencil);
    glClear(glTargets);

	if (scissorEnabled)
		glEnable(GL_SCISSOR_TEST);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgGraphicsDevice* device = renderTarget->device;
	pgRectangle viewport = renderTarget->device->viewport;
	pgRectangle scissorArea = renderTarget->device->scissorArea;

	if (renderTarget->swapChain != NULL)
	{
		pgMakeCurrent(&renderTarget->swapChain->context);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
		glDrawBuffer(GL_BACK);
	}
	else
	{
		static GLenum buffers[] = 
		{
			GL_COLOR_ATTACHMENT0,
			GL_COLOR_ATTACHMENT1,
			GL_COLOR_ATTACHMENT2,
			GL_COLOR_ATTACHMENT3,
		};

		PG_ASSERT(PG_MAX_COLOR_ATTACHMENTS == sizeof(buffers) / sizeof(GLenum), "Attachment count mismatch.");

		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->id);
		glDrawBuffers(renderTarget->count, buffers);
	}

	PG_ASSERT_NO_GL_ERRORS();

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
		pgDie("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT.");
	case GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT:
		pgDie("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT.");
	case GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER:
		pgDie("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER.");
	case GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER:
		pgDie("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER.");
	case GL_FRAMEBUFFER_UNSUPPORTED:
		pgDie("Frame buffer status: GL_FRAMEBUFFER_UNSUPPORTED.");
	case GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE:
		pgDie("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE.");
	case GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS:
		pgDie("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS.");
	default:
		pgDie("The frame buffer is incomplete for an unknown reason.");
	}
}

#endif