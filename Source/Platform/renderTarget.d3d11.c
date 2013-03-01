#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget, pgAttachment* attachments, pgInt32 count)
{
	pgInt32 i;
	for (i = 0; i < count; ++i)
	{
		ID3D11RenderTargetView* renderTargetView;
		if (attachments[i].attachment != PG_DEPTH_STENCIL_ATTACHMENT)
		{
			D3DCALL(ID3D11Device_CreateRenderTargetView(DEVICE(renderTarget), (ID3D11Resource*)texture->ptr, NULL, renderTargetView),
				"Failed to create render target.");
		}

		switch (attachments[i].attachment)
		{
		case PG_DEPTH_STENCIL_ATTACHMENT:
			break;
		case PG_COLOR_ATTACHMENT_0:
			break;
		case PG_COLOR_ATTACHMENT_1:
			break;
		case PG_COLOR_ATTACHMENT_2:
			break;
		case PG_COLOR_ATTACHMENT_3:
			break;
		default:
			PG_NO_SWITCH_DEFAULT;
		}
	}
	/*
	renderTarget->ds = NULL;*/
}

pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget)
{
	pgInt32
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