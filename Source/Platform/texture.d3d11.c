#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid CreateTexture2D(pgTexture* texture, pgVoid* data, pgSurfaceFormat surfaceFormat);
static pgVoid CreateCubeMap(pgTexture* texture, pgVoid* data, pgSurfaceFormat surfaceFormat);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgVoid* data)
{
	switch (texture->desc.type)
	{
	case PG_TEXTURE_2D:
		CreateTexture2D(texture, data, texture->desc.format);
		break;
	case PG_TEXTURE_CUBE_MAP:
		CreateCubeMap(texture, data, texture->desc.format);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

pgVoid pgDestroyTextureCore(pgTexture* texture)
{
	ID3D11ShaderResourceView_Release(texture->resourceView);
	ID3D11Texture2D_Release(texture->ptr);
}

pgVoid pgBindTextureCore(pgTexture* texture, pgInt32 slot)
{
	ID3D11DeviceContext_VSSetShaderResources(CONTEXT(texture), slot, 1, &texture->resourceView);
	ID3D11DeviceContext_PSSetShaderResources(CONTEXT(texture), slot, 1, &texture->resourceView);
}

pgVoid pgGenerateMipmapsCore(pgTexture* texture)
{
	ID3D11DeviceContext_GenerateMips(CONTEXT(texture), texture->resourceView);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid CreateTexture2D(pgTexture* texture, pgVoid* data, pgSurfaceFormat surfaceFormat)
{
	pgInt32 length, width, height;
	pgUint32 i;
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	D3D11_SUBRESOURCE_DATA initialData[32];
	pgUint8* dataPtr = (pgUint8*)data;

	desc.Width = texture->desc.width;
	desc.Height = texture->desc.height;
	desc.MipLevels = pgMipmapCount(texture->desc.width, texture->desc.height);
	desc.ArraySize = 1;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = 0;
	
	pgConvertSurfaceFormat(surfaceFormat, &desc.Format, &length);

	PG_ASSERT_IN_RANGE(desc.MipLevels + 1, 0, sizeof(initialData) / sizeof(D3D11_SUBRESOURCE_DATA));
	width = texture->desc.width;
	height = texture->desc.height;
	for (i = 0; i < desc.MipLevels + 1; ++i)
	{
		initialData[i].pSysMem = dataPtr;
		initialData[i].SysMemPitch = width * length;
		initialData[i].SysMemSlicePitch = 0;
		dataPtr += initialData[i].SysMemPitch * height;
		width /= 2;
		height /= 2;

		width = width < 1 ? 1 : width;
		height = height < 1 ? 1 : height;
	}

	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(texture), &desc, initialData, &texture->ptr), "Failed to create texture.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	viewDesc.Texture2D.MipLevels = (UINT)-1;
	viewDesc.Texture2D.MostDetailedMip = 0;
	
	D3DCALL(ID3D11Device_CreateShaderResourceView(DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for texture.");
}

static pgVoid CreateCubeMap(pgTexture* texture, pgVoid* data, pgSurfaceFormat surfaceFormat)
{
	pgInt32 length, i, width;
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	D3D11_SUBRESOURCE_DATA initialData[6];
	int faces[] = { 5, 1, 4, 0, 3, 2 };  // Maps the Pegasus cubemap order to D3D11 order 

	width = texture->desc.width / 6;
	desc.Width = width;
	desc.Height = texture->desc.height;
	desc.MipLevels = 1;//pgMipmapCount(width, texture->desc.height);
	desc.ArraySize = 6;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = D3D11_RESOURCE_MISC_TEXTURECUBE;
	
	pgConvertSurfaceFormat(surfaceFormat, &desc.Format, &length);

	for (i = 0; i < 6; ++i)
	{
		initialData[faces[i]].pSysMem = (pgInt8*)data + i * (width * length);
		initialData[faces[i]].SysMemPitch = width * length * 6;
		initialData[faces[i]].SysMemSlicePitch = 0;
	}

	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(texture), &desc, initialData, &texture->ptr), "Failed to create cube map.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
	viewDesc.TextureCube.MipLevels = desc.MipLevels;
	viewDesc.TextureCube.MostDetailedMip = desc.MipLevels - 1;
	
	D3DCALL(ID3D11Device_CreateShaderResourceView(DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for cube map.");
}

#endif