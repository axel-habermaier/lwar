#ifndef prelude_h__
#define prelude_h__

//====================================================================================================================
// Visual Studio specific macros
//====================================================================================================================
#ifdef _MSC_VER

	#define WINDOWS

	// Allows emitting #pragma directives within macros
	#define PG_PRAGMA(pragma)	__pragma(pragma)
	// Disables a specific warning
	#define PG_DISABLE_WARNING(number) PG_PRAGMA(warning(push)) PG_PRAGMA(warning(disable : number))
	// Enables a specific warning that has been disabled by PG_DISABLE_WARNING before
	#define PG_ENABLE_WARNING(number)	PG_PRAGMA(warning(pop))

	// Disable warnings
	#pragma warning(disable : 4514)	// 'function' : unreferenced inline function has been removed
	#pragma warning(disable : 4820)	// 'n' bytes padding added after data member 'member'
	#pragma warning(disable : 4206) // nonstandard extension used: translation unit is empty
	#pragma warning(disable : 4201) // nonstandard extension used: nameless struct/union
	#define _CRT_SECURE_NO_WARNINGS // No warnings about non-secure standard library functions

	#define PG_INLINE __forceinline
	#define PG_NORETURN __declspec(noreturn)

	#ifdef DEBUG	
		#define PG_NO_SWITCH_DEFAULT \
			PG_ASSERT_NOT_REACHED("Default case of switch statement should never be executed.")
		#define PG_DEBUG_BREAK __debugbreak()
	#else	
		#define PG_NO_SWITCH_DEFAULT __assume(0)
		#define PG_DEBUG_BREAK 
	#endif

	// Wrapping a multi-line macro inside the multi-line begin/end macros ensures that the macro can be used like an 
	// ordinary function (i.e. it works correctly within single-line if statements, for instance). Warning 4127 is disabled
	// to avoid the warning about the constant expression inside the while
	#define PG_MULTILINE_MACRO_BEGIN PG_DISABLE_WARNING(4127) do {
	#define PG_MULTILINE_MACRO_END } while (PG_FALSE) PG_ENABLE_WARNING(4127)

//====================================================================================================================
// Linux specific macros
//====================================================================================================================
#elif defined(__GNUC__)

	#define LINUX

	#define PG_INLINE inline
	#define PG_NORETURN __attribute__ ((noreturn))
	#define PG_DEBUG_BREAK 

	#ifdef DEBUG	
		#define PG_NO_SWITCH_DEFAULT \
			PG_ASSERT_NOT_REACHED("Default case of switch statement should never be executed.")
	#else	
		#define PG_NO_SWITCH_DEFAULT __builtin_unreachable()
	#endif

	// Wrapping a multi-line macro inside the multi-line begin/end macros ensures that the macro can be used like an 
	// ordinary function (i.e. it works correctly within single-line if statements, for instance). 
	#define PG_MULTILINE_MACRO_BEGIN do {
	#define PG_MULTILINE_MACRO_END } while (PG_FALSE)

#else
#error Unsupported OS or compiler
#endif

//====================================================================================================================
// Helper and assert macros
//====================================================================================================================

// Can be used to silence warnings about unused parameters
#define PG_UNUSED(x) (void)(x)

// Fails compilation if the condition evaluates to false at compile-time
#define PG_COMPILE_TIME_ASSERT(name, x) typedef int assert_##name[(x) * 2 - 1]

// Allocates an object of the given type and stores the pointer in the given pointer variable. If the allocation
// fails, the application is terminated.
#define PG_ALLOC(type, ptr) \
	PG_MULTILINE_MACRO_BEGIN \
	(ptr) = (type*)malloc(sizeof(type)); \
	if ((ptr) == NULL) \
		PG_DIE("Out of memory: Failed to allocate %d bytes for an object of type '" #type "'.", sizeof(type)); \
	memset((void*)(ptr), 0, sizeof(type)); \
	PG_MULTILINE_MACRO_END

// Allocates an array of the given type and length and stores the pointer in the given pointer variable. If the allocation
// fails, the application is terminated.
#define PG_ALLOC_ARRAY(type, length, ptr) \
	PG_MULTILINE_MACRO_BEGIN \
	(ptr) = (type*)malloc(sizeof(type) * length); \
	if ((ptr) == NULL) \
		PG_DIE("Out of memory: Failed to allocate %d bytes for an array of type '" #type "'.", sizeof(type) * length); \
	memset((void*)(ptr), 0, sizeof(type) * length); \
	PG_MULTILINE_MACRO_END

#define PG_FREE(variable) free((void*)(variable));

#ifdef DEBUG
	#define PG_ASSERT(cond, fmt, ...)										\
		PG_MULTILINE_MACRO_BEGIN											\
		if (!(cond))														\
		{																	\
			PG_ERROR("Assertion '" #cond "' failed. " fmt, ##__VA_ARGS__);	\
			PG_DEBUG_BREAK;													\
			PG_DIE("Aborted due to assertion violation");					\
		}																	\
		PG_MULTILINE_MACRO_END

	#define PG_ASSERT_NOT_REACHED(fmt, ...) PG_DIE(fmt, ##__VA_ARGS__)
	#define PG_ASSERT_NOT_NULL(ptr) PG_ASSERT((ptr) != NULL, "Pointer '" #ptr "' is null.")
	#define PG_ASSERT_NULL(ptr) PG_ASSERT((ptr) == NULL, "Pointer '" #ptr "' must be null.")
	#define PG_ASSERT_IN_RANGE(v, lower, upper) PG_ASSERT(v >= lower && v <= upper, "'" #v "' is out of range.")
#else
	#define PG_ASSERT(cond, fmt, ...) PG_UNUSED(cond)
	#define PG_ASSERT_NOT_REACHED(fmt, ...) 
	#define PG_ASSERT_NOT_NULL(ptr) PG_UNUSED(ptr)
	#define PG_ASSERT_NULL(ptr) PG_UNUSED(ptr)
	#define PG_ASSERT_IN_RANGE(v, lower, upper)	\
		PG_MULTILINE_MACRO_BEGIN				\
		PG_UNUSED(v);							\
		PG_UNUSED(lower);						\
		PG_UNUSED(upper);						\
		PG_MULTILINE_MACRO_END
#endif

//====================================================================================================================
// Standard C library and standard Pegasus includes
//====================================================================================================================
#include <stddef.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>

#include "pg.h"
#include "pgWindow.h"
#include "pgGraphics.h"

//====================================================================================================================
// Pegasus logging, state and internal functions
//====================================================================================================================

typedef struct
{
	pgBool			initialized;
	pgLogCallback	logCallback;
} pgLibraryState;

extern pgLibraryState pgState;

pgString pgFormat(pgString message, ...);
PG_NORETURN pgVoid pgNoReturn();

#define PG_DIE(fmt, ...)													\
	PG_MULTILINE_MACRO_BEGIN												\
	pgState.logCallback(PG_LOGTYPE_FATAL, pgFormat(fmt, ##__VA_ARGS__));    \
	PG_DEBUG_BREAK;															\
	pgNoReturn();															\
	PG_MULTILINE_MACRO_END

#define PG_ERROR(fmt, ...) pgState.logCallback(PG_LOGTYPE_ERROR, pgFormat(fmt, ##__VA_ARGS__))
#define PG_WARN(fmt, ...) pgState.logCallback(PG_LOGTYPE_WARNING, pgFormat(fmt, ##__VA_ARGS__))
#define PG_INFO(fmt, ...) pgState.logCallback(PG_LOGTYPE_INFO, pgFormat(fmt, ##__VA_ARGS__))

#ifdef DEBUG
	#define PG_DEBUG(fmt, ...) pgState.logCallback(PG_LOGTYPE_DEBUG, pgFormat(fmt, ##__VA_ARGS__))
#else
	#define PG_DEBUG(fmt, ...)
#endif

//====================================================================================================================
// Platform-specific includes
//====================================================================================================================

#ifdef WINDOWS
	#include "win32.h"	
	#ifdef OPENGL3
		#include "gl3.h"
	#elif defined(DIRECT3D11)
		#include "d3d11.h"
	#else
		#error Unsupported graphics API
	#endif
#elif defined(LINUX)
	#include "linux.h"	
	#include "gl3.h"
#else
	#error Unsupported compiler, operating system, or operating system/graphics API combination
#endif

//====================================================================================================================
// Window
//====================================================================================================================

#define PG_WINDOW_MIN_OVERLAP 100
#define PG_KEY_COUNT 108 + 1		// Must be max(pgKey) + 1
#define PG_BUTTON_COUNT 5 + 1		// Must be max(pgMouseButton) + 1

struct pgWindow
{
	pgWindowCallbacks	callbacks;
	pgWindowPlacement	placement;
	pgSwapChain*		swapChain;
	pgBool				mouseCaptured;
	pgBool				fullscreen;
	pgBool				focused;
	pgBool				closing;

	pgBool				keyState[PG_KEY_COUNT];
	pgInt32				scanCode[PG_KEY_COUNT];
	pgBool				buttonState[PG_BUTTON_COUNT];
	PG_WINDOW_PLATFORM
};

typedef enum
{
	PG_MESSAGE_INVALID = 0,
	PG_MESSAGE_CLOSING,
	PG_MESSAGE_LOST_FOCUS,
	PG_MESSAGE_GAINED_FOCUS,
	PG_MESSAGE_CHARACTER_ENTERED,
	PG_MESSAGE_KEY_DOWN,
	PG_MESSAGE_KEY_UP,
	PG_MESSAGE_MOUSE_WHEEL,
	PG_MESSAGE_MOUSE_DOWN,
	PG_MESSAGE_MOUSE_UP,
	PG_MESSAGE_MOUSE_MOVED,
	PG_MESSAGE_MOUSE_ENTERED,
	PG_MESSAGE_MOUSE_LEFT
} pgMessageType;

typedef struct
{
	pgMessageType type;
	union {
		pgUint16 character;
		pgKey key;
		pgMouseButton button;
		pgInt32 delta;
	};
	union {
		pgInt32 scanCode;
		struct {
			pgInt32 x;
			pgInt32 y;
		};
	};
	pgBool doubleClick;
} pgMessage;

pgVoid pgOpenWindowCore(pgWindow* window, pgString title);
pgVoid pgCloseWindowCore(pgWindow* window);

pgBool pgProcessWindowEvent(pgWindow* window, pgMessage* message);
pgVoid pgSetWindowTitleCore(pgWindow* window, pgString title);
pgVoid pgGetWindowPlacementCore(pgWindow* window);
pgVoid pgSetWindowSizeCore(pgWindow* window);
pgVoid pgSetWindowPositionCore(pgWindow* window);
pgVoid pgSetWindowModeCore(pgWindow* window);

pgVoid pgCaptureMouseCore(pgWindow* window);
pgVoid pgReleaseMouseCore(pgWindow* window);

pgVoid pgConstrainWindowPlacement(pgWindowPlacement* placement);
pgRectangle pgGetDesktopArea();
pgVoid pgGetMousePositionCore(pgWindow* window, pgInt32* x, pgInt32* y);

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgBool pgRectangleEqual(pgRectangle* r1, pgRectangle* r2);
pgInt32 pgClamp(pgInt32 value, pgInt32 min, pgInt32 max);

//====================================================================================================================
// Graphics device
//====================================================================================================================

#define PG_INPUT_BINDINGS_COUNT			 8
#define PG_TEXTURE_SLOT_COUNT			16
#define PG_SAMPLER_SLOT_COUNT			16
#define PG_CONSTANT_BUFFER_SLOT_COUNT	14
#define PG_MAX_MIPMAPS					16
#define PG_MAX_COLOR_ATTACHMENTS		 4

struct pgGraphicsDevice
{
	pgSamplerState*			samplers[PG_SAMPLER_SLOT_COUNT];
	pgBuffer*				constantBuffers[PG_CONSTANT_BUFFER_SLOT_COUNT];
	pgTexture*				textures[PG_TEXTURE_SLOT_COUNT];
	pgShader*				vertexShader;
	pgShader*				fragmentShader;
	pgPrimitiveType			primitiveType;
	pgDepthStencilState*	depthStencilState;
	pgBlendState*			blendState;
	pgRasterizerState*		rasterizerState;
	pgRectangle				viewport;
	pgRectangle				scissorArea;
	pgRenderTarget*			renderTarget;
	pgInputLayout*			inputLayout;
	PG_GRAPHICS_DEVICE_PLATFORM
};

pgVoid pgCreateGraphicsDeviceCore(pgGraphicsDevice* device);
pgVoid pgDestroyGraphicsDeviceCore(pgGraphicsDevice* device);

pgVoid pgSetViewportCore(pgGraphicsDevice* device, pgRectangle viewport);
pgVoid pgSetScissorAreaCore(pgGraphicsDevice* device, pgRectangle scissorArea);
pgVoid pgSetPrimitiveTypeCore(pgGraphicsDevice* device, pgPrimitiveType primitiveType);

pgVoid pgDrawCore(pgGraphicsDevice* device, pgInt32 primitiveCount, pgInt32 offset);
pgVoid pgDrawIndexedCore(pgGraphicsDevice* device, pgInt32 indexCount, pgInt32 indexOffset, pgInt32 vertexOffset);

pgInt32 pgPrimitiveCountToVertexCount(pgGraphicsDevice* device, pgInt32 primitiveCount);
pgVoid pgPrintDeviceInfoCore(pgGraphicsDevice* device);
pgVoid pgValidateDeviceState(pgGraphicsDevice* device);

//====================================================================================================================
// Shader
//====================================================================================================================

struct pgShader
{
	pgGraphicsDevice*	device;
	pgShaderType		type;
	PG_SHADER_PLATFORM
};

pgVoid pgCreateVertexShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end, pgShaderInput* inputs, pgInt32 inputCount);
pgVoid pgCreateFragmentShaderCore(pgShader* shader, pgUint8* shaderData, pgUint8* end);
pgVoid pgDestroyShaderCore(pgShader* shader);

pgVoid pgBindShaderCore(pgShader* shader);

//====================================================================================================================
// Buffer
//====================================================================================================================

struct pgBuffer
{
	pgGraphicsDevice* device;
	pgInt32 size;
	PG_BUFFER_PLATFORM
};

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgBufferType type, pgResourceUsage usage, pgVoid* data);
pgVoid pgDestroyBufferCore(pgBuffer* buffer);

pgVoid* pgMapBufferCore(pgBuffer* buffer, pgMapMode mode);
pgVoid* pgMapBufferRangeCore(pgBuffer* buffer, pgMapMode mode, pgInt32 offset, pgInt32 byteCount);
pgVoid pgUnmapBufferCore(pgBuffer* buffer);
pgVoid pgBindConstantBufferCore(pgBuffer* buffer, pgInt32 slot);

//====================================================================================================================
// Input layout
//====================================================================================================================

struct pgInputLayout
{
	pgGraphicsDevice* device;
	PG_INPUT_LAYOUT_PLATFORM
};

pgVoid pgCreateInputLayoutCore(pgInputLayout* inputLayout, pgBuffer* indexBuffer, pgInt32 indexOffset, 
							   pgIndexSize indexSize, pgInputBinding* inputBindings, pgInt32 bindingsCount);
pgVoid pgDestroyInputLayoutCore(pgInputLayout* inputLayout);

pgVoid pgBindInputLayoutCore(pgInputLayout* inputLayout);

//====================================================================================================================
// Texture
//====================================================================================================================

struct pgTexture
{
	pgGraphicsDevice*		device;
	pgTextureDescription	desc;
	PG_TEXTURE_PLATFORM
};

pgVoid pgCreateTextureCore(pgTexture* texture, pgSurface* surfaces);
pgVoid pgDestroyTextureCore(pgTexture* texture);

pgVoid pgBindTextureCore(pgTexture* texture, pgInt32 slot);
pgVoid pgGenerateMipmapsCore(pgTexture* texture);

pgBool pgIsCompressedFormat(pgSurfaceFormat format);
pgBool pgIsFloatingPointFormat(pgSurfaceFormat format);
pgBool pgIsDepthStencilFormat(pgSurfaceFormat format);

//====================================================================================================================
// Render target
//====================================================================================================================

struct pgRenderTarget
{
	pgGraphicsDevice*	device;
	pgInt32				width;
	pgInt32				height;
	pgTexture*			colorBuffers[PG_MAX_COLOR_ATTACHMENTS];
	pgInt32				count;
	pgTexture*			depthStencil;
	PG_RENDER_TARGET_PLATFORM
};

pgVoid pgCreateRenderTargetCore(pgRenderTarget* renderTarget);
pgVoid pgDestroyRenderTargetCore(pgRenderTarget* renderTarget);

pgVoid pgClearColorCore(pgRenderTarget* renderTarget, pgColor color);
pgVoid pgClearDepthStencilCore(pgRenderTarget* renderTarget, pgBool clearDepth, pgBool clearStencil, pgFloat32 depth, pgUint8 stencil);
pgVoid pgBindRenderTargetCore(pgRenderTarget* renderTarget);

//====================================================================================================================
// Blend state
//====================================================================================================================

struct pgBlendState
{
	pgGraphicsDevice* device;
	PG_BLEND_STATE_PLATFORM
};

pgVoid pgCreateBlendStateCore(pgBlendState* blendState, pgBlendDesc* description);
pgVoid pgDestroyBlendStateCore(pgBlendState* blendState);

pgVoid pgBindBlendStateCore(pgBlendState* blendState);

//====================================================================================================================
// Rasterizer state
//====================================================================================================================

struct pgRasterizerState
{
	pgGraphicsDevice* device;
	PG_RASTERIZER_STATE_PLATFORM
};

pgVoid pgCreateRasterizerStateCore(pgRasterizerState* rasterizerState, pgRasterizerDesc* description);
pgVoid pgDestroyRasterizerStateCore(pgRasterizerState* rasterizerState);

pgVoid pgBindRasterizerStateCore(pgRasterizerState* rasterizerState);

//====================================================================================================================
// Depth stencil state
//====================================================================================================================

struct pgDepthStencilState
{
	pgGraphicsDevice* device;
	PG_DEPTH_STENCIL_STATE_PLATFORM
};

pgVoid pgCreateDepthStencilStateCore(pgDepthStencilState* depthStencilState, pgDepthStencilDesc* description);
pgVoid pgDestroyDepthStencilStateCore(pgDepthStencilState* depthStencilState);

pgVoid pgBindDepthStencilStateCore(pgDepthStencilState* depthStencilState);

//====================================================================================================================
// Sampler state
//====================================================================================================================

struct pgSamplerState
{
	pgGraphicsDevice* device;
	PG_SAMPLER_STATE_PLATFORM
};

pgVoid pgCreateSamplerStateCore(pgSamplerState* samplerState, pgSamplerDesc* description);
pgVoid pgDestroySamplerStateCore(pgSamplerState* samplerState);

pgVoid pgBindSamplerStateCore(pgSamplerState* samplerState, pgInt32 slot);

//====================================================================================================================
// Query
//====================================================================================================================

struct pgQuery
{
	pgGraphicsDevice*	device;
	pgBool				isActive;
	pgQueryType			type;
	PG_QUERY_PLATFORM
};

pgVoid pgCreateQueryCore(pgQuery* query);
pgVoid pgDestroyQueryCore(pgQuery* query);

pgVoid pgBeginQueryCore(pgQuery* query);
pgVoid pgEndQueryCore(pgQuery* query);

pgVoid pgGetQueryDataCore(pgQuery* query, pgVoid* data, pgInt32 size);
pgBool pgIsQueryDataAvailableCore(pgQuery* query);

//====================================================================================================================
// Swap chain
//====================================================================================================================

struct pgSwapChain
{
	pgGraphicsDevice*	device;
	pgRenderTarget		renderTarget;
	pgWindow*			window;
	pgBool				fullscreen;
	pgInt32				windowedWidth;
	pgInt32				windowedHeight;
	pgInt32				fullscreenWidth;
	pgInt32				fullscreenHeight;
	PG_SWAP_CHAIN_PLATFORM
};

pgVoid pgCreateSwapChainCore(pgSwapChain* swapChain, pgWindow* window);
pgVoid pgDestroySwapChainCore(pgSwapChain* swapChain);

pgVoid pgPresentCore(pgSwapChain* swapChain);
pgBool pgSwapChainFullscreenCore(pgSwapChain* swapChain, pgInt32 width, pgInt32 height);
pgVoid pgSwapChainWindowedCore(pgSwapChain* swapChain);

pgVoid pgResizeSwapChain(pgSwapChain* swapChain, pgInt32 width, pgInt32 height);
pgVoid pgResizeSwapChainCore(pgSwapChain* swapChain, pgInt32 width, pgInt32 height);

#endif
