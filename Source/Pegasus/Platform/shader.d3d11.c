#include "prelude.h"

#ifdef PG_GRAPHICS_DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateVertexShaderCore(pgShader* shader, pgUInt8* shaderData, pgUInt8* end)
{
	size_t byteCodeLength = end - shaderData;

	PG_D3DCALL(ID3D11Device_CreateVertexShader(PG_DEVICE(shader), shaderData, byteCodeLength, NULL, &shader->ptr.vertexShader),
		"Failed to create vertex shader.");
}

pgVoid pgCreateFragmentShaderCore(pgShader* shader, pgUInt8* shaderData, pgUInt8* end)
{
	size_t byteCodeLength = end - shaderData;

	PG_D3DCALL(ID3D11Device_CreatePixelShader(shader->device->ptr, shaderData, byteCodeLength, NULL, &shader->ptr.pixelShader), 
		"Failed to create pixel shader.");
}

pgVoid pgDestroyShaderCore(pgShader* shader)
{
	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		PG_SAFE_RELEASE(ID3D11VertexShader, shader->ptr.vertexShader);
		break;
	case PG_FRAGMENT_SHADER:
		PG_SAFE_RELEASE(ID3D11PixelShader, shader->ptr.pixelShader);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

pgVoid pgCreateProgramCore(pgProgram* program)
{
	PG_UNUSED(program);
	// Nothing to do here
}

pgVoid pgDestroyProgramCore(pgProgram* program)
{
	PG_UNUSED(program);
	// Nothing to do here
}

pgVoid pgBindProgramCore(pgProgram* program)
{
	if (program->device->vertexShader != program->vertexShader)
	{
		pgShader* shader = program->vertexShader;
		ID3D11DeviceContext_VSSetShader(PG_CONTEXT(shader), shader->ptr.vertexShader, NULL, 0);
	}

	if (program->device->fragmentShader != program->fragmentShader)
	{
		pgShader* shader = program->fragmentShader;
		ID3D11DeviceContext_PSSetShader(PG_CONTEXT(shader), shader->ptr.pixelShader, NULL, 0);
	}
}

#endif