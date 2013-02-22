#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgRenderTarget* pgCreateRenderTarget(pgGraphicsDevice* device, pgTexture* texture)
{
	pgRenderTarget* renderTarget;
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(texture);

	PG_ALLOC(pgRenderTarget, renderTarget);
	renderTarget->device = device;
	renderTarget->width = texture->desc.width;
	renderTarget->height = texture->desc.height;
	pgCreateRenderTargetCore(renderTarget, texture);
	return renderTarget;
}

pgVoid pgDestroyRenderTarget(pgRenderTarget* renderTarget)
{
	PG_ASSERT_NOT_NULL(renderTarget);

	if (renderTarget->device->state.renderTarget == renderTarget)
		renderTarget->device->state.renderTarget = NULL;

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

	if (renderTarget->device->state.renderTarget == renderTarget)
		return;

	renderTarget->device->state.renderTarget = renderTarget;
	pgBindRenderTargetCore(renderTarget);

	// Reset viewport and scissor rectangle
	pgSetViewport(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
	pgSetScissorRect(renderTarget->device, 0, 0, renderTarget->width, renderTarget->height);
}