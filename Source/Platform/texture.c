#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgTexture* pgCreateTexture(pgGraphicsDevice* device, pgTextureDescription* description, pgSurface* surfaces)
{
	pgTexture* texture;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);
	PG_ASSERT(description->arraySize == 1, "Texture arrays are currently not supported.");
	PG_ASSERT(description->type != PG_TEXTURE_1D, "1D textures are currently not supported.");
	PG_ASSERT(description->type != PG_TEXTURE_3D, "3D textures are currently not supported.");
	PG_ASSERT_IN_RANGE(description->mipmaps, 1, PG_MAX_MIPMAPS);
	PG_ASSERT((description->flags & PG_TEXTURE_GENERATE_MIPMAPS) == 0 || description->mipmaps == 1, 
		"Cannot set mipmaps for a texture that has the generate mipmaps flag set.");
	PG_ASSERT((description->flags & PG_TEXTURE_BIND_DEPTH_STENCIL) == 0 || !pgIsCompressedFormat(description->format),
		"The bind depth stencil flag is invalid for compressed textures.");
	PG_ASSERT((description->flags & PG_TEXTURE_BIND_RENDER_TARGET) == 0 || !pgIsCompressedFormat(description->format),
		"The bind render target flag is invalid for compressed textures.");

	PG_ALLOC(pgTexture, texture);
	texture->device = device;
	texture->desc = *description;
	pgCreateTextureCore(texture, surfaces);
	return texture;
}

pgVoid pgDestroyTexture(pgTexture* texture)
{
	if (texture == NULL)
		return;

	pgDestroyTextureCore(texture);
	PG_FREE(texture);
}

pgVoid pgBindTexture(pgTexture* texture, pgInt32 slot)
{
	PG_ASSERT_NOT_NULL(texture);
	PG_ASSERT_IN_RANGE(slot, 0, PG_TEXTURE_SLOT_COUNT);

	if (texture->device->textures[slot] == texture)
		return;

	texture->device->textures[slot] = texture;
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

pgBool pgIsDepthStencilFormat(pgSurfaceFormat format)
{
	return format == PG_SURFACE_DEPTH24_STENCIL8;
}