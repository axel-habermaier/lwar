#include "prelude.h"
#include <float.h>

//====================================================================================================================
// Blend state exported functions
//====================================================================================================================

pgBlendState* pgCreateBlendState(pgGraphicsDevice* device, pgBlendDesc* description)
{
	pgBlendState* state;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);

	PG_ALLOC(pgBlendState, state);
	state->device = device;
	pgCreateBlendStateCore(state, description);
	return state;
}

pgVoid pgDestroyBlendState(pgBlendState* blendState)
{
	if (blendState == NULL)
		return;

	if (blendState->device->blendState == blendState)
		blendState->device->blendState = NULL;

	pgDestroyBlendStateCore(blendState);
	PG_FREE(blendState);
}

pgVoid pgBindBlendState(pgBlendState* blendState)
{
	PG_ASSERT_NOT_NULL(blendState);

	if (blendState->device->blendState == blendState)
		return;

	blendState->device->blendState = blendState;
	pgBindBlendStateCore(blendState);

	++blendState->device->statistics.blendStateBindingCount;
}

pgVoid pgSetBlendDescDefaults(pgBlendDesc* description)
{
	PG_ASSERT_NOT_NULL(description);

	description->blendEnabled = PG_FALSE,
	description->blendOperation = PG_BLEND_OP_ADD;
	description->blendOperationAlpha = PG_BLEND_OP_ADD;
	description->destinationBlend = PG_BLEND_ZERO;
	description->destinationBlendAlpha = PG_BLEND_ZERO;
	description->sourceBlend = PG_BLEND_ONE;
	description->sourceBlendAlpha = PG_BLEND_ONE;
	description->writeMask = PG_COLOR_WRITE_ENABLE_ALL;
}

//====================================================================================================================
// Depth stencil state exported functions
//====================================================================================================================

pgDepthStencilState* pgCreateDepthStencilState(pgGraphicsDevice* device, pgDepthStencilDesc* description)
{
	pgDepthStencilState* state;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);

	PG_ALLOC(pgDepthStencilState, state);
	state->device = device;
	pgCreateDepthStencilStateCore(state, description);
	return state;
}

pgVoid pgDestroyDepthStencilState(pgDepthStencilState* depthStencilState)
{
	if (depthStencilState == NULL)
		return;

	if (depthStencilState->device->depthStencilState == depthStencilState)
		depthStencilState->device->depthStencilState = NULL;

	pgDestroyDepthStencilStateCore(depthStencilState);
	PG_FREE(depthStencilState);
}

pgVoid pgBindDepthStencilState(pgDepthStencilState* depthStencilState)
{
	PG_ASSERT_NOT_NULL(depthStencilState);

	if (depthStencilState->device->depthStencilState == depthStencilState)
		return;

	depthStencilState->device->depthStencilState = depthStencilState;
	pgBindDepthStencilStateCore(depthStencilState);

	++depthStencilState->device->statistics.depthStencilStateBindingCount;
}

pgVoid pgSetDepthStencilDescDefaults(pgDepthStencilDesc* description)
{
	PG_ASSERT_NOT_NULL(description);

	description->backFace.fail = PG_STENCIL_OP_KEEP;
	description->backFace.depthFail = PG_STENCIL_OP_KEEP;
	description->backFace.pass = PG_STENCIL_OP_KEEP;
	description->backFace.func = PG_COMPARISON_ALWAYS;
	description->depthEnabled = PG_TRUE;
	description->depthFunction = PG_COMPARISON_LESS;
	description->depthWriteEnabled = PG_TRUE;
	description->frontFace.fail = PG_STENCIL_OP_KEEP;
	description->frontFace.depthFail = PG_STENCIL_OP_KEEP;
	description->frontFace.pass = PG_STENCIL_OP_KEEP;
	description->frontFace.func = PG_COMPARISON_ALWAYS;
	description->stencilEnabled = PG_FALSE;
	description->stencilReadMask = 0xff;
	description->stencilWriteMask = 0xff;
}

//====================================================================================================================
// Rasterizer state exported functions
//====================================================================================================================

pgRasterizerState* pgCreateRasterizerState(pgGraphicsDevice* device, pgRasterizerDesc* description)
{
	pgRasterizerState* state;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);

	PG_ALLOC(pgRasterizerState, state);
	state->device = device;
	pgCreateRasterizerStateCore(state, description);
	return state;
}

pgVoid pgDestroyRasterizerState(pgRasterizerState* rasterizerState)
{
	if (rasterizerState == NULL)
		return;

	if (rasterizerState->device->rasterizerState == rasterizerState)
		rasterizerState->device->rasterizerState = NULL;

	pgDestroyRasterizerStateCore(rasterizerState);
	PG_FREE(rasterizerState);
}

pgVoid pgBindRasterizerState(pgRasterizerState* rasterizerState)
{
	PG_ASSERT_NOT_NULL(rasterizerState);

	if (rasterizerState->device->rasterizerState == rasterizerState)
		return;

	rasterizerState->device->rasterizerState = rasterizerState;
	pgBindRasterizerStateCore(rasterizerState);

	++rasterizerState->device->statistics.rasterizerStateBindingCount;
}

pgVoid pgSetRasterizerDescDefaults(pgRasterizerDesc* description)
{
	PG_ASSERT_NOT_NULL(description);

	description->antialiasedLineEnabled = PG_FALSE;
	description->cullMode = PG_CULL_BACK;
	description->depthBias = 0;
	description->depthBiasClamp = 0;
	description->depthClipEnabled = PG_TRUE;
	description->fillMode = PG_FILL_SOLID;
	description->frontIsCounterClockwise = PG_FALSE;
	description->multisampleEnabled = PG_FALSE;
	description->scissorEnabled = PG_FALSE;
	description->slopeScaledDepthBias = 0;
}

//====================================================================================================================
// Sampler state exported functions
//====================================================================================================================

pgSamplerState* pgCreateSamplerState(pgGraphicsDevice* device, pgSamplerDesc* description)
{
	pgSamplerState* state;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(description);

	PG_ALLOC(pgSamplerState, state);
	state->device = device;
	pgCreateSamplerStateCore(state, description);
	return state;
}

pgVoid pgDestroySamplerState(pgSamplerState* samplerState)
{
	if (samplerState == NULL)
		return;

	pgDestroySamplerStateCore(samplerState);
	PG_FREE(samplerState);
}

pgVoid pgBindSamplerState(pgSamplerState* samplerState, pgInt32 slot)
{
	PG_ASSERT_NOT_NULL(samplerState);
	PG_ASSERT_IN_RANGE(slot, 0, PG_TEXTURE_SLOT_COUNT);

	if (samplerState->device->samplers[slot] == samplerState)
		return;

	samplerState->device->samplers[slot] = samplerState;
	pgBindSamplerStateCore(samplerState, slot);

	++samplerState->device->statistics.samplerStateBindingCount;
}

pgVoid pgSetSamplerDescDefaults(pgSamplerDesc* description)
{
	PG_ASSERT_NOT_NULL(description);

	description->addressU = PG_TEXTURE_ADDRESS_CLAMP;
	description->addressV = PG_TEXTURE_ADDRESS_CLAMP;
	description->addressW = PG_TEXTURE_ADDRESS_CLAMP;
	description->borderColor.red = 0;
	description->borderColor.green = 0;
	description->borderColor.blue = 0;
	description->borderColor.alpha = 0;
	description->comparison = PG_COMPARISON_NEVER;
	description->filter = PG_TEXTURE_FILTER_BILINEAR;
	description->maximumAnisotropy = 16;
	description->maximumLod = FLT_MAX;
	description->minimumLod = FLT_MIN;
	description->mipLodBias = 0.0f;
}