#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgTexture* pgCreateTexture(pgGraphicsDevice* device, pgTextureDescription* description, pgSurface* surfaces)
{
	pgTexture* texture;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);
	PG_ASSERT_NOT_NULL(surfaces);

	PG_ALLOC(pgTexture, texture);
	texture->device = device;
	texture->desc = *description;
	pgCreateTextureCore(texture, surfaces);
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