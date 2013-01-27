#include "prelude.h"

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
	pgCreateShaderCore(shader, type, shaderData);

	return shader;
}

pgVoid pgDestroyShader(pgShader* shader)
{
	PG_ASSERT_NOT_NULL(shader);

	pgDestroyShaderCore(shader);
	PG_FREE(shader);
}

pgVoid pgBindShader(pgShader* shader)
{
	PG_ASSERT_NOT_NULL(shader);
	pgBindShaderCore(shader);
}