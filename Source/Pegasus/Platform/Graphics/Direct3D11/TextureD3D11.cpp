#include "Prelude.hpp"

using namespace Direct3D11;

Texture* GraphicsDevice::InitializeTexture(TextureDescription* desc, Surface* surfaces)
{
	auto texture = PG_NEW(Texture);

	switch (desc->Type)
	{
	case TextureType::Texture1D:
		CreateTexture1D(texture, desc, surfaces);
		break;
	case TextureType::Texture2D:
		CreateTexture2D(texture, desc, surfaces);
		break;
	case TextureType::CubeMap:
		CreateCubeMap(texture, desc, surfaces);
		break;
	case TextureType::Texture3D:
		CreateTexture3D(texture, desc, surfaces);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	return texture;
}

void GraphicsDevice::FreeTexture(Texture* texture)
{
	PG_DELETE(texture);
}

void GraphicsDevice::BindTexture(Texture* texture, int32 slot)
{
	_context->VSSetShaderResources(safe_static_cast<UINT>(slot), 1, texture->View.GetAddressOf());
	_context->PSSetShaderResources(safe_static_cast<UINT>(slot), 1, texture->View.GetAddressOf());
}

void GraphicsDevice::UnbindTexture(Texture* texture, int32 slot)
{
	PG_UNUSED(texture);

	ID3D11ShaderResourceView* resourceView[] = { nullptr };
	_context->VSSetShaderResources(safe_static_cast<UINT>(slot), 1, resourceView);
	_context->PSSetShaderResources(safe_static_cast<UINT>(slot), 1, resourceView);
}

void GraphicsDevice::GenerateMipmaps(Texture* texture)
{
	_context->GenerateMips(texture->View.Get());
}

void GraphicsDevice::SetTextureName(Texture* texture, const char* name)
{
	SetName(texture->Obj, name);
	SetName(texture->View, name);
}

void GraphicsDevice::CreateTexture1D(Texture* texture, const TextureDescription* desc, const Surface* surfaces)
{
	PG_UNUSED(texture);
	PG_UNUSED(desc);
	PG_UNUSED(surfaces);

	PG_DIE("Not implemented.");
}

void GraphicsDevice::CreateTexture2D(Texture* texture, const TextureDescription* desc, const Surface* surfaces)
{
	D3D11_SUBRESOURCE_DATA data[Graphics::MaxSurfaceCount];
	InitializeResourceData(data, desc, surfaces);

	D3D11_TEXTURE2D_DESC d3d11Desc;
	InitializeDesc2D(desc, d3d11Desc);

	ID3D11Texture2D* tex2D;
	CheckResult(_device->CreateTexture2D(&d3d11Desc, surfaces == nullptr ? nullptr : data, &tex2D), "Failed to create texture.");
	texture->Obj.Attach(tex2D);

	if (Enum::HasFlag(desc->Flags, TextureFlags::DepthStencil))
		return;

	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	Memory::Set(&viewDesc, 0);
	viewDesc.Format = d3d11Desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	viewDesc.Texture2D.MipLevels = static_cast<UINT>(-1);
	viewDesc.Texture2D.MostDetailedMip = 0;

	CheckResult(
		_device->CreateShaderResourceView(texture->Obj.Get(), &viewDesc, &texture->View),
		"Failed to create shader resource view for texture.");
}

void GraphicsDevice::CreateTexture3D(Texture* texture, const TextureDescription* desc, const Surface* surfaces)
{
	PG_UNUSED(texture);
	PG_UNUSED(desc);
	PG_UNUSED(surfaces);

	PG_DIE("Not implemented.");
}

void GraphicsDevice::CreateCubeMap(Texture* texture, const TextureDescription* desc, const Surface* surfaces)
{
	D3D11_SUBRESOURCE_DATA data[Graphics::MaxSurfaceCount];
	InitializeResourceData(data, desc, surfaces);

	D3D11_TEXTURE2D_DESC d3d11Desc;
	InitializeDesc2D(desc, d3d11Desc);
	d3d11Desc.ArraySize *= 6;
	d3d11Desc.MiscFlags |= D3D11_RESOURCE_MISC_TEXTURECUBE;

	ID3D11Texture2D* cubeMap;
	CheckResult(_device->CreateTexture2D(&d3d11Desc, surfaces == nullptr ? nullptr : data, &cubeMap), "Failed to create cube map.");
	texture->Obj.Attach(cubeMap);

	if (Enum::HasFlag(desc->Flags, TextureFlags::DepthStencil))
		return;

	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;
	Memory::Set(&viewDesc, 0);
	viewDesc.Format = d3d11Desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
	viewDesc.TextureCube.MipLevels = static_cast<UINT>(-1);
	viewDesc.TextureCube.MostDetailedMip = 0;

	CheckResult(
		_device->CreateShaderResourceView(texture->Obj.Get(), &viewDesc, &texture->View),
		"Failed to create shader resource view for cube map.");
}

void GraphicsDevice::InitializeDesc2D(const TextureDescription* desc, D3D11_TEXTURE2D_DESC& d3d11Desc)
{
	Memory::Set(&d3d11Desc, 0);
	d3d11Desc.Width = safe_static_cast<UINT>(desc->Width);
	d3d11Desc.Height = safe_static_cast<UINT>(desc->Height);
	d3d11Desc.MipLevels = safe_static_cast<UINT>(desc->Mipmaps);
	d3d11Desc.ArraySize = 1;
	d3d11Desc.SampleDesc.Count = 1;
	d3d11Desc.SampleDesc.Quality = 0;
	d3d11Desc.Usage = D3D11_USAGE_DEFAULT;
	d3d11Desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	d3d11Desc.CPUAccessFlags = 0;
	d3d11Desc.MiscFlags = 0;
	d3d11Desc.Format = Map(desc->Format);

	if (Enum::HasFlag(desc->Flags, TextureFlags::RenderTarget))
		d3d11Desc.BindFlags |= D3D11_BIND_RENDER_TARGET;

	if (Enum::HasFlag(desc->Flags, TextureFlags::DepthStencil))
		d3d11Desc.BindFlags = D3D11_BIND_DEPTH_STENCIL;

	if (Enum::HasFlag(desc->Flags, TextureFlags::GenerateMipmaps))
	{
		d3d11Desc.MiscFlags |= D3D11_RESOURCE_MISC_GENERATE_MIPS;
		d3d11Desc.BindFlags |= D3D11_BIND_RENDER_TARGET;
		d3d11Desc.MipLevels = 0;
	}
}

void GraphicsDevice::InitializeResourceData(D3D11_SUBRESOURCE_DATA* resourceData, const TextureDescription* desc, const Surface* surfaces)
{
	if (surfaces == nullptr)
		return;

	auto mipmaps = desc->Mipmaps;
	if (Enum::HasFlag(desc->Flags, TextureFlags::GenerateMipmaps))
		mipmaps = 0;

	if (desc->Type == TextureType::CubeMap)
	{
		int32 faces[] = { 5, 1, 4, 0, 3, 2 };

		for (auto i = 0; i < 6; ++i)
		{
			for (auto j = 0; j < mipmaps; ++j)
			{
				auto d3dIndex = faces[i] * mipmaps + j;
				auto pgIndex = i * mipmaps + j;
				resourceData[d3dIndex].pSysMem = surfaces[pgIndex].Data;
				resourceData[d3dIndex].SysMemPitch = safe_static_cast<UINT>(surfaces[pgIndex].Stride);
				resourceData[d3dIndex].SysMemSlicePitch = safe_static_cast<UINT>(surfaces[pgIndex].SizeInBytes);
			}
		}
	}
	else
	{
		for (auto i = 0; i < mipmaps; ++i)
		{
			resourceData[i].pSysMem = surfaces[i].Data;
			resourceData[i].SysMemPitch = safe_static_cast<UINT>(surfaces[i].Stride);
			resourceData[i].SysMemSlicePitch = safe_static_cast<UINT>(surfaces[i].SizeInBytes);
		}
	}
}
