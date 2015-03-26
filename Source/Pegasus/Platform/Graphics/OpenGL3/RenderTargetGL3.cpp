#include "Prelude.hpp"

using namespace OpenGL3;

RenderTarget* GraphicsDevice::InitializeRenderTarget(Texture** colorBuffers, int32 colorBufferCount, Texture* depthStencil, int32 width, int32 height)
{
	auto renderTarget = PG_NEW(RenderTarget);
	renderTarget->Obj = Allocate(&GraphicsDevice::glGenFramebuffers, "Render Target");
	renderTarget->Width = width;
	renderTarget->Height = height;

	uint32 buffers[] =
	{
		GL_COLOR_ATTACHMENT0,
		GL_COLOR_ATTACHMENT1,
		GL_COLOR_ATTACHMENT2,
		GL_COLOR_ATTACHMENT3
	};

	static_assert(Graphics::MaxColorBuffers == sizeof(buffers) / sizeof(uint32), "Color buffer count mismatch.");
	glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->Obj);

	if (depthStencil != nullptr)
		glFramebufferTexture2D(GL_DRAW_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, depthStencil->Type, depthStencil->Obj, 0);

	for (auto i = 0; i < colorBufferCount; ++i)
		glFramebufferTexture2D(GL_DRAW_FRAMEBUFFER, buffers[i], colorBuffers[i]->Type, colorBuffers[i]->Obj, 0);

	ValidateFramebufferCompleteness();
	glDrawBuffers(colorBufferCount, buffers);
	RebindRenderTarget();

	return renderTarget;
}

void GraphicsDevice::FreeRenderTarget(RenderTarget* renderTarget)
{
	if (renderTarget == nullptr)
		return;

	glDeleteFramebuffers(1, &renderTarget->Obj);
	PG_DELETE(renderTarget);
}

void GraphicsDevice::BindRenderTarget(RenderTarget* renderTarget)
{
	BindRenderTargetGL(renderTarget);

	// We have to update the viewport and scissor rectangle as the the new render target might have a different size 
	// than the old one; viewports and scissor rectangles depend on the size of the currently bound render target
	// as the Y coordinate has to be inverted.
	// Without the following, code that sets the viewport/scissor rectangle before binding the render
	// target would not work correctly
	SetViewport(_boundViewportLeft, _boundViewportTop, _boundViewportWidth, _boundViewportHeight);
	SetScissorArea(_boundScissorLeft, _boundScissorTop, _boundScissorWidth, _boundScissorHeight);
}

void GraphicsDevice::GetRenderTargetSize(RenderTarget* renderTarget, int32* width, int32* height)
{
	*width = renderTarget->Width;
	*height = renderTarget->Height;
}

void GraphicsDevice::ClearColor(RenderTarget* renderTarget, Color* color)
{
	PG_UNUSED(renderTarget);

	auto scissorEnabled = _boundScissorEnabled;
	EnableScissor(GL_FALSE);

	SetClearColor(*color);
	glClear(GL_COLOR_BUFFER_BIT);

	EnableScissor(scissorEnabled);
}

void GraphicsDevice::ClearDepthStencil(RenderTarget* renderTarget, bool clearDepth, bool clearStencil, float32 depth, byte stencil)
{
	PG_UNUSED(renderTarget);

	auto scissorEnabled = _boundScissorEnabled;
	auto depthWritesEnabled = _boundDepthWritesEnabled;

	EnableScissor(false);
	EnableDepthWrites(true);

	uint32 targets = 0;
	if (clearDepth)
		targets |= GL_DEPTH_BUFFER_BIT;
	if (clearStencil)
		targets |= GL_STENCIL_BUFFER_BIT;

	SetClearDepth(depth);
	SetClearStencil(stencil);
	glClear(targets);

	EnableScissor(scissorEnabled);
	EnableDepthWrites(depthWritesEnabled);
}

void GraphicsDevice::ValidateFramebufferCompleteness()
{
	auto status = glCheckFramebufferStatus(GL_DRAW_FRAMEBUFFER);

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
		PG_DIE("The frame buffer is incomplete for an unknown reason (error code %d).", status);
	}
}

void GraphicsDevice::RebindRenderTarget()
{
	if (_boundRenderTarget != nullptr)
		BindRenderTargetGL(_boundRenderTarget);
}

void GraphicsDevice::BindRenderTargetGL(RenderTarget* renderTarget)
{
	PG_ASSERT_NOT_NULL(renderTarget);

	if (renderTarget->SwapChain != nullptr)
	{
		if (SDL_GL_MakeCurrent(renderTarget->SwapChain->Window, _context) != 0)
			PG_DIE("Failed to make OpenGL context current: %s.", SDL_GetError());

		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
	}
	else
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, renderTarget->Obj);

	_boundRenderTarget = renderTarget;
}

void GraphicsDevice::SetRenderTargetName(RenderTarget* renderTarget, const char* name)
{
	PG_UNUSED(renderTarget);
	PG_UNUSED(name);
}