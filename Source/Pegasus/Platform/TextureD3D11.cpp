#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/Texture.hpp"
#include "Graphics/GraphicsDevice.hpp"
#include "Math/Math.hpp"
#include "Utilities/Enumeration.hpp"

static void InitializeDesc2D(const TextureDesc& desc, D3D11_TEXTURE2D_DESC& d3d11Desc);
static void InitializeResourceData(D3D11_SUBRESOURCE_DATA* resourceData, TextureType type, const TextureDesc& desc, const Surface* surfaces);

void GraphicsDevice::Initialize(Texture* texture, const Surface* surfaces)
{
	switch (texture->_type)
	{
	case TextureType::Texture1D:
		CreateTexture1D(texture, surfaces);
		break;
	case TextureType::Texture2D:
		CreateTexture2D(texture, surfaces);
		break;
	case TextureType::CubeMap:
		CreateCubeMap(texture, surfaces);
		break;
	case TextureType::Texture3D:
		CreateTexture3D(texture, surfaces);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

void GraphicsDevice::Free(Texture* texture)
{
	for (auto i = 0; i < Graphics::TextureSlotCount; ++i)
		UnsetState(&_boundTextures[i], texture);

	Direct3D11::Release(texture->_texture);
	Direct3D11::Release(texture->_view);
}

void GraphicsDevice::Bind(const Texture* texture, uint32 slot)
{
	if (!ChangeState(&_boundTextures[slot], texture))
		return;

	_context->VSSetShaderResources(slot, 1, &texture->_view);
	_context->PSSetShaderResources(slot, 1, &texture->_view);
}

void GraphicsDevice::Unbind(const Texture* texture, uint32 slot)
{
	PG_UNUSED(texture);

	if (!ChangeState(&_boundTextures[slot], static_cast<const Texture*>(nullptr)))
		return;

	ID3D11ShaderResourceView* resourceView[] = { nullptr };
	_context->VSSetShaderResources(slot, 1, resourceView);
	_context->PSSetShaderResources(slot, 1, resourceView);
}

void GraphicsDevice::GenerateMipmaps(Texture* texture)
{
	_context->GenerateMips(texture->_view);
}

void GraphicsDevice::SetName(Texture* texture, const char* name)
{
	Direct3D11::SetName(texture->_texture, name);

	if (texture->_view != nullptr)
		Direct3D11::SetName(texture->_view, name);
}

void GraphicsDevice::CreateTexture1D(Texture* texture, const Surface* surfaces)
{
	PG_UNUSED(texture);
	PG_UNUSED(surfaces);

	PG_DIE("Not implemented.");
}

void GraphicsDevice::CreateTexture2D(Texture* texture, const Surface* surfaces)
{
	D3D11_SUBRESOURCE_DATA data[Graphics::MaxSurfaceCount];
	InitializeResourceData(data, texture->_type, texture->_desc, surfaces);

	D3D11_TEXTURE2D_DESC d3d11Desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;

	InitializeDesc2D(texture->_desc, d3d11Desc);
	auto textureData = surfaces == nullptr ? nullptr : data;

	auto texturePtr = reinterpret_cast<ID3D11Texture2D**>(&texture->_texture);
	Direct3D11::CheckResult(_device->CreateTexture2D(&d3d11Desc, textureData, texturePtr), "Failed to create texture.");

	if ((texture->_desc.Flags & TextureFlags::BindDepthStencil) == TextureFlags::BindDepthStencil)
		return;

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = d3d11Desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	viewDesc.Texture2D.MipLevels = (UINT)-1;
	viewDesc.Texture2D.MostDetailedMip = 0;

	Direct3D11::CheckResult(_device->CreateShaderResourceView(texture->_texture, &viewDesc, &texture->_view),
							"Failed to create shader resource view for texture.");
}

void GraphicsDevice::CreateTexture3D(Texture* texture, const Surface* surfaces)
{
	PG_UNUSED(texture);
	PG_UNUSED(surfaces);

	PG_DIE("Not implemented.");
}

void GraphicsDevice::CreateCubeMap(Texture* texture, const Surface* surfaces)
{
	D3D11_SUBRESOURCE_DATA data[Graphics::MaxSurfaceCount];
	InitializeResourceData(data, texture->_type, texture->_desc, surfaces);

	D3D11_TEXTURE2D_DESC d3d11Desc;
	D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;

	auto textureData = surfaces == nullptr ? nullptr : data;

	InitializeDesc2D(texture->_desc, d3d11Desc);
	d3d11Desc.ArraySize *= 6;
	d3d11Desc.MiscFlags |= D3D11_RESOURCE_MISC_TEXTURECUBE;

	auto texturePtr = reinterpret_cast<ID3D11Texture2D**>(&texture->_texture);
	Direct3D11::CheckResult(_device->CreateTexture2D(&d3d11Desc, textureData, texturePtr), "Failed to create cube map.");

	if ((texture->_desc.Flags & TextureFlags::BindDepthStencil) == TextureFlags::BindDepthStencil)
		return;

	memset(&viewDesc, 0, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
	viewDesc.Format = d3d11Desc.Format;
	viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
	viewDesc.TextureCube.MipLevels = (UINT)-1;
	viewDesc.TextureCube.MostDetailedMip = 0;

	Direct3D11::CheckResult(_device->CreateShaderResourceView(texture->_texture, &viewDesc, &texture->_view),
							"Failed to create shader resource view for cube map.");
}

static void InitializeDesc2D(const TextureDesc& desc, D3D11_TEXTURE2D_DESC& d3d11Desc)
{
	Memory::Set(&d3d11Desc, 0);
	d3d11Desc.Width = Math::ToUInt32(desc.Width);
	d3d11Desc.Height = Math::ToUInt32(desc.Height);
	d3d11Desc.MipLevels = desc.MipmapCount;
	d3d11Desc.ArraySize = 1;
	d3d11Desc.SampleDesc.Count = 1;
	d3d11Desc.SampleDesc.Quality = 0;
	d3d11Desc.Usage = D3D11_USAGE_DEFAULT;
	d3d11Desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
	d3d11Desc.CPUAccessFlags = 0;
	d3d11Desc.MiscFlags = 0;
	d3d11Desc.Format = Direct3D11::Map(desc.Format);

	if ((desc.Flags & TextureFlags::BindRenderTarget) == TextureFlags::BindRenderTarget)
		d3d11Desc.BindFlags |= D3D11_BIND_RENDER_TARGET;

	if ((desc.Flags & TextureFlags::BindDepthStencil) == TextureFlags::BindDepthStencil)
		d3d11Desc.BindFlags = D3D11_BIND_DEPTH_STENCIL;

	if ((desc.Flags & TextureFlags::GenerateMipmaps) == TextureFlags::GenerateMipmaps)
	{
		d3d11Desc.MiscFlags |= D3D11_RESOURCE_MISC_GENERATE_MIPS;
		d3d11Desc.BindFlags |= D3D11_BIND_RENDER_TARGET;
		d3d11Desc.MipLevels = 0;
	}
}

static void InitializeResourceData(D3D11_SUBRESOURCE_DATA* resourceData, TextureType type, const TextureDesc& desc, const Surface* surfaces)
{
	auto mipmaps = desc.MipmapCount;
	if ((desc.Flags & TextureFlags::GenerateMipmaps) == TextureFlags::GenerateMipmaps)
		mipmaps = 0;

	if (surfaces == nullptr)
		return;

	if (type == TextureType::CubeMap)
	{
		uint32 faces[] = { 5, 1, 4, 0, 3, 2 };

		for (uint32 i = 0; i < 6; ++i)
		{
			for (uint32 j = 0; j < mipmaps; ++j)
			{
				auto d3dIndex = faces[i] * mipmaps + j;
				auto pgIndex = i * mipmaps + j;
				resourceData[d3dIndex].pSysMem = surfaces[pgIndex].Data;
				resourceData[d3dIndex].SysMemPitch = surfaces[pgIndex].Stride;
				resourceData[d3dIndex].SysMemSlicePitch = surfaces[pgIndex].SizeInBytes;
			}
		}
	}
	else
	{
		for (uint32 i = 0; i < mipmaps; ++i)
		{
			resourceData[i].pSysMem = surfaces[i].Data;
			resourceData[i].SysMemPitch = surfaces[i].Stride;
			resourceData[i].SysMemSlicePitch = surfaces[i].SizeInBytes;
		}
	}
}

#endif