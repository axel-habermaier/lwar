#include "Prelude.hpp"

using namespace OpenGL3;

Texture* GraphicsDevice::InitializeTexture(TextureDescription* desc, Surface* surfaces)
{
	auto texture = PG_NEW(Texture);
	texture->Obj = Allocate(&GraphicsDevice::glGenTextures, "Texture");
	texture->Format = Map(desc->Format);

	switch (desc->Type)
	{
	case TextureType::Texture2D:
	{
		texture->Type = GL_TEXTURE_2D;
		glBindTexture(texture->Type, texture->Obj);

		if (surfaces != nullptr)
		{
			for (auto i = 0; i < desc->Mipmaps; ++i)
				UploadTexture(surfaces[i], desc->Format, GL_TEXTURE_2D, i);
		}
		else
		{
			Surface surface = { 0 };
			surface.Width = desc->Width;
			surface.Height = desc->Height;

			UploadTexture(surface, desc->Format, GL_TEXTURE_2D, 0);
		}
		break;
	}
	case TextureType::CubeMap:
	{
		texture->Type = GL_TEXTURE_CUBE_MAP;
		glBindTexture(texture->Type, texture->Obj);

		uint32 faces[] =
		{
			GL_TEXTURE_CUBE_MAP_NEGATIVE_Z,
			GL_TEXTURE_CUBE_MAP_NEGATIVE_X,
			GL_TEXTURE_CUBE_MAP_POSITIVE_Z,
			GL_TEXTURE_CUBE_MAP_POSITIVE_X,
			GL_TEXTURE_CUBE_MAP_NEGATIVE_Y,
			GL_TEXTURE_CUBE_MAP_POSITIVE_Y
		};

		if (surfaces != nullptr)
		{
			for (int i = 0; i < 6; ++i)
			{
				for (auto j = 0; j < desc->Mipmaps; ++j)
				{
					auto index = i * desc->Mipmaps + j;
					UploadTexture(surfaces[index], desc->Format, faces[i], j);
				}
			}
		}
		else
		{
			Surface surface = { 0 };
			surface.Width = desc->Width;
			surface.Height = desc->Height;

			for (int i = 0; i < 6; ++i)
				UploadTexture(surface, desc->Format, faces[i], 0);
		}
		break;
	}
	case TextureType::Texture1D:
	case TextureType::Texture3D:
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	if (Enum::HasFlag(desc->Flags, TextureFlags::GenerateMipmaps))
		glGenerateMipmap(GL_TEXTURE_2D);

	RebindTexture();
	return texture;
}

void GraphicsDevice::FreeTexture(Texture* texture)
{
	if (texture == nullptr)
		return;

	glDeleteTextures(1, &texture->Obj);
	PG_DELETE(texture);
}

void GraphicsDevice::BindTexture(Texture* texture, int32 slot)
{
	ChangeActiveTexture(slot);
	glBindTexture(texture->Type, texture->Obj);

	_boundTexture = texture->Obj;
	_boundTextureType = texture->Type;
}

void GraphicsDevice::UnbindTexture(Texture* texture, int32 slot)
{
	ChangeActiveTexture(slot);
	glBindTexture(texture->Type, 0);

	_boundTexture = 0;
	_boundTextureType = texture->Type;
}

void GraphicsDevice::GenerateMipmaps(Texture* texture)
{
	glBindTexture(texture->Type, texture->Obj);
	glGenerateMipmap(texture->Type);

	RebindTexture();
}

void GraphicsDevice::UploadTexture(const Surface& surface, SurfaceFormat format, uint32 target, int32 level)
{
	auto internalFormat = GetInternalFormat(format);
	auto glFormat = Map(format);
	auto type = Graphics::IsFloatingPointFormat(format) ? GL_FLOAT : GL_UNSIGNED_BYTE;

	if (Graphics::IsDepthStencilFormat(format))
		type = GL_UNSIGNED_INT_24_8;

	if (Graphics::IsCompressedFormat(format))
		glCompressedTexImage2D(target, level, internalFormat, surface.Width, surface.Height, 0, surface.SizeInBytes, surface.Data);
	else
	{
		glTexImage2D(
			target, level, safe_static_cast<int32>(internalFormat), surface.Width,
			surface.Height, 0, glFormat, safe_static_cast<uint32>(type), surface.Data);
	}
}

void GraphicsDevice::RebindTexture()
{
	if (_boundTexture != 0)
		glBindTexture(_boundTextureType, _boundTexture);
}

void GraphicsDevice::SetTextureName(Texture* texture, const char* name)
{
	PG_UNUSED(texture);
	PG_UNUSED(name);
}