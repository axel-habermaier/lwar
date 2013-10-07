#ifndef gl3_h__
#define gl3_h__

//====================================================================================================================
// OS-specific includes and definitions
//====================================================================================================================

#include "glExt.h"

#ifdef WINDOWS

	#include "win32.h"
	#include "wglExt.h"

	typedef struct 
	{
		HWND	hwnd;
		HDC		hdc;
		HGLRC	hrc;
		LONG	wndStyle;
		LONG	wndExStyle;
		LONG	x;
		LONG	y;
		LONG	width;
		LONG	height;
		pgBool  maximized;
	} pgContext;

#elif defined(LINUX)

	#include "glxExt.h"

	typedef struct 
	{
		Window			window;
		GLXContext		ctx;
		GLXFBConfig*	configs;
		XVisualInfo*	visuals;
		SizeID			prevMode;
		pgBool			fullscreen;
	} pgContext;

#else
	#error Unsupported operating system
#endif

//====================================================================================================================
// Error handling
//====================================================================================================================

pgVoid pgCheckGLError(pgString file, pgInt32 line);

#ifdef DEBUG
	#define PG_ASSERT_NO_GL_ERRORS() pgCheckGLError(__FILE__, __LINE__)
#else
	#define PG_ASSERT_NO_GL_ERRORS()
#endif

#define PG_GL_ALLOC(type, func, handle)													\
	PG_MULTILINE_MACRO_BEGIN															\
	PG_ASSERT((handle) == 0, "The handle has already been allocated.");					\
	func(1, &(handle));																	\
	PG_ASSERT_NO_GL_ERRORS();															\
	if ((handle) == 0)																	\
		PG_DIE("Failed to allocate an OpenGL object of type '%s'.", type);				\
	PG_MULTILINE_MACRO_END

#define PG_GL_FREE(func, handle)														\
	PG_MULTILINE_MACRO_BEGIN															\
	if ((handle) != 0)																	\
	func(1, &(handle));																	\
	PG_ASSERT_NO_GL_ERRORS();															\
	(handle) = 0;																		\
	PG_MULTILINE_MACRO_END

//====================================================================================================================
// OpenGL context functions
//====================================================================================================================

pgVoid pgCreateContext(pgContext* context);
pgVoid pgDestroyContext(pgContext* context);

pgVoid pgBindContext(pgContext* context, pgGraphicsDevice* device, pgWindow* window);
pgVoid pgDestroyBoundContext(pgContext* context);

pgVoid pgCreateContextWindow(pgContext* context);
pgVoid pgDestroyContextWindow(pgContext* context);
pgVoid pgSetPixelFormat(pgContext* context);
pgBool pgContextFullscreen(pgContext* context, pgInt32 width, pgInt32 height);
pgVoid pgContextWindowed(pgContext* context);

pgVoid pgInitializeContextExtensions(pgContext* context);
pgVoid pgMakeCurrent(pgContext* context);
pgVoid pgSwapBuffers(pgContext* context);

//====================================================================================================================
// OpenGL-specific data types
//====================================================================================================================

#define PG_GRAPHICS_DEVICE_PLATFORM			\
	pgContext		context;				\
	GLenum			glPrimitiveType;		\
	GLuint			pipeline;				\
	/* Begin device state */				\
	GLboolean		depthWritesEnabled;		\
	GLboolean		scissorEnabled;			\
	GLboolean		blendEnabled;			\
	GLboolean		depthTestEnabled;		\
	GLboolean		cullFaceEnabled;		\
	GLboolean		depthClampEnabled;		\
	GLboolean		multisampleEnabled;		\
	GLboolean		stencilTestEnabled;		\
	GLboolean		antialiasedLineEnabled;	\
	pgInt32			activeTexture;			\
	GLenum			cullFace;				\
	GLenum			polygonMode;			\
	pgFloat32		slopeScaledDepthBias;	\
	pgFloat32		depthBiasClamp;			\
	GLenum			frontFace;				\
	GLenum			depthFunc;				\
	pgColor			clearColor;				\
	pgFloat32		depthClear;				\
	pgUint8			stencilClear;			\
	GLenum			blendOperation;			\
	GLenum			blendOperationAlpha;	\
	GLenum			sourceBlend;			\
	GLenum			destinationBlend;		\
	GLenum			sourceBlendAlpha;		\
	GLenum			destinationBlendAlpha;	\
	GLboolean		colorMask[4];			\
	pgRectangle		currentViewport;		\
	pgRectangle		currentScissorArea;	

#define PG_SWAP_CHAIN_PLATFORM \
	pgContext context;

#define PG_SHADER_PLATFORM	\
	GLuint id;				\
	GLenum glType;			\
	GLenum bit;

#define PG_BUFFER_PLATFORM	\
	GLuint	id;				\
	GLenum	glType;			\

#define PG_INPUT_LAYOUT_PLATFORM \
	GLuint			id;					\
	GLenum			indexType;			\
	GLint			indexSizeInBytes;	\
	GLint			indexOffset;

#define PG_TEXTURE_PLATFORM \
	GLuint id;				\
	GLenum glType;			\
	GLenum glBoundType;	

#define PG_RENDER_TARGET_PLATFORM	\
	GLuint			id;				\
	pgSwapChain*	swapChain;

#define PG_BLEND_STATE_PLATFORM \
	pgBlendDesc description;

#define PG_RASTERIZER_STATE_PLATFORM \
	pgRasterizerDesc description;

#define PG_DEPTH_STENCIL_STATE_PLATFORM \
	pgDepthStencilDesc description;

#define PG_SAMPLER_STATE_PLATFORM \
	GLuint id;

#define PG_QUERY_PLATFORM \
	GLuint id;			  \
	GLsync sync;

//====================================================================================================================
// Conversion functions
//====================================================================================================================

GLenum pgConvertBlendOperation(pgBlendOperation blendOperation);
GLenum pgConvertBlendOption(pgBlendOption blendOption);
GLenum pgConvertComparison(pgComparison comparison);
GLenum pgConvertFillMode(pgFillMode fillMode);
GLvoid pgConvertIndexSize(pgIndexSize indexSize, GLenum* type, GLint* size);
GLenum pgConvertMapMode(pgMapMode mapMode);
GLenum pgConvertPrimitiveType(pgPrimitiveType primitiveType);
GLenum pgConvertResourceUsage(pgResourceUsage resourceUsage);
GLenum pgConvertStencilOperation(pgStencilOperation stencilOperation);
GLvoid pgConvertSurfaceFormat(pgSurfaceFormat surfaceFormat, GLenum* internalFormat, GLenum* format);
GLenum pgConvertTextureAddressMode(pgTextureAddressMode addressMode);
GLvoid pgConvertTextureFilter(pgTextureFilter textureFilter, GLenum* minFilter, GLenum* magFilter);
GLvoid pgConvertVertexDataFormat(pgVertexDataFormat vertexDataFormat, GLenum* type, GLsizei* size);
GLenum pgConvertBufferType(pgBufferType bufferType);
GLvoid pgConvertTextureType(pgTextureType textureType, GLenum* type, GLenum* boundType);

//====================================================================================================================
// State change functions
//====================================================================================================================

pgVoid pgChangeActiveTexture(pgGraphicsDevice* device, pgInt32 slot);
pgVoid pgEnableScissor(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableBlend(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableDepthTest(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableCullFace(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableDepthClamp(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableMultisample(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableAntialiasedLine(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableStencilTest(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgEnableDepthWrites(pgGraphicsDevice* device, GLboolean enabled);
pgVoid pgSetCullFace(pgGraphicsDevice* device, GLenum cullFace);
pgVoid pgSetFrontFace(pgGraphicsDevice* device, GLenum frontFace);
pgVoid pgSetPolygonMode(pgGraphicsDevice* device, GLenum mode);
pgVoid pgSetPolygonOffset(pgGraphicsDevice* device, pgFloat32 slopeScaledDepthBias, pgFloat32 depthBiasClamp);
pgVoid pgSetDepthFunc(pgGraphicsDevice* device, GLenum func);
pgVoid pgSetClearColor(pgGraphicsDevice* device, pgColor color);
pgVoid pgSetClearDepth(pgGraphicsDevice* device, pgFloat32 depth);
pgVoid pgSetClearStencil(pgGraphicsDevice* device, pgUint8 stencil);
pgVoid pgSetBlendEquation(pgGraphicsDevice* device, GLenum blendOperation, GLenum blendOperationAlpha);
pgVoid pgSetBlendFuncs(pgGraphicsDevice* device, GLenum sourceBlend, GLenum destinationBlend, GLenum sourceBlendAlpha, GLenum destinationBlendAlpha);
pgVoid pgSetColorMask(pgGraphicsDevice* device, GLboolean mask[4]);

#endif
