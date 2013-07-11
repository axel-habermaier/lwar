#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Set debug names functions
//====================================================================================================================

#ifdef DEBUG

#define PG_SET_NAME(type, obj) \
	type##_SetPrivateData(obj, &WKPDID_D3DDebugObjectName, (UINT)strlen(name) + 1, name);

PG_API_EXPORT pgVoid pgSetTextureName(pgTexture* texture, pgString name)
{
	PG_ASSERT_NOT_NULL(texture);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11Texture2D, texture->ptr);
	PG_SET_NAME(ID3D11ShaderResourceView, texture->resourceView);
}

PG_API_EXPORT pgVoid pgSetShaderName(pgShader* shader, pgString name)
{
	PG_ASSERT_NOT_NULL(shader);
	PG_ASSERT_NOT_NULL(name);

	switch (shader->type)
	{
	case PG_VERTEX_SHADER:
		PG_SET_NAME(ID3D11VertexShader, shader->ptr.vertexShader);
		break;
	case PG_GEOMETRY_SHADER:
		PG_SET_NAME(ID3D11GeometryShader, shader->ptr.geometryShader);
		break;
	case PG_FRAGMENT_SHADER:
		PG_SET_NAME(ID3D11PixelShader, shader->ptr.pixelShader);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

PG_API_EXPORT pgVoid pgSetBufferName(pgBuffer* buffer, pgString name)
{
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11Buffer, buffer->ptr);
}

PG_API_EXPORT pgVoid pgSetRenderTargetName(pgRenderTarget* renderTarget, pgString name)
{
	pgInt32 i;

	PG_ASSERT_NOT_NULL(renderTarget);
	PG_ASSERT_NOT_NULL(name);
	
	for (i = 0; i < renderTarget->count; ++i)
		PG_SET_NAME(ID3D11RenderTargetView, renderTarget->cbPtr[i]);

	PG_SET_NAME(ID3D11DepthStencilView, renderTarget->dsPtr);
}

PG_API_EXPORT pgVoid pgSetBlendStateName(pgBlendState* blendState, pgString name)
{
	PG_ASSERT_NOT_NULL(blendState);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11BlendState, blendState->ptr);
}

PG_API_EXPORT pgVoid pgSetDepthStencilStateName(pgDepthStencilState* depthStencilState, pgString name)
{
	PG_ASSERT_NOT_NULL(depthStencilState);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11DepthStencilState, depthStencilState->ptr);
}

PG_API_EXPORT pgVoid pgSetRasterizerStateName(pgRasterizerState* rasterizerState, pgString name)
{
	PG_ASSERT_NOT_NULL(rasterizerState);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11RasterizerState, rasterizerState->ptr);
}

PG_API_EXPORT pgVoid pgSetSamplerStateName(pgSamplerState* samplerState, pgString name)
{
	PG_ASSERT_NOT_NULL(samplerState);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11SamplerState, samplerState->ptr);
}

PG_API_EXPORT pgVoid pgSetQueryName(pgQuery* query, pgString name)
{
	PG_ASSERT_NOT_NULL(query);
	PG_ASSERT_NOT_NULL(name);

	PG_SET_NAME(ID3D11Query, query->ptr);
}

#endif

//====================================================================================================================
// Enumeration value conversion functions
//====================================================================================================================

D3D11_BLEND_OP pgConvertBlendOperation(pgBlendOperation blendOperation)
{
	switch(blendOperation)
	{
	case PG_BLEND_OP_ADD:
		return D3D11_BLEND_OP_ADD;
	case PG_BLEND_OP_SUBTRACT:
		return D3D11_BLEND_OP_SUBTRACT;
	case PG_BLEND_OP_REVERSE_SUBTRACT:
		return D3D11_BLEND_OP_REV_SUBTRACT;
	case PG_BLEND_OP_MINIMUM:
		return D3D11_BLEND_OP_MIN;
	case PG_BLEND_OP_MAXIMUM:
		return D3D11_BLEND_OP_MAX;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_BLEND pgConvertBlendOption(pgBlendOption blendOption)
{
	switch(blendOption)
	{
	case PG_BLEND_ZERO:
		return D3D11_BLEND_ZERO;
	case PG_BLEND_ONE:
		return D3D11_BLEND_ONE;
	case PG_BLEND_SOURCE_COLOR:
		return D3D11_BLEND_SRC_COLOR;
	case PG_BLEND_INVERSE_SOURCE_COLOR:
		return D3D11_BLEND_INV_SRC_COLOR;
	case PG_BLEND_SOURCE_ALPHA:
		return D3D11_BLEND_SRC_ALPHA;
	case PG_BLEND_INVERSE_SOURCE_ALPHA:
		return D3D11_BLEND_INV_SRC_ALPHA;
	case PG_BLEND_DESTINATION_ALPHA:
		return D3D11_BLEND_DEST_ALPHA;
	case PG_BLEND_INVERSE_DESTINATION_ALPHA:
		return D3D11_BLEND_INV_DEST_ALPHA;
	case PG_BLEND_DESTINATION_COLOR:
		return D3D11_BLEND_DEST_COLOR;
	case PG_BLEND_INVERSE_DESTINATION_COLOR:
		return D3D11_BLEND_INV_DEST_COLOR;
	case PG_BLEND_SOURCE_ALPHA_SATURATE:
		return D3D11_BLEND_SRC_ALPHA_SAT;
	case PG_BLEND_BLEND_FACTOR:
		return D3D11_BLEND_BLEND_FACTOR;
	case PG_BLEND_INVERSE_BLEND_FACTOR:
		return D3D11_BLEND_INV_BLEND_FACTOR;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_COMPARISON_FUNC pgConvertComparison(pgComparison comparison)
{
	switch(comparison)
	{
	case PG_COMPARISON_ALWAYS:
		return D3D11_COMPARISON_ALWAYS;
	case PG_COMPARISON_EQUAL:
		return D3D11_COMPARISON_EQUAL;
	case PG_COMPARISON_GREATER:
		return D3D11_COMPARISON_GREATER;
	case PG_COMPARISON_LESS:
		return D3D11_COMPARISON_LESS;
	case PG_COMPARISON_GREATER_EQUAL:
		return D3D11_COMPARISON_GREATER_EQUAL;
	case PG_COMPARISON_LESS_EQUAL:
		return D3D11_COMPARISON_LESS_EQUAL;
	case PG_COMPARISON_NEVER:
		return D3D11_COMPARISON_NEVER;
	case PG_COMPARISON_NOT_EQUAL:
		return D3D11_COMPARISON_NOT_EQUAL;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

UINT8 pgConvertColorWriteChannels(pgColorWriteChannels channels)
{
	UINT8 mask = 0;

	if ((channels & PG_COLOR_WRITE_ENABLE_RED) == PG_COLOR_WRITE_ENABLE_RED)
		mask |= D3D11_COLOR_WRITE_ENABLE_RED;
	if ((channels & PG_COLOR_WRITE_ENABLE_GREEN) == PG_COLOR_WRITE_ENABLE_GREEN)
		mask |= D3D11_COLOR_WRITE_ENABLE_GREEN;
	if ((channels & PG_COLOR_WRITE_ENABLE_BLUE) == PG_COLOR_WRITE_ENABLE_BLUE)
		mask |= D3D11_COLOR_WRITE_ENABLE_BLUE;
	if ((channels & PG_COLOR_WRITE_ENABLE_ALPHA) == PG_COLOR_WRITE_ENABLE_ALPHA)
		mask |= D3D11_COLOR_WRITE_ENABLE_ALPHA;

	return mask;
}

D3D11_CULL_MODE pgConvertCullMode(pgCullMode cullMode)
{
	switch (cullMode)
	{
	case PG_CULL_NONE:
		return D3D11_CULL_NONE;
	case PG_CULL_FRONT:
		return D3D11_CULL_FRONT;
	case PG_CULL_BACK:
		return D3D11_CULL_BACK;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_FILL_MODE pgConvertFillMode(pgFillMode fillMode)
{
	switch(fillMode)
	{
	case PG_FILL_WIREFRAME:
		return D3D11_FILL_WIREFRAME;
	case PG_FILL_SOLID:
		return D3D11_FILL_SOLID;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

DXGI_FORMAT pgConvertIndexSize(pgIndexSize indexSize)
{
	switch(indexSize)
	{
	case PG_INDEX_SIZE_16:
		return DXGI_FORMAT_R16_UINT;
	case PG_INDEX_SIZE_32:
		return DXGI_FORMAT_R32_UINT;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_MAP pgConvertMapMode(pgMapMode mapMode)
{
	switch(mapMode)
	{
	case PG_MAP_READ:				
		return D3D11_MAP_READ;
	case PG_MAP_WRITE:				
		return D3D11_MAP_WRITE;
	case PG_MAP_WRITE_DISCARD:
		return D3D11_MAP_WRITE_DISCARD;
	case PG_MAP_WRITE_NO_OVERWRITE:	
		return D3D11_MAP_WRITE_NO_OVERWRITE;
	case PG_MAP_READ_WRITE:			
		return D3D11_MAP_READ_WRITE;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_PRIMITIVE_TOPOLOGY pgConvertPrimitiveType(pgPrimitiveType primitiveType)
{
	switch(primitiveType)
	{
	case PG_PRIMITIVE_LINES:
		return D3D11_PRIMITIVE_TOPOLOGY_LINELIST;
	case PG_PRIMITIVE_LINESTRIP:
		return D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP;
	case PG_PRIMITIVE_TRIANGLES:
		return D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
	case PG_PRIMITIVE_TRIANGLESTRIP:
		return D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_USAGE pgConvertResourceUsage(pgResourceUsage resourceUsage)
{
	switch(resourceUsage)
	{
	case PG_USAGE_STAGING:
		return D3D11_USAGE_STAGING;
	case PG_USAGE_STATIC:
		return D3D11_USAGE_IMMUTABLE;
	case PG_USAGE_DEFAULT:
		return D3D11_USAGE_DEFAULT;
	case PG_USAGE_DYNAMIC:
		return D3D11_USAGE_DYNAMIC;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_STENCIL_OP pgConvertStencilOperation(pgStencilOperation stencilOperation)
{
	switch(stencilOperation)
	{
	case PG_STENCIL_OP_KEEP:
		return D3D11_STENCIL_OP_KEEP;
	case PG_STENCIL_OP_ZERO:
		return D3D11_STENCIL_OP_ZERO;
	case PG_STENCIL_OP_REPLACE:
		return D3D11_STENCIL_OP_REPLACE;
	case PG_STENCIL_OP_INCREMENT_AND_CLAMP:
		return D3D11_STENCIL_OP_INCR_SAT;
	case PG_STENCIL_OP_DECREMENT_AND_CLAMP:
		return D3D11_STENCIL_OP_DECR_SAT;
	case PG_STENCIL_OP_INVERT:
		return D3D11_STENCIL_OP_INVERT;
	case PG_STENCIL_OP_INCREMENT:
		return D3D11_STENCIL_OP_INCR;
	case PG_STENCIL_OP_DECREMENT:
		return D3D11_STENCIL_OP_DECR;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

DXGI_FORMAT pgConvertSurfaceFormat(pgSurfaceFormat surfaceFormat)
{
	switch(surfaceFormat)
	{
	case PG_SURFACE_R8:
		return DXGI_FORMAT_R8_UNORM;
	case PG_SURFACE_RGBA8:
		return DXGI_FORMAT_R8G8B8A8_UNORM;
	case PG_SURFACE_DEPTH24_STENCIL8:
		return DXGI_FORMAT_D24_UNORM_S8_UINT;
	case PG_SURFACE_BC1:
		return DXGI_FORMAT_BC1_UNORM;
	case PG_SURFACE_BC2:
		return DXGI_FORMAT_BC2_UNORM;
	case PG_SURFACE_BC3:
		return DXGI_FORMAT_BC3_UNORM;
	case PG_SURFACE_BC4:
		return DXGI_FORMAT_BC4_UNORM;
	case PG_SURFACE_BC5:
		return DXGI_FORMAT_BC5_UNORM;
	case PG_SURFACE_RGBA16F:
		return DXGI_FORMAT_R16G16B16A16_FLOAT;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_TEXTURE_ADDRESS_MODE pgConvertTextureAddressMode(pgTextureAddressMode addressMode)
{
	switch(addressMode)
	{
	case PG_TEXTURE_ADDRESS_WRAP:
		return D3D11_TEXTURE_ADDRESS_WRAP;
	case PG_TEXTURE_ADDRESS_CLAMP:
		return D3D11_TEXTURE_ADDRESS_CLAMP;
	case PG_TEXTURE_ADDRESS_BORDER:
		return D3D11_TEXTURE_ADDRESS_BORDER;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_FILTER pgConvertTextureFilter(pgTextureFilter textureFilter)
{
	switch(textureFilter)
	{
	case PG_TEXTURE_FILTER_NEAREST_NO_MIPMAPS:
	case PG_TEXTURE_FILTER_NEAREST:
		return D3D11_FILTER_MIN_MAG_MIP_POINT;
	case PG_TEXTURE_FILTER_BILINEAR_NO_MIPMAPS:
	case PG_TEXTURE_FILTER_BILINEAR:
		return D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT;
	case PG_TEXTURE_FILTER_TRILINEAR:
		return D3D11_FILTER_MIN_MAG_MIP_LINEAR;
	case PG_TEXTURE_FILTER_ANISOTROPIC:
		return D3D11_FILTER_ANISOTROPIC;
	default:						
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_BIND_FLAG pgConvertBufferType(pgBufferType bufferType)
{
	switch (bufferType)
	{
	case PG_CONSTANT_BUFFER:
		return D3D11_BIND_CONSTANT_BUFFER;
	case PG_VERTEX_BUFFER:
		return D3D11_BIND_VERTEX_BUFFER;
	case PG_INDEX_BUFFER:
		return D3D11_BIND_INDEX_BUFFER;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

pgVoid pgConvertVertexDataSemantics(pgDataSemantics semantics, pgInt32* semanticIndex, pgString* semanticName)
{
	*semanticIndex = 0;

	switch (semantics)
	{
	case PG_VERTEX_SEMANTICS_POSITION:
		*semanticName = "POSITION";
		break;
	case PG_VERTEX_SEMANTICS_COLOR0:		
		*semanticName = "COLOR";
		*semanticIndex = 0;
		break;
	case PG_VERTEX_SEMANTICS_COLOR1:		
		*semanticName = "COLOR";
		*semanticIndex = 1;
		break;
	case PG_VERTEX_SEMANTICS_COLOR2:		
		*semanticName = "COLOR";
		*semanticIndex = 2;
		break;
	case PG_VERTEX_SEMANTICS_COLOR3:		
		*semanticName = "COLOR";
		*semanticIndex = 3;
		break;
	case PG_VERTEX_SEMANTICS_TEXCOORDS0:			
		*semanticName = "TEXCOORD";
		*semanticIndex = 0;
		break;
	case PG_VERTEX_SEMANTICS_TEXCOORDS1:			
		*semanticName = "TEXCOORD";
		*semanticIndex = 1;
		break;
	case PG_VERTEX_SEMANTICS_TEXCOORDS2:			
		*semanticName = "TEXCOORD";
		*semanticIndex = 2;
		break;
	case PG_VERTEX_SEMANTICS_TEXCOORDS3:			
		*semanticName = "TEXCOORD";
		*semanticIndex = 3;
		break;
	case PG_VERTEX_SEMANTICS_NORMAL:	
		*semanticName = "NORMAL";
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

DXGI_FORMAT pgConvertVertexDataFormat(pgVertexDataFormat format)
{
	switch (format)
	{
	case PG_VERTEX_FORMAT_SINGLE:
		return DXGI_FORMAT_R32_FLOAT;
	case PG_VERTEX_FORMAT_VECTOR2:
		return DXGI_FORMAT_R32G32_FLOAT;
	case PG_VERTEX_FORMAT_VECTOR3:
		return DXGI_FORMAT_R32G32B32_FLOAT;
	case PG_VERTEX_FORMAT_VECTOR4:
		return DXGI_FORMAT_R32G32B32A32_FLOAT;
	case PG_VERTEX_FORMAT_COLOR:
		return DXGI_FORMAT_R8G8B8A8_UNORM;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

D3D11_QUERY pgConvertQueryType(pgQueryType type)
{
	switch (type)
	{
	case PG_TIMESTAMP_QUERY:
		return D3D11_QUERY_TIMESTAMP;
	case PG_TIMESTAMP_DISJOINT_QUERY:
		return D3D11_QUERY_TIMESTAMP_DISJOINT;
	case PG_OCCLUSION_QUERY:
		return D3D11_QUERY_OCCLUSION;
	case PG_SYNCED_QUERY:
		return D3D11_QUERY_EVENT;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

#endif