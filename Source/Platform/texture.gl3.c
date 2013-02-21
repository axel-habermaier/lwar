#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid UploadTexture(pgTexture* texture, pgSurface* surface, GLenum target, GLint level);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgSurface* surfaces)
{
	GLint boundTexture;
	pgUint32 i;
	
	glGenTextures(1, &texture->id);
	PG_CHECK_GL_HANDLE("Texture", texture->id);

	pgConvertTextureType(texture->desc.type, &texture->glType, &texture->glBoundType);
	
	glGetIntegerv(texture->glBoundType, &boundTexture);
	glBindTexture(texture->glType, texture->id);

	switch (texture->desc.type)
	{
	case PG_TEXTURE_2D:
		for (i = 0; i < texture->desc.surfaceCount; ++i)
			UploadTexture(texture, &surfaces[i], GL_TEXTURE_2D, i);
		break;
	case PG_TEXTURE_CUBE_MAP:
	{
		pgInt32 j;
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
				UploadTexture(texture, &surfaces[index], faces[i], j);
			}
		}
		break;
	}
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	glBindTexture(texture->glType, boundTexture);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyTextureCore(pgTexture* texture)
{
	glDeleteTextures(1, &texture->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindTextureCore(pgTexture* texture, pgInt32 slot)
{
	glActiveTexture(GL_TEXTURE0 + slot);
	glBindTexture(texture->glType, texture->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgGenerateMipmapsCore(pgTexture* texture)
{
	GLint boundTexture;
	glGetIntegerv(texture->glBoundType, &boundTexture);

	pgBindTexture(texture, 0);
	glGenerateMipmap(texture->glType);
	PG_ASSERT_NO_GL_ERRORS();

	glBindTexture(texture->glType, boundTexture);
	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid UploadTexture(pgTexture* texture, pgSurface* surface, GLenum target, GLint level)
{
	GLenum internalFormat, format;
	pgBool isCompressed = texture->desc.format > 2104;
	pgConvertSurfaceFormat(texture->desc.format, &internalFormat, &format);

	if (isCompressed)
		glCompressedTexImage2D(target, level, internalFormat, surface->width, surface->height, 0, surface->size, surface->data);
	else
		glTexImage2D(target, level, internalFormat, surface->width, surface->height, 0, format, GL_UNSIGNED_BYTE, surface->data);
}

#endif