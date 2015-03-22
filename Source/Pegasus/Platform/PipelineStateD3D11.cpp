#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/GraphicsDevice.hpp"
#include "Graphics/PipelineState.hpp"

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// BlendState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
void GraphicsDevice::Initialize(BlendState* state, const BlendDesc& desc)
{
	D3D11_BLEND_DESC d3d11Desc;
	d3d11Desc.AlphaToCoverageEnable = false;
	d3d11Desc.IndependentBlendEnable = false;
	d3d11Desc.RenderTarget[0].BlendEnable = desc.BlendEnabled;
	d3d11Desc.RenderTarget[0].BlendOp = Direct3D11::Map(desc.Operation);
	d3d11Desc.RenderTarget[0].SrcBlend = Direct3D11::Map(desc.SourceBlend);
	d3d11Desc.RenderTarget[0].DestBlend = Direct3D11::Map(desc.DestinationBlend);
	d3d11Desc.RenderTarget[0].BlendOpAlpha = Direct3D11::Map(desc.OperationAlpha);
	d3d11Desc.RenderTarget[0].SrcBlendAlpha = Direct3D11::Map(desc.SourceBlendAlpha);
	d3d11Desc.RenderTarget[0].DestBlendAlpha = Direct3D11::Map(desc.DestinationBlendAlpha);
	d3d11Desc.RenderTarget[0].RenderTargetWriteMask = Direct3D11::Map(desc.WriteMask);

	Direct3D11::CheckResult(_device->CreateBlendState(&d3d11Desc, &state->_state), "Failed to create blend state.");
}

void GraphicsDevice::Free(BlendState* state)
{
	UnsetState(&_boundBlendState, state);
	Direct3D11::Release(state->_state);
}

void GraphicsDevice::Bind(const BlendState* state)
{
	if (ChangeState(&_boundBlendState, state))
		_context->OMSetBlendState(state->_state, nullptr, UInt32::MaxValue);
}

void GraphicsDevice::SetName(BlendState* state, const char* name)
{
	Direct3D11::SetName(state->_state, name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// DepthStencilState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
void GraphicsDevice::Initialize(DepthStencilState* state, const DepthStencilDesc& desc)
{
	D3D11_DEPTH_STENCIL_DESC d3d11Desc;
	d3d11Desc.BackFace.StencilFunc = Direct3D11::Map(desc.BackFace.Func);
	d3d11Desc.BackFace.StencilDepthFailOp = Direct3D11::Map(desc.BackFace.DepthFail);
	d3d11Desc.BackFace.StencilFailOp = Direct3D11::Map(desc.BackFace.Fail);
	d3d11Desc.BackFace.StencilPassOp = Direct3D11::Map(desc.BackFace.Pass);
	d3d11Desc.FrontFace.StencilFunc = Direct3D11::Map(desc.FrontFace.Func);
	d3d11Desc.FrontFace.StencilDepthFailOp = Direct3D11::Map(desc.FrontFace.DepthFail);
	d3d11Desc.FrontFace.StencilFailOp = Direct3D11::Map(desc.FrontFace.Fail);
	d3d11Desc.FrontFace.StencilPassOp = Direct3D11::Map(desc.FrontFace.Pass);
	d3d11Desc.DepthFunc = Direct3D11::Map(desc.DepthFunction);
	d3d11Desc.DepthWriteMask = desc.DepthWriteEnabled ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO;
	d3d11Desc.DepthEnable = desc.DepthTestEnabled;
	d3d11Desc.StencilEnable = desc.StencilEnabled;
	d3d11Desc.StencilReadMask = desc.StencilReadMask;
	d3d11Desc.StencilWriteMask = desc.StencilWriteMask;

	Direct3D11::CheckResult(_device->CreateDepthStencilState(&d3d11Desc, &state->_state), "Failed to create depth stencil state.");
}

void GraphicsDevice::Free(DepthStencilState* state)
{
	UnsetState(&_boundDepthStencilState, state);
	Direct3D11::Release(state->_state);
}

void GraphicsDevice::Bind(const DepthStencilState* state)
{
	if (ChangeState(&_boundDepthStencilState, state))
		_context->OMSetDepthStencilState(state->_state, 0);
}

void GraphicsDevice::SetName(DepthStencilState* state, const char* name)
{
	Direct3D11::SetName(state->_state, name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// RasterizerState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
void GraphicsDevice::Initialize(RasterizerState* state, const RasterizerDesc& desc)
{
	D3D11_RASTERIZER_DESC d3d11Desc;
	d3d11Desc.CullMode = Direct3D11::Map(desc.RasterCullMode);
	d3d11Desc.FillMode = Direct3D11::Map(desc.RasterFillMode);
	d3d11Desc.DepthBias = desc.DepthBias;
	d3d11Desc.DepthBiasClamp = desc.DepthBiasClamp;
	d3d11Desc.AntialiasedLineEnable = desc.AntialiasedLineEnabled;
	d3d11Desc.DepthClipEnable = desc.DepthClipEnabled;
	d3d11Desc.FrontCounterClockwise = desc.FrontIsCounterClockwise;
	d3d11Desc.MultisampleEnable = desc.MultisampleEnabled;
	d3d11Desc.ScissorEnable = desc.ScissorEnabled;
	d3d11Desc.SlopeScaledDepthBias = desc.SlopeScaledDepthBias;

	Direct3D11::CheckResult(_device->CreateRasterizerState(&d3d11Desc, &state->_state), "Failed to create rasterizer state.");
}

void GraphicsDevice::Free(RasterizerState* state)
{
	UnsetState(&_boundRasterizerState, state);
	Direct3D11::Release(state->_state);
}

void GraphicsDevice::Bind(const RasterizerState* state)
{
	if (ChangeState(&_boundRasterizerState, state))
		_context->RSSetState(state->_state);
}

void GraphicsDevice::SetName(RasterizerState* state, const char* name)
{
	Direct3D11::SetName(state->_state, name);
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// SamplerState
//-------------------------------------------------------------------------------------------------------------------------------------------------------
void GraphicsDevice::Initialize(SamplerState* state, const SamplerDesc& desc)
{
	D3D11_SAMPLER_DESC d3d11Desc;
	d3d11Desc.AddressU = Direct3D11::Map(desc.AddressU);
	d3d11Desc.AddressV = Direct3D11::Map(desc.AddressV);
	d3d11Desc.AddressW = Direct3D11::Map(desc.AddressW);
	d3d11Desc.BorderColor[0] = desc.BorderColor.Red;
	d3d11Desc.BorderColor[1] = desc.BorderColor.Green;
	d3d11Desc.BorderColor[2] = desc.BorderColor.Blue;
	d3d11Desc.BorderColor[3] = desc.BorderColor.Alpha;
	d3d11Desc.ComparisonFunc = Direct3D11::Map(desc.TextureComparison);
	d3d11Desc.Filter = Direct3D11::Map(desc.Filter);
	d3d11Desc.MaxAnisotropy = desc.MaximumAnisotropy;
	d3d11Desc.MaxLOD = desc.MaximumLod;
	d3d11Desc.MinLOD = desc.MinimumLod;
	d3d11Desc.MipLODBias = desc.MipLodBias;

	if (desc.Filter == TextureFilter::NearestNoMipmaps || desc.Filter == TextureFilter::BilinearNoMipmaps)
	{
		if (d3d11Desc.MaxLOD == Float32::MaxValue)
			d3d11Desc.MaxLOD = 0;
		if (d3d11Desc.MinLOD == Float32::MinValue)
			d3d11Desc.MinLOD = 0;
	}

	Direct3D11::CheckResult(_device->CreateSamplerState(&d3d11Desc, &state->_state), "Failed to create sampler state.");
}

void GraphicsDevice::Free(SamplerState* state)
{
	for (auto i = 0; i < Graphics::TextureSlotCount; ++i)
		UnsetState(&_boundSamplerStates[i], state);

	Direct3D11::Release(state->_state);
}

void GraphicsDevice::Bind(const SamplerState* state, uint32 slot)
{
	if (!ChangeState(&_boundSamplerStates[slot], state))
		return;

	_context->VSSetSamplers(slot, 1, &state->_state);
	_context->PSSetSamplers(slot, 1, &state->_state);
}

void GraphicsDevice::SetName(SamplerState* state, const char* name)
{
	Direct3D11::SetName(state->_state, name);
}

#endif