#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgTexture* pgCreateTexture(pgGraphicsDevice* device, pgTextureType type, pgVoid* data, pgInt32 width, pgInt32 height, pgInt32 depth, pgSurfaceFormat format)
{
	pgTexture* texture;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(width, 0, 8192);
	PG_ASSERT_IN_RANGE(height, 0, 8192);

	PG_ALLOC(pgTexture, texture);
	texture->device = device;
	texture->width = width;
	texture->height = height;
	texture->depth = depth;
	texture->type = type;
	pgCreateTextureCore(texture, data, format);
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