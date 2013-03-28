#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgShader** pgGetBoundShader(pgShader* shader);
static pgInt32 pgReadShaderInputs(pgUint8** shaderData, pgUint8* end, pgShaderInput* inputs);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgShader* pgCreateShader(pgGraphicsDevice* device, pgShaderType type, pgVoid* shaderData, pgInt32 length)
{
	pgShader* shader;
	pgUint8* data = (pgUint8*)shaderData;
	pgUint8* end = data + length;
	pgShaderInput inputs[PG_INPUT_BINDINGS_COUNT];
	pgInt32 inputCount = 0;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(shaderData);
	PG_ASSERT_IN_RANGE(length, 0, INT32_MAX);

	if (type == PG_VERTEX_SHADER)
		inputCount = pgReadShaderInputs(&data, end, inputs);

	PG_ALLOC(pgShader, shader);
	shader->device = device;
	shader->type = type;
	pgCreateShaderCore(shader, data, end, inputs, inputCount);

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

static pgInt32 pgReadShaderInputs(pgUint8** shaderData, pgUint8* end, pgShaderInput* inputs)
{
	pgInt32 i, count;

	count = **shaderData;
	++(*shaderData);

	for (i = 0; i < count && *shaderData < end; ++i)
	{
		inputs[i].semantics = (pgDataSemantics)**shaderData;
		++(*shaderData);
		inputs[i].format = (pgVertexDataFormat)**shaderData;
		++(*shaderData);
	}

	if (*shaderData >= end)
		pgDie("Incomplete shader input specification.");

	return count;
}
