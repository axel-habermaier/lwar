#include "Prelude.hpp"

using namespace OpenGL3;

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// BlendState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
BlendState* GraphicsDevice::InitializeBlendState(BlendDescription* desc)
{
	auto state = PG_NEW(BlendState);
	state->Desc = *desc;
	return state;
}

void GraphicsDevice::FreeBlendState(BlendState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindBlendState(BlendState* state)
{
	if (state->Desc.BlendEnabled)
	{
		EnableBlend(true);
		SetBlendEquation(Map(state->Desc.BlendOperation), Map(state->Desc.BlendOperationAlpha));

		SetBlendFuncs(Map(state->Desc.SourceBlend), Map(state->Desc.DestinationBlend), Map(state->Desc.SourceBlendAlpha), Map(state->Desc.DestinationBlendAlpha));

		int32 colorMask[4];
		colorMask[0] = static_cast<int32>(Enum::HasFlag(state->Desc.WriteMask, ColorWriteChannels::Alpha));
		colorMask[1] = static_cast<int32>(Enum::HasFlag(state->Desc.WriteMask, ColorWriteChannels::Red));
		colorMask[2] = static_cast<int32>(Enum::HasFlag(state->Desc.WriteMask, ColorWriteChannels::Green));
		colorMask[3] = static_cast<int32>(Enum::HasFlag(state->Desc.WriteMask, ColorWriteChannels::Blue));
		SetColorMask(colorMask);
	}
	else
		EnableBlend(false);
}

void GraphicsDevice::SetBlendStateName(BlendState* state, const char* name)
{
	PG_UNUSED(state);
	PG_UNUSED(name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// DepthStencilState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
DepthStencilState* GraphicsDevice::InitializeDepthStencilState(DepthStencilDescription* desc)
{
	auto state = PG_NEW(DepthStencilState);
	state->Desc  = *desc;
	return state;
}

void GraphicsDevice::FreeDepthStencilState(DepthStencilState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindDepthStencilState(DepthStencilState* state)
{
	auto setupStencilState = [&](uint32 face, const StencilOperationDescription& stencilDesc)
	{
		glStencilFuncSeparate(face, Map(stencilDesc.StencilFunction), state->Desc.StencilReadMask, state->Desc.StencilWriteMask);
		glStencilOpSeparate(face, Map(stencilDesc.FailOperation), Map(stencilDesc.DepthFailOperation), Map(stencilDesc.PassOperation));
	};

	if (state->Desc.DepthEnabled)
	{
		EnableDepthTest(true);
		SetDepthFunc(Map(state->Desc.DepthFunction));
	}
	else
		EnableDepthTest(false);

	if (state->Desc.StencilEnabled)
	{
		EnableStencilTest(true);

		setupStencilState(GL_FRONT, state->Desc.FrontFace);
		setupStencilState(GL_BACK, state->Desc.BackFace);
	}
	else
		EnableStencilTest(false);

	EnableDepthWrites(static_cast<int32>(state->Desc.DepthWriteEnabled));
}

void GraphicsDevice::SetDepthStencilStateName(DepthStencilState* state, const char* name)
{
	PG_UNUSED(state);
	PG_UNUSED(name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// RasterizerState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
RasterizerState* GraphicsDevice::InitializeRasterizerState(RasterizerDescription* desc)
{
	auto state = PG_NEW(RasterizerState);
	state->Desc = *desc;
	return state;
}

void GraphicsDevice::FreeRasterizerState(RasterizerState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindRasterizerState(RasterizerState* state)
{
	// TODO: What about depth bias?
	SetPolygonMode(Map(state->Desc.FillMode));
	SetPolygonOffset(state->Desc.SlopeScaledDepthBias, state->Desc.DepthBiasClamp);
	SetFrontFace(static_cast<uint32>(state->Desc.FrontIsCounterClockwise ? GL_CCW : GL_CW));

	switch (state->Desc.CullMode)
	{
	case CullMode::Front:
		EnableCullFace(true);
		SetCullFace(GL_FRONT);
		break;
	case CullMode::Back:
		EnableCullFace(true);
		SetCullFace(GL_BACK);
		break;
	case CullMode::None:
		EnableCullFace(false);
		break;
	}

	EnableScissor(static_cast<int32>(state->Desc.ScissorEnabled));
	EnableDepthClamp(static_cast<int32>(state->Desc.DepthClipEnabled));
	EnableMultisample(static_cast<int32>(state->Desc.MultisampleEnabled));
	EnableAntialiasedLine(static_cast<int32>(state->Desc.AntialiasedLineEnabled));
}

void GraphicsDevice::SetRasterizerStateName(RasterizerState* state, const char* name)
{
	PG_UNUSED(state);
	PG_UNUSED(name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// SamplerState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
SamplerState* GraphicsDevice::InitializeSamplerState(SamplerDescription* desc)
{
	auto state = PG_NEW(SamplerState);
	state->Obj = Allocate(&GraphicsDevice::glGenSamplers, "Sampler state");

	float32 borderColor[4];
	borderColor[0] = desc->BorderColor.Red / 255.0f;
	borderColor[1] = desc->BorderColor.Green / 255.0f;
	borderColor[2] = desc->BorderColor.Blue / 255.0f;
	borderColor[3] = desc->BorderColor.Alpha / 255.0f;

	glSamplerParameteri(state->Obj, GL_TEXTURE_MIN_FILTER, GetMinFilter(desc->Filter));
	glSamplerParameteri(state->Obj, GL_TEXTURE_MAG_FILTER, GetMagFilter(desc->Filter));
	glSamplerParameteri(state->Obj, GL_TEXTURE_WRAP_S, Map(desc->AddressU));
	glSamplerParameteri(state->Obj, GL_TEXTURE_WRAP_T, Map(desc->AddressV));
	glSamplerParameteri(state->Obj, GL_TEXTURE_WRAP_R, Map(desc->AddressW));
	glSamplerParameteri(state->Obj, GL_TEXTURE_MAX_ANISOTROPY_EXT, desc->MaximumAnisotropy);
	glSamplerParameterfv(state->Obj, GL_TEXTURE_BORDER_COLOR, borderColor);
	glSamplerParameterf(state->Obj, GL_TEXTURE_MIN_LOD, desc->MinimumLod);
	glSamplerParameterf(state->Obj, GL_TEXTURE_MAX_LOD, desc->MaximumLod);
	glSamplerParameterf(state->Obj, GL_TEXTURE_LOD_BIAS, desc->MipLodBias);
	glSamplerParameteri(state->Obj, GL_TEXTURE_COMPARE_FUNC, static_cast<int32>(Map(desc->Comparison)));

	return state;
}

void GraphicsDevice::FreeSamplerState(SamplerState* state)
{
	if (state == nullptr)
		return;

	glDeleteSamplers(1, &state->Obj);
	PG_DELETE(state);
}

void GraphicsDevice::BindSamplerState(SamplerState* state, int32 slot)
{
	glBindSampler(safe_static_cast<uint32>(slot), state->Obj);
}

void GraphicsDevice::SetSamplerStateName(SamplerState* state, const char* name)
{
	PG_UNUSED(state);
	PG_UNUSED(name);
}