#include "prelude.h"

#ifdef PG_GRAPHICS_OPENGL3

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid SetupStencilState(pgDepthStencilDesc* desc, GLenum face, pgStencilOpDesc* stencilDesc)
{
	glStencilFuncSeparate(face, pgConvertComparison(stencilDesc->func), desc->stencilReadMask, desc->stencilWriteMask);
	glStencilOpSeparate(face, 
		pgConvertStencilOperation(stencilDesc->fail), 
		pgConvertStencilOperation(stencilDesc->depthFail),
		pgConvertStencilOperation(stencilDesc->pass));
}

//====================================================================================================================
// Blend state core functions
//====================================================================================================================

pgVoid pgCreateBlendStateCore(pgBlendState* blendState, pgBlendDesc* description)
{
	blendState->description = *description;
}

pgVoid pgDestroyBlendStateCore(pgBlendState* blendState)
{
	// Blend state objects are not supported by OpenGL
	PG_UNUSED(blendState);
}

pgVoid pgBindBlendStateCore(pgBlendState* blendState)
{
	pgBlendDesc* desc = &blendState->description;
	
	if (desc->blendEnabled)
	{
		GLboolean colorMask[4];

		pgEnableBlend(blendState->device, GL_TRUE);
		pgSetBlendEquation(blendState->device, pgConvertBlendOperation(desc->blendOperation), pgConvertBlendOperation(desc->blendOperationAlpha));
		pgSetBlendFuncs(blendState->device, pgConvertBlendOption(desc->sourceBlend), pgConvertBlendOption(desc->destinationBlend),
			pgConvertBlendOption(desc->sourceBlendAlpha), pgConvertBlendOption(desc->destinationBlendAlpha));

		colorMask[0] = (desc->writeMask & PG_COLOR_WRITE_ENABLE_ALPHA) == PG_COLOR_WRITE_ENABLE_ALPHA;
		colorMask[1] = (desc->writeMask & PG_COLOR_WRITE_ENABLE_RED) == PG_COLOR_WRITE_ENABLE_RED;
		colorMask[2] = (desc->writeMask & PG_COLOR_WRITE_ENABLE_GREEN) == PG_COLOR_WRITE_ENABLE_GREEN;
		colorMask[3] = (desc->writeMask & PG_COLOR_WRITE_ENABLE_BLUE) == PG_COLOR_WRITE_ENABLE_BLUE;
		pgSetColorMask(blendState->device, colorMask);
	}
	else
		pgEnableBlend(blendState->device, GL_FALSE);
}

//====================================================================================================================
// Rasterizer state core functions
//====================================================================================================================

pgVoid pgCreateRasterizerStateCore(pgRasterizerState* rasterizerState, pgRasterizerDesc* description)
{
	rasterizerState->description = *description;
}

pgVoid pgDestroyRasterizerStateCore(pgRasterizerState* rasterizerState)
{
	// Rasterizer state objects are not supported by OpenGL
	PG_UNUSED(rasterizerState);
}

pgVoid pgBindRasterizerStateCore(pgRasterizerState* rasterizerState)
{
	// TODO: What about depth bias?
	pgRasterizerDesc* desc = &rasterizerState->description;

	pgSetPolygonMode(rasterizerState->device, pgConvertFillMode(desc->fillMode));
	pgSetPolygonOffset(rasterizerState->device, desc->slopeScaledDepthBias, desc->depthBiasClamp);
	pgSetFrontFace(rasterizerState->device, desc->frontIsCounterClockwise ? GL_CCW : GL_CW);

	switch (desc->cullMode)
	{
	case PG_CULL_FRONT:
		pgEnableCullFace(rasterizerState->device, GL_TRUE);
		pgSetCullFace(rasterizerState->device, GL_FRONT);
		break;
	case PG_CULL_BACK:
		pgEnableCullFace(rasterizerState->device, GL_TRUE);
		pgSetCullFace(rasterizerState->device, GL_BACK);
		break;
	default:
		pgEnableCullFace(rasterizerState->device, GL_FALSE);
		break;
	}

	pgEnableScissor(rasterizerState->device, (GLboolean)desc->scissorEnabled);
	pgEnableDepthClamp(rasterizerState->device, (GLboolean)desc->depthClipEnabled);
	pgEnableMultisample(rasterizerState->device, (GLboolean)desc->multisampleEnabled);
	pgEnableAntialiasedLine(rasterizerState->device, (GLboolean)desc->antialiasedLineEnabled);
}

//====================================================================================================================
// Depth stencil state core functions
//====================================================================================================================

pgVoid pgCreateDepthStencilStateCore(pgDepthStencilState* depthStencilState, pgDepthStencilDesc* description)
{
	depthStencilState->description = *description;
}

pgVoid pgDestroyDepthStencilStateCore(pgDepthStencilState* depthStencilState)
{
	// Depth stencil state objects are not supported by OpenGL
	PG_UNUSED(depthStencilState);
}

pgVoid pgBindDepthStencilStateCore(pgDepthStencilState* depthStencilState)
{
	pgDepthStencilDesc* desc = &depthStencilState->description;

	if (desc->depthEnabled)
	{
		pgEnableDepthTest(depthStencilState->device, GL_TRUE);
		pgSetDepthFunc(depthStencilState->device, pgConvertComparison(desc->depthFunction));
	}
	else
		pgEnableDepthTest(depthStencilState->device, GL_FALSE);

	if (desc->stencilEnabled)
	{
		pgEnableStencilTest(depthStencilState->device, GL_TRUE);

		SetupStencilState(desc, GL_FRONT, &desc->frontFace);
		SetupStencilState(desc, GL_BACK, &desc->backFace);
	}
	else
		pgEnableStencilTest(depthStencilState->device, GL_FALSE);

	pgEnableDepthWrites(depthStencilState->device, (GLboolean)desc->depthWriteEnabled);
}

//====================================================================================================================
// Sampler state core functions
//====================================================================================================================

pgVoid pgCreateSamplerStateCore(pgSamplerState* samplerState, pgSamplerDesc* description)
{
	GLenum minFilter, magFilter;
	GLfloat borderColor[4];
	borderColor[0] = description->borderColor.red;
	borderColor[1] = description->borderColor.green;
	borderColor[2] = description->borderColor.blue;
	borderColor[3] = description->borderColor.alpha;

	PG_GL_ALLOC("Sampler State", glGenSamplers, samplerState->id);

	pgConvertTextureFilter(description->filter, &minFilter, &magFilter);
	glSamplerParameteri(samplerState->id, GL_TEXTURE_MIN_FILTER, minFilter);
	glSamplerParameteri(samplerState->id, GL_TEXTURE_MAG_FILTER, magFilter);
	glSamplerParameteri(samplerState->id, GL_TEXTURE_WRAP_S, pgConvertTextureAddressMode(description->addressU));
	glSamplerParameteri(samplerState->id, GL_TEXTURE_WRAP_T, pgConvertTextureAddressMode(description->addressV));
	glSamplerParameteri(samplerState->id, GL_TEXTURE_WRAP_R, pgConvertTextureAddressMode(description->addressW));
	glSamplerParameteri(samplerState->id, GL_TEXTURE_MAX_ANISOTROPY_EXT, description->maximumAnisotropy);
	glSamplerParameterfv(samplerState->id, GL_TEXTURE_BORDER_COLOR, borderColor);
	glSamplerParameterf(samplerState->id, GL_TEXTURE_MIN_LOD, description->minimumLod);
	glSamplerParameterf(samplerState->id, GL_TEXTURE_MAX_LOD, description->maximumLod);
	glSamplerParameterf(samplerState->id, GL_TEXTURE_LOD_BIAS, description->mipLodBias);
	glSamplerParameteri(samplerState->id, GL_TEXTURE_COMPARE_FUNC, pgConvertComparison(description->comparison));
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDestroySamplerStateCore(pgSamplerState* samplerState)
{
	PG_GL_FREE(glDeleteSamplers, samplerState->id);
}

pgVoid pgBindSamplerStateCore(pgSamplerState* samplerState, pgInt32 slot)
{
	glBindSampler(slot, samplerState->id);
	PG_ASSERT_NO_GL_ERRORS();
}

#endif