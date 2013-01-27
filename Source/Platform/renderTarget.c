#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgRenderTarget* pgCreateRenderTarget(pgGraphicsDevice* device, pgTexture2D* texture2D)
{
	pgRenderTarget* renderTarget;
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(texture2D);

	PG_ALLOC(pgRenderTarget, renderTarget);
	renderTarget->device = device;
	renderTarget->width = texture2D->width;
	renderTarget->height = texture2D->height;
	pgCreateRenderTargetCore(renderTarget, texture2D);
	return renderTarget;
}

pgVoid pgDestroyRenderTarget(pgRenderTarget* renderTarget)
{
	PG_ASSERT_NOT_NULL(renderTarget);

	pgDestroyRenderTargetCore(renderTarget);
	PG_FREE(renderTarget);
}

pgVoid pgClear(pgRenderTarget* renderTarget, pgClearTargets targets, pgColor color, pgFloat32 depth, pgUint8 stencil)
{
	PG_ASSERT_NOT_NULL(renderTarget);
	pgClearCore(renderTarget, targets, color, depth, stencil);
}

pgVoid pgBindRenderTarget(pgRenderTarget* renderTarget)
{
	PG_ASSERT_NOT_NULL(renderTarget);
	pgBindRenderTargetCore(renderTarget);

	// Reset viewport and scissor rectangle
	pgSetViewport(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
	pgSetScissorRect(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
}