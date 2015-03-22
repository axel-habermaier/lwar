#ifndef d3d11_h__
#define d3d11_h__

// Disable all warnings for the D3D headers
#pragma warning(push, 0)

// We're using D3D's C interface
#define CINTERFACE
#define COBJMACROS
#define INITGUID

#include <dxgi.h>
#include <d3d11.h>

#ifdef DEBUG
	struct IDXGI_Debug;
#endif

#pragma warning(pop)

//====================================================================================================================
// Direct3D-specific defines and error handling
//====================================================================================================================

#define PG_DEVICE(obj) (obj)->device->ptr
#define PG_CONTEXT(obj) (obj)->device->context

#define PG_D3DCALL(call, msg)			\
	PG_MULTILINE_MACRO_BEGIN			\
	HRESULT hr = (call);				\
	if (FAILED(hr))						\
		pgWin32DieWithError(msg, hr);	\
	PG_MULTILINE_MACRO_END

#define PG_SAFE_RELEASE(type, obj)  \
	PG_MULTILINE_MACRO_BEGIN		\
	if ((obj) != NULL)				\
		type##_Release(obj);		\
	(obj) = NULL;					\
	PG_MULTILINE_MACRO_END

//====================================================================================================================
// Direct3D-specific data types
//====================================================================================================================

#define PG_GRAPHICS_DEVICE_PLATFORM		\
	ID3D11Device*			ptr;		\
	ID3D11DeviceContext*	context;	\
	IDXGIFactory*			factory;	\
	IDXGIAdapter*			adapter;	\
	IUnknown*				debug;		

#define PG_SWAP_CHAIN_PLATFORM		\
	IDXGISwapChain*			ptr;	\
	DXGI_FORMAT				format;	

#define PG_SHADER_PLATFORM						\
	union {										\
		ID3D11VertexShader*		vertexShader;	\
		ID3D11GeometryShader*	geometryShader;	\
		ID3D11PixelShader*		pixelShader;	\
	} ptr;

#define PG_PROGRAM_PLATFORM

#define PG_BUFFER_PLATFORM	\
	ID3D11Buffer* ptr;

#define PG_INPUT_LAYOUT_PLATFORM							\
	pgBuffer*			indexBuffer;						\
	pgInt32				indexOffset;						\
	DXGI_FORMAT			indexFormat;						\
	pgInputBinding		bindings[PG_INPUT_BINDINGS_COUNT];	\
	pgInt32				bindingsCount;						\
	ID3D11InputLayout*  inputLayout;

#define PG_TEXTURE_PLATFORM					\
	ID3D11Texture2D*			ptr;		\
	ID3D11ShaderResourceView*	resourceView;

#define PG_RENDER_TARGET_PLATFORM								\
	ID3D11RenderTargetView* cbPtr[PG_MAX_COLOR_ATTACHMENTS];	\
	ID3D11DepthStencilView* dsPtr;						

#define PG_BLEND_STATE_PLATFORM \
	ID3D11BlendState* ptr;

#define PG_RASTERIZER_STATE_PLATFORM \
	ID3D11RasterizerState* ptr;

#define PG_DEPTH_STENCIL_STATE_PLATFORM \
	ID3D11DepthStencilState* ptr;

#define PG_SAMPLER_STATE_PLATFORM \
	ID3D11SamplerState* ptr;

#define PG_QUERY_PLATFORM \
	ID3D11Query* ptr;

//====================================================================================================================
// Conversion functions
//====================================================================================================================

D3D11_BLEND_OP pgConvertBlendOperation(pgBlendOperation blendOperation);
D3D11_BLEND pgConvertBlendOption(pgBlendOption blendOption);
D3D11_COMPARISON_FUNC pgConvertComparison(pgComparison comparison);
UINT8 pgConvertColorWriteChannels(pgColorWriteChannels channels);
D3D11_CULL_MODE pgConvertCullMode(pgCullMode cullMode);
D3D11_FILL_MODE pgConvertFillMode(pgFillMode fillMode);
DXGI_FORMAT pgConvertIndexSize(pgIndexSize indexSize);
D3D11_MAP pgConvertMapMode(pgMapMode mapMode);
D3D11_PRIMITIVE_TOPOLOGY pgConvertPrimitiveType(pgPrimitiveType primitiveType);
D3D11_USAGE pgConvertResourceUsage(pgResourceUsage resourceUsage);
D3D11_STENCIL_OP pgConvertStencilOperation(pgStencilOperation stencilOperation);
DXGI_FORMAT pgConvertSurfaceFormat(pgSurfaceFormat surfaceFormat);
D3D11_TEXTURE_ADDRESS_MODE pgConvertTextureAddressMode(pgTextureAddressMode addressMode);
D3D11_FILTER pgConvertTextureFilter(pgTextureFilter textureFilter);
D3D11_BIND_FLAG pgConvertBufferType(pgBufferType bufferType);
pgVoid pgConvertVertexDataSemantics(pgDataSemantics semantics, pgInt32* semanticIndex, pgString* semanticName);
DXGI_FORMAT pgConvertVertexDataFormat(pgVertexDataFormat format);
D3D11_QUERY pgConvertQueryType(pgQueryType type);

#endif