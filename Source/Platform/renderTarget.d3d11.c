#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgInt32 i;

	renderTarget->dsPtr = NULL;
	if (renderTarget->depthStencil != NULL)
	{
		D3DCALL(ID3D11Device_CreateDepthStencilView(DEVICE(renderTarget), (ID3D11Resource*)renderTarget->depthStencil->ptr, NULL, &renderTarget->dsPtr),
			"Failed to create depth stencil view.");
	}

	for (i = 0; i < renderTarget->count; ++i)
	{
		D3DCALL(ID3D11Device_CreateRenderTargetView(DEVICE(renderTarget), (ID3D11Resource*)renderTarget->colorBuffers[i]->ptr, NULL, &renderTarget->cbPtr[i]),
			"Failed to create render target view.");
	}
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgInt32 i;
	for (i = 0; i < renderTarget->count; ++i)
		ID3D11RenderTargetView_Release(renderTarget->cbPtr[i]);

	if (renderTarget->depthStencil != NULL)
		ID3D11DepthStencilView_Release(renderTarget->dsPtr);
}

pgVoid pgClearColorCore(pgRenderTarget* renderTarget, pgColor color)
{
	pgInt32 i;
	FLOAT d3dColor[4];
	d3dColor[0] = color.red;
	d3dColor[1] = color.green;
	d3dColor[2] = color.blue;
	d3dColor[3] = color.alpha;

	for (i = 0; i < renderTarget->count; ++i)
		ID3D11DeviceContext_ClearRenderTargetView(CONTEXT(renderTarget), renderTarget->cbPtr[i], d3dColor);
}

pgVoid pgClearDepthStencilCore(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil)
{
	UINT flags = 0;
	if (clearDepth)
		flags |= D3D11_CLEAR_DEPTH;
	if (clearStencil)
		flags |= D3D11_CLEAR_STENCIL;

	ID3D11DeviceContext_ClearDepthStencilView(CONTEXT(renderTarget), renderTarget->dsPtr, flags, depth, stencil);
}

pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget)
{
	ID3D11DeviceContext_OMSetRenderTargets(CONTEXT(renderTarget), renderTarget->count, renderTarget->cbPtr, renderTarget->dsPtr);
}

#endif