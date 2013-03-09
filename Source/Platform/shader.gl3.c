#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Compile(pgShader* shader, pgUint8* shaderCode);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end, pgShaderInput* input, pgInt32 inputCount)
{
	PG_UNUSED(end);
	PG_UNUSED(input);
	PG_UNUSED(inputCount);

	pgConvertShaderType(shader->type, &shader->glType, &shader->bit);
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
		pgDie("Failed to create OpenGL shader object.");

	glGetProgramiv(shader->id, GL_LINK_STATUS, &success);
	glGetProgramInfoLog(shader->id, sizeof(buffer), &logLength, buffer);

	if (success == GL_FALSE)
		pgDie("Shader compilation failed: %s", buffer);

	if (logLength != 0)
		pgWarn("%s", buffer);

	PG_ASSERT_NO_GL_ERRORS();
}

#endif