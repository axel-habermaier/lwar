#include "prelude.h"

#ifdef PG_GRAPHICS_DIRECT3D11

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateInputLayoutCore(pgInputLayout* inputLayout, pgBuffer* indexBuffer, pgInt32 indexOffset,
							   pgIndexSize indexSize, pgInputBinding* inputBindings, pgInt32 bindingsCount,
							   pgByte* signatureShader, pgUInt32 signatureShaderLength)
{
	pgInt32 i;
	D3D11_INPUT_ELEMENT_DESC inputDescs[PG_INPUT_BINDINGS_COUNT];

	for (i = 0; i < bindingsCount; ++i)
	{
		pgInt32 semanticIndex;
		pgString semanticName;
		pgConvertVertexDataSemantics(inputBindings[i].semantics, &semanticIndex, &semanticName);

		inputDescs[i].AlignedByteOffset = 0;
		inputDescs[i].InputSlotClass = inputBindings[i].instanceStepRate == 0 ? D3D11_INPUT_PER_VERTEX_DATA : D3D11_INPUT_PER_INSTANCE_DATA;
		inputDescs[i].InstanceDataStepRate = inputBindings[i].instanceStepRate;
		inputDescs[i].Format = pgConvertVertexDataFormat(inputBindings[i].format);
		inputDescs[i].SemanticIndex = semanticIndex;
		inputDescs[i].SemanticName = semanticName;
		inputDescs[i].InputSlot = (UINT)inputBindings[i].semantics;
	}

	PG_D3DCALL(ID3D11Device_CreateInputLayout(PG_DEVICE(inputLayout), inputDescs, bindingsCount,
		signatureShader, signatureShaderLength, &inputLayout->inputLayout),
		"Failed to create input layout.");

	inputLayout->indexBuffer = indexBuffer;
	inputLayout->indexFormat = pgConvertIndexSize(indexSize);
	inputLayout->indexOffset = indexOffset;
	inputLayout->bindingsCount = bindingsCount;

	for (i = 0; i < bindingsCount; ++i)
		inputLayout->bindings[i] = inputBindings[i];
}

pgVoid pgDestroyInputLayoutCore(pgInputLayout* inputLayout)
{
	PG_SAFE_RELEASE(ID3D11InputLayout, inputLayout->inputLayout);
}

pgVoid pgBindInputLayoutCore(pgInputLayout* inputLayout)
{
	pgInt32 i;

	for (i = 0; i < inputLayout->bindingsCount; ++i)
	{
		pgInputBinding* binding = &inputLayout->bindings[i];
		UINT slot = (UINT)binding->semantics;
		UINT stride = binding->stride;
		UINT offset = binding->offset;

		ID3D11DeviceContext_IASetVertexBuffers(PG_CONTEXT(inputLayout), slot, 1, &binding->vertexBuffer->ptr, &stride, &offset);
	}

	if (inputLayout->indexBuffer != NULL)
		ID3D11DeviceContext_IASetIndexBuffer(PG_CONTEXT(inputLayout), inputLayout->indexBuffer->ptr, inputLayout->indexFormat, inputLayout->indexOffset);

	ID3D11DeviceContext_IASetInputLayout(PG_CONTEXT(inputLayout), inputLayout->inputLayout);
}

#endif