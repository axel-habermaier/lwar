#include "Prelude.hpp"

using namespace OpenGL3;

PG_DECLARE_DEVICEINTERFACE_API(GraphicsDevice, GL3)

GraphicsDevice::GraphicsDevice(DeviceInterface* deviceInterface)
{
	PG_ASSERT_NOT_NULL(deviceInterface);

	deviceInterface->_this = this;
	PG_INITIALIZE_DEVICEINTERFACE(GraphicsDevice, GL3, deviceInterface);

	_contextWindow = SDL_CreateWindow("CtxWnd", 0, 0, 1, 1, SDL_WINDOW_HIDDEN | SDL_WINDOW_OPENGL);
	if (_contextWindow == nullptr)
		PG_DIE("Failed to create the OpenGL context window: %s.", SDL_GetError());

	_context = SDL_GL_CreateContext(_contextWindow);
	if (_context == nullptr)
		PG_DIE("Failed to initialize the OpenGL context. OpenGL 3.3 is not supported by your graphics card.");

	if (SDL_GL_MakeCurrent(_contextWindow, _context) != 0)
		PG_DIE("Failed to make OpenGL context current: %s.", SDL_GetError());

	_bindings.Initialize();
}

GraphicsDevice::~GraphicsDevice()
{
	SDL_GL_DeleteContext(_context);
	SDL_DestroyWindow(_contextWindow);
}

void GraphicsDevice::PrintDeviceInfo()
{
	PG_INFO("OpenGL renderer: %s (%s)", glGetString(GL_RENDERER), glGetString(GL_VENDOR));
	PG_INFO("OpenGL version: %s", glGetString(GL_VERSION));
	PG_INFO("OpenGL GLSL version: %s", glGetString(GL_SHADING_LANGUAGE_VERSION));
}

void GraphicsDevice::SetPrimitiveType(PrimitiveType primitiveType)
{
	_boundPrimitiveType = Map(primitiveType);
}

void GraphicsDevice::SetViewport(int32 left, int32 top, int32 width, int32 height)
{
	_boundViewportTop = top;
	top = FlipY(top, height);

	if (!Graphics::ChangeState(&_boundViewportLeft, left) &
		!Graphics::ChangeState(&_boundViewportTopGL, top) &
		!Graphics::ChangeState(&_boundViewportWidth, width) &
		!Graphics::ChangeState(&_boundViewportHeight, height))
		return;

	glViewport(left, top, width, height);
}

void GraphicsDevice::SetScissorArea(int32 left, int32 top, int32 width, int32 height)
{
	_boundScissorTop = top;
	top = FlipY(top, height);

	if (!Graphics::ChangeState(&_boundScissorLeft, left) &
		!Graphics::ChangeState(&_boundScissorTopGL, top) &
		!Graphics::ChangeState(&_boundScissorWidth, width) &
		!Graphics::ChangeState(&_boundScissorHeight, height))
		return;

	glScissor(left, top, width, height);
}

void GraphicsDevice::Draw(int32 vertexCount, int32 vertexOffset)
{
	glBindVertexArray(_boundVertexLayout->Obj);
	glDrawArrays(_boundPrimitiveType, vertexOffset, vertexCount);
	glBindVertexArray(0);
}

void GraphicsDevice::DrawIndexed(int32 indexCount, int32 indexOffset, int32 vertexOffset)
{
	auto offset = reinterpret_cast<void*>(static_cast<size_t>((indexOffset + _boundVertexLayout->IndexOffset) * _boundVertexLayout->IndexSize));

	glBindVertexArray(_boundVertexLayout->Obj);
	glDrawElementsBaseVertex(_boundPrimitiveType, indexCount, _boundVertexLayout->IndexType, offset, vertexOffset);
	glBindVertexArray(0);
}

void GraphicsDevice::DrawInstanced(int32 instanceCount, int32 vertexCount, int32 vertexOffset, int32 instanceOffset)
{
	glBindVertexArray(_boundVertexLayout->Obj);
	glDrawArraysInstancedBaseInstance(_boundPrimitiveType, vertexOffset, vertexCount, instanceCount, safe_static_cast<uint32>(instanceOffset));
	glBindVertexArray(0);
}

void GraphicsDevice::DrawIndexedInstanced(int32 instanceCount, int32 indexCount, int32 indexOffset, int32 vertexOffset, int32 instanceOffset)
{
	auto offset = reinterpret_cast<void*>(static_cast<size_t>((indexOffset + _boundVertexLayout->IndexOffset) * _boundVertexLayout->IndexSize));

	glBindVertexArray(_boundVertexLayout->Obj);

	glDrawElementsInstancedBaseVertexBaseInstance(
		_boundPrimitiveType,
		indexCount,
		_boundVertexLayout->IndexType,
		offset,
		instanceCount,
		vertexOffset, safe_static_cast<uint32>(instanceOffset));

	glBindVertexArray(0);
}

void GraphicsDevice::CheckErrors()
{
#ifdef DEBUG
	auto glErrorOccurred = false;
	for (auto glError = glGetError(); glError != GL_NO_ERROR; glError = glGetError())
	{
		const char* msg = "";
		switch (glError)
		{
		case GL_INVALID_ENUM:		msg = "GL_INVALID_ENUM";		break;
		case GL_INVALID_VALUE:		msg = "GL_INVALID_VALUE";		break;
		case GL_INVALID_OPERATION:	msg = "GL_INVALID_OPERATION";	break;
		case GL_OUT_OF_MEMORY:		msg = "GL_OUT_OF_MEMORY";		break;
		}

		PG_ERROR("OpenGL error: %s", msg);
		glErrorOccurred = true;
	}

	if (glErrorOccurred)
		PG_DIE("Stopped after OpenGL error.");
#endif
}

uint32 GraphicsDevice::Allocate(void(GraphicsDevice::*genFunc)(int32, uint32*), const char* typeString)
{
	PG_ASSERT_NOT_NULL(genFunc);
	PG_ASSERT_NOT_NULL(typeString);

	uint32 handle = 0;
	(this->*genFunc)(1, &handle);

	if (handle == 0)
		PG_DIE("Failed to allocate an OpenGL object of type '%s'.", typeString);

	return handle;
}

int32 GraphicsDevice::FlipY(int32 top, int32 height)
{
	return _boundRenderTarget == nullptr ? 0 : _boundRenderTarget->Height - height - top;
}

void GraphicsDevice::Enable(uint32 capability, int32* current, int32 enabled)
{
	if (*current == enabled)
		return;

	*current = enabled;

	if (enabled)
		glEnable(capability);
	else
		glDisable(capability);
}

void GraphicsDevice::ChangeActiveTexture(int32 slot)
{
	if (Graphics::ChangeState(&_boundActiveTexture, static_cast<uint32>(GL_TEXTURE0 + slot)))
		glActiveTexture(static_cast<uint32>(GL_TEXTURE0 + slot));
}

void GraphicsDevice::EnableScissor(int32 enabled)
{
	Enable(GL_SCISSOR_TEST, &_boundScissorEnabled, enabled);
}

void GraphicsDevice::EnableBlend(int32 enabled)
{
	Enable(GL_BLEND, &_boundBlendEnabled, enabled);
}

void GraphicsDevice::EnableDepthTest(int32 enabled)
{
	Enable(GL_DEPTH_TEST, &_boundDepthTestEnabled, enabled);
}

void GraphicsDevice::EnableCullFace(int32 enabled)
{
	Enable(GL_CULL_FACE, &_boundCullFaceEnabled, enabled);
}

void GraphicsDevice::EnableDepthClamp(int32 enabled)
{
	Enable(GL_DEPTH_CLAMP, &_boundDepthClampEnabled, enabled);
}

void GraphicsDevice::EnableMultisample(int32 enabled)
{
	Enable(GL_MULTISAMPLE, &_boundMultisampleEnabled, enabled);
}

void GraphicsDevice::EnableAntialiasedLine(int32 enabled)
{
	Enable(GL_LINE_SMOOTH, &_boundAntialiasedLineEnabled, enabled);
}

void GraphicsDevice::EnableStencilTest(int32 enabled)
{
	Enable(GL_STENCIL_TEST, &_boundStencilTestEnabled, enabled);
}

void GraphicsDevice::EnableDepthWrites(int32 enabled)
{
	if (Graphics::ChangeState(&_boundDepthWritesEnabled, enabled))
		glDepthMask(enabled);
}

void GraphicsDevice::SetCullFace(uint32 cullFace)
{
	if (Graphics::ChangeState(&_boundCullFace, cullFace))
		glCullFace(cullFace);
}

void GraphicsDevice::SetFrontFace(uint32 frontFace)
{
	if (Graphics::ChangeState(&_boundFrontFace, frontFace))
		glFrontFace(frontFace);
}

void GraphicsDevice::SetPolygonMode(uint32 mode)
{
	if (Graphics::ChangeState(&_boundPolygonMode, mode))
		glPolygonMode(GL_FRONT_AND_BACK, mode);
}

void GraphicsDevice::SetPolygonOffset(float32 slopeScaledDepthBias, float32 depthBiasClamp)
{
	if (Graphics::ChangeState(&_boundSlopeScaledDepthBias, slopeScaledDepthBias) & !Graphics::ChangeState(&_boundDepthBiasClamp, depthBiasClamp))
		glPolygonOffset(slopeScaledDepthBias, depthBiasClamp);
}

void GraphicsDevice::SetDepthFunc(uint32 func)
{
	if (Graphics::ChangeState(&_boundDepthFunc, func))
		glDepthFunc(func);
}

void GraphicsDevice::SetClearColor(Color color)
{
	if (!Graphics::ChangeState(&_boundClearColorRed, color.Red) &
		!Graphics::ChangeState(&_boundClearColorGreen, color.Green) &
		!Graphics::ChangeState(&_boundClearColorBlue, color.Blue) &
		!Graphics::ChangeState(&_boundClearColorAlpha, color.Alpha))
		return;

	glClearColor(color.Red / 255.0f, color.Green / 255.0f, color.Blue / 255.0f, color.Alpha / 255.0f);
}

void GraphicsDevice::SetClearDepth(float32 depth)
{
	if (Graphics::ChangeState(&_boundDepthClear, depth))
		glClearDepth(depth);
}

void GraphicsDevice::SetClearStencil(int32 stencil)
{
	if (Graphics::ChangeState(&_boundStencilClear, stencil))
		glClearStencil(stencil);
}

void GraphicsDevice::SetBlendEquation(uint32 blendOperation, uint32 blendOperationAlpha)
{
	if (!Graphics::ChangeState(&_boundBlendOperation, blendOperation) & !Graphics::ChangeState(&_boundBlendOperationAlpha, blendOperationAlpha))
		return;

	glBlendEquationSeparate(blendOperation, blendOperationAlpha);
}

void GraphicsDevice::SetBlendFuncs(uint32 sourceBlend, uint32 destinationBlend, uint32 sourceBlendAlpha, uint32 destinationBlendAlpha)
{
	if (!Graphics::ChangeState(&_boundSourceBlend, sourceBlend) & !Graphics::ChangeState(&_boundDestinationBlend, destinationBlend) &
		!Graphics::ChangeState(&_boundSourceBlendAlpha, sourceBlendAlpha)& !Graphics::ChangeState(&_boundDestinationBlendAlpha, destinationBlendAlpha))
		return;

	glBlendFuncSeparate(sourceBlend, destinationBlend, sourceBlendAlpha, destinationBlendAlpha);
}

void GraphicsDevice::SetColorMask(int32 mask[4])
{
	if (!Graphics::ChangeState(&_boundColorMaskRed, mask[0]) & !Graphics::ChangeState(&_boundColorMaskGreen, mask[1]) &
		!Graphics::ChangeState(&_boundColorMaskBlue, mask[2]) & !Graphics::ChangeState(&_boundColorMaskAlpha, mask[3]))
		return;

	glColorMask(mask[0], mask[1], mask[2], mask[3]);
}

void* GraphicsDevice::ToPointer(uint32 value)
{
	return reinterpret_cast<void*>(static_cast<size_t>(value));
}

void* GraphicsDevice::ToPointer(int32 value)
{
	return reinterpret_cast<void*>(static_cast<size_t>(value));
}