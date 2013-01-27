#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid Compile(pgShader* shader, pgUint8* shaderCode)
{
	static GLchar buffer[4096];
	GLint success, logLength;
	const GLchar* code = (GLchar*)shaderCode;

	shader->id = glCreateShaderProgramv(shader->type, 1, &code);
	PG_CHECK_GL_HANDLE("Shader", shader->id);

	glGetProgramiv(shader->id, GL_LINK_STATUS, &success);
	glGetProgramInfoLog(shader->id, sizeof(buffer), &logLength, buffer);

	if (success == GL_FALSE)
		pgDie("Shader compilation failed: %s", buffer);

	if (logLength != 0)
		pgWarn("%s", buffer);

	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateShaderCore(pgShader* shader, pgShaderType type, pgVoid* shaderData)
{
	pgConvertShaderType(type, &shader->type, &shader->bit);
	Compile(shader, (pgUint8*)shaderData + sizeof(pgInt32));
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

#endif