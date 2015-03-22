#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/GraphicsDevice.hpp"
#include "Graphics/SwapChain.hpp"
#include "UserInterface/NativeWindow.hpp"
#include "Math/Math.hpp"

static const DXGI_FORMAT SwapChainFormat = DXGI_FORMAT_R8G8B8A8_UNORM;

void GraphicsDevice::Initialize(SwapChain* swapChain, const NativeWindow& window)
{
	DXGI_SWAP_CHAIN_DESC d3d11Desc = { 0 };

	auto size = window.GetSize();
	auto hwnd = *static_cast<const HWND*>(window.GetPlatformHandle());

	d3d11Desc.BufferCount = 2;
	d3d11Desc.Flags = 0;
	d3d11Desc.Windowed = true;
	d3d11Desc.BufferDesc.Width = Math::ToUInt32(size.Width);
	d3d11Desc.BufferDesc.Height = Math::ToUInt32(size.Height);
	d3d11Desc.BufferDesc.RefreshRate.Numerator = 0;
	d3d11Desc.BufferDesc.RefreshRate.Denominator = 0;
	d3d11Desc.BufferDesc.Format = SwapChainFormat;
	d3d11Desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	d3d11Desc.OutputWindow = hwnd;
	d3d11Desc.SampleDesc.Count = 1;
	d3d11Desc.SampleDesc.Quality = 0;
	d3d11Desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

	Direct3D11::CheckResult(_factory->CreateSwapChain(_device, &d3d11Desc, &swapChain->_swapChain), "Failed to create swap chain.");
	Direct3D11::CheckResult(_factory->MakeWindowAssociation(hwnd, DXGI_MWA_NO_WINDOW_CHANGES), "Failed to make window association.");

	swapChain->_backBuffer._depthStencil = nullptr;
	swapChain->_backBuffer._colorBuffers[0] = nullptr;
}

void GraphicsDevice::Free(SwapChain* swapChain)
{
	UnsetState(&_boundRenderTarget, &swapChain->_backBuffer);
	Direct3D11::Release(swapChain->_swapChain);
}

void GraphicsDevice::Present(SwapChain* swapChain)
{
	swapChain->_swapChain->Present(0, 0);
}

void GraphicsDevice::Resize(SwapChain* swapChain, const Size& size)
{
	if (swapChain->_backBuffer._size == size || size == Size::Zero)
		return;

	ReleaseBackBuffer(swapChain);

	Direct3D11::CheckResult(swapChain->_swapChain->ResizeBuffers(2, Math::ToUInt32(size.Width), Math::ToUInt32(size.Height),
		SwapChainFormat, 0), "Failed to resize swap chain buffers.");

	swapChain->_backBuffer._size = size;
	InitializeBackBuffer(swapChain);
}

void GraphicsDevice::ReleaseBackBuffer(SwapChain* swapChain)
{
	if (swapChain->_backBuffer._colorBuffers[0] == nullptr)
		return;

	_context->OMSetRenderTargets(0, nullptr, nullptr);
	UnsetState(&_boundRenderTarget, &swapChain->_backBuffer);

	swapChain->_backBuffer._colorBuffers[0]->Release();
	swapChain->_backBuffer._colorBuffers[0] = nullptr;
}

void GraphicsDevice::InitializeBackBuffer(SwapChain* swapChain)
{
	ID3D11Texture2D* texture;

	Direct3D11::CheckResult(swapChain->_swapChain->GetBuffer(0, __uuidof(texture), reinterpret_cast<void**>(&texture)),
							"Failed to get back buffer from swap chain.");

	Direct3D11::CheckResult(_device->CreateRenderTargetView(texture, nullptr, &swapChain->_backBuffer._colorBuffers[0]),
							"Failed to initialize back buffer render target.");

	texture->Release();
	swapChain->_backBuffer.Bind();

	PG_DEBUG_ONLY(swapChain->_backBuffer.SetName("Back Buffer"));
}

void GraphicsDevice::ChangeToWindowedCore(SwapChain* swapChain, Size size)
{
	DXGI_MODE_DESC desc;
	Memory::Set(&desc, 0);
	desc.Format = SwapChainFormat;
	desc.Width = Math::ToUInt32(size.Width);
	desc.Height = Math::ToUInt32(size.Height);
	desc.RefreshRate.Numerator = 0;
	desc.RefreshRate.Denominator = 0;

	if (swapChain->_swapChain->SetFullscreenState(false, nullptr) != S_OK)
		PG_ERROR("Error while leaving fullscreen mode.");

	if (swapChain->_swapChain->ResizeTarget(&desc) != S_OK)
		PG_ERROR("Error while resizing swap chain target.");
}

bool GraphicsDevice::ChangeToFullscreenCore(SwapChain* swapChain, Size resolution)
{
	// See also http://msdn.microsoft.com/en-us/library/windows/desktop/ee417025(v=vs.85).aspx#full-screen_issues

	DXGI_MODE_DESC desc;
	Memory::Set(&desc, 0);
	desc.Format = SwapChainFormat;
	desc.Width = Math::ToUInt32(resolution.Width);
	desc.Height = Math::ToUInt32(resolution.Height);
	desc.RefreshRate.Numerator = 60;
	desc.RefreshRate.Denominator = 1;

	if (swapChain->_swapChain->ResizeTarget(&desc) != S_OK)
	{
		PG_ERROR("Error while resizing swap chain target.");
		return false;
	}

	if (swapChain->_swapChain->SetFullscreenState(true, nullptr) != S_OK)
	{
		PG_ERROR("Error while entering fullscreen mode.");
		return false;
	}

	if (swapChain->_swapChain->ResizeTarget(&desc) != S_OK)
	{
		PG_ERROR("Error while resizing swap chain target.");
		return false;
	}

	Resize(swapChain, resolution);
	Present(swapChain);

	return true;
}

#endif