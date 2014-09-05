#include "prelude.h"

#ifdef PG_GRAPHICS_OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgCompile(pgShader* shader, GLenum shaderType, pgUint8* shaderCode, pgUint8* end);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateVertexShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end, pgShaderInput* input, pgInt32 inputCount)
{
	PG_UNUSED(input);
	PG_UNUSED(inputCount);

	pgCompile(shader, GL_VERTEX_SHADER, shaderData, end);
}

pgVoid pgCreateFragmentShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end)
{
	pgCompile(shader, GL_FRAGMENT_SHADER, shaderData, end);
}

pgVoid pgDestroyShaderCore(pgShader* shader)
{
	glDeleteShader(shader->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgCreateProgramCore(pgProgram* program)
{
	GLchar buffer[4096];
	GLint success, logLength;

	program->id = glCreateProgram();
	PG_ASSERT_NO_GL_ERRORS();
	if (program->id == 0)
		PG_DIE("Failed to create OpenGL program object.");

	glAttachShader(program->id, program->vertexShader->id);
	glAttachShader(program->id, program->fragmentShader->id);
	PG_ASSERT_NO_GL_ERRORS();

	glLinkProgram(program->id);
	PG_ASSERT_NO_GL_ERRORS();

	glGetProgramiv(program->id, GL_LINK_STATUS, &success);
	glGetProgramInfoLog(program->id, sizeof(buffer) / sizeof(GLchar), &logLength, buffer);

	if (success == GL_FALSE)
		PG_DIE("Program linking failed: %s", buffer);

	if (logLength != 0)
		PG_WARN("%s", buffer);

	PG_ASSERT_NO_GL_ERRORS();

	glDetachShader(program->id, program->vertexShader->id);
	glDetachShader(program->id, program->fragmentShader->id);
	PG_ASSERT_NO_GL_ERRORS();

	program->device->program = NULL;
}

pgVoid pgDestroyProgramCore(pgProgram* program)
{
	glDeleteProgram(program->id);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgBindProgramCore(pgProgram* program)
{
	glUseProgram(program->id);
	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgCompile(pgShader* shader, GLenum shaderType, pgUint8* shaderCode, pgUint8* end)
{
	GLchar buffer[4096];
	GLint success, logLength;
	GLint shaderLength = (GLint)(end - shaderCode) - 1;
	const GLchar* code = (GLchar*)shaderCode;

	shader->id = glCreateShader(shaderType);
	PG_ASSERT_NO_GL_ERRORS();
	if (shader->id == 0)
		PG_DIE("Failed to create OpenGL shader object.");

	glShaderSource(shader->id, 1, &code, &shaderLength);
	PG_ASSERT_NO_GL_ERRORS();

	glCompileShader(shader->id);
	PG_ASSERT_NO_GL_ERRORS();

	glGetShaderiv(shader->id, GL_COMPILE_STATUS, &success);
	glGetShaderInfoLog(shader->id, sizeof(buffer) / sizeof(GLchar), &logLength, buffer);
	if (success == GL_FALSE)
		PG_DIE("Shader compilation failed: %s", buffer);

	if (logLength != 0)
		PG_WARN("%s", buffer);
}

#endif