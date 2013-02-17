#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgTexture* pgCreateTexture(pgGraphicsDevice* device, pgTextureDesc* description, pgVoid* data)
{
	pgTexture* texture;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);
	PG_ASSERT_IN_RANGE(description->width, 0, 8192);
	PG_ASSERT_IN_RANGE(description->height, 0, 8192);

	PG_ALLOC(pgTexture, texture);
	texture->device = device;
	texture->desc = *description;
	pgCreateTextureCore(texture, data);
	return texture;
}

pgVoid pgDestroyTexture(pgTexture* texture)
{
	PG_ASSERT_NOT_NULL(texture);

	pgDestroyTextureCore(texture);
	PG_FREE(texture);
}

pgVoid pgBindTexture(pgTexture* texture, pgInt32 slot)
{
	PG_ASSERT_NOT_NULL(texture);
	PG_ASSERT_IN_RANGE(slot, 0, PG_TEXTURE_SLOT_COUNT);

	pgBindTextureCore(texture, slot);
}

pgVoid pgGenerateMipmaps(pgTexture* texture)
{
	PG_ASSERT_NOT_NULL(texture);
	pgGenerateMipmapsCore(texture);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgInt32 pgMipmapCount(pgInt32 width, pgInt32 height)
{
	pgInt32 count = 0;
	while (width > 1 || height > 1)
	{
		width /= 2;
		height /= 2;
		
		width = width < 1 ? 1 : width;
		height = height < 1 ? 1 : height;
		++count;
	}
	return count;
}