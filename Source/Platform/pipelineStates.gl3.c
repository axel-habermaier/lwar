#include "prelude.h"

#ifdef OPENGL3

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

static pgVoid Toggle(GLenum capability, pgBool enable)
{
	if (enable)
		glEnable(capability);
	else
		glDisable(capability);
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
		GLboolean writeA, writeR, writeG, writeB;

		glEnable(GL_BLEND);
		glBlendEquationSeparate(pgConvertBlendOperation(desc->blendOperation), pgConvertBlendOperation(desc->blendOperationAlpha));
		glBlendFuncSeparate(pgConvertBlendOption(desc->sourceBlend), pgConvertBlendOption(desc->destinationBlend),
			pgConvertBlendOption(desc->sourceBlendAlpha), pgConvertBlendOption(desc->destinationBlendAlpha));

		writeA = (desc->writeMask & PG_COLOR_WRITE_ENABLE_ALPHA) == PG_COLOR_WRITE_ENABLE_ALPHA;
		writeR = (desc->writeMask & PG_COLOR_WRITE_ENABLE_RED) == PG_COLOR_WRITE_ENABLE_RED;
		writeG = (desc->writeMask & PG_COLOR_WRITE_ENABLE_GREEN) == PG_COLOR_WRITE_ENABLE_GREEN;
		writeB = (desc->writeMask & PG_COLOR_WRITE_ENABLE_BLUE) == PG_COLOR_WRITE_ENABLE_BLUE;
		glColorMask(writeR, writeG, writeB, writeA);
	}
	else
		glDisable(GL_BLEND);

	PG_ASSERT_NO_GL_ERRORS();
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

	glPolygonMode(GL_FRONT_AND_BACK, pgConvertFillMode(desc->fillMode));
	glPolygonOffset(desc->slopeScaledDepthBias, desc->depthBiasClamp);
	glFrontFace(desc->frontIsCounterClockwise ? GL_CCW : GL_CW);
	PG_ASSERT_NO_GL_ERRORS();

	switch (desc->cullMode)
	{
	case PG_CULL_FRONT:
		glEnable(GL_CULL_FACE);
		glCullFace(GL_FRONT);
		break;
	case PG_CULL_BACK:
		glEnable(GL_CULL_FACE);
		glCullFace(GL_BACK);
		break;
	default:
		glDisable(GL_CULL_FACE);
		break;
	}
	PG_ASSERT_NO_GL_ERRORS();

	Toggle(GL_SCISSOR_TEST, desc->scissorEnabled);
	Toggle(GL_DEPTH_CLAMP, desc->depthClipEnabled);
	Toggle(GL_MULTISAMPLE, desc->multisampleEnabled);
	Toggle(GL_LINE_SMOOTH, desc->antialiasedLineEnabled);

	PG_ASSERT_NO_GL_ERRORS();
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
		glEnable(GL_DEPTH_TEST);
		glDepthMask((GLboolean)desc->depthEnabled);
		glDepthFunc(pgConvertComparison(desc->depthFunction));
	}
	else
		glDisable(GL_DEPTH_TEST);

	if (desc->stencilEnabled)
	{
		glEnable(GL_STENCIL_TEST);

		SetupStencilState(desc, GL_FRONT, &desc->frontFace);
		SetupStencilState(desc, GL_BACK, &desc->backFace);
	}
	else
		glDisable(GL_STENCIL_TEST);

	PG_ASSERT_NO_GL_ERRORS();
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