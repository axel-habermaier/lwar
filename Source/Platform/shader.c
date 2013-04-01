#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgShader** pgGetBoundShader(pgShader* shader);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgShader* pgCreateVertexShader(pgGraphicsDevice* device, pgVoid* shaderData, pgInt32 length, pgShaderInput* inputs, pgInt32 inputCount)
{
	pgShader* shader;
	pgUint8* data = (pgUint8*)shaderData;
	pgUint8* end = data + length;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(shaderData);
	PG_ASSERT_IN_RANGE(length, 0, INT32_MAX);
	PG_ASSERT_NOT_NULL(inputs);
	PG_ASSERT_IN_RANGE(inputCount, 0, INT32_MAX);

	PG_ALLOC(pgShader, shader);
	shader->device = device;
	shader->type = PG_VERTEX_SHADER;
	pgCreateVertexShaderCore(shader, data, end, inputs, inputCount);

	return shader;
}

pgShader* pgCreateFragmentShader(pgGraphicsDevice* device, pgVoid* shaderData, pgInt32 length)
{
	pgShader* shader;
	pgUint8* data = (pgUint8*)shaderData;
	pgUint8* end = data + length;

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
	pgShader** boundShader = pgGetBoundShader(shader);

	if (shader == NULL)
		return;

	PG_ASSERT_NOT_NULL(boundShader);
	if (*boundShader == shader)
		*boundShader = NULL;

	pgDestroyShaderCore(shader);
	PG_FREE(shader);
}

pgVoid pgBindShader(pgShader* shader)
{
	pgShader** boundShader = pgGetBoundShader(shader);
	PG_ASSERT_NOT_NULL(shader);

	if (*boundShader == shader)
		return;

	*boundShader = shader;
	pgBindShaderCore(shader);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgShader** pgGetBoundShader(pgShader* shader)
{
	if (shader == NULL)
		return NULL;

	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		return  &shader->device->vertexShader;
	case PG_FRAGMENT_SHADER:
		return &shader->device->fragmentShader;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}