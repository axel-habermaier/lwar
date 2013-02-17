#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgVoid* data)
{
	GLenum internalFormat, format;
	GLint boundTexture;
	GLint length, i;
	GLint width, height;
	pgUint8* dataPtr = (pgUint8*)data;
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
	pgConvertSurfaceFormat(texture->desc.format, &internalFormat, &format, &length);

	glGetIntegerv(texture->glBoundType, &boundTexture);
	glBindTexture(texture->glType, texture->id);

	switch (texture->desc.type)
	{
	case PG_TEXTURE_2D:
		width = texture->desc.width;
		height = texture->desc.height;

		for (i = 0; i < pgMipmapCount(texture->desc.width, texture->desc.height) + 1; ++i)
		{
			glPixelStorei(GL_UNPACK_ROW_LENGTH, width);
			glTexImage2D(GL_TEXTURE_2D, i, internalFormat, width, height, 0, format, GL_UNSIGNED_BYTE, dataPtr);

			dataPtr += width * height * length;
			width /= 2;
			height /= 2;

			width = width < 1 ? 1 : width;
			height = height < 1 ? 1 : height;
		}
		break;
	case PG_TEXTURE_CUBE_MAP:
		glPixelStorei(GL_UNPACK_ROW_LENGTH, texture->desc.width);
		width = texture->desc.width / 6;
		for (i = 0; i < 6; ++i)
		{
			pgVoid* faceData = (pgInt8*)data + i * (width * length);
			glTexImage2D(faces[i], 0, internalFormat, width, texture->desc.height, 0, format, GL_UNSIGNED_BYTE, faceData);
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