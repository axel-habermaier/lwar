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

	swapChain->renderTarget.rt = NULL;
	swapChain->renderTarget.ds = NULL;
	InitializeBackBuffer(swapChain);

	// Initially, we set the viewport to match the back buffer size
	pgSetViewport(swapChain->device, 0, 0, width, height);
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

	D3DCALL(IDXGISwapChain_GetBuffer(swapChain->ptr, 0, &IID_ID3D11Texture2D, &tex), 
		"Failed to get backbuffer from swap chain.");

	D3DCALL(ID3D11Device_CreateRenderTargetView(DEVICE(swapChain), (ID3D11Resource*)tex, NULL, &swapChain->renderTarget.rt), 
		"Failed to initialize backbuffer render target.");

	ID3D11Texture2D_Release(tex);
	ID3D11DeviceContext_OMSetRenderTargets(CONTEXT(swapChain), 1, &swapChain->renderTarget.rt, NULL);
}

static pgVoid ReleaseBackBuffer(pgSwapChain* swapChain)
{
	if (swapChain->renderTarget.rt == NULL)
		return;

	ID3D11DeviceContext_OMSetRenderTargets(CONTEXT(swapChain), 0, NULL, NULL);
	ID3D11RenderTargetView_Release(swapChain->renderTarget.rt);
	swapChain->renderTarget.rt = NULL;
}

#endif