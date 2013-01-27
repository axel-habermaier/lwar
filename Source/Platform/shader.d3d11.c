#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions
//====================================================================================================================

typedef struct
{
	pgUint8* data;
	pgInt32  pos;
} Buffer;

static pgInt32 ReadInt32(Buffer* buffer);
static pgString ReadString(Buffer* buffer);
static D3D11_INPUT_ELEMENT_DESC* ReadInputElements(Buffer* buffer, pgInt32* size);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateShaderCore(pgShader* shader, pgShaderType type, pgVoid* shaderData)
{
	Buffer buffer;
	pgInt32 byteCodeLength;
	pgVoid* byteCode = (pgUint8*)shaderData + 4;
	buffer.data = (pgUint8*)shaderData;
	buffer.pos = 0;

	byteCodeLength = ReadInt32(&buffer);
	switch (type)
	{
	case PG_VERTEX_SHADER:
	{
		D3D11_INPUT_ELEMENT_DESC* inputDescs;
		pgInt32 inputDescCount;
		int i;

		D3DCALL(ID3D11Device_CreateVertexShader(DEVICE(shader), byteCode, byteCodeLength, NULL, &shader->ptr.vertexShader),
			"Failed to create vertex shader.");

		buffer.pos = sizeof(pgInt32) + byteCodeLength;
		inputDescs = ReadInputElements(&buffer, &inputDescCount);
		D3DCALL(ID3D11Device_CreateInputLayout(DEVICE(shader), inputDescs, inputDescCount, byteCode, byteCodeLength, &shader->inputLayout), 
			"Failed to create input layout.");

		for (i = 0; i < inputDescCount; ++i)
			PG_FREE(inputDescs[i].SemanticName);
		break;
	}
	case PG_FRAGMENT_SHADER:
		D3DCALL(ID3D11Device_CreatePixelShader(shader->device->ptr, byteCode, byteCodeLength, NULL, &shader->ptr.pixelShader), 
			"Failed to create pixel shader.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	shader->type = type;
}

pgVoid pgDestroyShaderCore(pgShader* shader)
{
	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		ID3D11VertexShader_Release(shader->ptr.vertexShader);
		ID3D11InputLayout_Release(shader->inputLayout);
		break;
	case PG_FRAGMENT_SHADER:
		ID3D11PixelShader_Release(shader->ptr.pixelShader);
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
		ID3D11DeviceContext_VSSetShader(CONTEXT(shader), shader->ptr.vertexShader, NULL, 0);
		ID3D11DeviceContext_IASetInputLayout(CONTEXT(shader), shader->inputLayout);
		break;
	case PG_FRAGMENT_SHADER:
		ID3D11DeviceContext_PSSetShader(CONTEXT(shader), shader->ptr.pixelShader, NULL, 0);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgInt32 ReadInt32(Buffer* buffer)
{
	pgInt32 res = buffer->data[buffer->pos] | buffer->data[buffer->pos + 1] << 8 | buffer->data[buffer->pos + 2] << 16 | 
		buffer->data[buffer->pos + 3] << 24;
	buffer->pos += 4;

	return res;
}

static pgString ReadString(Buffer* buffer)
{
	pgChar* str;
	pgInt32 size;

	PG_ASSERT_NOT_NULL(buffer);
	
	size = ReadInt32(buffer);
	PG_ALLOC_ARRAY(pgChar, size + 1, str);

	memcpy(str, buffer->data + buffer->pos, size);
	str[size] = '\0';
	buffer->pos += size;

	return str;
}

static D3D11_INPUT_ELEMENT_DESC* ReadInputElements(Buffer* buffer, pgInt32* size)
{
	static D3D11_INPUT_ELEMENT_DESC descs[PG_INPUT_BINDINGS_COUNT];
	pgInt32 i;
	
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_NOT_NULL(size);

	*size = ReadInt32(buffer);
	for (i = 0; i < *size; ++i)
	{
		descs[i].AlignedByteOffset = ReadInt32(buffer);
		descs[i].InputSlotClass = (D3D11_INPUT_CLASSIFICATION)ReadInt32(buffer);
		descs[i].Format = (DXGI_FORMAT)ReadInt32(buffer);
		descs[i].InstanceDataStepRate = ReadInt32(buffer);
		descs[i].SemanticIndex = ReadInt32(buffer);
		descs[i].SemanticName = ReadString(buffer);
		descs[i].InputSlot = pgConvertVertexDataSemantics((pgVertexDataSemantics)ReadInt32(buffer));
	}

	return descs;
}

#endif