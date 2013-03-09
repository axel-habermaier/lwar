#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgInitTextureDesc2D(pgTexture* texture, D3D11_TEXTURE2D_DESC* desc);
static D3D11_SUBRESOURCE_DATA* pgInitResourceData(pgTexture* texture, pgSurface* surface);
static pgVoid pgCreateTexture1D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);
static pgVoid pgCreateTexture2D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);
static pgVoid pgCreateTexture3D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);
static pgVoid pgCreateCubeMap(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data);

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateTextureCore(pgTexture* texture, pgSurface* surfaces)
{
	D3D11_SUBRESOURCE_DATA* data = pgInitResourceData(texture, surfaces);

	switch (texture->desc.type)
	{
	case PG_TEXTURE_1D:
		pgCreateTexture1D(texture, data);
		break;
	case PG_TEXTURE_2D:
		pgCreateTexture2D(texture, data);
		break;
	case PG_TEXTURE_CUBE_MAP:
		pgCreateCubeMap(texture, data);
		break;
	case PG_TEXTURE_3D:
		pgCreateTexture3D(texture, data);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_FREE(data);
}

pgVoid pgDestroyTextureCore(pgTexture* texture)
{
	PG_SAFE_RELEASE(ID3D11ShaderResourceView, texture->resourceView);
	PG_SAFE_RELEASE(ID3D11Texture2D, texture->ptr);
}

pgVoid pgBindTextureCore(pgTexture* texture, pgInt32 slot)
{
	ID3D11DeviceContext_VSSetShaderResources(PG_CONTEXT(texture), slot, 1, &texture->resourceView);
	ID3D11DeviceContext_PSSetShaderResources(PG_CONTEXT(texture), slot, 1, &texture->resourceView);
}

pgVoid pgGenerateMipmapsCore(pgTexture* texture)
{
	ID3D11DeviceContext_GenerateMips(PG_CONTEXT(texture), texture->resourceView);
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgInitTextureDesc2D(pgTexture* texture, D3D11_TEXTURE2D_DESC* desc)
{
	desc->Width = texture->desc.width;
	desc->Height = texture->desc.height;
	desc->MipLevels = texture->desc.mipmaps;
	desc->ArraySize = texture->desc.arraySize;
	desc->SampleDesc.Count = 1;
	desc->SampleDesc.Quality = 0;
	desc->Usage = D3D11_USAGE_DEFAULT;
	desc->BindFlags = D3D11_BIND_SHADER_RESOURCE;
	desc->CPUAccessFlags = 0;
	desc->MiscFlags = 0;
	desc->Format = pgConvertSurfaceFormat(texture->desc.format);

	if (texture->desc.flags & PG_TEXTURE_BIND_RENDER_TARGET)
		desc->BindFlags |= D3D11_BIND_RENDER_TARGET;

	if (texture->desc.flags & PG_TEXTURE_BIND_DEPTH_STENCIL)
		desc->BindFlags = D3D11_BIND_DEPTH_STENCIL;

	if (texture->desc.flags & PG_TEXTURE_GENERATE_MIPMAPS)
	{
		desc->MiscFlags |= D3D11_RESOURCE_MISC_GENERATE_MIPS;
		desc->BindFlags |= D3D11_BIND_RENDER_TARGET;
	}
}

static D3D11_SUBRESOURCE_DATA* pgInitResourceData(pgTexture* texture, pgSurface* surfaces)
{
	pgUint32 i;
	D3D11_SUBRESOURCE_DATA* data = NULL;

	if (texture->desc.flags & PG_TEXTURE_GENERATE_MIPMAPS)
		texture->desc.mipmaps = 0; // Autogenerate all mipmap levels

	if (surfaces == NULL)
		return NULL;

	PG_ALLOC_ARRAY(D3D11_SUBRESOURCE_DATA, texture->desc.surfaceCount, data);

	if (texture->desc.type == PG_TEXTURE_CUBE_MAP)
	{
		pgUint32 j;
		pgUint32 faces[] = { 5, 1, 4, 0, 3, 2 };

		for (i = 0; i < 6; ++i)
		{
			for (j = 0; j < texture->desc.mipmaps; ++j)
			{
				int d3dIndex = faces[i] * texture->desc.mipmaps + j;
				int pgIndex = i * texture->desc.mipmaps + j;
				data[d3dIndex].pSysMem = surfaces[pgIndex].data;
				data[d3dIndex].SysMemPitch = surfaces[pgIndex].stride;
				data[d3dIndex].SysMemSlicePitch = surfaces[pgIndex].size;
			}
		}
	}
	else
	{
		for (i = 0; i < texture->desc.surfaceCount; ++i)
		{
			data[i].pSysMem = surfaces[i].data;
			data[i].SysMemPitch = surfaces[i].stride;
			data[i].SysMemSlicePitch = surfaces[i].size;
		}
	}

	return data;
}

static pgVoid pgCreateTexture1D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	PG_UNUSED(texture);
	PG_UNUSED(data);
}

static pgVoid pgCreateTexture2D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;

	pgInitTextureDesc2D(texture, &desc);
	PG_D3DCALL(ID3D11Device_CreateTexture2D(PG_DEVICE(texture), &desc, data, &texture->ptr), "Failed to create texture.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	viewDesc.Texture2D.MipLevels = (UINT)-1;
	viewDesc.Texture2D.MostDetailedMip = 0;

	if (texture->desc.flags & PG_TEXTURE_BIND_DEPTH_STENCIL)
	{
		texture->resourceView = NULL;
		return;
	}
	
	PG_D3DCALL(ID3D11Device_CreateShaderResourceView(PG_DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for texture.");
}

static pgVoid pgCreateCubeMap(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	D3D11_TEXTURE2D_DESC desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;

	pgInitTextureDesc2D(texture, &desc);
	desc.ArraySize *= 6;
	desc.MiscFlags |= D3D11_RESOURCE_MISC_TEXTURECUBE;
	
	PG_D3DCALL(ID3D11Device_CreateTexture2D(PG_DEVICE(texture), &desc, data, &texture->ptr), "Failed to create cube map.");

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
	viewDesc.TextureCube.MipLevels = (UINT)-1;
	viewDesc.TextureCube.MostDetailedMip = 0;
	
	PG_D3DCALL(ID3D11Device_CreateShaderResourceView(PG_DEVICE(texture), (ID3D11Resource*)texture->ptr, &viewDesc, &texture->resourceView), 
		"Failed to create shader resource view for cube map.");
}

static pgVoid pgCreateTexture3D(pgTexture* texture, D3D11_SUBRESOURCE_DATA* data)
{
	PG_UNUSED(texture);
	PG_UNUSED(data);
}

#endif