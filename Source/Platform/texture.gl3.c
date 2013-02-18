#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgVoid* data)
{
	GLenum internalFormat, format;
	GLint boundTexture;
	pgInt32 componentCount, i, numMipmaps;
	pgMipmap mipmaps[PG_MAX_MIPMAPS];
	GLenum faces[] = 
	{ 
		GL_TEXTURE_CUBE_MAP_NEGATIVE_Z, 
		GL_TEXTURE_CUBE_MAP_NEGATIVE_X, 
		GL_TEXTURE_CUBE_MAP_POSITIVE_Z, 
		GL_TEXTURE_CUBE_MAP_POSITIVE_X, 
		GL_TEXTURE_CUBE_MAP_NEGATIVE_Y,
		GL_TEXTURE_CUBE_MAP_POSITIVE_Y
	};

	glGenTextures(1, &texture->id);
	PG_CHECK_GL_HANDLE("Texture", texture->id);

	pgConvertTextureType(texture->desc.type, &texture->glType, &texture->glBoundType);
	pgConvertSurfaceFormat(texture->desc.format, &internalFormat, &format, &componentCount);

	glGetIntegerv(texture->glBoundType, &boundTexture);
	glBindTexture(texture->glType, texture->id);

	switch (texture->desc.type)
	{
	case PG_TEXTURE_2D:
		numMipmaps = pgMipmaps(data, texture->desc.width, texture->desc.height, componentCount, mipmaps);
		for (i = 0; i < numMipmaps; ++i)
		{
			glPixelStorei(GL_UNPACK_ROW_LENGTH, mipmaps[i].width);
			glTexImage2D(GL_TEXTURE_2D, i, internalFormat, mipmaps[i].width, mipmaps[i].height, 0, format, GL_UNSIGNED_BYTE, mipmaps[i].data);
		}
		break;
	case PG_TEXTURE_CUBE_MAP:
		for (i = 0; i < 6; ++i)
		{
			pgInt32 j;
			pgMipmap* last;
			numMipmaps = pgMipmaps(data, texture->desc.width, texture->desc.height, componentCount, mipmaps);
			last = &mipmaps[numMipmaps - 1];
			data = (pgUint8*)last->data + last->width * last->height * componentCount;

			for (j = 0; j < numMipmaps; ++j)
			{
				glPixelStorei(GL_UNPACK_ROW_LENGTH, mipmaps[j].width);
				glTexImage2D(faces[i], j, internalFormat, mipmaps[j].width, mipmaps[j].height, 0, format, GL_UNSIGNED_BYTE, mipmaps[j].data);
			}
		}
		break;
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

#endif