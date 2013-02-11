#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static GLint GetBoundBuffer(GLenum bufferType);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgBufferType type, pgResourceUsage usage, pgVoid* data, pgInt32 size)
{
	GLint boundBuffer;

	glGenBuffers(1, &buffer->id);
	PG_CHECK_GL_HANDLE("Buffer", buffer->id);

	buffer->type = pgConvertBufferType(type);
	buffer->size = size;

	boundBuffer = GetBoundBuffer(buffer->type);
	glBindBuffer(buffer->type, buffer->id);
	glBufferData(buffer->type, size, data, pgConvertResourceUsage(usage));
	glBindBuffer(buffer->type, boundBuffer);
	
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyBufferCore(pgBuffer* buffer)
{
	glDeleteBuffers(1, &buffer->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid* pgMapBufferCore(pgBuffer* buffer, pgMapMode mode)
{
	pgVoid* mappedBuffer;
	GLint boundBuffer;

	boundBuffer = GetBoundBuffer(buffer->type);
	glBindBuffer(buffer->type, buffer->id);
	mappedBuffer = glMapBufferRange(buffer->type, 0, buffer->size, pgConvertMapMode(mode));
	glBindBuffer(buffer->type, boundBuffer);
	PG_ASSERT_NO_GL_ERRORS();

	if (mappedBuffer == NULL)
		pgDie("Failed to map buffer.");
	
	return mappedBuffer;
}

pgVoid pgUnmapBufferCore(pgBuffer* buffer)
{
	GLboolean success;
	GLint boundBuffer;

	boundBuffer = GetBoundBuffer(buffer->type);
	glBindBuffer(buffer->type, buffer->id);
	success = glUnmapBuffer(buffer->type);
	glBindBuffer(buffer->type, boundBuffer);
	PG_ASSERT_NO_GL_ERRORS();

	if (!success)
		pgDie("Failed to unmap buffer.");
}

pgVoid pgBindConstantBufferCore(pgBuffer* buffer, pgInt32 slot)
{
	PG_ASSERT(buffer->type == GL_UNIFORM_BUFFER, "Buffer is not a constant buffer");

	glBindBufferBase(GL_UNIFORM_BUFFER, slot, buffer->id);
	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static GLint GetBoundBuffer(GLenum bufferType)
{
	GLint buffer;

	switch (bufferType)
	{
	case GL_ELEMENT_ARRAY_BUFFER:
		glGetIntegerv(GL_ELEMENT_ARRAY_BUFFER_BINDING, &buffer);
		break;
	case GL_ARRAY_BUFFER:
		glGetIntegerv(GL_ARRAY_BUFFER_BINDING, &buffer);
		break;
	case GL_UNIFORM_BUFFER:
		glGetIntegerv(GL_UNIFORM_BUFFER_BINDING, &buffer);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	return buffer;
}

#endif