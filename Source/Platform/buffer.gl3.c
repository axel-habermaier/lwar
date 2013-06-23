#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgBufferType type, pgResourceUsage usage, pgVoid* data, pgInt32 size)
{
	PG_GL_ALLOC("Buffer", glGenBuffers, buffer->id);
	buffer->type = pgConvertBufferType(type);
	buffer->size = size;

	glNamedBufferDataEXT(buffer->id, size, data, pgConvertResourceUsage(usage));
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyBufferCore(pgBuffer* buffer)
{
	PG_GL_FREE(glDeleteBuffers, buffer->id);
}

pgVoid* pgMapBufferCore(pgBuffer* buffer, pgMapMode mode)
{
	pgVoid* mappedBuffer;

	mappedBuffer = glMapNamedBufferRangeEXT(buffer->id, 0, buffer->size, pgConvertMapMode(mode));
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
	PG_ASSERT(buffer->type == GL_UNIFORM_BUFFER, "Buffer is not a constant buffer");

	glBindBufferBase(GL_UNIFORM_BUFFER, slot, buffer->id);
	PG_ASSERT_NO_GL_ERRORS();
}

#endif