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
		PG_D3DCALL(ID3D11Device_CreateDepthStencilView(PG_DEVICE(renderTarget), (ID3D11Resource*)renderTarget->depthStencil->ptr, NULL, &renderTarget->dsPtr),
			"Failed to create depth stencil view.");
	}

	for (i = 0; i < renderTarget->count; ++i)
	{
		PG_D3DCALL(ID3D11Device_CreateRenderTargetView(PG_DEVICE(renderTarget), (ID3D11Resource*)renderTarget->colorBuffers[i]->ptr, NULL, &renderTarget->cbPtr[i]),
			"Failed to create render target view.");
	}
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgInt32 i;
	for (i = 0; i < renderTarget->count; ++i)
		PG_SAFE_RELEASE(ID3D11RenderTargetView, renderTarget->cbPtr[i]);

	PG_SAFE_RELEASE(ID3D11DepthStencilView, renderTarget->dsPtr);
}

pgVoid pgClearColorCore(pgRenderTarget* renderTarget, pgColor color)
{
	pgInt32 i;
	FLOAT d3dColor[4];
	d3dColor[0] = color.red / 255.0f;
	d3dColor[1] = color.green / 255.0f;
	d3dColor[2] = color.blue / 255.0f;
	d3dColor[3] = color.alpha / 255.0f;

	for (i = 0; i < renderTarget->count; ++i)
		ID3D11DeviceContext_ClearRenderTargetView(PG_CONTEXT(renderTarget), renderTarget->cbPtr[i], d3dColor);
}

pgVoid pgClearDepthStencilCore(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil)
{
	UINT flags = 0;

	PG_ASSERT(renderTarget->dsPtr != NULL, "Cannot clear depth stencil of a render target without a depth stencil buffer.");

	if (clearDepth)
		flags |= D3D11_CLEAR_DEPTH;
	if (clearStencil)
		flags |= D3D11_CLEAR_STENCIL;

	ID3D11DeviceContext_ClearDepthStencilView(PG_CONTEXT(renderTarget), renderTarget->dsPtr, flags, depth, stencil);
}

pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget)
{
	ID3D11DeviceContext_OMSetRenderTargets(PG_CONTEXT(renderTarget), renderTarget->count, renderTarget->cbPtr, renderTarget->dsPtr);
}

#endif