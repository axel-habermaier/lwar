#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgShader* pgCreateVertexShader(pgGraphicsDevice* device, pgVoid* shaderData, pgInt32 length)
{
	pgShader* shader;
	pgUInt8* data = (pgUInt8*)shaderData;
	pgUInt8* end = data + length;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(shaderData);
	PG_ASSERT_IN_RANGE(length, 0, INT32_MAX);

	PG_ALLOC(pgShader, shader);
	shader->device = device;
	shader->type = PG_VERTEX_SHADER;
	pgCreateVertexShaderCore(shader, data, end);

	return shader;
}

pgShader* pgCreateFragmentShader(pgGraphicsDevice* device, pgVoid* shaderData, pgInt32 length)
{
	pgShader* shader;
	pgUInt8* data = (pgUInt8*)shaderData;
	pgUInt8* end = data + length;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(shaderData);
	PG_ASSERT_IN_RANGE(length, 0, INT32_MAX);

	PG_ALLOC(pgShader, shader);
	shader->device = device;
	shader->type = PG_FRAGMENT_SHADER;
	pgCreateFragmentShaderCore(shader, data, end);

	return shader;
}

pgVoid pgDestroyShader(pgShader* shader)
{
	if (shader == NULL)
		return;

	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		if (shader->device->vertexShader == shader)
			shader->device->vertexShader = NULL;
		break;
	case PG_FRAGMENT_SHADER:
		if (shader->device->fragmentShader == shader)
			shader->device->fragmentShader = NULL;
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	pgDestroyShaderCore(shader);
	PG_FREE(shader);
}

pgProgram* pgCreateProgram(pgGraphicsDevice* device, pgShader* vertexShader, pgShader* fragmentShader)
{
	pgProgram* program;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(vertexShader);
	PG_ASSERT_NOT_NULL(fragmentShader);
	PG_ASSERT(vertexShader->type == PG_VERTEX_SHADER, "Vertex shader has unexpected shader type.");
	PG_ASSERT(fragmentShader->type == PG_FRAGMENT_SHADER, "Fragment shader has unexpected shader type.");
	
	PG_ALLOC(pgProgram, program);
	program->device = device;
	program->vertexShader = vertexShader;
	program->fragmentShader = fragmentShader;

	pgCreateProgramCore(program);
	return program;
}

pgVoid pgDestroyProgram(pgProgram* program)
{
	if (program == NULL)
		return;

	if (program->device->program == program)
		program->device->program = NULL;

	pgDestroyProgramCore(program);
	PG_FREE(program);
}

pgVoid pgBindProgram(pgProgram* program)
{
	PG_ASSERT_NOT_NULL(program);

	if (program->device->program == program)
		return;

	pgBindProgramCore(program);

	program->device->program = program;
	program->device->vertexShader = program->vertexShader;
	program->device->fragmentShader = program->fragmentShader;
}