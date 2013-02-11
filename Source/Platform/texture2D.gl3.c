#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTexture2DCore(pgTexture2D* texture2D, pgVoid* data, pgSurfaceFormat surfaceFormat)
{
	GLenum internalFormat, format;
	GLint boundTexture;

	glGenTextures(1, &texture2D->id);
	PG_CHECK_GL_HANDLE("Texture 2D", texture2D->id);

	pgConvertSurfaceFormat(surfaceFormat, &internalFormat, &format);
	glGetIntegerv(GL_TEXTURE_BINDING_2D, &boundTexture);
	glBindTexture(GL_TEXTURE_2D, texture2D->id);
	glTexImage2D(GL_TEXTURE_2D, 0, internalFormat, texture2D->width, texture2D->height, 0, format, GL_UNSIGNED_BYTE, data);
	glBindTexture(GL_TEXTURE_2D, boundTexture);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyTexture2DCore(pgTexture2D* texture2D)
{
	glDeleteTextures(1, &texture2D->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindTextureCore(pgTexture2D* texture2D, pgInt32 slot)
{
	glActiveTexture(GL_TEXTURE0 + slot);
	glBindTexture(GL_TEXTURE_2D, texture2D->id);
	PG_ASSERT_NO_GL_ERRORS();
}

#endif