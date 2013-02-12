#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget, pgTexture* texture)
{
	D3DCALL(ID3D11Device_CreateRenderTargetView(DEVICE(renderTarget), (ID3D11Resource*)texture->ptr, NULL, &renderTarget->rt),
		"Failed to create render target.");
	renderTarget->ds = NULL;
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	if (renderTarget->ds != NULL)
		ID3D11DepthStencilView_Release(renderTarget->ds);

	ID3D11RenderTargetView_Release(renderTarget->rt);
}

pgVoid pgClearCore(pgRenderTarget* renderTarget, pgClearTargets targets, pgColor color, pgFloat32 depth, pgUint8 stencil)
{
	FLOAT d3dColor[4];
	d3dColor[0] = color.red;
	d3dColor[1] = color.green;
	d3dColor[2] = color.blue;
	d3dColor[3] = color.alpha;

	if ((targets & PG_CLEAR_COLOR) == PG_CLEAR_COLOR)
		ID3D11DeviceContext_ClearRenderTargetView(CONTEXT(renderTarget), renderTarget->rt, d3dColor);

	if (renderTarget->ds != NULL)
	{
		UINT flags = 0;
		if ((targets & PG_CLEAR_DEPTH) == PG_CLEAR_DEPTH)
			flags |= D3D11_CLEAR_DEPTH;
		if ((targets & PG_CLEAR_STENCIL) == PG_CLEAR_STENCIL)
			flags |= D3D11_CLEAR_STENCIL;

		if (flags != 0)
			ID3D11DeviceContext_ClearDepthStencilView(CONTEXT(renderTarget), renderTarget->ds, flags, depth, stencil);
	}
}

pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget)
{
	ID3D11DeviceContext_OMSetRenderTargets(CONTEXT(renderTarget), 1, &renderTarget->rt, renderTarget->ds);
}

#endif