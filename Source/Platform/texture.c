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
	PG_ASSERT(description->arraySize == 1, "Texture arrays are currently not supported.");
	PG_ASSERT(description->type != PG_TEXTURE_1D, "1D textures are currently not supported.");
	PG_ASSERT(description->type != PG_TEXTURE_3D, "3D textures are currently not supported.");

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

	if (texture->device->state.textures[slot] == texture)
		return;

	texture->device->state.textures[slot] = texture;
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

pgBool pgIsCompressedFormat(pgSurfaceFormat format)
{
	return format > 2150;
}

pgBool pgIsFloatingPointFormat(pgSurfaceFormat format)
{
	return format > 2120 && format < 2150;
}