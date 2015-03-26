#include "Prelude.hpp"

using namespace Direct3D11;

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// BlendState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
BlendState* GraphicsDevice::InitializeBlendState(BlendDescription* desc)
{
	D3D11_BLEND_DESC d3d11Desc;
	d3d11Desc.AlphaToCoverageEnable = false;
	d3d11Desc.IndependentBlendEnable = false;
	d3d11Desc.RenderTarget[0].BlendEnable = desc->BlendEnabled;
	d3d11Desc.RenderTarget[0].BlendOp = Map(desc->BlendOperation);
	d3d11Desc.RenderTarget[0].SrcBlend = Map(desc->SourceBlend);
	d3d11Desc.RenderTarget[0].DestBlend = Map(desc->DestinationBlend);
	d3d11Desc.RenderTarget[0].BlendOpAlpha = Map(desc->BlendOperationAlpha);
	d3d11Desc.RenderTarget[0].SrcBlendAlpha = Map(desc->SourceBlendAlpha);
	d3d11Desc.RenderTarget[0].DestBlendAlpha = Map(desc->DestinationBlendAlpha);
	d3d11Desc.RenderTarget[0].RenderTargetWriteMask = Map(desc->WriteMask);

	auto state = PG_NEW(BlendState);;
	CheckResult(_device->CreateBlendState(&d3d11Desc, &state->Obj), "Failed to create blend state.");
	return state;
}

void GraphicsDevice::FreeBlendState(BlendState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindBlendState(BlendState* state)
{
	_context->OMSetBlendState(state->Obj.Get(), nullptr, std::numeric_limits<uint32>::max());
}

void GraphicsDevice::SetBlendStateName(BlendState* state, const char* name)
{
	SetName(state->Obj, name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// DepthStencilState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
DepthStencilState* GraphicsDevice::InitializeDepthStencilState(DepthStencilDescription* desc)
{
	D3D11_DEPTH_STENCIL_DESC d3d11Desc;
	d3d11Desc.BackFace.StencilFunc = Map(desc->BackFace.StencilFunction);
	d3d11Desc.BackFace.StencilDepthFailOp = Map(desc->BackFace.DepthFailOperation);
	d3d11Desc.BackFace.StencilFailOp = Map(desc->BackFace.FailOperation);
	d3d11Desc.BackFace.StencilPassOp = Map(desc->BackFace.PassOperation);
	d3d11Desc.FrontFace.StencilFunc = Map(desc->FrontFace.StencilFunction);
	d3d11Desc.FrontFace.StencilDepthFailOp = Map(desc->FrontFace.DepthFailOperation);
	d3d11Desc.FrontFace.StencilFailOp = Map(desc->FrontFace.FailOperation);
	d3d11Desc.FrontFace.StencilPassOp = Map(desc->FrontFace.PassOperation);
	d3d11Desc.DepthFunc = Map(desc->DepthFunction);
	d3d11Desc.DepthWriteMask = desc->DepthWriteEnabled ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO;
	d3d11Desc.DepthEnable = desc->DepthEnabled;
	d3d11Desc.StencilEnable = desc->StencilEnabled;
	d3d11Desc.StencilReadMask = desc->StencilReadMask;
	d3d11Desc.StencilWriteMask = desc->StencilWriteMask;

	auto state = PG_NEW(DepthStencilState);
	CheckResult(_device->CreateDepthStencilState(&d3d11Desc, &state->Obj), "Failed to create depth stencil state.");
	return state;
}

void GraphicsDevice::FreeDepthStencilState(DepthStencilState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindDepthStencilState(DepthStencilState* state)
{
	_context->OMSetDepthStencilState(state->Obj.Get(), 0);
}

void GraphicsDevice::SetDepthStencilStateName(DepthStencilState* state, const char* name)
{
	SetName(state->Obj, name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// RasterizerState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
RasterizerState* GraphicsDevice::InitializeRasterizerState(RasterizerDescription* desc)
{
	D3D11_RASTERIZER_DESC d3d11Desc;
	d3d11Desc.CullMode = Map(desc->CullMode);
	d3d11Desc.FillMode = Map(desc->FillMode);
	d3d11Desc.DepthBias = desc->DepthBias;
	d3d11Desc.DepthBiasClamp = desc->DepthBiasClamp;
	d3d11Desc.AntialiasedLineEnable = desc->AntialiasedLineEnabled;
	d3d11Desc.DepthClipEnable = desc->DepthClipEnabled;
	d3d11Desc.FrontCounterClockwise = desc->FrontIsCounterClockwise;
	d3d11Desc.MultisampleEnable = desc->MultisampleEnabled;
	d3d11Desc.ScissorEnable = desc->ScissorEnabled;
	d3d11Desc.SlopeScaledDepthBias = desc->SlopeScaledDepthBias;

	auto state = PG_NEW(RasterizerState);
	CheckResult(_device->CreateRasterizerState(&d3d11Desc, &state->Obj), "Failed to create rasterizer state.");
	return state;
}

void GraphicsDevice::FreeRasterizerState(RasterizerState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindRasterizerState(RasterizerState* state)
{
	_context->RSSetState(state->Obj.Get());
}

void GraphicsDevice::SetRasterizerStateName(RasterizerState* state, const char* name)
{
	SetName(state->Obj, name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// SamplerState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
SamplerState* GraphicsDevice::InitializeSamplerState(SamplerDescription* desc)
{
	D3D11_SAMPLER_DESC d3d11Desc;
	d3d11Desc.AddressU = Map(desc->AddressU);
	d3d11Desc.AddressV = Map(desc->AddressV);
	d3d11Desc.AddressW = Map(desc->AddressW);
	d3d11Desc.BorderColor[0] = desc->BorderColor.Red / 255.0f;
	d3d11Desc.BorderColor[1] = desc->BorderColor.Green / 255.0f;
	d3d11Desc.BorderColor[2] = desc->BorderColor.Blue / 255.0f;
	d3d11Desc.BorderColor[3] = desc->BorderColor.Alpha / 255.0f;
	d3d11Desc.ComparisonFunc = Map(desc->Comparison);
	d3d11Desc.Filter = Map(desc->Filter);
	d3d11Desc.MaxAnisotropy = safe_static_cast<UINT>(desc->MaximumAnisotropy);
	d3d11Desc.MaxLOD = desc->MaximumLod;
	d3d11Desc.MinLOD = desc->MinimumLod;
	d3d11Desc.MipLODBias = desc->MipLodBias;

	if (desc->Filter == TextureFilter::NearestNoMipmaps || desc->Filter == TextureFilter::BilinearNoMipmaps)
	{
		if (d3d11Desc.MaxLOD == std::numeric_limits<float32>::max())
			d3d11Desc.MaxLOD = 0;
		if (d3d11Desc.MinLOD == std::numeric_limits<float32>::min())
			d3d11Desc.MinLOD = 0;
	}

	auto state = PG_NEW(SamplerState);
	CheckResult(_device->CreateSamplerState(&d3d11Desc, &state->Obj), "Failed to create sampler state.");
	return state;
}

void GraphicsDevice::FreeSamplerState(SamplerState* state)
{
	PG_DELETE(state);
}

void GraphicsDevice::BindSamplerState(SamplerState* state, int32 slot)
{
	_context->VSSetSamplers(safe_static_cast<UINT>(slot), 1, state->Obj.GetAddressOf());
	_context->PSSetSamplers(safe_static_cast<UINT>(slot), 1, state->Obj.GetAddressOf());
}

void GraphicsDevice::SetSamplerStateName(SamplerState* state, const char* name)
{
	SetName(state->Obj, name);
}
