#include "Prelude.hpp"

using namespace Direct3D11;;

void GraphicsDevice::SetName(const ComPtr<ID3D11DeviceChild>& obj, const char* name)
{
#ifdef DEBUG
	if (obj != nullptr && name != nullptr)
		obj->SetPrivateData(WKPDID_D3DDebugObjectName, safe_static_cast<UINT>(strlen(name) + 1), name);
#else
	PG_UNUSED(obj);
	PG_UNUSED(name);
#endif
}

void GraphicsDevice::CheckResult(HRESULT hr, const char* msg)
{
	if (FAILED(hr))
		PG_DIE("%s", Win32::GetError(msg, static_cast<DWORD>(hr)));
}

UINT GraphicsDevice::GetSemanticIndex(DataSemantics semantics)
{
	switch (semantics)
	{
	case DataSemantics::Position:
		return 0;
	case DataSemantics::Color0:
		return 0;
	case DataSemantics::Color1:
		return 1;
	case DataSemantics::Color2:
		return 2;
	case DataSemantics::Color3:
		return 3;
	case DataSemantics::TexCoords0:
		return 0;
	case DataSemantics::TexCoords1:
		return 1;
	case DataSemantics::TexCoords2:
		return 2;
	case DataSemantics::TexCoords3:
		return 3;
	case DataSemantics::Normal:
		return 0;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

LPCSTR GraphicsDevice::GetSemanticName(DataSemantics semantics)
{
	switch (semantics)
	{
	case DataSemantics::Position:
		return "POSITION";
	case DataSemantics::Color0:
		return "COLOR";
	case DataSemantics::Color1:
		return "COLOR";
	case DataSemantics::Color2:
		return "COLOR";
	case DataSemantics::Color3:
		return "COLOR";
	case DataSemantics::TexCoords0:
		return "TEXCOORD";
	case DataSemantics::TexCoords1:
		return "TEXCOORD";
	case DataSemantics::TexCoords2:
		return "TEXCOORD";
	case DataSemantics::TexCoords3:
		return "TEXCOORD";
	case DataSemantics::Normal:
		return "NORMAL";
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_BLEND_OP GraphicsDevice::Map(BlendOperation blendOperation)
{
	switch (blendOperation)
	{
	case BlendOperation::Add:
		return D3D11_BLEND_OP_ADD;
	case BlendOperation::Subtract:
		return D3D11_BLEND_OP_SUBTRACT;
	case BlendOperation::ReverseSubtract:
		return D3D11_BLEND_OP_REV_SUBTRACT;
	case BlendOperation::Minimum:
		return D3D11_BLEND_OP_MIN;
	case BlendOperation::Maximum:
		return D3D11_BLEND_OP_MAX;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_BLEND GraphicsDevice::Map(BlendOption blendOption)
{
	switch (blendOption)
	{
	case BlendOption::Zero:
		return D3D11_BLEND_ZERO;
	case BlendOption::One:
		return D3D11_BLEND_ONE;
	case BlendOption::SourceColor:
		return D3D11_BLEND_SRC_COLOR;
	case BlendOption::InverseSourceColor:
		return D3D11_BLEND_INV_SRC_COLOR;
	case BlendOption::SourceAlpha:
		return D3D11_BLEND_SRC_ALPHA;
	case BlendOption::InverseSourceAlpha:
		return D3D11_BLEND_INV_SRC_ALPHA;
	case BlendOption::DestinationAlpha:
		return D3D11_BLEND_DEST_ALPHA;
	case BlendOption::InverseDestinationAlpha:
		return D3D11_BLEND_INV_DEST_ALPHA;
	case BlendOption::DestinationColor:
		return D3D11_BLEND_DEST_COLOR;
	case BlendOption::InverseDestinationColor:
		return D3D11_BLEND_INV_DEST_COLOR;
	case BlendOption::SourceAlphaSaturate:
		return D3D11_BLEND_SRC_ALPHA_SAT;
	case BlendOption::BlendFactor:
		return D3D11_BLEND_BLEND_FACTOR;
	case BlendOption::InverseBlendFactor:
		return D3D11_BLEND_INV_BLEND_FACTOR;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_COMPARISON_FUNC GraphicsDevice::Map(Comparison comparison)
{
	switch (comparison)
	{
	case Comparison::Always:
		return D3D11_COMPARISON_ALWAYS;
	case Comparison::Equal:
		return D3D11_COMPARISON_EQUAL;
	case Comparison::Greater:
		return D3D11_COMPARISON_GREATER;
	case Comparison::Less:
		return D3D11_COMPARISON_LESS;
	case Comparison::GreaterEqual:
		return D3D11_COMPARISON_GREATER_EQUAL;
	case Comparison::LessEqual:
		return D3D11_COMPARISON_LESS_EQUAL;
	case Comparison::Never:
		return D3D11_COMPARISON_NEVER;
	case Comparison::NotEqual:
		return D3D11_COMPARISON_NOT_EQUAL;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

byte GraphicsDevice::Map(ColorWriteChannels channels)
{
	byte mask = 0;

	if ((channels & ColorWriteChannels::Red) == ColorWriteChannels::Red)
		mask |= D3D11_COLOR_WRITE_ENABLE_RED;
	if ((channels & ColorWriteChannels::Green) == ColorWriteChannels::Green)
		mask |= D3D11_COLOR_WRITE_ENABLE_GREEN;
	if ((channels & ColorWriteChannels::Blue) == ColorWriteChannels::Blue)
		mask |= D3D11_COLOR_WRITE_ENABLE_BLUE;
	if ((channels & ColorWriteChannels::Alpha) == ColorWriteChannels::Alpha)
		mask |= D3D11_COLOR_WRITE_ENABLE_ALPHA;

	return mask;
}

D3D11_CULL_MODE GraphicsDevice::Map(CullMode cullMode)
{
	switch (cullMode)
	{
	case CullMode::None:
		return D3D11_CULL_NONE;
	case CullMode::Front:
		return D3D11_CULL_FRONT;
	case CullMode::Back:
		return D3D11_CULL_BACK;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_FILL_MODE GraphicsDevice::Map(FillMode fillMode)
{
	switch (fillMode)
	{
	case FillMode::Wireframe:
		return D3D11_FILL_WIREFRAME;
	case FillMode::Solid:
		return D3D11_FILL_SOLID;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

DXGI_FORMAT GraphicsDevice::Map(IndexSize indexSize)
{
	switch (indexSize)
	{
	case IndexSize::SixteenBits:
		return DXGI_FORMAT_R16_UINT;
	case IndexSize::ThirtyTwoBits:
		return DXGI_FORMAT_R32_UINT;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_MAP GraphicsDevice::Map(MapMode mapMode)
{
	switch (mapMode)
	{
	case MapMode::Read:
		return D3D11_MAP_READ;
	case MapMode::Write:
		return D3D11_MAP_WRITE;
	case MapMode::WriteDiscard:
		return D3D11_MAP_WRITE_DISCARD;
	case MapMode::WriteNoOverwrite:
		return D3D11_MAP_WRITE_NO_OVERWRITE;
	case MapMode::ReadWrite:
		return D3D11_MAP_READ_WRITE;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_PRIMITIVE_TOPOLOGY GraphicsDevice::Map(PrimitiveType primitiveType)
{
	switch (primitiveType)
	{
	case PrimitiveType::LineList:
		return D3D11_PRIMITIVE_TOPOLOGY_LINELIST;
	case PrimitiveType::LineStrip:
		return D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP;
	case PrimitiveType::TriangleList:
		return D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
	case PrimitiveType::TriangleStrip:
		return D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_USAGE GraphicsDevice::Map(ResourceUsage resourceUsage)
{
	switch (resourceUsage)
	{
	case ResourceUsage::Staging:
		return D3D11_USAGE_STAGING;
	case ResourceUsage::Static:
		return D3D11_USAGE_IMMUTABLE;
	case ResourceUsage::Default:
		return D3D11_USAGE_DEFAULT;
	case ResourceUsage::Dynamic:
		return D3D11_USAGE_DYNAMIC;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_STENCIL_OP GraphicsDevice::Map(StencilOperation stencilOperation)
{
	switch (stencilOperation)
	{
	case StencilOperation::Keep:
		return D3D11_STENCIL_OP_KEEP;
	case StencilOperation::Zero:
		return D3D11_STENCIL_OP_ZERO;
	case StencilOperation::Replace:
		return D3D11_STENCIL_OP_REPLACE;
	case StencilOperation::IncrementAndClamp:
		return D3D11_STENCIL_OP_INCR_SAT;
	case StencilOperation::DecrementAndClamp:
		return D3D11_STENCIL_OP_DECR_SAT;
	case StencilOperation::Invert:
		return D3D11_STENCIL_OP_INVERT;
	case StencilOperation::Increment:
		return D3D11_STENCIL_OP_INCR;
	case StencilOperation::Decrement:
		return D3D11_STENCIL_OP_DECR;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

DXGI_FORMAT GraphicsDevice::Map(SurfaceFormat surfaceFormat)
{
	switch (surfaceFormat)
	{
	case SurfaceFormat::R8:
		return DXGI_FORMAT_R8_UNORM;
	case SurfaceFormat::Rgba8:
		return DXGI_FORMAT_R8G8B8A8_UNORM;
	case SurfaceFormat::Depth24Stencil8:
		return DXGI_FORMAT_D24_UNORM_S8_UINT;
	case SurfaceFormat::Bc1:
		return DXGI_FORMAT_BC1_UNORM;
	case SurfaceFormat::Bc2:
		return DXGI_FORMAT_BC2_UNORM;
	case SurfaceFormat::Bc3:
		return DXGI_FORMAT_BC3_UNORM;
	case SurfaceFormat::Bc4:
		return DXGI_FORMAT_BC4_UNORM;
	case SurfaceFormat::Bc5:
		return DXGI_FORMAT_BC5_UNORM;
	case SurfaceFormat::Rgba16F:
		return DXGI_FORMAT_R16G16B16A16_FLOAT;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_TEXTURE_ADDRESS_MODE GraphicsDevice::Map(TextureAddressMode addressMode)
{
	switch (addressMode)
	{
	case TextureAddressMode::Wrap:
		return D3D11_TEXTURE_ADDRESS_WRAP;
	case TextureAddressMode::Clamp:
		return D3D11_TEXTURE_ADDRESS_CLAMP;
	case TextureAddressMode::Border:
		return D3D11_TEXTURE_ADDRESS_BORDER;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_FILTER GraphicsDevice::Map(TextureFilter textureFilter)
{
	switch (textureFilter)
	{
	case TextureFilter::NearestNoMipmaps:
	case TextureFilter::Nearest:
		return D3D11_FILTER_MIN_MAG_MIP_POINT;
	case TextureFilter::BilinearNoMipmaps:
	case TextureFilter::Bilinear:
		return D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT;
	case TextureFilter::Trilinear:
		return D3D11_FILTER_MIN_MAG_MIP_LINEAR;
	case TextureFilter::Anisotropic:
		return D3D11_FILTER_ANISOTROPIC;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

DXGI_FORMAT GraphicsDevice::Map(VertexDataFormat format)
{
	switch (format)
	{
	case VertexDataFormat::Single:
		return DXGI_FORMAT_R32_FLOAT;
	case VertexDataFormat::Vector2:
		return DXGI_FORMAT_R32G32_FLOAT;
	case VertexDataFormat::Vector3:
		return DXGI_FORMAT_R32G32B32_FLOAT;
	case VertexDataFormat::Vector4:
		return DXGI_FORMAT_R32G32B32A32_FLOAT;
	case VertexDataFormat::Color:
		return DXGI_FORMAT_R8G8B8A8_UNORM;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}
