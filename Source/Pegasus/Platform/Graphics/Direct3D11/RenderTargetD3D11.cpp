#include "Prelude.hpp"

using namespace Direct3D11;

RenderTarget* GraphicsDevice::InitializeRenderTarget(Texture** colorBuffers, int32 colorBufferCount, Texture* depthStencil, int32 width, int32 height)
{
	auto renderTarget = PG_NEW(RenderTarget);
	renderTarget->ColorBufferCount = safe_static_cast<UINT>(colorBufferCount);
	renderTarget->Width = width;
	renderTarget->Height = height;

	if (depthStencil != nullptr)
		CheckResult(_device->CreateDepthStencilView(depthStencil->Obj.Get(), nullptr, &renderTarget->DepthStencil), "Failed to create depth stencil view.");

	for (auto i = 0u; i < renderTarget->ColorBufferCount; ++i)
		CheckResult(_device->CreateRenderTargetView(colorBuffers[i]->Obj.Get(), nullptr, &renderTarget->ColorBuffers[i]), "Failed to create color buffer.");

	return renderTarget;
}

void GraphicsDevice::FreeRenderTarget(RenderTarget* renderTarget)
{
	PG_DELETE(renderTarget);
}

void GraphicsDevice::BindRenderTarget(RenderTarget* renderTarget)
{
	ID3D11RenderTargetView* colorBuffers[Graphics::MaxColorBuffers];
	for (auto i = 0u; i < renderTarget->ColorBufferCount; ++i)
		colorBuffers[i] = renderTarget->ColorBuffers[i].Get();

	_context->OMSetRenderTargets(renderTarget->ColorBufferCount, colorBuffers, renderTarget->DepthStencil.Get());
}

void GraphicsDevice::GetRenderTargetSize(RenderTarget* renderTarget, int32* width, int32* height)
{
	*width = renderTarget->Width;
	*height = renderTarget->Height;
}

void GraphicsDevice::ClearColor(RenderTarget* renderTarget, Color* color)
{
	FLOAT d3dColor[4];
	d3dColor[0] = color->Red / 255.0f;
	d3dColor[1] = color->Green / 255.0f;
	d3dColor[2] = color->Blue / 255.0f;
	d3dColor[3] = color->Alpha / 255.0f;

	for (auto i = 0u; i < renderTarget->ColorBufferCount; ++i)
		_context->ClearRenderTargetView(renderTarget->ColorBuffers[i].Get(), d3dColor);
}

void GraphicsDevice::ClearDepthStencil(RenderTarget* renderTarget, bool clearDepth, bool clearStencil, float32 depth, byte stencil)
{
	UINT flags = 0;
	if (clearDepth)
		flags |= D3D11_CLEAR_DEPTH;
	if (clearStencil)
		flags |= D3D11_CLEAR_STENCIL;

	_context->ClearDepthStencilView(renderTarget->DepthStencil.Get(), flags, depth, stencil);
}

void GraphicsDevice::SetRenderTargetName(RenderTarget* renderTarget, const char* name)
{
	SetName(renderTarget->DepthStencil, Format("Depth Stencil Buffer of '%s'", name).c_str());

	for (auto i = 0u; i < renderTarget->ColorBufferCount; ++i)
		SetName(renderTarget->ColorBuffers[i], Format("Color Buffer #%d of '%s'", i, name).c_str());
}
