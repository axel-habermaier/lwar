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
	pgInt32 componentCount, i;
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	D3D11_SUBRESOURCE_DATA initialData[PG_MAX_MIPMAPS];
	pgMipmap mipmaps[PG_MAX_MIPMAPS];
	pgInt32 numMipmaps;

	desc.Width = texture->desc.width;
	desc.Height = texture->desc.height;
	desc.MipLevels = 0;
	desc.ArraySize = 1;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = 0;
	
	pgConvertSurfaceFormat(surfaceFormat, &desc.Format, &componentCount);
	numMipmaps = pgMipmaps(data, texture->desc.width, texture->desc.height, componentCount, mipmaps);
	for (i = 0; i < numMipmaps; ++i)
	{
		initialData[i].pSysMem = mipmaps[i].data;
		initialData[i].SysMemPitch = mipmaps[i].width * componentCount;
		initialData[i].SysMemSlicePitch = 0;
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
	pgInt32 componentCount, i, j;
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	D3D11_SUBRESOURCE_DATA initialData[6 * PG_MAX_MIPMAPS];
	int faces[] = { 5, 1, 4, 0, 3, 2 };  // Maps the Pegasus cubemap order to D3D11 order 

	desc.Width = texture->desc.width;
	desc.Height = texture->desc.height;
	desc.MipLevels = 0;
	desc.ArraySize = 6;
	desc.SampleDesc.Count = 1;
	desc.SampleDesc.Quality = 0;
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = D3D11_RESOURCE_MISC_TEXTURECUBE;
	
	pgConvertSurfaceFormat(surfaceFormat, &desc.Format, &componentCount);
	for (i = 0; i < 6; ++i)
	{
		pgMipmap mipmaps[PG_MAX_MIPMAPS];
		pgInt32 numMipmaps = pgMipmaps(data, texture->desc.width, texture->desc.height, componentCount, mipmaps);
		pgMipmap* last = &mipmaps[numMipmaps - 1];
		data = (pgUint8*)last->data + last->width * last->height * componentCount;

		for (j = 0; j < numMipmaps; ++j)
		{
			pgInt32 index = faces[i] * numMipmaps + j;
			initialData[index].pSysMem = mipmaps[j].data;
			initialData[index].SysMemPitch = mipmaps[j].width * componentCount;
			initialData[index].SysMemSlicePitch = 0;
		}
	}

	D3DCALL(ID3D11Device_CreateTexture2D(DEVICE(texture), &desc, initialData, &texture->ptr), "Failed to create cube map.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
	viewDesc.TextureCube.MipLevels = (UINT)-1;
	viewDesc.TextureCube.MostDetailedMip = 0;
	
	D3DCALL(ID3D11Device_CreateShaderResourceView(DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for cube map.");
}

#endif