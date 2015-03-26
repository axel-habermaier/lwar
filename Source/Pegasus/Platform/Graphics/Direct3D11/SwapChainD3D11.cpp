#include "Prelude.hpp"

using namespace Direct3D11;

SwapChain* GraphicsDevice::InitializeSwapChain(NativeWindow* window)
{
	DXGI_SWAP_CHAIN_DESC d3d11Desc = { 0 };
	d3d11Desc.BufferCount = 2;
	d3d11Desc.Flags = 0;
	d3d11Desc.Windowed = true;
	d3d11Desc.BufferDesc.Width = 800;
	d3d11Desc.BufferDesc.Height = 600;
	d3d11Desc.BufferDesc.RefreshRate.Numerator = 0;
	d3d11Desc.BufferDesc.RefreshRate.Denominator = 0;
	d3d11Desc.BufferDesc.Format = SwapChainFormat;
	d3d11Desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	d3d11Desc.OutputWindow = window->GetHWND();
	d3d11Desc.SampleDesc.Count = 1;
	d3d11Desc.SampleDesc.Quality = 0;
	d3d11Desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

	auto swapChain = PG_NEW(SwapChain);
	swapChain->BackBuffer.ColorBufferCount = 1;

	CheckResult(_factory->CreateSwapChain(_device.Get(), &d3d11Desc, &swapChain->Obj), "Failed to create swap chain.");
	CheckResult(_factory->MakeWindowAssociation(window->GetHWND(), DXGI_MWA_NO_WINDOW_CHANGES), "Failed to disable DXGI window message handling.");

	return swapChain;
}

void GraphicsDevice::FreeSwapChain(SwapChain* swapChain)
{
	PG_DELETE(swapChain);
}

void GraphicsDevice::PresentSwapChain(SwapChain* swapChain)
{
	swapChain->Obj->Present(0, 0);
}

void GraphicsDevice::ResizeSwapChain(SwapChain* swapChain, int32 width, int32 height)
{
	ReleaseBackBuffer(swapChain);

	CheckResult(
		swapChain->Obj->ResizeBuffers(2, safe_static_cast<UINT>(width), safe_static_cast<UINT>(height), SwapChainFormat, 0),
		"Failed to resize swap chain buffers.");

	swapChain->BackBuffer.Width = width;
	swapChain->BackBuffer.Height = height;;
	InitializeBackBuffer(swapChain);
}

RenderTarget* GraphicsDevice::GetBackBuffer(SwapChain* swapChain)
{
	return &swapChain->BackBuffer;
}

void GraphicsDevice::ReleaseBackBuffer(SwapChain* swapChain)
{
	if (swapChain->BackBuffer.ColorBuffers[0] == nullptr)
		return;

	_context->OMSetRenderTargets(0, nullptr, nullptr);
	swapChain->BackBuffer.ColorBuffers[0].Reset();
}

void GraphicsDevice::InitializeBackBuffer(SwapChain* swapChain)
{
	ComPtr<ID3D11Texture2D> texture;

	CheckResult(
		swapChain->Obj->GetBuffer(0, __uuidof(texture), reinterpret_cast<void**>(texture.GetAddressOf())),
		"Failed to get back buffer from swap chain.");

	CheckResult(
		_device->CreateRenderTargetView(texture.Get(), nullptr, &swapChain->BackBuffer.ColorBuffers[0]),
		"Failed to initialize back buffer render target.");

	BindRenderTarget(&swapChain->BackBuffer);
	PG_DEBUG_ONLY(SetName(swapChain->BackBuffer.ColorBuffers[0], "Back Buffer"));
}

void GraphicsDevice::ChangeToWindowedCore(SwapChain* swapChain, int32 width, int32 height)
{
	DXGI_MODE_DESC desc = { 0 };
	desc.Format = SwapChainFormat;
	desc.Width = safe_static_cast<UINT>(width);
	desc.Height = safe_static_cast<UINT>(height);
	desc.RefreshRate.Numerator = 0;
	desc.RefreshRate.Denominator = 0;

	if (swapChain->Obj->SetFullscreenState(false, nullptr) != S_OK)
		PG_ERROR("Error while leaving fullscreen mode.");

	if (swapChain->Obj->ResizeTarget(&desc) != S_OK)
		PG_ERROR("Error while resizing swap chain target.");
}

bool GraphicsDevice::ChangeToFullscreenCore(SwapChain* swapChain, int32 width, int32 height)
{
	// See also http://msdn.microsoft.com/en-us/library/windows/desktop/ee417025(v=vs.85).aspx#full-screen_issues

	DXGI_MODE_DESC desc = { 0 };
	desc.Format = SwapChainFormat;
	desc.Width = safe_static_cast<UINT>(width);
	desc.Height = safe_static_cast<UINT>(height);
	desc.RefreshRate.Numerator = 60;
	desc.RefreshRate.Denominator = 1;

	if (swapChain->Obj->ResizeTarget(&desc) != S_OK)
	{
		PG_ERROR("Error while resizing swap chain target.");
		return false;
	}

	if (swapChain->Obj->SetFullscreenState(true, nullptr) != S_OK)
	{
		PG_ERROR("Error while entering fullscreen mode.");
		return false;
	}

	if (swapChain->Obj->ResizeTarget(&desc) != S_OK)
	{
		PG_ERROR("Error while resizing swap chain target.");
		return false;
	}

	ResizeSwapChain(swapChain, width, height);
	PresentSwapChain(swapChain);

	return true;
}