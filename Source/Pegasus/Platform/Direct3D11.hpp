#pragma once

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Prelude.hpp"
#include "Utilities/Win32.hpp"

PG_DISABLE_WARNING(4917)
PG_DISABLE_WARNING(4365)
PG_DISABLE_WARNING(4061)
PG_DISABLE_WARNING(4625)
PG_DISABLE_WARNING(4626)
PG_DISABLE_WARNING(4668)
PG_DISABLE_WARNING(4986)

#define D3D11_NO_HELPERS

#include <dxgi.h>
#include <d3d11.h>

PG_ENABLE_WARNING(4917)
PG_ENABLE_WARNING(4365)
PG_ENABLE_WARNING(4061)
PG_ENABLE_WARNING(4625)
PG_ENABLE_WARNING(4626)
PG_ENABLE_WARNING(4668)
PG_ENABLE_WARNING(4986)

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Forward declarations
//-------------------------------------------------------------------------------------------------------------------------------------------------------
enum class DataSemantics;
enum class BlendOperation;
enum class BlendOption;
enum class Comparison;
enum class ColorWriteChannels;
enum class CullMode;
enum class FillMode;
enum class IndexSize;
enum class MapMode;
enum class PrimitiveType;
enum class ResourceUsage;
enum class StencilOperation;
enum class SurfaceFormat;
enum class TextureAddressMode;
enum class TextureFilter;
enum class VertexDataFormat;

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Direct3D11 helper functions
//-------------------------------------------------------------------------------------------------------------------------------------------------------
namespace Direct3D11
{
	UINT GetSemanticIndex(DataSemantics semantics);
	LPCSTR GetSemanticName(DataSemantics semantics);

	void CheckResult(HRESULT hr, const char* msg);
	void SetName(ID3D11DeviceChild* obj, const char* name);
	void Release(IUnknown* obj);

	D3D11_BLEND_OP Map(BlendOperation blendOperation);
	D3D11_BLEND Map(BlendOption blendOption);
	D3D11_COMPARISON_FUNC Map(Comparison comparison);
	byte Map(ColorWriteChannels channels);
	D3D11_CULL_MODE Map(CullMode cullMode);
	D3D11_FILL_MODE Map(FillMode fillMode);
	DXGI_FORMAT Map(IndexSize indexSize);
	D3D11_MAP Map(MapMode mapMode);
	D3D11_PRIMITIVE_TOPOLOGY Map(PrimitiveType primitiveType);
	D3D11_USAGE Map(ResourceUsage resourceUsage);
	D3D11_STENCIL_OP Map(StencilOperation stencilOperation);
	DXGI_FORMAT Map(SurfaceFormat surfaceFormat);
	D3D11_TEXTURE_ADDRESS_MODE Map(TextureAddressMode addressMode);
	D3D11_FILTER Map(TextureFilter textureFilter);
	DXGI_FORMAT Map(VertexDataFormat format);
}

#endif