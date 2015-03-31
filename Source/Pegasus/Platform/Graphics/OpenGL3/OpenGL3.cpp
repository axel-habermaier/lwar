#include "Prelude.hpp"

using namespace OpenGL3;

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Helper functions
//-------------------------------------------------------------------------------------------------------------------------------------------------------

uint32 GraphicsDevice::GetInternalFormat(SurfaceFormat format)
{
	switch (format)
	{
	case SurfaceFormat::Rgba8:
		return GL_RGBA;
	case SurfaceFormat::R8:
		return GL_R8;
	case SurfaceFormat::Bc1:
		return GL_COMPRESSED_RGB_S3TC_DXT1_EXT;
	case SurfaceFormat::Bc2:
		return GL_COMPRESSED_RGBA_S3TC_DXT3_EXT;
	case SurfaceFormat::Bc3:
		return GL_COMPRESSED_RGBA_S3TC_DXT5_EXT;
	case SurfaceFormat::Bc4:
		return GL_COMPRESSED_RED_RGTC1;
	case SurfaceFormat::Bc5:
		return GL_COMPRESSED_RG_RGTC2;
	case SurfaceFormat::Rgba16F:
		return GL_RGBA;
	case SurfaceFormat::Depth24Stencil8:
		return GL_DEPTH_STENCIL;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

int32 GraphicsDevice::GetIndexSizeInBytes(IndexSize indexSize)
{
	switch (indexSize)
	{
	case IndexSize::SixteenBits:
		return 2;
	case IndexSize::ThirtyTwoBits:
		return 4;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::GetVertexDataType(VertexDataFormat format)
{
	switch (format)
	{
	case VertexDataFormat::Single:
	case VertexDataFormat::Vector2:
	case VertexDataFormat::Vector3:
	case VertexDataFormat::Vector4:
		return GL_FLOAT;
	case VertexDataFormat::Color:
		// The type of color inputs in the shader is Vector4; however, we only send RGBA8 values and let the shader
		// convert the data to save 12 bytes per vertex
		return GL_UNSIGNED_BYTE;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

int32 GraphicsDevice::GetVertexDataComponentCount(VertexDataFormat format)
{
	switch (format)
	{
	case VertexDataFormat::Single:
		return 1;
	case VertexDataFormat::Vector2:
		return  2;
	case VertexDataFormat::Vector3:
		return 3;
	case VertexDataFormat::Vector4:
		return 4;
	case VertexDataFormat::Color:
		// The type of color inputs in the shader is Vector4; however, we only send RGBA8 values and let the shader
		// convert the data to save 12 bytes per vertex
		return 4;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

int32 GraphicsDevice::GetMinFilter(TextureFilter filter)
{
	switch (filter)
	{
	case TextureFilter::Nearest:
		return GL_NEAREST_MIPMAP_NEAREST;
	case TextureFilter::NearestNoMipmaps:
		return GL_NEAREST;
	case TextureFilter::Bilinear:
		return GL_LINEAR_MIPMAP_NEAREST;
	case TextureFilter::BilinearNoMipmaps:
		return GL_LINEAR;
	case TextureFilter::Trilinear:
		return GL_LINEAR_MIPMAP_LINEAR;
	case TextureFilter::Anisotropic:
		return GL_LINEAR_MIPMAP_LINEAR;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

int32 GraphicsDevice::GetMagFilter(TextureFilter filter)
{
	switch (filter)
	{
	case TextureFilter::Nearest:
		return GL_NEAREST;
	case TextureFilter::NearestNoMipmaps:
		return GL_NEAREST;
	case TextureFilter::Bilinear:
		return GL_LINEAR;
	case TextureFilter::BilinearNoMipmaps:
		return GL_LINEAR;
	case TextureFilter::Trilinear:
		return GL_LINEAR;
	case TextureFilter::Anisotropic:
		return GL_LINEAR;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(BlendOperation blendOperation)
{
	switch (blendOperation)
	{
	case BlendOperation::Add:
		return GL_FUNC_ADD;
	case BlendOperation::Subtract:
		return GL_FUNC_SUBTRACT;
	case BlendOperation::ReverseSubtract:
		return GL_FUNC_REVERSE_SUBTRACT;
	case BlendOperation::Minimum:
		return GL_MIN;
	case BlendOperation::Maximum:
		return GL_MAX;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(BlendOption blendOption)
{
	switch (blendOption)
	{
	case BlendOption::Zero:
		return GL_ZERO;
	case BlendOption::One:
		return GL_ONE;
	case BlendOption::SourceColor:
		return GL_SRC_COLOR;
	case BlendOption::InverseSourceColor:
		return GL_ONE_MINUS_SRC_COLOR;
	case BlendOption::SourceAlpha:
		return GL_SRC_ALPHA;
	case BlendOption::InverseSourceAlpha:
		return GL_ONE_MINUS_SRC_ALPHA;
	case BlendOption::DestinationAlpha:
		return GL_DST_ALPHA;
	case BlendOption::InverseDestinationAlpha:
		return GL_ONE_MINUS_DST_ALPHA;
	case BlendOption::DestinationColor:
		return GL_DST_COLOR;
	case BlendOption::InverseDestinationColor:
		return GL_ONE_MINUS_DST_COLOR;
	case BlendOption::SourceAlphaSaturate:
		return GL_SRC_ALPHA_SATURATE;
	case BlendOption::BlendFactor:
		return GL_CONSTANT_COLOR;
	case BlendOption::InverseBlendFactor:
		return GL_ONE_MINUS_CONSTANT_COLOR;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(Comparison comparison)
{
	switch (comparison)
	{
	case Comparison::Always:
		return GL_ALWAYS;
	case Comparison::Equal:
		return GL_EQUAL;
	case Comparison::Greater:
		return GL_GREATER;
	case Comparison::Less:
		return GL_LESS;
	case Comparison::GreaterEqual:
		return GL_GEQUAL;
	case Comparison::LessEqual:
		return GL_LEQUAL;
	case Comparison::Never:
		return GL_NEVER;
	case Comparison::NotEqual:
		return GL_NOTEQUAL;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(FillMode fillMode)
{
	switch (fillMode)
	{
	case FillMode::Wireframe:
		return GL_LINE;
	case FillMode::Solid:
		return GL_FILL;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(IndexSize indexSize)
{
	switch (indexSize)
	{
	case IndexSize::SixteenBits:
		return GL_UNSIGNED_SHORT;
	case IndexSize::ThirtyTwoBits:
		return GL_UNSIGNED_INT;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(MapMode mapMode)
{
	switch (mapMode)
	{
	case MapMode::Read:
		return GL_MAP_READ_BIT;
	case MapMode::Write:
		return GL_MAP_WRITE_BIT;
	case MapMode::WriteDiscard:
	case MapMode::WriteNoOverwrite:
		return GL_MAP_WRITE_BIT | GL_MAP_INVALIDATE_RANGE_BIT | GL_MAP_UNSYNCHRONIZED_BIT;
	case MapMode::ReadWrite:
		return GL_MAP_READ_BIT | GL_MAP_WRITE_BIT;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(PrimitiveType primitiveType)
{
	switch (primitiveType)
	{
	case PrimitiveType::LineList:
		return GL_LINES;
	case PrimitiveType::LineStrip:
		return GL_LINE_STRIP;
	case PrimitiveType::TriangleList:
		return GL_TRIANGLES;
	case PrimitiveType::TriangleStrip:
		return GL_TRIANGLE_STRIP;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(ResourceUsage resourceUsage)
{
	switch (resourceUsage)
	{
	case ResourceUsage::Staging:
		return GL_DYNAMIC_READ;
	case ResourceUsage::Static:
		return GL_STATIC_DRAW;
	case ResourceUsage::Default:
		return GL_STREAM_DRAW;
	case ResourceUsage::Dynamic:
		return GL_DYNAMIC_DRAW;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(StencilOperation stencilOperation)
{
	switch (stencilOperation)
	{
	case StencilOperation::Keep:
		return GL_KEEP;
	case StencilOperation::Zero:
		return GL_ZERO;
	case StencilOperation::Replace:
		return GL_REPLACE;
	case StencilOperation::IncrementAndClamp:
		return GL_INCR;
	case StencilOperation::DecrementAndClamp:
		return GL_DECR;
	case StencilOperation::Invert:
		return GL_INVERT;
	case StencilOperation::Increment:
		return GL_INCR_WRAP;
	case StencilOperation::Decrement:
		return GL_DECR_WRAP;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

uint32 GraphicsDevice::Map(SurfaceFormat surfaceFormat)
{
	switch (surfaceFormat)
	{
	case SurfaceFormat::Rgba8:
		return GL_RGBA;
	case SurfaceFormat::R8:
		return GL_R8;
	case SurfaceFormat::Bc1:
		return GL_COMPRESSED_RGB_S3TC_DXT1_EXT;
	case SurfaceFormat::Bc2:
		return GL_COMPRESSED_RGBA_S3TC_DXT3_EXT;
	case SurfaceFormat::Bc3:
		return GL_COMPRESSED_RGBA_S3TC_DXT5_EXT;
	case SurfaceFormat::Bc4:
		return GL_COMPRESSED_RED_RGTC1;
	case SurfaceFormat::Bc5:
		return GL_COMPRESSED_RG_RGTC2;
	case SurfaceFormat::Rgba16F:
		return GL_RGBA;
	case SurfaceFormat::Depth24Stencil8:
		return GL_DEPTH_STENCIL;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

int32 GraphicsDevice::Map(TextureAddressMode addressMode)
{
	switch (addressMode)
	{
	case TextureAddressMode::Wrap:
		return GL_REPEAT;
	case TextureAddressMode::Clamp:
		return GL_CLAMP_TO_EDGE;
	case TextureAddressMode::Border:
		return GL_CLAMP_TO_BORDER;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}
