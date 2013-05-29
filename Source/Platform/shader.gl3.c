#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Compile(pgShader* shader, pgUint8* shaderCode);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateVertexShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end, pgShaderInput* input, pgInt32 inputCount)
{
	PG_UNUSED(end);
	PG_UNUSED(input);
	PG_UNUSED(inputCount);

	shader->glType = GL_VERTEX_SHADER;
	shader->bit = GL_VERTEX_SHADER_BIT;
	Compile(shader, shaderData);
}

pgVoid pgCreateFragmentShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end)
{
	PG_UNUSED(end);

	shader->glType = GL_FRAGMENT_SHADER;
	shader->bit = GL_FRAGMENT_SHADER_BIT;
	Compile(shader, shaderData);
}

pgVoid pgDestroyShaderCore(pgShader* shader)
{
	glDeleteProgram(shader->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindShaderCore(pgShader* shader)
{
	glUseProgramStages(shader->device->pipeline, shader->bit, shader->id);
	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Compile(pgShader* shader, pgUint8* shaderCode)
{
	static GLchar buffer[4096];
	GLint success, logLength;
	const GLchar* code = (GLchar*)shaderCode;

	shader->id = glCreateShaderProgramv(shader->glType, 1, &code);
	PG_ASSERT_NO_GL_ERRORS();
	if (shader->id == 0)
		PG_DIE("Failed to create OpenGL shader object.");

	glGetProgramiv(shader->id, GL_LINK_STATUS, &success);
	glGetProgramInfoLog(shader->id, sizeof(buffer), &logLength, buffer);

	if (success == GL_FALSE)
		PG_DIE("Shader compilation failed: %s", buffer);

	if (logLength != 0)
		PG_WARN("%s", buffer);

	PG_ASSERT_NO_GL_ERRORS();
}

#endif