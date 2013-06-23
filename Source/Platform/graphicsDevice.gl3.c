#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Local state and helper functions
//====================================================================================================================

static pgBool areContextExtsInitialized;
static pgBool areOpenGLExtsInitialized;

static pgBool GlExtSupported(int extension, pgString extensionName);
static pgVoid InitializeOpenGLExtensions();
static pgInt32 FlipY(pgGraphicsDevice* device, pgInt32 top, pgInt32 height);

#ifdef DEBUG
static void APIENTRY DebugCallback(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, const GLchar* message, GLvoid* userParam);
#endif

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateGraphicsDeviceCore(pgGraphicsDevice* device)
{
	// Invalidate the cached device state
	size_t offset = offsetof(pgGraphicsDevice, depthWritesEnabled);
	memset((pgByte*)device + offset, -1, sizeof(pgGraphicsDevice) - offset);

	pgCreateContextWindow(&device->context);
	pgSetPixelFormat(&device->context);

	if (!areContextExtsInitialized)
	{
		pgInitializeContextExtensions(&device->context);
		areContextExtsInitialized = PG_TRUE;
	}

	pgCreateContext(&device->context);
	pgMakeCurrent(&device->context);

	if (!areOpenGLExtsInitialized)
	{
		InitializeOpenGLExtensions();
		areOpenGLExtsInitialized = PG_TRUE;
	}

	PG_GL_ALLOC("Program Pipeline", glGenProgramPipelines, device->pipeline);
	glBindProgramPipeline(device->pipeline);
	PG_ASSERT_NO_GL_ERRORS();

#ifdef DEBUG
	glDebugMessageCallbackARB(&DebugCallback, NULL);
	glEnable(GL_DEBUG_OUTPUT_SYNCHRONOUS_ARB);
	PG_ASSERT_NO_GL_ERRORS();
#endif
}

pgVoid pgDestroyGraphicsDeviceCore(pgGraphicsDevice* device)
{
	PG_GL_FREE(glDeleteProgramPipelines, device->pipeline);

	pgDestroyContext(&device->context);
	pgDestroyContextWindow(&device->context);
}

pgVoid pgSetViewportCore(pgGraphicsDevice* device, pgRectangle viewport)
{
	viewport.top = FlipY(device, viewport.top, viewport.height);
	if (pgRectangleEqual(&viewport, &device->currentViewport))
		return;

	device->currentViewport = viewport;
	glViewport(viewport.left, viewport.top, viewport.width, viewport.height);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgSetScissorAreaCore(pgGraphicsDevice* device, pgRectangle scissorArea)
{
	scissorArea.top = FlipY(device, scissorArea.top, scissorArea.height);
	if (pgRectangleEqual(&scissorArea, &device->currentScissorArea))
		return;

	device->currentScissorArea = scissorArea;
	glScissor(scissorArea.left, scissorArea.top, scissorArea.width, scissorArea.height);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgSetPrimitiveTypeCore(pgGraphicsDevice* device, pgPrimitiveType primitiveType)
{
	device->glPrimitiveType = pgConvertPrimitiveType(primitiveType);
}

pgVoid pgDrawCore(pgGraphicsDevice* device, pgInt32 primitiveCount, pgInt32 offset)
{
	glDrawArrays(device->glPrimitiveType, offset, pgPrimitiveCountToVertexCount(device, primitiveCount));
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgDrawIndexedCore(pgGraphicsDevice* device, pgInt32 indexCount, pgInt32 indexOffset, pgInt32 vertexOffset)
{
	pgVoid* offset = (pgVoid*)(size_t)((indexOffset + device->inputLayout->indexOffset) * device->inputLayout->indexSizeInBytes);
	glDrawElementsBaseVertex(device->glPrimitiveType, indexCount, device->inputLayout->indexType, offset, vertexOffset);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgPrintDeviceInfoCore(pgGraphicsDevice* device)
{
	pgMakeCurrent(&device->context);

	PG_INFO("OpenGL renderer: %s (%s)", glGetString(GL_RENDERER), glGetString(GL_VENDOR));
	PG_INFO("OpenGL version: %s", glGetString(GL_VERSION));
	PG_INFO("OpenGL GLSL version: %s", glGetString(GL_SHADING_LANGUAGE_VERSION));

	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgBool GlExtSupported(int extension, pgString extensionName)
{
	if (extension != ogl_LOAD_SUCCEEDED)
	{
		PG_ERROR("Extension '%s' is not supported.", extensionName);
		return PG_FALSE;
	}

	return PG_TRUE;
}

static pgVoid InitializeOpenGLExtensions()
{
	GLint major, minor;
	pgBool glExtsSupported = PG_TRUE;

	if (ogl_LoadFunctions() == ogl_LOAD_FAILED)
		PG_DIE("OpenGL initialization failed.");

	glGetIntegerv(GL_MAJOR_VERSION, &major);
	glGetIntegerv(GL_MINOR_VERSION, &minor);

	if (major < 3 || (major == 3 && minor < 3))
		PG_DIE("Only OpenGL %d.%d seems to be supported. OpenGL 3.3 is required.", major, minor);

	glExtsSupported &= GlExtSupported(ogl_ext_ARB_sampler_objects, "ARB_sampler_objects");
	glExtsSupported &= GlExtSupported(ogl_ext_ARB_separate_shader_objects, "ARB_separate_shader_objects");
	glExtsSupported &= GlExtSupported(ogl_ext_ARB_shading_language_420pack, "ARB_shading_language_420pack");
	glExtsSupported &= GlExtSupported(ogl_ext_EXT_texture_filter_anisotropic, "EXT_texture_filter_anisotropic");
	glExtsSupported &= GlExtSupported(ogl_ext_EXT_texture_compression_s3tc, "EXT_texture_compression_s3tc");
	glExtsSupported &= GlExtSupported(ogl_ext_EXT_direct_state_access, "EXT_direct_state_access");
#ifdef DEBUG
	glExtsSupported &= GlExtSupported(ogl_ext_ARB_debug_output, "ARB_debug_output");
#endif

	if (!glExtsSupported)
		PG_DIE("Incompatible graphics card. Not all required OpenGL extenions are supported.");
}

static pgInt32 FlipY(pgGraphicsDevice* device, pgInt32 top, pgInt32 height)
{
	return device->renderTarget->height - height - top;
}

#ifdef DEBUG
static void APIENTRY DebugCallback(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, const GLchar* message, GLvoid* userParam)
{
	PG_UNUSED(source);
	PG_UNUSED(id);
	PG_UNUSED(severity);
	PG_UNUSED(length);
	PG_UNUSED(userParam);

	switch (type)
	{
	case GL_DEBUG_TYPE_ERROR_ARB:
	case GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR_ARB:
	case GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR_ARB:
		PG_ERROR("OpenGL error: %s", message);
		return;
	default:
		PG_WARN("OpenGL info: %s", message);
	}
}
#endif

#endif
