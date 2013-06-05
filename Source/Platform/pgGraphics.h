#ifndef pgGraphics_h__
#define pgGraphics_h__

#include "pg.h"

//====================================================================================================================
// Graphics types
//====================================================================================================================

typedef struct pgWindow					pgWindow;
typedef struct pgGraphicsDevice			pgGraphicsDevice;
typedef struct pgSwapChain				pgSwapChain;
typedef struct pgShader					pgShader;
typedef struct pgBuffer					pgBuffer;
typedef struct pgInputLayout			pgInputLayout;
typedef struct pgTexture				pgTexture;
typedef struct pgRenderTarget			pgRenderTarget;
typedef struct pgBlendState				pgBlendState;
typedef struct pgRasterizerState		pgRasterizerState;
typedef struct pgDepthStencilState		pgDepthStencilState;
typedef struct pgSamplerState			pgSamplerState;
typedef struct pgQuery					pgQuery;

typedef struct
{
	pgUint8 red;
	pgUint8 green;
	pgUint8 blue;
	pgUint8 alpha;
} pgColor;

typedef enum
{
	PG_API_OPENGL_3							= 701,
	PG_API_DIRECT3D_11						= 702
} pgGraphicsApi;

typedef enum
{
	PG_BLEND_OP_ADD							= 1001,
	PG_BLEND_OP_SUBTRACT					= 1002,
	PG_BLEND_OP_REVERSE_SUBTRACT			= 1003,
	PG_BLEND_OP_MINIMUM						= 1004,
	PG_BLEND_OP_MAXIMUM						= 1005
} pgBlendOperation;

typedef enum
{
	PG_BLEND_ZERO							= 1101,
	PG_BLEND_ONE							= 1102,
	PG_BLEND_SOURCE_COLOR					= 1103,
	PG_BLEND_INVERSE_SOURCE_COLOR			= 1104,
	PG_BLEND_SOURCE_ALPHA					= 1105,
	PG_BLEND_INVERSE_SOURCE_ALPHA			= 1106,
	PG_BLEND_DESTINATION_ALPHA				= 1107,
	PG_BLEND_INVERSE_DESTINATION_ALPHA		= 1108,
	PG_BLEND_DESTINATION_COLOR				= 1109,
	PG_BLEND_INVERSE_DESTINATION_COLOR		= 1110,
	PG_BLEND_SOURCE_ALPHA_SATURATE			= 1111,
	PG_BLEND_BLEND_FACTOR					= 1112,
	PG_BLEND_INVERSE_BLEND_FACTOR			= 1113
} pgBlendOption;

typedef enum
{
	PG_COLOR_WRITE_ENABLE_NONE				= 0,
	PG_COLOR_WRITE_ENABLE_RED				= 1,
	PG_COLOR_WRITE_ENABLE_GREEN				= 2,
	PG_COLOR_WRITE_ENABLE_BLUE				= 4,
	PG_COLOR_WRITE_ENABLE_ALPHA				= 8,
	PG_COLOR_WRITE_ENABLE_ALL				= PG_COLOR_WRITE_ENABLE_RED | PG_COLOR_WRITE_ENABLE_GREEN |
											  PG_COLOR_WRITE_ENABLE_BLUE | PG_COLOR_WRITE_ENABLE_ALPHA
} pgColorWriteChannels;

typedef enum
{
	PG_COMPARISON_NEVER						= 1201,
	PG_COMPARISON_ALWAYS					= 1202,
	PG_COMPARISON_LESS						= 1203,
	PG_COMPARISON_LESS_EQUAL				= 1204,
	PG_COMPARISON_GREATER					= 1205,
	PG_COMPARISON_GREATER_EQUAL				= 1206,
	PG_COMPARISON_EQUAL						= 1207,
	PG_COMPARISON_NOT_EQUAL					= 1208
} pgComparison;

typedef enum
{
	PG_CULL_NONE							= 1301,
	PG_CULL_FRONT							= 1302,
	PG_CULL_BACK							= 1303
} pgCullMode;

typedef enum
{
	PG_FILL_WIREFRAME						= 1501,
	PG_FILL_SOLID							= 1502
} pgFillMode;

typedef enum
{
	PG_INDEX_SIZE_16						= 1601,
	PG_INDEX_SIZE_32						= 1602
} pgIndexSize;

typedef enum
{
	PG_MAP_READ								= 1701,
	PG_MAP_WRITE							= 1702,
	PG_MAP_READ_WRITE						= 1703,
	PG_MAP_WRITE_DISCARD					= 1704,
	PG_MAP_WRITE_NO_OVERWRITE				= 1705
} pgMapMode;

typedef enum
{
	PG_PRIMITIVE_TRIANGLES					= 1801,
	PG_PRIMITIVE_TRIANGLESTRIP				= 1802,
	PG_PRIMITIVE_LINES						= 1803,
	PG_PRIMITIVE_LINESTRIP					= 1804
} pgPrimitiveType;

typedef enum
{
	PG_USAGE_DEFAULT						= 1900,
	PG_USAGE_STATIC							= 1901,
	PG_USAGE_DYNAMIC						= 1902,
	PG_USAGE_STAGING						= 1903
} pgResourceUsage;

typedef enum
{
	PG_STENCIL_OP_KEEP						= 2001,
	PG_STENCIL_OP_ZERO						= 2002,
	PG_STENCIL_OP_REPLACE					= 2003,
	PG_STENCIL_OP_INCREMENT_AND_CLAMP		= 2004,
	PG_STENCIL_OP_DECREMENT_AND_CLAMP		= 2005,
	PG_STENCIL_OP_INVERT					= 2006,
	PG_STENCIL_OP_INCREMENT					= 2007,
	PG_STENCIL_OP_DECREMENT					= 2008
} pgStencilOperation;

typedef enum
{
	PG_SURFACE_RGBA8						= 2101,
	PG_SURFACE_R8							= 2102,
	PG_SURFACE_DEPTH24_STENCIL8				= 2103,
	PG_SURFACE_RGBA16F						= 2121,
	PG_SURFACE_BC1							= 2151,
	PG_SURFACE_BC2							= 2152,
	PG_SURFACE_BC3							= 2153,
	PG_SURFACE_BC4							= 2154,
	PG_SURFACE_BC5							= 2155
} pgSurfaceFormat;

typedef enum
{
	PG_TEXTURE_ADDRESS_CLAMP				= 2201,
	PG_TEXTURE_ADDRESS_WRAP					= 2202,
	PG_TEXTURE_ADDRESS_BORDER				= 2203
} pgTextureAddressMode;

typedef enum
{
	PG_TEXTURE_FILTER_NEAREST				= 2301,
	PG_TEXTURE_FILTER_BILINEAR				= 2302,
	PG_TEXTURE_FILTER_TRILINEAR				= 2303,
	PG_TEXTURE_FILTER_ANISOTROPIC			= 2304,
	PG_TEXTURE_FILTER_NEAREST_NO_MIPMAPS	= 2305,
	PG_TEXTURE_FILTER_BILINEAR_NO_MIPMAPS	= 2306,
} pgTextureFilter;

typedef enum
{
	PG_VERTEX_FORMAT_SINGLE					= 100,
	PG_VERTEX_FORMAT_VECTOR2				= 101,
	PG_VERTEX_FORMAT_VECTOR3				= 102,
	PG_VERTEX_FORMAT_VECTOR4				= 103,
	PG_VERTEX_FORMAT_COLOR					= 104
} pgVertexDataFormat;

typedef enum
{
	PG_VERTEX_SEMANTICS_POSITION			= 0,
	PG_VERTEX_SEMANTICS_COLOR0				= 1,
	PG_VERTEX_SEMANTICS_COLOR1				= 2,
	PG_VERTEX_SEMANTICS_COLOR2				= 3,
	PG_VERTEX_SEMANTICS_COLOR3				= 4,
	PG_VERTEX_SEMANTICS_TEXCOORDS0			= 5,
	PG_VERTEX_SEMANTICS_TEXCOORDS1			= 6,
	PG_VERTEX_SEMANTICS_TEXCOORDS2			= 7,
	PG_VERTEX_SEMANTICS_TEXCOORDS3			= 8,
	PG_VERTEX_SEMANTICS_NORMAL				= 9,
} pgDataSemantics;

typedef enum
{
	PG_VERTEX_SHADER						= 2601,
	PG_FRAGMENT_SHADER						= 2602,
	PG_GEOMETRY_SHADER						= 2603
} pgShaderType;

typedef enum
{
	PG_CONSTANT_BUFFER						= 2701,
	PG_VERTEX_BUFFER						= 2702,
	PG_INDEX_BUFFER							= 2703
} pgBufferType;

typedef enum
{
	PG_TIMESTAMP_QUERY						= 2801,
	PG_TIMESTAMP_DISJOINT_QUERY				= 2802,
	PG_OCCLUSION_QUERY						= 2803
} pgQueryType;

typedef enum
{
	PG_TEXTURE_1D							= 2901,
	PG_TEXTURE_2D							= 2902,
	PG_TEXTURE_CUBE_MAP						= 2903,
	PG_TEXTURE_3D							= 2904
} pgTextureType;

typedef enum
{
	PG_TEXTURE_GENERATE_MIPMAPS				= 1,
	PG_TEXTURE_BIND_RENDER_TARGET			= 2,
	PG_TEXTURE_BIND_DEPTH_STENCIL			= 4,
} pgTextureFlags;

typedef enum
{
	PG_DEPTH_STENCIL_ATTACHMENT				= 3001,
	PG_COLOR_ATTACHMENT_0					= 3002,
	PG_COLOR_ATTACHMENT_1					= 3003,
	PG_COLOR_ATTACHMENT_2					= 3004,
	PG_COLOR_ATTACHMENT_3					= 3005,
} pgAttachmentPoint;

typedef struct
{
	pgUint64	frequency;
	pgBool		valid;
} pgTimestampDisjointQueryData;

typedef struct
{
	pgVertexDataFormat		format;
	pgDataSemantics			semantics;
} pgShaderInput;

typedef struct
{
	pgVertexDataFormat		format;
	pgDataSemantics			semantics;
	pgInt32					stride;
	pgInt32					offset;
	pgBuffer*				vertexBuffer;
} pgInputBinding;

typedef struct
{
	pgBlendOperation		blendOperation;
	pgBlendOperation		blendOperationAlpha;
	pgBlendOption			destinationBlend;
	pgBlendOption			destinationBlendAlpha;
	pgBlendOption			sourceBlend;
	pgBlendOption			sourceBlendAlpha;
	pgColorWriteChannels	writeMask;
	pgBool					blendEnabled;
} pgBlendDesc;

typedef struct
{
	pgStencilOperation	fail;
	pgStencilOperation	depthFail;
	pgStencilOperation	pass;
	pgComparison		func;
} pgStencilOpDesc;

typedef struct
{
	pgStencilOpDesc backFace;
	pgStencilOpDesc frontFace;
	pgComparison	depthFunction;
	pgBool			depthEnabled;
	pgBool			depthWriteEnabled;
	pgBool			stencilEnabled;
	pgUint8			stencilReadMask;
	pgUint8			stencilWriteMask;
} pgDepthStencilDesc;

typedef struct
{
	pgFillMode		fillMode;
	pgCullMode		cullMode;
	pgInt32			depthBias;
	pgFloat32		depthBiasClamp;
	pgFloat32		slopeScaledDepthBias;
	pgBool			antialiasedLineEnabled;
	pgBool			depthClipEnabled;
	pgBool			frontIsCounterClockwise;
	pgBool			multisampleEnabled;
	pgBool			scissorEnabled;
} pgRasterizerDesc;

typedef struct
{
	pgTextureAddressMode	addressU;
    pgTextureAddressMode	addressV;
    pgTextureAddressMode	addressW;
    pgColor					borderColor;
    pgComparison			comparison;
    pgTextureFilter			filter;
    pgInt32					maximumAnisotropy;
    pgFloat32				maximumLod;
    pgFloat32				minimumLod;
    pgFloat32				mipLodBias;
} pgSamplerDesc;

typedef struct
{
	pgUint32				width;
	pgUint32				height;
	pgUint32				depth;
	pgUint32				arraySize;
	pgTextureType			type;
	pgSurfaceFormat			format;
	pgUint32				mipmaps;
	pgUint32				surfaceCount;
	pgTextureFlags			flags;
} pgTextureDescription;

typedef struct
{
	pgUint32				width;
	pgUint32				height;
	pgUint32				depth;
	pgUint32				size;
	pgUint32				stride;
	pgUint8*				data;
} pgSurface;

typedef struct
{
	pgInt32 left;
	pgInt32 top;
	pgInt32 width;
	pgInt32 height;
} pgRectangle;

//====================================================================================================================
// Graphics functions
//====================================================================================================================

PG_API_EXPORT pgGraphicsApi pgGetGraphicsApi();

//====================================================================================================================
// Graphics device functions
//====================================================================================================================

PG_API_EXPORT pgGraphicsDevice* pgCreateGraphicsDevice();
PG_API_EXPORT pgVoid pgDestroyGraphicsDevice(pgGraphicsDevice* device);

PG_API_EXPORT pgVoid pgSetViewport(pgGraphicsDevice* device, pgRectangle viewport);
PG_API_EXPORT pgVoid pgSetScissorArea(pgGraphicsDevice* device, pgRectangle scissorArea);
PG_API_EXPORT pgVoid pgSetPrimitiveType(pgGraphicsDevice* device, pgPrimitiveType primitiveType);

PG_API_EXPORT pgVoid pgDraw(pgGraphicsDevice* device, pgInt32 primitiveCount, pgInt32 offset);
PG_API_EXPORT pgVoid pgDrawIndexed(pgGraphicsDevice* device, pgInt32 indexCount, pgInt32 indexOffset, pgInt32 vertexOffset);

//====================================================================================================================
// Swap chain functions
//====================================================================================================================

PG_API_EXPORT pgSwapChain* pgCreateSwapChain(pgGraphicsDevice* device, pgWindow* window);
PG_API_EXPORT pgVoid pgDestroySwapChain(pgSwapChain* swapChain);

PG_API_EXPORT pgVoid pgPresent(pgSwapChain* swapChain);
PG_API_EXPORT pgRenderTarget* pgGetBackBuffer(pgSwapChain* swapChain);
PG_API_EXPORT pgBool pgUpdateSwapChainState(pgSwapChain* swapChain, pgInt32 width, pgInt32 height, pgBool fullscreen);

//====================================================================================================================
// Shader functions
//====================================================================================================================

PG_API_EXPORT pgShader* pgCreateVertexShader(pgGraphicsDevice* device, pgVoid* shaderData, pgInt32 length, pgShaderInput* inputs, pgInt32 inputCount);
PG_API_EXPORT pgShader* pgCreateFragmentShader(pgGraphicsDevice* device, pgVoid* shaderData, pgInt32 length);
PG_API_EXPORT pgVoid pgDestroyShader(pgShader* shader);

PG_API_EXPORT pgVoid pgBindShader(pgShader* shader);

//====================================================================================================================
// Buffer functions
//====================================================================================================================

PG_API_EXPORT pgBuffer* pgCreateBuffer(pgGraphicsDevice* device, pgBufferType type, pgResourceUsage usage, pgVoid* data, pgInt32 size);
PG_API_EXPORT pgVoid pgDestroyBuffer(pgBuffer* buffer);

PG_API_EXPORT pgVoid* pgMapBuffer(pgBuffer* buffer, pgMapMode mode);
PG_API_EXPORT pgVoid pgUnmapBuffer(pgBuffer* buffer);
PG_API_EXPORT pgVoid pgBindConstantBuffer(pgBuffer* buffer, pgInt32 slot);

//====================================================================================================================
// Input layout functions
//====================================================================================================================

PG_API_EXPORT pgInputLayout* pgCreateInputLayout(pgGraphicsDevice* device, pgBuffer* indexBuffer, pgInt32 indexOffset, 
												 pgIndexSize indexSize, pgInputBinding* inputBindings, pgInt32 bindingsCount);
PG_API_EXPORT pgVoid pgDestroyInputLayout(pgInputLayout* inputLayout);

PG_API_EXPORT pgVoid pgBindInputLayout(pgInputLayout* inputLayout);

//====================================================================================================================
// Texture functions
//====================================================================================================================

PG_API_EXPORT pgTexture* pgCreateTexture(pgGraphicsDevice* device, pgTextureDescription* description, pgSurface* surfaces);
PG_API_EXPORT pgVoid pgDestroyTexture(pgTexture* texture);

PG_API_EXPORT pgVoid pgBindTexture(pgTexture* texture, pgInt32 slot);
PG_API_EXPORT pgVoid pgGenerateMipmaps(pgTexture* texture);

//====================================================================================================================
// Render target functions
//====================================================================================================================

PG_API_EXPORT pgRenderTarget* pgCreateRenderTarget(pgGraphicsDevice* device, pgTexture** colorBuffers, pgInt32 count, pgTexture* depthStencil);
PG_API_EXPORT pgVoid pgDestroyRenderTarget(pgRenderTarget* renderTarget);

PG_API_EXPORT pgVoid pgClearColor(pgRenderTarget* renderTarget, pgColor color);
PG_API_EXPORT pgVoid pgClearDepthStencil(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil);
PG_API_EXPORT pgVoid pgBindRenderTarget(pgRenderTarget* renderTarget);

//====================================================================================================================
// Blend state functions
//====================================================================================================================

PG_API_EXPORT pgBlendState* pgCreateBlendState(pgGraphicsDevice* device, pgBlendDesc* description);
PG_API_EXPORT pgVoid pgDestroyBlendState(pgBlendState* blendState);

PG_API_EXPORT pgVoid pgBindBlendState(pgBlendState* blendState);
PG_API_EXPORT pgVoid pgSetBlendDescDefaults(pgBlendDesc* description);

//====================================================================================================================
// Depth stencil state functions
//====================================================================================================================

PG_API_EXPORT pgDepthStencilState* pgCreateDepthStencilState(pgGraphicsDevice* device, pgDepthStencilDesc* description);
PG_API_EXPORT pgVoid pgDestroyDepthStencilState(pgDepthStencilState* depthStencilState);

PG_API_EXPORT pgVoid pgBindDepthStencilState(pgDepthStencilState* depthStencilState);
PG_API_EXPORT pgVoid pgSetDepthStencilDescDefaults(pgDepthStencilDesc* description);

//====================================================================================================================
// Rasterizer state functions
//====================================================================================================================

PG_API_EXPORT pgRasterizerState* pgCreateRasterizerState(pgGraphicsDevice* device, pgRasterizerDesc* description);
PG_API_EXPORT pgVoid pgDestroyRasterizerState(pgRasterizerState* rasterizerState);

PG_API_EXPORT pgVoid pgBindRasterizerState(pgRasterizerState* rasterizerState);
PG_API_EXPORT pgVoid pgSetRasterizerDescDefaults(pgRasterizerDesc* description);

//====================================================================================================================
// Sampler state functions
//====================================================================================================================

PG_API_EXPORT pgSamplerState* pgCreateSamplerState(pgGraphicsDevice* device, pgSamplerDesc* description);
PG_API_EXPORT pgVoid pgDestroySamplerState(pgSamplerState* samplerState);

PG_API_EXPORT pgVoid pgBindSamplerState(pgSamplerState* samplerState, pgInt32 slot);
PG_API_EXPORT pgVoid pgSetSamplerDescDefaults(pgSamplerDesc* description);

//====================================================================================================================
// Query functions
//====================================================================================================================

PG_API_EXPORT pgQuery* pgCreateQuery(pgGraphicsDevice* device, pgQueryType type);
PG_API_EXPORT pgVoid pgDestroyQuery(pgQuery* query);

PG_API_EXPORT pgVoid pgBeginQuery(pgQuery* query);
PG_API_EXPORT pgVoid pgEndQuery(pgQuery* query);

PG_API_EXPORT pgVoid pgGetQueryData(pgQuery* query, pgVoid* data, pgInt32 size);
PG_API_EXPORT pgBool pgIsQueryDataAvailable(pgQuery* query);

//====================================================================================================================
// Set debug names functions
//====================================================================================================================

#ifdef DEBUG
PG_API_EXPORT pgVoid pgSetTextureName(pgTexture* texture, pgString name);
PG_API_EXPORT pgVoid pgSetShaderName(pgShader* shader, pgString name);
PG_API_EXPORT pgVoid pgSetBufferName(pgBuffer* buffer, pgString name);
PG_API_EXPORT pgVoid pgSetRenderTargetName(pgRenderTarget* renderTarget, pgString name);
PG_API_EXPORT pgVoid pgSetBlendStateName(pgBlendState* blendState, pgString name);
PG_API_EXPORT pgVoid pgSetDepthStencilStateName(pgDepthStencilState* depthStencilState, pgString name);
PG_API_EXPORT pgVoid pgSetRasterizerStateName(pgRasterizerState* rasterizerState, pgString name);
PG_API_EXPORT pgVoid pgSetSamplerStateName(pgSamplerState* samplerState, pgString name);
PG_API_EXPORT pgVoid pgSetQueryName(pgQuery* query, pgString name);
#endif

#endif