#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions
//====================================================================================================================

static D3D11_SUBRESOURCE_DATA* InitResourceData(pgTexture* texture, pgSurface* surface);
static pgVoid CreateTexture1D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);
static pgVoid CreateTexture2D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);
static pgVoid CreateTexture3D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);
static pgVoid CreateCubeMap(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgSurface* surfaces)
{
	D3D11_SUBRESOURCE_DATA* data = InitResourceData(texture, surfaces);

	switch (texture->desc.type)
	{
	case PG_TEXTURE_1D:
		CreateTexture1D(texture, data);
		break;
	case PG_TEXTURE_2D:
		CreateTexture2D(texture, data);
		break;
	case PG_TEXTURE_CUBE_MAP:
		CreateCubeMap(texture, data);
		break;
	case PG_TEXTURE_3D:
		CreateTexture3D(texture, data);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_FREE(data);
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

static D3D11_SUBRESOURCE_DATA* InitResourceData(pgTexture* texture, pgSurface* surfaces)
{
	pgUint32 i;
	D3D11_SUBRESOURCE_DATA* data = NULL;
	PG_ALLOC_ARRAY(D3D11_SUBRESOURCE_DATA, texture->desc.surfaceCount, data);

	for (i = 0; i < texture->desc.surfaceCount; ++i)
	{
		data[i].pSysMem = surfaces[i].data;
		data[i].SysMemPitch = surfaces[i].stride;
		data[i].SysMemSlicePitch = 0;//surfaces[i].size;
	}

	return data;
}

static pgVoid CreateTexture1D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	PG_UNUSED(texture);
	PG_UNUSED(data);

	pgDie("1D textures are currently not supported.");
}

static pgVoid CreateTexture2D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;

	desc.Width = texture->desc.width;
	desc.Height = texture->desc.height;
	desc.MipLevels = 0;
	desc.ArraySize = texture->desc.arraySize;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = 0;
	desc.Format = pgConvertSurfaceFormat(texture->desc.format);

	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(texture), &desc, data, &texture->ptr), "Failed to create texture.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	viewDesc.Texture2D.MipLevels = (UINT)-1;
	viewDesc.Texture2D.MostDetailedMip = 0;
	
	D3DCALL(ID3D11Device_CreateShaderResourceView(DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for texture.");
}

static pgVoid CreateCubeMap(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	//int faces[] = { 5, 1, 4, 0, 3, 2 };  // Maps the Pegasus cubemap order to D3D11 order 

	desc.Width = texture->desc.width;
	desc.Height = texture->desc.height;
	desc.MipLevels = 0;
	desc.ArraySize = 6 * texture->desc.arraySize;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = D3D11_RESOURCE_MISC_TEXTURECUBE;
	desc.Format = pgConvertSurfaceFormat(texture->desc.format);
	
	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(texture), &desc, data, &texture->ptr), "Failed to create cube map.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
	viewDesc.TextureCube.MipLevels = (UINT)-1;
	viewDesc.TextureCube.MostDetailedMip = 0;
	
	D3DCALL(ID3D11Device_CreateShaderResourceView(DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for cube map.");
}

static pgVoid CreateTexture3D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	PG_UNUSED(texture);
	PG_UNUSED(data);

	pgDie("3D textures are currently not supported.");
}

#endif