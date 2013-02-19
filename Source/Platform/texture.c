#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgTexture* pgCreateTexture(pgGraphicsDevice* device, pgTextureType type, pgSurfaceFormat format, pgMipmap* mipmaps)
{
	pgTexture* texture;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(mipmaps);

	PG_ALLOC(pgTexture, texture);
	texture->device = device;
	texture->width = mipmaps[0].width;
	texture->height = mipmaps[0].height;
	pgCreateTextureCore(texture, type, format, mipmaps);
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