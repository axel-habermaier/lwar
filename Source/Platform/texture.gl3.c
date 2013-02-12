#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid CopyCubeMapFace();

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgVoid* data, pgSurfaceFormat surfaceFormat)
{
	GLenum internalFormat, format;
	GLint boundTexture;
	GLint length, i;
	GLint width = texture->width / 6;
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

	pgConvertTextureType(texture->type, &texture->glType, &texture->glBoundType);
	pgConvertSurfaceFormat(surfaceFormat, &internalFormat, &format, &length);

	glGetIntegerv(texture->glBoundType, &boundTexture);
	glBindTexture(texture->glType, texture->id);

	switch (texture->type)
	{
	case PG_TEXTURE_2D:
		glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
		glTexImage2D(GL_TEXTURE_2D, 0, internalFormat, texture->width, texture->height, 0, format, GL_UNSIGNED_BYTE, data);
		break;
	case PG_TEXTURE_CUBE_MAP:
		glPixelStorei(GL_UNPACK_ROW_LENGTH, texture->width);

		for (i = 0; i < 6; ++i)
		{
			pgVoid* faceData = (pgInt8*)data + i * (width * length);
			glTexImage2D(faces[i], 0, internalFormat, width, texture->height, 0, format, GL_UNSIGNED_BYTE, faceData);
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

#endif