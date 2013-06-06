#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgVoid InitializeBackBuffer(pgSwapChain* swapChain);
static pgVoid ReleaseBackBuffer(pgSwapChain* swapChain);

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateSwapChainCore(pgSwapChain* swapChain, pgWindow* window)
{
	pgInt32 width = window->placement.width;
	pgInt32 height = window->placement.height;
	DXGI_SWAP_CHAIN_DESC desc;

	swapChain->renderTarget.count = 1;
	swapChain->format = DXGI_FORMAT_R8G8B8A8_UNORM;

	memset(&desc, 0, sizeof(DXGI_SWAP_CHAIN_DESC));
	desc.BufferCount = 2;
	desc.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
	desc.Windowed = PG_TRUE;
	desc.BufferDesc.Width = width;
	desc.BufferDesc.Height = height;
	desc.BufferDesc.RefreshRate.Numerator = 0;
	desc.BufferDesc.RefreshRate.Denominator = 0;
	desc.BufferDesc.Format = swapChain->format;
	desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	desc.OutputWindow = window->hwnd;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

	PG_D3DCALL(IDXGIFactory_CreateSwapChain(swapChain->device->factory, (IUnknown*)PG_DEVICE(swapChain), &desc, &swapChain->ptr), 
		"Failed to create swap chain.");

	// Ignore the user pressing Alt+Enter to switch between fullscreen and windowed mode; this doesn't work reliably
	PG_D3DCALL(IDXGIFactory_MakeWindowAssociation(swapChain->device->factory, window->hwnd, DXGI_MWA_NO_WINDOW_CHANGES),
		"Failed to make window association.");

	pgResizeSwapChain(swapChain, width, height);
}

pgVoid pgDestroySwapChainCore(pgSwapChain* swapChain)
{
	// Full screen mode must be left before the swap chain can be destroyed safely
	PG_D3DCALL(IDXGISwapChain_SetFullscreenState(swapChain->ptr, PG_FALSE, NULL), "Error while leaving fullscreen mode.");

	ReleaseBackBuffer(swapChain);
	PG_SAFE_RELEASE(IDXGISwapChain, swapChain->ptr);
}

pgVoid pgPresentCore(pgSwapChain* swapChain)
{
	IDXGISwapChain_Present(swapChain->ptr, 0, 0);
}

pgVoid pgResizeSwapChainCore(pgSwapChain* swapChain, pgInt32 width, pgInt32 height)
{
	swapChain->renderTarget.width = width;
	swapChain->renderTarget.height = height;

	ReleaseBackBuffer(swapChain);
	IDXGISwapChain_ResizeBuffers(swapChain->ptr, 2, width, height, swapChain->format, DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH);
	InitializeBackBuffer(swapChain);
}

// See also http://msdn.microsoft.com/en-us/library/windows/desktop/ee417025(v=vs.85).aspx#full-screen_issues
pgBool pgSwapChainFullscreenCore(pgSwapChain* swapChain, pgInt32 width, pgInt32 height)
{
	DXGI_MODE_DESC desc;
	memset(&desc, 0, sizeof(desc));
	desc.Format = swapChain->format;
	desc.Width = width;
	desc.Height = height;
	desc.RefreshRate.Numerator = 60;
	desc.RefreshRate.Denominator = 1;

	if (IDXGISwapChain_ResizeTarget(swapChain->ptr, &desc) != S_OK)
	{
		PG_ERROR("Error while resizing swap chain target.");
		return PG_FALSE;
	}

	if (IDXGISwapChain_SetFullscreenState(swapChain->ptr, PG_TRUE, NULL) != S_OK)
	{
		PG_ERROR("Error while entering fullscreen mode.");
		return PG_FALSE;
	}

	if (IDXGISwapChain_ResizeTarget(swapChain->ptr, &desc) != S_OK)
	{
		PG_ERROR("Error while resizing swap chain target.");
		return PG_FALSE;
	}

	pgResizeSwapChain(swapChain, width, height);
	pgPresent(swapChain);

	return PG_TRUE;
}

pgVoid pgSwapChainWindowedCore(pgSwapChain* swapChain)
{
	DXGI_MODE_DESC desc;
	memset(&desc, 0, sizeof(desc));
	desc.Format = swapChain->format;
	desc.Width = swapChain->windowedWidth;
	desc.Height = swapChain->windowedHeight;
	desc.RefreshRate.Numerator = 0;
	desc.RefreshRate.Denominator = 0;

	if (IDXGISwapChain_SetFullscreenState(swapChain->ptr, PG_FALSE, NULL) != S_OK)
		PG_ERROR("Error while leaving fullscreen mode.");

	if (IDXGISwapChain_ResizeTarget(swapChain->ptr, &desc) != S_OK)
		PG_ERROR("Error while resizing swap chain target.");
}

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgVoid InitializeBackBuffer(pgSwapChain* swapChain)
{
	ID3D11Texture2D* tex;
	D3D11_TEXTURE2D_DESC depthStencilDesc;

	PG_D3DCALL(IDXGISwapChain_GetBuffer(swapChain->ptr, 0, &IID_ID3D11Texture2D, &tex), 
		"Failed to get backbuffer from swap chain.");

	PG_D3DCALL(ID3D11Device_CreateRenderTargetView(PG_DEVICE(swapChain), (ID3D11Resource*)tex, NULL, &swapChain->renderTarget.cbPtr[0]), 
		"Failed to initialize backbuffer render target.");

	ID3D11Texture2D_Release(tex);

	depthStencilDesc.Width = swapChain->renderTarget.width;
	depthStencilDesc.Height = swapChain->renderTarget.height;
	depthStencilDesc.MipLevels = 1;
	depthStencilDesc.ArraySize = 1;
	depthStencilDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
	depthStencilDesc.SampleDesc.Count = 1;
	depthStencilDesc.SampleDesc.Quality = 0;
	depthStencilDesc.Usage = D3D11_USAGE_DEFAULT;
	depthStencilDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
	depthStencilDesc.CPUAccessFlags = 0;
	depthStencilDesc.MiscFlags = 0;

	PG_D3DCALL(ID3D11Device_CreateTexture2D(PG_DEVICE(swapChain), &depthStencilDesc, NULL, &tex), 
		"Failed to initialize depth stencil buffer of swap chain.");

	PG_D3DCALL(ID3D11Device_CreateDepthStencilView(PG_DEVICE(swapChain), (ID3D11Resource*)tex, NULL, &swapChain->renderTarget.dsPtr),
		"Failed to initialize depth stencil view of swap chain.");

	ID3D11Texture2D_Release(tex);

	pgBindRenderTarget(&swapChain->renderTarget);
}

static pgVoid ReleaseBackBuffer(pgSwapChain* swapChain)
{
	if (swapChain->renderTarget.cbPtr[0] == NULL)
		return;

	swapChain->device->renderTarget = NULL;
	ID3D11DeviceContext_OMSetRenderTargets(PG_CONTEXT(swapChain), 0, NULL, NULL);

	ID3D11RenderTargetView_Release(swapChain->renderTarget.cbPtr[0]);
	ID3D11DepthStencilView_Release(swapChain->renderTarget.dsPtr);

	swapChain->renderTarget.cbPtr[0] = NULL;
	swapChain->renderTarget.dsPtr = NULL;
}

#endif