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