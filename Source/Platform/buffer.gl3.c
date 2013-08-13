#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgResourceUsage usage, pgVoid* data)
{
	PG_GL_ALLOC("Buffer", glGenBuffers, buffer->id);
	buffer->glType = pgConvertBufferType(buffer->type);

	glNamedBufferDataEXT(buffer->id, buffer->size, data, pgConvertResourceUsage(usage));
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyBufferCore(pgBuffer* buffer)
{
	PG_GL_FREE(glDeleteBuffers, buffer->id);
}

pgVoid* pgMapBufferCore(pgBuffer* buffer, pgMapMode mode)
{
	return pgMapBufferRange(buffer, mode, 0, buffer->size);
}

pgVoid* pgMapBufferRangeCore(pgBuffer* buffer, pgMapMode mode, pgInt32 offset, pgInt32 byteCount)
{
	pgVoid* mappedBuffer;

	mappedBuffer = glMapNamedBufferRangeEXT(buffer->id, (GLintptr)offset, (GLsizeiptr)byteCount, pgConvertMapMode(mode));
	PG_ASSERT_NO_GL_ERRORS();

	if (mappedBuffer == NULL)
		PG_DIE("Failed to map buffer.");
	
	return mappedBuffer;
}

pgVoid pgUnmapBufferCore(pgBuffer* buffer)
{
	GLboolean success;

	success = glUnmapNamedBufferEXT(buffer->id);
	PG_ASSERT_NO_GL_ERRORS();

	if (!success)
		PG_DIE("Failed to unmap buffer.");
}

pgVoid pgBindConstantBufferCore(pgBuffer* buffer, pgInt32 slot)
{
	glBindBufferBase(GL_UNIFORM_BUFFER, slot, buffer->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgUpdateConstantBufferCore(pgBuffer* buffer, pgVoid* data)
{
	glNamedBufferSubDataEXT(buffer->id, 0, buffer->size, data);
	PG_ASSERT_NO_GL_ERRORS();
}

#endif