#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateInputLayoutCore(pgInputLayout* inputLayout, pgBuffer* indexBuffer, pgInt32 indexOffset, 
							   pgIndexSize indexSize, pgInputBinding* inputBindings, pgInt32 bindingsCount)
{
	GLint i;
	PG_ASSERT(indexBuffer == NULL || (indexBuffer != NULL && indexBuffer->type == GL_ELEMENT_ARRAY_BUFFER), "Invalid index buffer.");

	glGenVertexArrays(1, &inputLayout->id);
	PG_CHECK_GL_HANDLE("Vertex Input Layout", inputLayout->id);

	glBindVertexArray(inputLayout->id);
	PG_ASSERT_NO_GL_ERRORS();

	if (indexBuffer != NULL)
	{
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer->id);
		PG_ASSERT_NO_GL_ERRORS();

		inputLayout->indexOffset = indexOffset;
		pgConvertIndexSize(indexSize, &inputLayout->indexType, &inputLayout->indexSizeInBytes);
	}

	for (i = 0; i < bindingsCount; ++i)
	{
		GLenum type;
		GLsizei size;
		GLboolean normalize;
		GLuint slot = pgConvertVertexDataSemantics(inputBindings[i].semantics);

		PG_ASSERT_NOT_NULL(inputBindings[i].vertexBuffer);
		PG_ASSERT(inputBindings[i].vertexBuffer->type == GL_ARRAY_BUFFER, "Invalid vertex buffer.");

		glBindBuffer(GL_ARRAY_BUFFER, inputBindings[i].vertexBuffer->id);
		glEnableVertexAttribArray(slot);
		PG_ASSERT_NO_GL_ERRORS();

		pgConvertVertexDataFormat(inputBindings[i].format, &type, &size);

		// Color values must be normalized
		normalize = inputBindings[i].semantics == PG_VERTEX_SEMANTICS_COLOR;
		glVertexAttribPointer(slot, size, type, normalize, inputBindings[i].stride, (void*)(size_t)inputBindings[i].offset);
		PG_ASSERT_NO_GL_ERRORS();
	}

	glBindVertexArray(0);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyInputLayoutCore(pgInputLayout* inputLayout)
{
	glDeleteVertexArrays(1, &inputLayout->id);
	PG_ASSERT_NO_GL_ERRORS();

	if (inputLayout->device->inputLayout == inputLayout)
		inputLayout->device->inputLayout = NULL;
}

pgVoid pgBindInputLayoutCore(pgInputLayout* inputLayout)
{
	glBindVertexArray(inputLayout->id);
	PG_ASSERT_NO_GL_ERRORS();

	inputLayout->device->inputLayout = inputLayout;
}

#endif
