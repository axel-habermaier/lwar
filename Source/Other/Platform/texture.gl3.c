#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// State
//====================================================================================================================

static pgTexture* boundTexture = NULL;

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgUploadTexture(pgTexture* texture, pgSurface* surface, GLenum target, GLint level);
static pgVoid pgAllocTextureData(pgTexture* texture);
static pgVoid pgRebindTexture();

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgSurface* surfaces)
{
	pgUint32 i;

	pgConvertTextureType(texture->desc.type, &texture->glType, &texture->glBoundType);
	
	PG_GL_ALLOC("Texture", glGenTextures, texture->id);
	glBindTexture(texture->glType, texture->id);
	PG_ASSERT_NO_GL_ERRORS();
	
	if (surfaces != NULL)
	{
		switch (texture->desc.type)
		{
		case PG_TEXTURE_2D:
			for (i = 0; i < texture->desc.surfaceCount; ++i)
				pgUploadTexture(texture, &surfaces[i], GL_TEXTURE_2D, i);
			break;
		case PG_TEXTURE_CUBE_MAP:
		{
			pgUint32 j;
			GLenum faces[] = 
			{ 
				GL_TEXTURE_CUBE_MAP_NEGATIVE_Z, 
				GL_TEXTURE_CUBE_MAP_NEGATIVE_X, 
				GL_TEXTURE_CUBE_MAP_POSITIVE_Z, 
				GL_TEXTURE_CUBE_MAP_POSITIVE_X, 
				GL_TEXTURE_CUBE_MAP_NEGATIVE_Y,
				GL_TEXTURE_CUBE_MAP_POSITIVE_Y
			};

			for (i = 0; i < 6; ++i)
			{
				for (j = 0; j < texture->desc.mipmaps; ++j)
				{
					int index = i * texture->desc.mipmaps + j;
					pgUploadTexture(texture, &surfaces[index], faces[i], j);
				}
			}
			break;
		}
		default:
			PG_NO_SWITCH_DEFAULT;
		}
	}
	else
		pgAllocTextureData(texture);

	if ((texture->desc.flags & PG_TEXTURE_GENERATE_MIPMAPS) != 0)
		glGenerateMipmap(texture->glType);

	PG_ASSERT_NO_GL_ERRORS();

	pgRebindTexture();
}

pgVoid pgDestroyTextureCore(pgTexture* texture)
{
	PG_GL_FREE(glDeleteTextures, texture->id);

	if (boundTexture == texture)
		boundTexture = NULL;
}

pgVoid pgBindTextureCore(pgTexture* texture, pgInt32 slot)
{
	pgChangeActiveTexture(texture->device, slot);
	glBindTexture(texture->glType, texture->id);
	PG_ASSERT_NO_GL_ERRORS();

	boundTexture = texture;
}

pgVoid pgUnbindTextureCore(pgTexture* texture, pgInt32 slot)
{
	pgChangeActiveTexture(texture->device, slot);
	glBindTexture(texture->glType, 0);
	PG_ASSERT_NO_GL_ERRORS();

	boundTexture = NULL;
}

pgVoid pgGenerateMipmapsCore(pgTexture* texture)
{
	glBindTexture(texture->glType, texture->id);
	glGenerateMipmap(texture->glType);
	PG_ASSERT_NO_GL_ERRORS();

	pgRebindTexture();
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgUploadTexture(pgTexture* texture, pgSurface* surface, GLenum target, GLint level)
{
	GLenum internalFormat, format, type;

	pgConvertSurfaceFormat(texture->desc.format, &internalFormat, &format);
	type = pgIsFloatingPointFormat(texture->desc.format) ? GL_FLOAT : GL_UNSIGNED_BYTE;

	if (pgIsDepthStencilFormat(texture->desc.format))
		type = GL_UNSIGNED_INT_24_8;

	if (pgIsCompressedFormat(texture->desc.format))
		glCompressedTexImage2D(target, level, internalFormat, surface->width, surface->height, 0, surface->size, surface->data);
	else
		glTexImage2D(target, level, internalFormat, surface->width, surface->height, 0, format, type, surface->data);

	PG_ASSERT_NO_GL_ERRORS();
}

static pgVoid pgAllocTextureData(pgTexture* texture)
{
	pgSurface surface;
	memset(&surface, 0, sizeof(surface));

	surface.width = texture->desc.width;
	surface.height = texture->desc.height;

	switch (texture->desc.type)
	{
	case PG_TEXTURE_2D:
		pgUploadTexture(texture, &surface, GL_TEXTURE_2D, 0);
		break;
	case PG_TEXTURE_CUBE_MAP:
		pgUploadTexture(texture, &surface, GL_TEXTURE_CUBE_MAP_NEGATIVE_Z, 0);
		pgUploadTexture(texture, &surface, GL_TEXTURE_CUBE_MAP_NEGATIVE_X, 0);
		pgUploadTexture(texture, &surface, GL_TEXTURE_CUBE_MAP_POSITIVE_Z, 0);
		pgUploadTexture(texture, &surface, GL_TEXTURE_CUBE_MAP_POSITIVE_X, 0);
		pgUploadTexture(texture, &surface, GL_TEXTURE_CUBE_MAP_NEGATIVE_Y, 0);
		pgUploadTexture(texture, &surface, GL_TEXTURE_CUBE_MAP_POSITIVE_Y, 0);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

static pgVoid pgRebindTexture()
{
	if (boundTexture != NULL)
		glBindTexture(boundTexture->glType, boundTexture->id);

	PG_ASSERT_NO_GL_ERRORS();
}

#endif