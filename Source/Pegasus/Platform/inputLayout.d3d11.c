#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateInputLayoutCore(pgInputLayout* inputLayout, pgBuffer* indexBuffer, pgInt32 indexOffset, 
							   pgIndexSize indexSize, pgInputBinding* inputBindings, pgInt32 bindingsCount)
{
	pgInt32 i;

	inputLayout->indexBuffer = indexBuffer;
	inputLayout->indexFormat = pgConvertIndexSize(indexSize);
	inputLayout->indexOffset = indexOffset;
	inputLayout->bindingsCount = bindingsCount;

	for (i = 0; i < bindingsCount; ++i)
		inputLayout->bindings[i] = inputBindings[i];
}

pgVoid pgDestroyInputLayoutCore(pgInputLayout* inputLayout)
{
	// Nothing to do here
	PG_UNUSED(inputLayout);
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
}

#endif