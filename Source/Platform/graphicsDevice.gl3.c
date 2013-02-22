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

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateGraphicsDeviceCore(pgGraphicsDevice* device)
{
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

	glGenProgramPipelines(1, &device->pipeline);
	PG_CHECK_GL_HANDLE("Program Pipeline", device->pipeline);

	glBindProgramPipeline(device->pipeline);
	PG_ASSERT_NO_GL_ERRORS();

	//glEnable(GL_TEXTURE_CUBE_MAP_SEAMLESS);
}

pgVoid pgDestroyGraphicsDeviceCore(pgGraphicsDevice* device)
{
	glDeleteProgramPipelines(1, &device->pipeline);
	PG_ASSERT_NO_GL_ERRORS();

	pgDestroyContext(&device->context);
	pgDestroyContextWindow(&device->context);
}

pgVoid pgSetViewportCore(pgGraphicsDevice* device, pgInt32 left, pgInt32 top, pgInt32 width, pgInt32 height)
{
	top = FlipY(device, top, height);
	glViewport(left, top, width, height);
	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgSetScissorRectCore(pgGraphicsDevice* device, pgInt32 left, pgInt32 top, pgInt32 width, pgInt32 height)
{
	top = FlipY(device, top, height);
	glScissor(left, top, width, height);
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
	void* offset = (void*)(size_t)((indexOffset + device->state.inputLayout->indexOffset) * device->state.inputLayout->indexSizeInBytes);
	glDrawElementsBaseVertex(device->glPrimitiveType, indexCount, device->state.inputLayout->indexType, offset, vertexOffset);

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgPrintDeviceInfoCore(pgGraphicsDevice* device)
{
	pgMakeCurrent(&device->context);

	pgInfo("OpenGL renderer: %s (%s)", glGetString(GL_RENDERER), glGetString(GL_VENDOR));
	pgInfo("OpenGL version: %s", glGetString(GL_VERSION));
	pgInfo("OpenGL GLSL version: %s", glGetString(GL_SHADING_LANGUAGE_VERSION));

	PG_ASSERT_NO_GL_ERRORS();
}

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgBool GlExtSupported(int extension, pgString extensionName)
{
	if (extension != ogl_LOAD_SUCCEEDED)
	{
		pgError("Extension '%s' is not supported.", extensionName);
		return PG_FALSE;
	}

	return PG_TRUE;
}

static pgVoid InitializeOpenGLExtensions()
{
	GLint major, minor;
	pgBool glExtsSupported = PG_TRUE;

	if (ogl_LoadFunctions() == ogl_LOAD_FAILED)
		pgDie("OpenGL initialization failed.");

	glGetIntegerv(GL_MAJOR_VERSION, &major);
	glGetIntegerv(GL_MINOR_VERSION, &minor);

	if (major < 3 || (major == 3 && minor < 3))
		pgDie("Only OpenGL %d.%d seems to be supported. OpenGL 3.3 is required.", major, minor);

	glExtsSupported &= GlExtSupported(ogl_ext_ARB_sampler_objects, "GL_ARB_sampler_objects");
	glExtsSupported &= GlExtSupported(ogl_ext_ARB_separate_shader_objects, "GL_ARB_separate_shader_objects");
	glExtsSupported &= GlExtSupported(ogl_ext_ARB_shading_language_420pack, "GL_ARB_shading_language_420pack");
	glExtsSupported &= GlExtSupported(ogl_ext_EXT_texture_filter_anisotropic, "GL_EXT_texture_filter_anisotropic");
	glExtsSupported &= GlExtSupported(ogl_ext_EXT_texture_compression_s3tc, "GL_EXT_texture_compression_s3tc");

	if (!glExtsSupported)
		pgDie("Incompatible graphics card. Not all required OpenGL extenions are supported.");
}

static pgInt32 FlipY(pgGraphicsDevice* device, pgInt32 top, pgInt32 height)
{
	return device->state.renderTarget->height - height - top;
}

#endif
