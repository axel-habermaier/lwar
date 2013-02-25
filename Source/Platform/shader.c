#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgShader** pgGetBoundShader(pgShader* shader);

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgShader* pgCreateShader(pgGraphicsDevice* device, pgShaderType type, pgVoid* shaderData)
{
	pgShader* shader;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(shaderData);

	PG_ALLOC(pgShader, shader);
	shader->device = device;
	shader->type = type;
	pgCreateShaderCore(shader, shaderData);

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
		return  &shader->device->state.vertexShader;
	case PG_FRAGMENT_SHADER:
		return &shader->device->state.fragmentShader;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}
