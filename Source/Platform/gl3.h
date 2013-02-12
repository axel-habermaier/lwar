#ifndef gl3_h__
#define gl3_h__

//====================================================================================================================
// OS-specific includes and definitions
//====================================================================================================================

#define GLEW_STATIC
#include "glExt.h"

#ifdef WINDOWS

	#include "win32.h"
	#include "wglExt.h"

	typedef struct 
	{
		HWND	hwnd;
		HDC		hdc;
		HGLRC	hrc;
	} pgContext;

#elif defined(LINUX)

	#include "glxExt.h"

	typedef struct 
	{
		Window			window;
		GLXContext		ctx;
		GLXFBConfig*	configs;
		XVisualInfo*	visuals;
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

#define PG_CHECK_GL_HANDLE(type, handle)												\
	PG_MULTILINE_MACRO_BEGIN															\
	PG_ASSERT_NO_GL_ERRORS();															\
	if ((handle) == 0)																	\
		pgDie("Failed to allocate an OpenGL object of type '%s'.", type);				\
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

pgVoid pgInitializeContextExtensions(pgContext* context);
pgVoid pgMakeCurrent(pgContext* context);
pgVoid pgSwapBuffers(pgContext* context);

//====================================================================================================================
// OpenGL-specific data types
//====================================================================================================================

#define PG_GRAPHICS_DEVICE_PLATFORM		\
	pgContext		context;			\
	pgRenderTarget*	renderTarget;		\
	pgInputLayout*	inputLayout;		\
	GLenum			glPrimitiveType;	\
	GLuint			pipeline;

#define PG_SWAP_CHAIN_PLATFORM \
	pgContext context;

#define PG_SHADER_PLATFORM	\
	GLuint id;				\
	GLenum type;			\
	GLenum bit;

#define PG_BUFFER_PLATFORM	\
	GLuint	id;				\
	GLenum	type;			\
	GLsizei size;			\

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
	GLuint id;				

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
GLvoid pgConvertSurfaceFormat(pgSurfaceFormat surfaceFormat, GLenum* internalFormat, GLenum* format, GLint* length);
GLenum pgConvertTextureAddressMode(pgTextureAddressMode addressMode);
GLvoid pgConvertTextureFilter(pgTextureFilter textureFilter, GLenum* minFilter, GLenum* magFilter);
GLvoid pgConvertVertexDataFormat(pgVertexDataFormat vertexDataFormat, GLenum* type, GLsizei* size);
GLvoid pgConvertShaderType(pgShaderType shaderType, GLenum* type, GLenum* bit);
GLenum pgConvertBufferType(pgBufferType bufferType);
GLint pgConvertVertexDataSemantics(pgVertexDataSemantics semantics);
GLvoid pgConvertTextureType(pgTextureType textureType, GLenum* type, GLenum* boundType);

#endif
