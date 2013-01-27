#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgTexture2D* pgCreateTexture2D(pgGraphicsDevice* device, pgVoid* data, pgInt32 width, pgInt32 height, pgSurfaceFormat format)
{
	pgTexture2D* texture2D;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(width, 0, 4096);
	PG_ASSERT_IN_RANGE(height, 0, 4096);

	PG_ALLOC(pgTexture2D, texture2D);
	texture2D->device = device;
	texture2D->width = width;
	texture2D->height = height;
	pgCreateTexture2DCore(texture2D, data, format);
	return texture2D;
}

pgVoid pgDestroyTexture2D(pgTexture2D* texture2D)
{
	PG_ASSERT_NOT_NULL(texture2D);

	pgDestroyTexture2DCore(texture2D);
	PG_FREE(texture2D);
}

pgVoid pgBindTexture(pgTexture2D* texture2D, pgInt32 slot)
{
	PG_ASSERT_NOT_NULL(texture2D);
	PG_ASSERT_IN_RANGE(slot, 0, PG_TEXTURE_SLOT_COUNT);

	pgBindTextureCore(texture2D, slot);
}