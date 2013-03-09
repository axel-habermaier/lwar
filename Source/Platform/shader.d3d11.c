#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgFillInputDescs(D3D11_INPUT_ELEMENT_DESC* descs, pgShaderInput* input, pgInt32 inputCount);
static pgVoid pgGetByteCode(pgUint8** shaderData, pgUint8* end);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end, pgShaderInput* inputs, pgInt32 inputCount)
{
	size_t byteCodeLength;
	D3D11_INPUT_ELEMENT_DESC inputDescs[PG_INPUT_BINDINGS_COUNT];
	pgFillInputDescs(inputDescs, inputs, inputCount);

	pgGetByteCode(&shaderData, end);
	byteCodeLength = end - shaderData;

	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		PG_D3DCALL(ID3D11Device_CreateVertexShader(PG_DEVICE(shader), shaderData, byteCodeLength, NULL, &shader->ptr.vertexShader),
			"Failed to create vertex shader.");

		PG_D3DCALL(ID3D11Device_CreateInputLayout(PG_DEVICE(shader), inputDescs, inputCount, shaderData, byteCodeLength, &shader->inputLayout), 
			"Failed to create input layout.");
		break;
	case PG_FRAGMENT_SHADER:
		PG_D3DCALL(ID3D11Device_CreatePixelShader(shader->device->ptr, shaderData, byteCodeLength, NULL, &shader->ptr.pixelShader), 
			"Failed to create pixel shader.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

pgVoid pgDestroyShaderCore(pgShader* shader)
{
	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		PG_SAFE_RELEASE(ID3D11VertexShader, shader->ptr.vertexShader);
		PG_SAFE_RELEASE(ID3D11InputLayout, shader->inputLayout);
		break;
	case PG_FRAGMENT_SHADER:
		PG_SAFE_RELEASE(ID3D11PixelShader, shader->ptr.pixelShader);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

pgVoid pgBindShaderCore(pgShader* shader)
{
	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		ID3D11DeviceContext_VSSetShader(PG_CONTEXT(shader), shader->ptr.vertexShader, NULL, 0);
		ID3D11DeviceContext_IASetInputLayout(PG_CONTEXT(shader), shader->inputLayout);
		break;
	case PG_FRAGMENT_SHADER:
		ID3D11DeviceContext_PSSetShader(PG_CONTEXT(shader), shader->ptr.pixelShader, NULL, 0);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgFillInputDescs(D3D11_INPUT_ELEMENT_DESC* descs, pgShaderInput* input, pgInt32 inputCount)
{
	pgInt32 i;

	for (i = 0; i < inputCount; ++i)
	{
		pgInt32 semanticIndex;
		pgString semanticName;
		pgConvertVertexDataSemantics(input[i].semantics, &semanticIndex, &semanticName);

		descs[i].AlignedByteOffset = 0;
		descs[i].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		descs[i].InstanceDataStepRate = 0;
		descs[i].Format = pgConvertVertexDataFormat(input[i].format);
		descs[i].SemanticIndex = semanticIndex;
		descs[i].SemanticName = semanticName;
		descs[i].InputSlot = pgGetInputSlot(input[i].semantics);
	}
}

static pgVoid pgGetByteCode(pgUint8** shaderData, pgUint8* end)
{
	while (**shaderData != 0 && *shaderData < end)
		++(*shaderData);

	++(*shaderData);
	if (*shaderData >= end)
		pgDie("The HLSL version of the shader has not been compiled.");
}

#endif