#include "prelude.h"

#ifdef PG_GRAPHICS_OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgResourceUsage usage, pgVoid* data)
{
	buffer->glType = pgConvertBufferType(buffer->type);

	PG_GL_ALLOC("Buffer", glGenBuffers, buffer->id);
	glBindBuffer(buffer->glType, buffer->id);
	PG_ASSERT_NO_GL_ERRORS();

	glBufferData(buffer->glType, buffer->size, data, pgConvertResourceUsage(usage));
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

	glBindBuffer(buffer->glType, buffer->id);
	mappedBuffer = glMapBufferRange(buffer->glType, (GLintptr)offset, (GLsizeiptr)byteCount, pgConvertMapMode(mode));
	PG_ASSERT_NO_GL_ERRORS();

	if (mappedBuffer == NULL)
		PG_DIE("Failed to map buffer.");
	
	return mappedBuffer;
}

pgVoid pgUnmapBufferCore(pgBuffer* buffer)
{
	GLboolean success;

	glBindBuffer(buffer->glType, buffer->id);
	success = glUnmapBuffer(buffer->glType);
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
	glBindBuffer(buffer->glType, buffer->id);
	glBufferSubData(buffer->glType, 0, buffer->size, data);
	PG_ASSERT_NO_GL_ERRORS();
}

#endif