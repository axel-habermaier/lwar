#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTexture2DCore(pgTexture2D* texture2D, pgVoid* data, pgSurfaceFormat surfaceFormat)
{
	pgInt32 length;
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	D3D11_SUBRESOURCE_DATA initialData;

	desc.Width = texture2D->width;
	desc.Height = texture2D->height;
	desc.MipLevels = 1;
	desc.ArraySize = 1;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = 0;
	
	pgConvertSurfaceFormat(surfaceFormat, &desc.Format, &length);

	initialData.pSysMem = data;
	initialData.SysMemPitch = texture2D->width * length;
	initialData.SysMemSlicePitch = 0;

	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(texture2D), &desc, &initialData, &texture2D->ptr), "Failed to create Texture2D.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	viewDesc.Texture2D.MipLevels = desc.MipLevels;
	viewDesc.Texture2D.MostDetailedMip = desc.MipLevels - 1;
	
	D3DCALL(ID3D11Device_CreateShaderResourceView(DEVICE(texture2D), (ID3D11Resource*)texture2D->ptr, &viewDesc, &texture2D->resourceView), 
		"Failed to create shader resource view for Texture2D.");
}

pgVoid pgDestroyTexture2DCore(pgTexture2D* texture2D)
{
	ID3D11ShaderResourceView_Release(texture2D->resourceView);
	ID3D11Texture2D_Release(texture2D->ptr);
}

pgVoid pgBindTextureCore(pgTexture2D* texture2D, pgInt32 slot)
{
	ID3D11DeviceContext_VSSetShaderResources(CONTEXT(texture2D), slot, 1, &texture2D->resourceView);
	ID3D11DeviceContext_PSSetShaderResources(CONTEXT(texture2D), slot, 1, &texture2D->resourceView);
}

#endif