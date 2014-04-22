#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgValidate(pgTexture** colorBuffers, pgInt32 count, pgTexture* depthStencil);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgRenderTarget* pgCreateRenderTarget(pgGraphicsDevice* device, pgTexture** colorBuffers, pgInt32 count, pgTexture* depthStencil)
{
	pgInt32 i;
	pgRenderTarget* renderTarget;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(count, 0, PG_MAX_COLOR_ATTACHMENTS);
	PG_ASSERT(count > 0 || depthStencil != NULL, "Cannot create render target without any buffer.");
	pgValidate(colorBuffers, count, depthStencil);

	PG_ALLOC(pgRenderTarget, renderTarget);
	renderTarget->device = device;
	renderTarget->depthStencil = depthStencil;
	renderTarget->count = count;

	for (i = 0; i < count; ++i)
		renderTarget->colorBuffers[i] = colorBuffers[i];

	if (count > 0)
	{
		renderTarget->width = colorBuffers[0]->desc.width;
		renderTarget->height = colorBuffers[0]->desc.height;
	}
	else
	{
		renderTarget->width = depthStencil->desc.width;
		renderTarget->height = depthStencil->desc.height;
	}

	pgCreateRenderTargetCore(renderTarget);
	return renderTarget;
}

pgVoid pgDestroyRenderTarget(pgRenderTarget* renderTarget)
{
	if (renderTarget == NULL)
		return;

	if (renderTarget->device->renderTarget == renderTarget)
		renderTarget->device->renderTarget = NULL;

	pgDestroyRenderTargetCore(renderTarget);
	PG_FREE(renderTarget);
}

pgVoid pgGetRenderTargetSize(pgRenderTarget* renderTarget, pgInt32* width, pgInt32* height)
{
	PG_ASSERT_NOT_NULL(renderTarget);
	PG_ASSERT_NOT_NULL(width);
	PG_ASSERT_NOT_NULL(height);

	*width = renderTarget->width;
	*height = renderTarget->height;
}

pgVoid pgClearColor(pgRenderTarget* renderTarget, pgColor color)
{
	PG_ASSERT_NOT_NULL(renderTarget);
	PG_ASSERT(renderTarget->count > 0, "Cannot clear color of a render target without any color buffers.");
	PG_ASSERT(renderTarget->device->renderTarget == renderTarget, "Cannot clear color of unbound render target.");

	pgClearColorCore(renderTarget, color);
}

pgVoid pgClearDepthStencil(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil)
{
	PG_ASSERT_NOT_NULL(renderTarget);
	PG_ASSERT(clearDepth || clearStencil, "Either depth or stencil clearing must be enabled.");
	PG_ASSERT(renderTarget->device->renderTarget == renderTarget, "Cannot clear depth stencil of unbound render target.");

	pgClearDepthStencilCore(renderTarget, clearDepth, clearStencil, depth, stencil);
}

pgVoid pgBindRenderTarget(pgRenderTarget* renderTarget)
{
	PG_ASSERT_NOT_NULL(renderTarget);

	if (renderTarget->device->renderTarget == renderTarget)
		return;

	renderTarget->device->renderTarget = renderTarget;
	pgBindRenderTargetCore(renderTarget);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgValidate(pgTexture** colorBuffers, pgInt32 count, pgTexture* depthStencil)
{
	pgInt32 i, j;

	pgUint32 width = colorBuffers[0]->desc.width;
	pgUint32 height = colorBuffers[0]->desc.height;
	pgUint32 depth = colorBuffers[0]->desc.depth;
	pgTextureType type = colorBuffers[0]->desc.type;

	PG_ASSERT(depthStencil == NULL || depthStencil->desc.flags & PG_TEXTURE_BIND_DEPTH_STENCIL, 
		"The texture cannot be attached as a depth stencil buffer.");

	for (i = 0; i < count; ++i)
	{
		PG_ASSERT(colorBuffers[i]->desc.width == width, "All attached textures must have the same width.");
		PG_ASSERT(colorBuffers[i]->desc.height == height, "All attached textures must have the same height.");
		PG_ASSERT(colorBuffers[i]->desc.depth == depth, "All attached textures must have the same depth.");
		PG_ASSERT(colorBuffers[i]->desc.type == type, "All attached textures must be of the same type.");
		PG_ASSERT(colorBuffers[i]->desc.flags & PG_TEXTURE_BIND_RENDER_TARGET, "The texture cannot be attached as a color buffer.");

		for (j = 0; j < i; ++j)
			PG_ASSERT(colorBuffers[i] != colorBuffers[j], "An attempt was made to attach the same texture more than once.");
	}
}