#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgValidateAttachments(pgAttachment* attachments, pgInt32 count);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgRenderTarget* pgCreateRenderTarget(pgGraphicsDevice* device, pgAttachment* attachments, pgInt32 count)
{
	pgRenderTarget* renderTarget;
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(attachments);
	PG_ASSERT_IN_RANGE(count, 1, PG_MAX_ATTACHMENTS);
	pgValidateAttachments(attachments, count);

	PG_ALLOC(pgRenderTarget, renderTarget);
	renderTarget->device = device;
	renderTarget->width = attachments[0].texture->desc.width;
	renderTarget->height = attachments[0].texture->desc.height;
	pgCreateRenderTargetCore(renderTarget, attachments, count);
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

pgVoid pgClear(pgRenderTarget* renderTarget, pgClearTargets targets, pgColor color, pgFloat32 depth, pgUint8 stencil)
{
	PG_ASSERT_NOT_NULL(renderTarget);
	pgClearCore(renderTarget, targets, color, depth, stencil);
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

static pgVoid pgValidateAttachments(pgAttachment* attachments, pgInt32 count)
{
	pgInt32 i, j;

	pgUint32 width = attachments[0].texture->desc.width;
	pgUint32 height = attachments[0].texture->desc.height;
	pgUint32 depth = attachments[0].texture->desc.depth;
	pgTextureType type = attachments[0].texture->desc.type;

	for (i = 0; i < count; ++i)
	{
		PG_ASSERT(attachments[i].texture->desc.width == width, "All attached textures must have the same width.");
		PG_ASSERT(attachments[i].texture->desc.height == height, "All attached textures must have the same height.");
		PG_ASSERT(attachments[i].texture->desc.depth == depth, "All attached textures must have the same depth.");
		PG_ASSERT(attachments[i].texture->desc.type == type, "All attached textures must be of the same type.");
		PG_ASSERT(attachments[i].texture->desc.flags & PG_TEXTURE_RENDERABLE, "The texture cannot be attached to a render target.");

		for (j = 0; j < i; ++j)
		{
			PG_ASSERT(attachments[i].attachment != attachments[j].attachment, 
				"An attempt was made to attach more than one texture to the same attachment point.");
			PG_ASSERT(attachments[i].texture != attachments[j].texture, 
				"An attempt was made to attach the same texture to more than one attachment point.");
		}
	}
}