#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/GraphicsDevice.hpp"
#include "Graphics/RenderTarget.hpp"
#include "Graphics/Texture.hpp"
#include "Graphics/Color.hpp"

void GraphicsDevice::Initialize(RenderTarget* renderTarget, Texture** colorBuffers, Texture* depthStencil)
{
	if (depthStencil != nullptr)
	{
		auto depthStencilPtr = reinterpret_cast<ID3D11DepthStencilView**>(&renderTarget->_depthStencil);
		Direct3D11::CheckResult(_device->CreateDepthStencilView(depthStencil->_texture, nullptr, depthStencilPtr),
								"Failed to create depth stencil view.");
	}

	for (auto i = 0u; i < renderTarget->_colorBufferCount; ++i)
	{
		renderTarget->_colorBuffers[i] = nullptr;
		auto colorBufferPtr = reinterpret_cast<ID3D11RenderTargetView**>(&renderTarget->_colorBuffers[i]);
		Direct3D11::CheckResult(_device->CreateRenderTargetView(colorBuffers[i]->_texture, nullptr, colorBufferPtr),
								"Failed to create render target view.");
	}
}

void GraphicsDevice::Free(RenderTarget* renderTarget)
{
	UnsetState(&_boundRenderTarget, renderTarget);

	for (auto i = 0u; i < renderTarget->_colorBufferCount; ++i)
		Direct3D11::Release(renderTarget->_colorBuffers[i]);

	Direct3D11::Release(renderTarget->_depthStencil);
}

void GraphicsDevice::Bind(const RenderTarget* renderTarget)
{
	if (ChangeState(&_boundRenderTarget, renderTarget))
		_context->OMSetRenderTargets(renderTarget->_colorBufferCount, renderTarget->_colorBuffers, renderTarget->_depthStencil);
}

void GraphicsDevice::ClearColor(RenderTarget* renderTarget, const Color& color)
{
	FLOAT d3dColor[4];
	d3dColor[0] = color.Red;
	d3dColor[1] = color.Green;
	d3dColor[2] = color.Blue;
	d3dColor[3] = color.Alpha;

	for (auto i = 0u; i < renderTarget->_colorBufferCount; ++i)
		_context->ClearRenderTargetView(renderTarget->_colorBuffers[i], d3dColor);
}

void GraphicsDevice::ClearDepthStencil(RenderTarget* renderTarget, bool clearDepth, bool clearStencil, float32 depth, byte stencil)
{
	UINT flags = 0;
	if (clearDepth)
		flags |= D3D11_CLEAR_DEPTH;
	if (clearStencil)
		flags |= D3D11_CLEAR_STENCIL;

	_context->ClearDepthStencilView(renderTarget->_depthStencil, flags, depth, stencil);
}

void GraphicsDevice::SetName(RenderTarget* renderTarget, const char* name)
{
	if (renderTarget->_depthStencil != nullptr)
		Direct3D11::SetName(renderTarget->_depthStencil, String::Format("Depth Stencil Buffer of '{0}'", name).ToCharArray());

	for (auto i = 0u; i < renderTarget->_colorBufferCount; ++i)
		Direct3D11::SetName(renderTarget->_colorBuffers[i], String::Format("Color Buffer {0} of '{1}'", i, name).ToCharArray());
}

#endif