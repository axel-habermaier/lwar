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
	pgInt32 width, height;
	DXGI_SWAP_CHAIN_DESC desc;
	pgGetWindowSize(window, &width, &height);

	swapChain->renderTarget.count = 1;
	swapChain->renderTarget.dsPtr = NULL;
	swapChain->renderTarget.cbPtr[0] = NULL;
	swapChain->format = DXGI_FORMAT_R8G8B8A8_UNORM;

	memset(&desc, 0, sizeof(DXGI_SWAP_CHAIN_DESC));
	desc.BufferCount = 2;
	desc.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
	desc.Windowed = PG_TRUE;
	desc.BufferDesc.Width = width;
	desc.BufferDesc.Height = height;
	desc.BufferDesc.RefreshRate.Numerator = 60;
	desc.BufferDesc.RefreshRate.Denominator = 1;
	desc.BufferDesc.Format = swapChain->format;
	desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	desc.OutputWindow = window->hwnd;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

	D3DCALL(IDXGIFactory_CreateSwapChain(swapChain->device->factory, (IUnknown*)DEVICE(swapChain), &desc, &swapChain->ptr), 
		"Failed to create swap chain.");

	// Instruct the runtime to ignore the user pressing ALT+Enter to switch between fullscreen and windowed
	// mode; this doesn't work reliably
	IDXGIFactory_MakeWindowAssociation(swapChain->device->factory, window->hwnd, DXGI_MWA_NO_ALT_ENTER);

	// Initially, we set the viewport to match the back buffer size
	pgSetViewport(swapChain->device, 0, 0, width, height);

	pgResizeSwapChain(swapChain, width, height);
}

pgVoid pgDestroySwapChainCore(pgSwapChain* swapChain)
{
	// Full screen mode must be left before the swap chain can be destroyed safely
	D3DCALL(IDXGISwapChain_SetFullscreenState(swapChain->ptr, PG_FALSE, NULL), "Error while leaving fullscreen mode.");

	ReleaseBackBuffer(swapChain);
	IDXGISwapChain_Release(swapChain->ptr);
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

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgVoid InitializeBackBuffer(pgSwapChain* swapChain)
{
	ID3D11Texture2D* tex;
	D3D11_TEXTURE2D_DESC depthStencilDesc;

	D3DCALL(IDXGISwapChain_GetBuffer(swapChain->ptr, 0, &IID_ID3D11Texture2D, &tex), 
		"Failed to get backbuffer from swap chain.");

	D3DCALL(ID3D11Device_CreateRenderTargetView(DEVICE(swapChain), (ID3D11Resource*)tex, NULL, &swapChain->renderTarget.cbPtr[0]), 
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

	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(swapChain), &depthStencilDesc, NULL, &tex), 
		"Failed to initialize depth stencil buffer of swap chain.");

	D3DCALL(ID3D11Device_CreateDepthStencilView(DEVICE(swapChain), (ID3D11Resource*)tex, NULL, &swapChain->renderTarget.dsPtr),
		"Failed to initialize depth stencil view of swap chain.");

	ID3D11Texture2D_Release(tex);

	pgBindRenderTarget(&swapChain->renderTarget);
}

static pgVoid ReleaseBackBuffer(pgSwapChain* swapChain)
{
	if (swapChain->renderTarget.cbPtr[0] == NULL)
		return;

	swapChain->device->renderTarget = NULL;
	ID3D11DeviceContext_OMSetRenderTargets(CONTEXT(swapChain), 0, NULL, NULL);

	ID3D11RenderTargetView_Release(swapChain->renderTarget.cbPtr[0]);
	ID3D11DepthStencilView_Release(swapChain->renderTarget.dsPtr);

	swapChain->renderTarget.cbPtr[0] = NULL;
	swapChain->renderTarget.dsPtr = NULL;
}

#endif