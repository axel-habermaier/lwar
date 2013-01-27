#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions and state
//====================================================================================================================

static struct BufferState
{
	GLuint vertexBuffer;
	GLuint indexBuffer;
	GLuint constantBuffer;
} state;

static pgVoid Rebind(GLenum bufferType);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgBufferType type, pgResourceUsage usage, pgVoid* data, pgInt32 size)
{
	glGenBuffers(1, &buffer->id);
	PG_CHECK_GL_HANDLE("Buffer", buffer->id);

	buffer->type = pgConvertBufferType(type);
	buffer->size = size;

	glBindBuffer(buffer->type, buffer->id);
	glBufferData(buffer->type, size, data, pgConvertResourceUsage(usage));
	Rebind(buffer->type);
	
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

	glBindBuffer(buffer->type, buffer->id);
	mappedBuffer = glMapBufferRange(buffer->type, 0, buffer->size, pgConvertMapMode(mode));
	Rebind(buffer->type);
	PG_ASSERT_NO_GL_ERRORS();

	if (mappedBuffer == NULL)
		pgDie("Failed to map buffer.");
	
	return mappedBuffer;
}

pgVoid pgUnmapBufferCore(pgBuffer* buffer)
{
	GLboolean success;

	glBindBuffer(buffer->type, buffer->id);
	success = glUnmapBuffer(buffer->type);
	Rebind(buffer->type);
	PG_ASSERT_NO_GL_ERRORS();

	if (!success)
		pgDie("Failed to unmap buffer.");
}

pgVoid pgBindConstantBufferCore(pgBuffer* buffer, pgInt32 slot)
{
	PG_ASSERT(buffer->type == GL_UNIFORM_BUFFER, "Buffer is not a constant buffer");

	glBindBufferBase(GL_UNIFORM_BUFFER, slot, buffer->id);
	PG_ASSERT_NO_GL_ERRORS();

	switch (buffer->type)
	{
	case GL_ELEMENT_ARRAY_BUFFER:
		state.indexBuffer = buffer->id;
		break;
	case GL_ARRAY_BUFFER:
		state.vertexBuffer = buffer->id;
		break;
	case GL_UNIFORM_BUFFER:
		state.constantBuffer = buffer->id;
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Rebind(GLenum bufferType)
{
	switch (bufferType)
	{
	case GL_ELEMENT_ARRAY_BUFFER:
		glBindBuffer(bufferType, state.indexBuffer);
		break;
	case GL_ARRAY_BUFFER:
		glBindBuffer(bufferType, state.vertexBuffer);
		break;
	case GL_UNIFORM_BUFFER:
		glBindBuffer(bufferType, state.constantBuffer);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

#endif