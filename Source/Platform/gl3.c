#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Error handling
//====================================================================================================================

pgVoid pgCheckGLError(pgString file, pgInt32 line)
{
	GLboolean glErrorOccurred = GL_FALSE;										
	GLenum glError;		

	for (glError = glGetError(); glError != GL_NO_ERROR; glError = glGetError())
	{																			
		GLchar* msg = "";														
		switch (glError)														
		{																		
		case GL_INVALID_ENUM:		msg = "GL_INVALID_ENUM";		break;		
		case GL_INVALID_VALUE:		msg = "GL_INVALID_VALUE";		break;		
		case GL_INVALID_OPERATION:	msg = "GL_INVALID_OPERATION";	break;		
		case GL_OUT_OF_MEMORY:		msg = "GL_OUT_OF_MEMORY";		break;		
		}		

		PG_ERROR("OpenGL error at %s(%d): %s", file, line, msg);			
		glErrorOccurred = GL_TRUE;												
	}		

	if (glErrorOccurred)														
		PG_DIE("Stopped after OpenGL error.");	
}

//====================================================================================================================
// Set debug names functions
//====================================================================================================================

#ifdef DEBUG

PG_API_EXPORT pgVoid pgSetTextureName(pgTexture* texture, pgString name)
{
	PG_UNUSED(texture);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetShaderName(pgShader* shader, pgString name)
{
	PG_UNUSED(shader);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetBufferName(pgBuffer* buffer, pgString name)
{
	PG_UNUSED(buffer);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetRenderTargetName(pgRenderTarget* renderTarget, pgString name)
{
	PG_UNUSED(renderTarget);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetBlendStateName(pgBlendState* blendState, pgString name)
{
	PG_UNUSED(blendState);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetDepthStencilStateName(pgDepthStencilState* depthStencilState, pgString name)
{
	PG_UNUSED(depthStencilState);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetRasterizerStateName(pgRasterizerState* rasterizerState, pgString name)
{
	PG_UNUSED(rasterizerState);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetSamplerStateName(pgSamplerState* samplerState, pgString name)
{
	PG_UNUSED(samplerState);
	PG_UNUSED(name);
}

PG_API_EXPORT pgVoid pgSetQueryName(pgQuery* query, pgString name)
{
	PG_UNUSED(query);
	PG_UNUSED(name);
}

#endif

//====================================================================================================================
// Enumeration value conversion functions
//====================================================================================================================

GLenum pgConvertBlendOperation(pgBlendOperation blendOperation)
{
	switch(blendOperation)
	{
	case PG_BLEND_OP_ADD:
		return GL_FUNC_ADD;
	case PG_BLEND_OP_SUBTRACT:
		return GL_FUNC_SUBTRACT;
	case PG_BLEND_OP_REVERSE_SUBTRACT:
		return GL_FUNC_REVERSE_SUBTRACT;
	case PG_BLEND_OP_MINIMUM:
		return GL_MIN;
	case PG_BLEND_OP_MAXIMUM:
		return GL_MAX;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertBlendOption(pgBlendOption blendOption)
{
	switch(blendOption)
	{
	case PG_BLEND_ZERO:
		return GL_ZERO;
	case PG_BLEND_ONE:
		return GL_ONE;
	case PG_BLEND_SOURCE_COLOR:
		return GL_SRC_COLOR;
	case PG_BLEND_INVERSE_SOURCE_COLOR:
		return GL_ONE_MINUS_SRC_COLOR;
	case PG_BLEND_SOURCE_ALPHA:
		return GL_SRC_ALPHA;
	case PG_BLEND_INVERSE_SOURCE_ALPHA:
		return GL_ONE_MINUS_SRC_ALPHA;
	case PG_BLEND_DESTINATION_ALPHA:
		return GL_DST_ALPHA;
	case PG_BLEND_INVERSE_DESTINATION_ALPHA:
		return GL_ONE_MINUS_DST_ALPHA;
	case PG_BLEND_DESTINATION_COLOR:
		return GL_DST_COLOR;
	case PG_BLEND_INVERSE_DESTINATION_COLOR:
		return GL_ONE_MINUS_DST_COLOR;
	case PG_BLEND_SOURCE_ALPHA_SATURATE:
		return GL_SRC_ALPHA_SATURATE;
	case PG_BLEND_BLEND_FACTOR:
		return GL_CONSTANT_COLOR;
	case PG_BLEND_INVERSE_BLEND_FACTOR:
		return GL_ONE_MINUS_CONSTANT_COLOR;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertComparison(pgComparison comparison)
{
	switch(comparison)
	{
	case PG_COMPARISON_ALWAYS:
		return GL_ALWAYS;
	case PG_COMPARISON_EQUAL:
		return GL_EQUAL;
	case PG_COMPARISON_GREATER:
		return GL_GREATER;
	case PG_COMPARISON_LESS:
		return GL_LESS;
	case PG_COMPARISON_GREATER_EQUAL:
		return GL_GEQUAL;
	case PG_COMPARISON_LESS_EQUAL:
		return GL_LEQUAL;
	case PG_COMPARISON_NEVER:
		return GL_NEVER;
	case PG_COMPARISON_NOT_EQUAL:
		return GL_NOTEQUAL;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertFillMode(pgFillMode fillMode)
{
	switch(fillMode)
	{
	case PG_FILL_WIREFRAME:
		return GL_LINE;
	case PG_FILL_SOLID:
		return GL_FILL;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLvoid pgConvertIndexSize(pgIndexSize indexSize, GLenum* type, GLint* size)
{
	switch(indexSize)
	{
	case PG_INDEX_SIZE_16:
		*type = GL_UNSIGNED_SHORT;
		*size = 2;
		break;
	case PG_INDEX_SIZE_32:
		*type = GL_UNSIGNED_INT;
		*size = 4;
		break;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertMapMode(pgMapMode mapMode)
{
	switch(mapMode)
	{
	case PG_MAP_READ:				
		return GL_MAP_READ_BIT;
	case PG_MAP_WRITE:				
		return GL_MAP_WRITE_BIT;
	case PG_MAP_WRITE_DISCARD:
	case PG_MAP_WRITE_NO_OVERWRITE:	
		return GL_MAP_WRITE_BIT | GL_MAP_INVALIDATE_BUFFER_BIT;
	case PG_MAP_READ_WRITE:			
		return GL_MAP_READ_BIT | GL_MAP_WRITE_BIT;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertPrimitiveType(pgPrimitiveType primitiveType)
{
	switch(primitiveType)
	{
	case PG_PRIMITIVE_LINES:
		return GL_LINES;
	case PG_PRIMITIVE_LINESTRIP:
		return GL_LINE_STRIP;
	case PG_PRIMITIVE_TRIANGLES:
		return GL_TRIANGLES;
	case PG_PRIMITIVE_TRIANGLESTRIP:
		return GL_TRIANGLE_STRIP;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertResourceUsage(pgResourceUsage resourceUsage)
{
	switch(resourceUsage)
	{
	case PG_USAGE_STAGING:
		return GL_DYNAMIC_READ;
	case PG_USAGE_STATIC:
		return GL_STATIC_DRAW;
	case PG_USAGE_DEFAULT:
		return GL_STREAM_DRAW;
	case PG_USAGE_DYNAMIC:
		return GL_DYNAMIC_DRAW;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertStencilOperation(pgStencilOperation stencilOperation)
{
	switch(stencilOperation)
	{
	case PG_STENCIL_OP_KEEP:
		return GL_KEEP;
	case PG_STENCIL_OP_ZERO:
		return GL_ZERO;
	case PG_STENCIL_OP_REPLACE:
		return GL_REPLACE;
	case PG_STENCIL_OP_INCREMENT_AND_CLAMP:
		return GL_INCR_WRAP;
	case PG_STENCIL_OP_DECREMENT_AND_CLAMP:
		return GL_DECR_WRAP;
	case PG_STENCIL_OP_INVERT:
		return GL_INVERT;
	case PG_STENCIL_OP_INCREMENT:
		return GL_INCR;
	case PG_STENCIL_OP_DECREMENT:
		return GL_DECR;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLvoid pgConvertSurfaceFormat(pgSurfaceFormat surfaceFormat, GLenum* internalFormat, GLenum* format)
{
	switch(surfaceFormat)
	{
	case PG_SURFACE_RGBA8:
		*internalFormat = GL_RGBA;
		*format = GL_RGBA;
		break;
	case PG_SURFACE_R8:
		*internalFormat = GL_R8;
		*format = GL_R8;
		break;
	case PG_SURFACE_BC1:
		*format = GL_COMPRESSED_RGB_S3TC_DXT1_EXT;
		*internalFormat = *format;
		break;
	case PG_SURFACE_BC2:
		*format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT;
		*internalFormat = *format;
		break;
	case PG_SURFACE_BC3:
		*format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT;
		*internalFormat = *format;
		break;
	case PG_SURFACE_BC4:
		*format = GL_COMPRESSED_RED_RGTC1;
		*internalFormat = *format;
		break;
	case PG_SURFACE_BC5:
		*format = GL_COMPRESSED_RG_RGTC2;
		*internalFormat = *format;
		break;
	case PG_SURFACE_RGBA16F:
		*format = GL_RGBA;
		*internalFormat = GL_RGBA16F;
		break;
	case PG_SURFACE_DEPTH24_STENCIL8:
		*format = GL_DEPTH_STENCIL;
		*internalFormat = GL_DEPTH24_STENCIL8;
		break;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertTextureAddressMode(pgTextureAddressMode addressMode)
{
	switch(addressMode)
	{
	case PG_TEXTURE_ADDRESS_WRAP:
		return GL_REPEAT;
	case PG_TEXTURE_ADDRESS_CLAMP:
		return GL_CLAMP_TO_EDGE;
	case PG_TEXTURE_ADDRESS_BORDER:
		return GL_CLAMP_TO_BORDER;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLvoid pgConvertTextureFilter(pgTextureFilter textureFilter, GLenum* minFilter, GLenum* magFilter)
{
	switch(textureFilter)
	{
	case PG_TEXTURE_FILTER_NEAREST:
		*minFilter = GL_NEAREST_MIPMAP_NEAREST;
		*magFilter = GL_NEAREST;
		break;
	case PG_TEXTURE_FILTER_NEAREST_NO_MIPMAPS:
		*minFilter = GL_NEAREST;
		*magFilter = GL_NEAREST;
		break;
	case PG_TEXTURE_FILTER_BILINEAR:
		*minFilter = GL_LINEAR_MIPMAP_NEAREST;
		*magFilter = GL_LINEAR;
		break;
	case PG_TEXTURE_FILTER_BILINEAR_NO_MIPMAPS:
		*minFilter = GL_LINEAR;
		*magFilter = GL_LINEAR;
		break;
	case PG_TEXTURE_FILTER_TRILINEAR:
		*minFilter = GL_LINEAR_MIPMAP_LINEAR;
		*magFilter = GL_LINEAR;
		break;
	case PG_TEXTURE_FILTER_ANISOTROPIC:
		*minFilter = GL_LINEAR_MIPMAP_LINEAR;
		*magFilter = GL_LINEAR;
		break;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLvoid pgConvertVertexDataFormat(pgVertexDataFormat vertexDataFormat, GLenum* type, GLsizei* size)
{
	switch(vertexDataFormat)
	{
	case PG_VERTEX_FORMAT_SINGLE:
		*type = GL_FLOAT;
		*size = 1;
		break;
	case PG_VERTEX_FORMAT_VECTOR2:
		*type = GL_FLOAT;
		*size = 2;
		break;
	case PG_VERTEX_FORMAT_VECTOR3:
		*type = GL_FLOAT;
		*size = 3;
		break;
	case PG_VERTEX_FORMAT_VECTOR4:
		*type = GL_FLOAT;
		*size = 4;
		break;
	case PG_VERTEX_FORMAT_COLOR:
		// The type of color inputs in the shader is Vector4; however, we only send RGBA8 values and let the shader
        // convert the data to save 12 bytes per vertex
		*type = GL_UNSIGNED_BYTE;
		*size = 4;
		break;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

GLenum pgConvertBufferType(pgBufferType bufferType)
{
	switch (bufferType)
	{
	case PG_CONSTANT_BUFFER:
		return GL_UNIFORM_BUFFER;
	case PG_VERTEX_BUFFER:
		return GL_ARRAY_BUFFER;
	case PG_INDEX_BUFFER:
		return GL_ELEMENT_ARRAY_BUFFER;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

GLvoid pgConvertTextureType(pgTextureType textureType, GLenum* type, GLenum* boundType)
{
	switch (textureType)
	{
	case PG_TEXTURE_2D:
		*type = GL_TEXTURE_2D;
		*boundType = GL_TEXTURE_BINDING_2D;
		return;
	case PG_TEXTURE_CUBE_MAP:
		*type = GL_TEXTURE_CUBE_MAP;
		*boundType = GL_TEXTURE_BINDING_CUBE_MAP;
		return;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

#endif