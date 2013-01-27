#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Blend state core functions
//====================================================================================================================

pgVoid pgCreateBlendStateCore(pgBlendState* blendState, pgBlendDesc* description)
{
	D3D11_BLEND_DESC desc;
	desc.AlphaToCoverageEnable = PG_FALSE;
	desc.IndependentBlendEnable = PG_FALSE;
	desc.RenderTarget[0].BlendEnable = description->blendEnabled;
	desc.RenderTarget[0].BlendOp = pgConvertBlendOperation(description->blendOperation);
	desc.RenderTarget[0].SrcBlend = pgConvertBlendOption(description->sourceBlend);
	desc.RenderTarget[0].DestBlend = pgConvertBlendOption(description->destinationBlend);
	desc.RenderTarget[0].BlendOpAlpha = pgConvertBlendOperation(description->blendOperationAlpha);
	desc.RenderTarget[0].SrcBlendAlpha = pgConvertBlendOption(description->sourceBlendAlpha);
	desc.RenderTarget[0].DestBlendAlpha = pgConvertBlendOption(description->destinationBlendAlpha);
	desc.RenderTarget[0].RenderTargetWriteMask = pgConvertColorWriteChannels(description->writeMask);

	D3DCALL(ID3D11Device_CreateBlendState(DEVICE(blendState), &desc, &blendState->ptr), "Failed to create blend state.");
}

pgVoid pgDestroyBlendStateCore(pgBlendState* blendState)
{
	ID3D11BlendState_Release(blendState->ptr);
}

pgVoid pgBindBlendStateCore(pgBlendState* blendState)
{
	ID3D11DeviceContext_OMSetBlendState(CONTEXT(blendState), blendState->ptr, NULL, INT32_MAX);
}

//====================================================================================================================
// Rasterizer state core functions
//====================================================================================================================

pgVoid pgCreateRasterizerStateCore(pgRasterizerState* rasterizerState, pgRasterizerDesc* description)
{
	D3D11_RASTERIZER_DESC desc;
	desc.CullMode = pgConvertCullMode(description->cullMode);
	desc.FillMode = pgConvertFillMode(description->fillMode);
	desc.DepthBias = description->depthBias;
	desc.DepthBiasClamp = description->depthBiasClamp;
	desc.AntialiasedLineEnable = description->antialiasedLineEnabled;
	desc.DepthClipEnable = description->depthClipEnabled;
	desc.FrontCounterClockwise = description->frontIsCounterClockwise;
	desc.MultisampleEnable = description->multisampleEnabled;
	desc.ScissorEnable = description->scissorEnabled;
	desc.SlopeScaledDepthBias = description->slopeScaledDepthBias;

	D3DCALL(ID3D11Device_CreateRasterizerState(DEVICE(rasterizerState), &desc, &rasterizerState->ptr), 
		"Failed to create rasterizer state.");
}

pgVoid pgDestroyRasterizerStateCore(pgRasterizerState* rasterizerState)
{
	ID3D11RasterizerState_Release(rasterizerState->ptr);
}

pgVoid pgBindRasterizerStateCore(pgRasterizerState* rasterizerState)
{
	ID3D11DeviceContext_RSSetState(CONTEXT(rasterizerState), rasterizerState->ptr);
}

//====================================================================================================================
// Depth stencil state core functions
//====================================================================================================================

pgVoid pgCreateDepthStencilStateCore(pgDepthStencilState* depthStencilState, pgDepthStencilDesc* description)
{
	D3D11_DEPTH_STENCIL_DESC desc;
	desc.BackFace.StencilFunc = pgConvertComparison(description->backFace.func);
	desc.BackFace.StencilDepthFailOp = pgConvertStencilOperation(description->backFace.depthFail);
	desc.BackFace.StencilFailOp = pgConvertStencilOperation(description->backFace.fail);
	desc.BackFace.StencilPassOp = pgConvertStencilOperation(description->backFace.pass);
	desc.FrontFace.StencilFunc = pgConvertComparison(description->frontFace.func);
	desc.FrontFace.StencilDepthFailOp = pgConvertStencilOperation(description->frontFace.depthFail);
	desc.FrontFace.StencilFailOp = pgConvertStencilOperation(description->frontFace.fail);
	desc.FrontFace.StencilPassOp = pgConvertStencilOperation(description->frontFace.pass);
	desc.DepthFunc = pgConvertComparison(description->depthFunction);
	desc.DepthWriteMask = description->depthWriteEnabled ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO;
	desc.DepthEnable = description->depthEnabled;
	desc.StencilEnable = description->stencilEnabled;
	desc.StencilReadMask = description->stencilReadMask;
	desc.StencilWriteMask = description->stencilWriteMask;

	D3DCALL(ID3D11Device_CreateDepthStencilState(DEVICE(depthStencilState), &desc, &depthStencilState->ptr), 
		"Failed to create depth stencil state.");
}

pgVoid pgDestroyDepthStencilStateCore(pgDepthStencilState* depthStencilState)
{
	ID3D11DepthStencilState_Release(depthStencilState->ptr);
}

pgVoid pgBindDepthStencilStateCore(pgDepthStencilState* depthStencilState)
{
	ID3D11DeviceContext_OMSetDepthStencilState(CONTEXT(depthStencilState), depthStencilState->ptr, 0);
}

//====================================================================================================================
// Sampler state core functions
//====================================================================================================================

pgVoid pgCreateSamplerStateCore(pgSamplerState* samplerState, pgSamplerDesc* description)
{
	D3D11_SAMPLER_DESC desc;
	desc.AddressU = pgConvertTextureAddressMode(description->addressU);
	desc.AddressV = pgConvertTextureAddressMode(description->addressV);
	desc.AddressW = pgConvertTextureAddressMode(description->addressW);
	desc.BorderColor[0] = description->borderColor.red;
	desc.BorderColor[1] = description->borderColor.green;
	desc.BorderColor[2] = description->borderColor.blue;
	desc.BorderColor[3] = description->borderColor.alpha;
	desc.ComparisonFunc = pgConvertComparison(description->comparison);
	desc.Filter = pgConvertTextureFilter(description->filter);
	desc.MaxAnisotropy = description->maximumAnisotropy;
	desc.MaxLOD = description->maximumLod;
	desc.MinLOD = description->minimumLod;
	desc.MipLODBias = description->mipLodBias;

	D3DCALL(ID3D11Device_CreateSamplerState(DEVICE(samplerState), &desc, &samplerState->ptr), "Failed to create sampler state.");
}

pgVoid pgDestroySamplerStateCore(pgSamplerState* samplerState)
{
	ID3D11SamplerState_Release(samplerState->ptr);
}

pgVoid pgBindSamplerStateCore(pgSamplerState* samplerState, pgInt32 slot)
{
	ID3D11DeviceContext_VSSetSamplers(CONTEXT(samplerState), slot, 1, &samplerState->ptr);
	ID3D11DeviceContext_PSSetSamplers(CONTEXT(samplerState), slot, 1, &samplerState->ptr);
}

#endif