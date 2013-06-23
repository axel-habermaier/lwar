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

	PG_GL_ALLOC("Vertex Input Layout", glGenVertexArrays, inputLayout->id);
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
		GLuint slot = (GLuint)inputBindings[i].semantics;

		PG_ASSERT_NOT_NULL(inputBindings[i].vertexBuffer);
		PG_ASSERT(inputBindings[i].vertexBuffer->type == GL_ARRAY_BUFFER, "Invalid vertex buffer.");

		glBindBuffer(GL_ARRAY_BUFFER, inputBindings[i].vertexBuffer->id);
		glEnableVertexAttribArray(slot);
		PG_ASSERT_NO_GL_ERRORS();

		pgConvertVertexDataFormat(inputBindings[i].format, &type, &size);

		// Color values must be normalized
		normalize = inputBindings[i].semantics == PG_VERTEX_SEMANTICS_COLOR0 ||
			inputBindings[i].semantics == PG_VERTEX_SEMANTICS_COLOR1 ||
			inputBindings[i].semantics == PG_VERTEX_SEMANTICS_COLOR2 ||
			inputBindings[i].semantics == PG_VERTEX_SEMANTICS_COLOR3;
		glVertexAttribPointer(slot, size, type, normalize, inputBindings[i].stride, (void*)(size_t)inputBindings[i].offset);
		PG_ASSERT_NO_GL_ERRORS();
	}

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroyInputLayoutCore(pgInputLayout* inputLayout)
{
	PG_GL_FREE(glDeleteVertexArrays, inputLayout->id);

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
