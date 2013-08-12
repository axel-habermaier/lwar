#include "prelude.h"

#if defined(OPENGL3)
	
#include <stdlib.h>
#include <string.h>
#include "glExt.h"

#if defined(__APPLE__)
#include <mach-o/dyld.h>

static void* AppleGLGetProcAddress (const GLubyte *name)
{
  static const struct mach_header* image = NULL;
  NSSymbol symbol;
  char* symbolName;
  if (NULL == image)
  {
    image = NSAddImage("/System/Library/Frameworks/OpenGL.framework/Versions/Current/OpenGL", NSADDIMAGE_OPTION_RETURN_ON_ERROR);
  }
  /* prepend a '_' for the Unix C symbol mangling convention */
  symbolName = malloc(strlen((const char*)name) + 2);
  strcpy(symbolName+1, (const char*)name);
  symbolName[0] = '_';
  symbol = NULL;
  /* if (NSIsSymbolNameDefined(symbolName))
	 symbol = NSLookupAndBindSymbol(symbolName); */
  symbol = image ? NSLookupSymbolInImage(image, symbolName, NSLOOKUPSYMBOLINIMAGE_OPTION_BIND | NSLOOKUPSYMBOLINIMAGE_OPTION_RETURN_ON_ERROR) : NULL;
  free(symbolName);
  return symbol ? NSAddressOfSymbol(symbol) : NULL;
}
#endif /* __APPLE__ */

#if defined(__sgi) || defined (__sun)
#include <dlfcn.h>
#include <stdio.h>

static void* SunGetProcAddress (const GLubyte* name)
{
  static void* h = NULL;
  static void* gpa;

  if (h == NULL)
  {
    if ((h = dlopen(NULL, RTLD_LAZY | RTLD_LOCAL)) == NULL) return NULL;
    gpa = dlsym(h, "glXGetProcAddress");
  }

  if (gpa != NULL)
    return ((void*(*)(const GLubyte*))gpa)(name);
  else
    return dlsym(h, (const char*)name);
}
#endif /* __sgi || __sun */

#if defined(_WIN32)

#ifdef _MSC_VER
#pragma warning(disable: 4055)
#pragma warning(disable: 4054)
#endif

static int TestPointer(const PROC pTest)
{
	ptrdiff_t iTest;
	if(!pTest) return 0;
	iTest = (ptrdiff_t)pTest;
	
	if(iTest == 1 || iTest == 2 || iTest == 3 || iTest == -1) return 0;
	
	return 1;
}

static PROC WinGetProcAddress(const char *name)
{
	HMODULE glMod = NULL;
	PROC pFunc = wglGetProcAddress((LPCSTR)name);
	if(TestPointer(pFunc))
	{
		return pFunc;
	}
	glMod = GetModuleHandleA("OpenGL32.dll");
	return (PROC)GetProcAddress(glMod, (LPCSTR)name);
}
	
#define IntGetProcAddress(name) WinGetProcAddress(name)
#else
	#if defined(__APPLE__)
		#define IntGetProcAddress(name) AppleGLGetProcAddress(name)
	#else
		#if defined(__sgi) || defined(__sun)
			#define IntGetProcAddress(name) SunGetProcAddress(name)
		#else /* GLX */
		    #include <GL/glx.h>

			#define IntGetProcAddress(name) (*glXGetProcAddressARB)((const GLubyte*)name)
		#endif
	#endif
#endif

int ogl_ext_ARB_sampler_objects = ogl_LOAD_FAILED;
int ogl_ext_ARB_separate_shader_objects = ogl_LOAD_FAILED;
int ogl_ext_ARB_shading_language_420pack = ogl_LOAD_FAILED;
int ogl_ext_EXT_texture_filter_anisotropic = ogl_LOAD_FAILED;
int ogl_ext_EXT_texture_compression_s3tc = ogl_LOAD_FAILED;
int ogl_ext_EXT_direct_state_access = ogl_LOAD_FAILED;
int ogl_ext_ARB_debug_output = ogl_LOAD_FAILED;

void (CODEGEN_FUNCPTR *_ptrc_glGenSamplers)(GLsizei , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteSamplers)(GLsizei , const GLuint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsSampler)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindSampler)(GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSamplerParameteri)(GLuint , GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSamplerParameteriv)(GLuint , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSamplerParameterf)(GLuint , GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSamplerParameterfv)(GLuint , GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSamplerParameterIiv)(GLuint , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSamplerParameterIuiv)(GLuint , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetSamplerParameteriv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetSamplerParameterIiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetSamplerParameterfv)(GLuint , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetSamplerParameterIuiv)(GLuint , GLenum , GLuint *) = NULL;

static int Load_ARB_sampler_objects()
{
	int numFailed = 0;
	_ptrc_glGenSamplers = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenSamplers");
	if(!_ptrc_glGenSamplers) numFailed++;
	_ptrc_glDeleteSamplers = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteSamplers");
	if(!_ptrc_glDeleteSamplers) numFailed++;
	_ptrc_glIsSampler = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsSampler");
	if(!_ptrc_glIsSampler) numFailed++;
	_ptrc_glBindSampler = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glBindSampler");
	if(!_ptrc_glBindSampler) numFailed++;
	_ptrc_glSamplerParameteri = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint ))IntGetProcAddress("glSamplerParameteri");
	if(!_ptrc_glSamplerParameteri) numFailed++;
	_ptrc_glSamplerParameteriv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLint *))IntGetProcAddress("glSamplerParameteriv");
	if(!_ptrc_glSamplerParameteriv) numFailed++;
	_ptrc_glSamplerParameterf = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLfloat ))IntGetProcAddress("glSamplerParameterf");
	if(!_ptrc_glSamplerParameterf) numFailed++;
	_ptrc_glSamplerParameterfv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLfloat *))IntGetProcAddress("glSamplerParameterfv");
	if(!_ptrc_glSamplerParameterfv) numFailed++;
	_ptrc_glSamplerParameterIiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLint *))IntGetProcAddress("glSamplerParameterIiv");
	if(!_ptrc_glSamplerParameterIiv) numFailed++;
	_ptrc_glSamplerParameterIuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLuint *))IntGetProcAddress("glSamplerParameterIuiv");
	if(!_ptrc_glSamplerParameterIuiv) numFailed++;
	_ptrc_glGetSamplerParameteriv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetSamplerParameteriv");
	if(!_ptrc_glGetSamplerParameteriv) numFailed++;
	_ptrc_glGetSamplerParameterIiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetSamplerParameterIiv");
	if(!_ptrc_glGetSamplerParameterIiv) numFailed++;
	_ptrc_glGetSamplerParameterfv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLfloat *))IntGetProcAddress("glGetSamplerParameterfv");
	if(!_ptrc_glGetSamplerParameterfv) numFailed++;
	_ptrc_glGetSamplerParameterIuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint *))IntGetProcAddress("glGetSamplerParameterIuiv");
	if(!_ptrc_glGetSamplerParameterIuiv) numFailed++;
	return numFailed;
}

void (CODEGEN_FUNCPTR *_ptrc_glUseProgramStages)(GLuint , GLbitfield , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glActiveShaderProgram)(GLuint , GLuint ) = NULL;
GLuint (CODEGEN_FUNCPTR *_ptrc_glCreateShaderProgramv)(GLenum , GLsizei , const GLchar* const *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindProgramPipeline)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteProgramPipelines)(GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenProgramPipelines)(GLsizei , GLuint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsProgramPipeline)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetProgramPipelineiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1i)(GLuint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1iv)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1f)(GLuint , GLint , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1fv)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1d)(GLuint , GLint , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1dv)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1ui)(GLuint , GLint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1uiv)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2i)(GLuint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2iv)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2f)(GLuint , GLint , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2fv)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2d)(GLuint , GLint , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2dv)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2ui)(GLuint , GLint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2uiv)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3i)(GLuint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3iv)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3f)(GLuint , GLint , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3fv)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3d)(GLuint , GLint , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3dv)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3ui)(GLuint , GLint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3uiv)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4i)(GLuint , GLint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4iv)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4f)(GLuint , GLint , GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4fv)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4d)(GLuint , GLint , GLdouble , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4dv)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4ui)(GLuint , GLint , GLuint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4uiv)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x3fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x2fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x4fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x2fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x4fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x3fv)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x3dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x2dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x4dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x2dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x4dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x3dv)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glValidateProgramPipeline)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetProgramPipelineInfoLog)(GLuint , GLsizei , GLsizei *, GLchar *) = NULL;

static int Load_ARB_separate_shader_objects()
{
	int numFailed = 0;
	_ptrc_glUseProgramStages = (void (CODEGEN_FUNCPTR *)(GLuint , GLbitfield , GLuint ))IntGetProcAddress("glUseProgramStages");
	if(!_ptrc_glUseProgramStages) numFailed++;
	_ptrc_glActiveShaderProgram = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glActiveShaderProgram");
	if(!_ptrc_glActiveShaderProgram) numFailed++;
	_ptrc_glCreateShaderProgramv = (GLuint (CODEGEN_FUNCPTR *)(GLenum , GLsizei , const GLchar* const *))IntGetProcAddress("glCreateShaderProgramv");
	if(!_ptrc_glCreateShaderProgramv) numFailed++;
	_ptrc_glBindProgramPipeline = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glBindProgramPipeline");
	if(!_ptrc_glBindProgramPipeline) numFailed++;
	_ptrc_glDeleteProgramPipelines = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteProgramPipelines");
	if(!_ptrc_glDeleteProgramPipelines) numFailed++;
	_ptrc_glGenProgramPipelines = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenProgramPipelines");
	if(!_ptrc_glGenProgramPipelines) numFailed++;
	_ptrc_glIsProgramPipeline = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsProgramPipeline");
	if(!_ptrc_glIsProgramPipeline) numFailed++;
	_ptrc_glGetProgramPipelineiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetProgramPipelineiv");
	if(!_ptrc_glGetProgramPipelineiv) numFailed++;
	_ptrc_glProgramUniform1i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint ))IntGetProcAddress("glProgramUniform1i");
	if(!_ptrc_glProgramUniform1i) numFailed++;
	_ptrc_glProgramUniform1iv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform1iv");
	if(!_ptrc_glProgramUniform1iv) numFailed++;
	_ptrc_glProgramUniform1f = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat ))IntGetProcAddress("glProgramUniform1f");
	if(!_ptrc_glProgramUniform1f) numFailed++;
	_ptrc_glProgramUniform1fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform1fv");
	if(!_ptrc_glProgramUniform1fv) numFailed++;
	_ptrc_glProgramUniform1d = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble ))IntGetProcAddress("glProgramUniform1d");
	if(!_ptrc_glProgramUniform1d) numFailed++;
	_ptrc_glProgramUniform1dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform1dv");
	if(!_ptrc_glProgramUniform1dv) numFailed++;
	_ptrc_glProgramUniform1ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint ))IntGetProcAddress("glProgramUniform1ui");
	if(!_ptrc_glProgramUniform1ui) numFailed++;
	_ptrc_glProgramUniform1uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform1uiv");
	if(!_ptrc_glProgramUniform1uiv) numFailed++;
	_ptrc_glProgramUniform2i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint ))IntGetProcAddress("glProgramUniform2i");
	if(!_ptrc_glProgramUniform2i) numFailed++;
	_ptrc_glProgramUniform2iv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform2iv");
	if(!_ptrc_glProgramUniform2iv) numFailed++;
	_ptrc_glProgramUniform2f = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat , GLfloat ))IntGetProcAddress("glProgramUniform2f");
	if(!_ptrc_glProgramUniform2f) numFailed++;
	_ptrc_glProgramUniform2fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform2fv");
	if(!_ptrc_glProgramUniform2fv) numFailed++;
	_ptrc_glProgramUniform2d = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble , GLdouble ))IntGetProcAddress("glProgramUniform2d");
	if(!_ptrc_glProgramUniform2d) numFailed++;
	_ptrc_glProgramUniform2dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform2dv");
	if(!_ptrc_glProgramUniform2dv) numFailed++;
	_ptrc_glProgramUniform2ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint , GLuint ))IntGetProcAddress("glProgramUniform2ui");
	if(!_ptrc_glProgramUniform2ui) numFailed++;
	_ptrc_glProgramUniform2uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform2uiv");
	if(!_ptrc_glProgramUniform2uiv) numFailed++;
	_ptrc_glProgramUniform3i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glProgramUniform3i");
	if(!_ptrc_glProgramUniform3i) numFailed++;
	_ptrc_glProgramUniform3iv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform3iv");
	if(!_ptrc_glProgramUniform3iv) numFailed++;
	_ptrc_glProgramUniform3f = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glProgramUniform3f");
	if(!_ptrc_glProgramUniform3f) numFailed++;
	_ptrc_glProgramUniform3fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform3fv");
	if(!_ptrc_glProgramUniform3fv) numFailed++;
	_ptrc_glProgramUniform3d = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glProgramUniform3d");
	if(!_ptrc_glProgramUniform3d) numFailed++;
	_ptrc_glProgramUniform3dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform3dv");
	if(!_ptrc_glProgramUniform3dv) numFailed++;
	_ptrc_glProgramUniform3ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint , GLuint , GLuint ))IntGetProcAddress("glProgramUniform3ui");
	if(!_ptrc_glProgramUniform3ui) numFailed++;
	_ptrc_glProgramUniform3uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform3uiv");
	if(!_ptrc_glProgramUniform3uiv) numFailed++;
	_ptrc_glProgramUniform4i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glProgramUniform4i");
	if(!_ptrc_glProgramUniform4i) numFailed++;
	_ptrc_glProgramUniform4iv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform4iv");
	if(!_ptrc_glProgramUniform4iv) numFailed++;
	_ptrc_glProgramUniform4f = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glProgramUniform4f");
	if(!_ptrc_glProgramUniform4f) numFailed++;
	_ptrc_glProgramUniform4fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform4fv");
	if(!_ptrc_glProgramUniform4fv) numFailed++;
	_ptrc_glProgramUniform4d = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glProgramUniform4d");
	if(!_ptrc_glProgramUniform4d) numFailed++;
	_ptrc_glProgramUniform4dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform4dv");
	if(!_ptrc_glProgramUniform4dv) numFailed++;
	_ptrc_glProgramUniform4ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint , GLuint , GLuint , GLuint ))IntGetProcAddress("glProgramUniform4ui");
	if(!_ptrc_glProgramUniform4ui) numFailed++;
	_ptrc_glProgramUniform4uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform4uiv");
	if(!_ptrc_glProgramUniform4uiv) numFailed++;
	_ptrc_glProgramUniformMatrix2fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix2fv");
	if(!_ptrc_glProgramUniformMatrix2fv) numFailed++;
	_ptrc_glProgramUniformMatrix3fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix3fv");
	if(!_ptrc_glProgramUniformMatrix3fv) numFailed++;
	_ptrc_glProgramUniformMatrix4fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix4fv");
	if(!_ptrc_glProgramUniformMatrix4fv) numFailed++;
	_ptrc_glProgramUniformMatrix2dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix2dv");
	if(!_ptrc_glProgramUniformMatrix2dv) numFailed++;
	_ptrc_glProgramUniformMatrix3dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix3dv");
	if(!_ptrc_glProgramUniformMatrix3dv) numFailed++;
	_ptrc_glProgramUniformMatrix4dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix4dv");
	if(!_ptrc_glProgramUniformMatrix4dv) numFailed++;
	_ptrc_glProgramUniformMatrix2x3fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix2x3fv");
	if(!_ptrc_glProgramUniformMatrix2x3fv) numFailed++;
	_ptrc_glProgramUniformMatrix3x2fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix3x2fv");
	if(!_ptrc_glProgramUniformMatrix3x2fv) numFailed++;
	_ptrc_glProgramUniformMatrix2x4fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix2x4fv");
	if(!_ptrc_glProgramUniformMatrix2x4fv) numFailed++;
	_ptrc_glProgramUniformMatrix4x2fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix4x2fv");
	if(!_ptrc_glProgramUniformMatrix4x2fv) numFailed++;
	_ptrc_glProgramUniformMatrix3x4fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix3x4fv");
	if(!_ptrc_glProgramUniformMatrix3x4fv) numFailed++;
	_ptrc_glProgramUniformMatrix4x3fv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix4x3fv");
	if(!_ptrc_glProgramUniformMatrix4x3fv) numFailed++;
	_ptrc_glProgramUniformMatrix2x3dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix2x3dv");
	if(!_ptrc_glProgramUniformMatrix2x3dv) numFailed++;
	_ptrc_glProgramUniformMatrix3x2dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix3x2dv");
	if(!_ptrc_glProgramUniformMatrix3x2dv) numFailed++;
	_ptrc_glProgramUniformMatrix2x4dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix2x4dv");
	if(!_ptrc_glProgramUniformMatrix2x4dv) numFailed++;
	_ptrc_glProgramUniformMatrix4x2dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix4x2dv");
	if(!_ptrc_glProgramUniformMatrix4x2dv) numFailed++;
	_ptrc_glProgramUniformMatrix3x4dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix3x4dv");
	if(!_ptrc_glProgramUniformMatrix3x4dv) numFailed++;
	_ptrc_glProgramUniformMatrix4x3dv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix4x3dv");
	if(!_ptrc_glProgramUniformMatrix4x3dv) numFailed++;
	_ptrc_glValidateProgramPipeline = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glValidateProgramPipeline");
	if(!_ptrc_glValidateProgramPipeline) numFailed++;
	_ptrc_glGetProgramPipelineInfoLog = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLsizei *, GLchar *))IntGetProcAddress("glGetProgramPipelineInfoLog");
	if(!_ptrc_glGetProgramPipelineInfoLog) numFailed++;
	return numFailed;
}

void (CODEGEN_FUNCPTR *_ptrc_glClientAttribDefaultEXT)(GLbitfield ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPushClientAttribDefaultEXT)(GLbitfield ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixLoadfEXT)(GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixLoaddEXT)(GLenum , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixMultfEXT)(GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixMultdEXT)(GLenum , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixLoadIdentityEXT)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixRotatefEXT)(GLenum , GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixRotatedEXT)(GLenum , GLdouble , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixScalefEXT)(GLenum , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixScaledEXT)(GLenum , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixTranslatefEXT)(GLenum , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixTranslatedEXT)(GLenum , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixFrustumEXT)(GLenum , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixOrthoEXT)(GLenum , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixPopEXT)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixPushEXT)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixLoadTransposefEXT)(GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixLoadTransposedEXT)(GLenum , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixMultTransposefEXT)(GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMatrixMultTransposedEXT)(GLenum , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureParameterfEXT)(GLuint , GLenum , GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureParameterfvEXT)(GLuint , GLenum , GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureParameteriEXT)(GLuint , GLenum , GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureParameterivEXT)(GLuint , GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureImage1DEXT)(GLuint , GLenum , GLint , GLenum , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureImage2DEXT)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureSubImage1DEXT)(GLuint , GLenum , GLint , GLint , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureSubImage2DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTextureImage1DEXT)(GLuint , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTextureImage2DEXT)(GLuint , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTextureSubImage1DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTextureSubImage2DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureImageEXT)(GLuint , GLenum , GLint , GLenum , GLenum , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureParameterfvEXT)(GLuint , GLenum , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureParameterivEXT)(GLuint , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureLevelParameterfvEXT)(GLuint , GLenum , GLint , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureLevelParameterivEXT)(GLuint , GLenum , GLint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureImage3DEXT)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureSubImage3DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTextureSubImage3DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexParameterfEXT)(GLenum , GLenum , GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexParameterfvEXT)(GLenum , GLenum , GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexParameteriEXT)(GLenum , GLenum , GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexParameterivEXT)(GLenum , GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexImage1DEXT)(GLenum , GLenum , GLint , GLenum , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexImage2DEXT)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexSubImage1DEXT)(GLenum , GLenum , GLint , GLint , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexSubImage2DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyMultiTexImage1DEXT)(GLenum , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyMultiTexImage2DEXT)(GLenum , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyMultiTexSubImage1DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyMultiTexSubImage2DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexImageEXT)(GLenum , GLenum , GLint , GLenum , GLenum , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexParameterfvEXT)(GLenum , GLenum , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexParameterivEXT)(GLenum , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexLevelParameterfvEXT)(GLenum , GLenum , GLint , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexLevelParameterivEXT)(GLenum , GLenum , GLint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexImage3DEXT)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexSubImage3DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyMultiTexSubImage3DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindMultiTextureEXT)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnableClientStateIndexedEXT)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisableClientStateIndexedEXT)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnableClientStateiEXT)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisableClientStateiEXT)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordPointerEXT)(GLenum , GLint , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexEnvfEXT)(GLenum , GLenum , GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexEnvfvEXT)(GLenum , GLenum , GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexEnviEXT)(GLenum , GLenum , GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexEnvivEXT)(GLenum , GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexGendEXT)(GLenum , GLenum , GLenum , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexGendvEXT)(GLenum , GLenum , GLenum , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexGenfEXT)(GLenum , GLenum , GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexGenfvEXT)(GLenum , GLenum , GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexGeniEXT)(GLenum , GLenum , GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexGenivEXT)(GLenum , GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexEnvfvEXT)(GLenum , GLenum , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexEnvivEXT)(GLenum , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexGendvEXT)(GLenum , GLenum , GLenum , GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexGenfvEXT)(GLenum , GLenum , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexGenivEXT)(GLenum , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetFloatIndexedvEXT)(GLenum , GLuint , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetDoubleIndexedvEXT)(GLenum , GLuint , GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetPointerIndexedvEXT)(GLenum , GLuint , GLvoid* *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetFloati_vEXT)(GLenum , GLuint , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetDoublei_vEXT)(GLenum , GLuint , GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetPointeri_vEXT)(GLenum , GLuint , GLvoid* *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTextureImage3DEXT)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTextureImage2DEXT)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTextureImage1DEXT)(GLuint , GLenum , GLint , GLenum , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTextureSubImage3DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTextureSubImage2DEXT)(GLuint , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTextureSubImage1DEXT)(GLuint , GLenum , GLint , GLint , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetCompressedTextureImageEXT)(GLuint , GLenum , GLint , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedMultiTexImage3DEXT)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedMultiTexImage2DEXT)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedMultiTexImage1DEXT)(GLenum , GLenum , GLint , GLenum , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedMultiTexSubImage3DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedMultiTexSubImage2DEXT)(GLenum , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedMultiTexSubImage1DEXT)(GLenum , GLenum , GLint , GLint , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetCompressedMultiTexImageEXT)(GLenum , GLenum , GLint , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramStringEXT)(GLuint , GLenum , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameter4dEXT)(GLuint , GLenum , GLuint , GLdouble , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameter4dvEXT)(GLuint , GLenum , GLuint , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameter4fEXT)(GLuint , GLenum , GLuint , GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameter4fvEXT)(GLuint , GLenum , GLuint , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedProgramLocalParameterdvEXT)(GLuint , GLenum , GLuint , GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedProgramLocalParameterfvEXT)(GLuint , GLenum , GLuint , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedProgramivEXT)(GLuint , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedProgramStringEXT)(GLuint , GLenum , GLenum , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameters4fvEXT)(GLuint , GLenum , GLuint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameterI4iEXT)(GLuint , GLenum , GLuint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameterI4ivEXT)(GLuint , GLenum , GLuint , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParametersI4ivEXT)(GLuint , GLenum , GLuint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameterI4uiEXT)(GLuint , GLenum , GLuint , GLuint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParameterI4uivEXT)(GLuint , GLenum , GLuint , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedProgramLocalParametersI4uivEXT)(GLuint , GLenum , GLuint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedProgramLocalParameterIivEXT)(GLuint , GLenum , GLuint , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedProgramLocalParameterIuivEXT)(GLuint , GLenum , GLuint , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureParameterIivEXT)(GLuint , GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureParameterIuivEXT)(GLuint , GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureParameterIivEXT)(GLuint , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTextureParameterIuivEXT)(GLuint , GLenum , GLenum , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexParameterIivEXT)(GLenum , GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexParameterIuivEXT)(GLenum , GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexParameterIivEXT)(GLenum , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultiTexParameterIuivEXT)(GLenum , GLenum , GLenum , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1fEXT)(GLuint , GLint , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2fEXT)(GLuint , GLint , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3fEXT)(GLuint , GLint , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4fEXT)(GLuint , GLint , GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1iEXT)(GLuint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2iEXT)(GLuint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3iEXT)(GLuint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4iEXT)(GLuint , GLint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1fvEXT)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2fvEXT)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3fvEXT)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4fvEXT)(GLuint , GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1ivEXT)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2ivEXT)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3ivEXT)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4ivEXT)(GLuint , GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x3fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x2fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x4fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x2fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x4fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x3fvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1uiEXT)(GLuint , GLint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2uiEXT)(GLuint , GLint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3uiEXT)(GLuint , GLint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4uiEXT)(GLuint , GLint , GLuint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1uivEXT)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2uivEXT)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3uivEXT)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4uivEXT)(GLuint , GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedBufferDataEXT)(GLuint , GLsizeiptr , const GLvoid *, GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedBufferSubDataEXT)(GLuint , GLintptr , GLsizeiptr , const GLvoid *) = NULL;
GLvoid* (CODEGEN_FUNCPTR *_ptrc_glMapNamedBufferEXT)(GLuint , GLenum ) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glUnmapNamedBufferEXT)(GLuint ) = NULL;
GLvoid* (CODEGEN_FUNCPTR *_ptrc_glMapNamedBufferRangeEXT)(GLuint , GLintptr , GLsizeiptr , GLbitfield ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFlushMappedNamedBufferRangeEXT)(GLuint , GLintptr , GLsizeiptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedCopyBufferSubDataEXT)(GLuint , GLuint , GLintptr , GLintptr , GLsizeiptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedBufferParameterivEXT)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedBufferPointervEXT)(GLuint , GLenum , GLvoid* *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedBufferSubDataEXT)(GLuint , GLintptr , GLsizeiptr , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureBufferEXT)(GLuint , GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexBufferEXT)(GLenum , GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedRenderbufferStorageEXT)(GLuint , GLenum , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedRenderbufferParameterivEXT)(GLuint , GLenum , GLint *) = NULL;
GLenum (CODEGEN_FUNCPTR *_ptrc_glCheckNamedFramebufferStatusEXT)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferTexture1DEXT)(GLuint , GLenum , GLenum , GLuint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferTexture2DEXT)(GLuint , GLenum , GLenum , GLuint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferTexture3DEXT)(GLuint , GLenum , GLenum , GLuint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferRenderbufferEXT)(GLuint , GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetNamedFramebufferAttachmentParameterivEXT)(GLuint , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenerateTextureMipmapEXT)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenerateMultiTexMipmapEXT)(GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferDrawBufferEXT)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferDrawBuffersEXT)(GLuint , GLsizei , const GLenum *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferReadBufferEXT)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetFramebufferParameterivEXT)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedRenderbufferStorageMultisampleEXT)(GLuint , GLsizei , GLenum , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedRenderbufferStorageMultisampleCoverageEXT)(GLuint , GLsizei , GLsizei , GLenum , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferTextureEXT)(GLuint , GLenum , GLuint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferTextureLayerEXT)(GLuint , GLenum , GLuint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNamedFramebufferTextureFaceEXT)(GLuint , GLenum , GLuint , GLint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTextureRenderbufferEXT)(GLuint , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexRenderbufferEXT)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1dEXT)(GLuint , GLint , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2dEXT)(GLuint , GLint , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3dEXT)(GLuint , GLint , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4dEXT)(GLuint , GLint , GLdouble , GLdouble , GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform1dvEXT)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform2dvEXT)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform3dvEXT)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniform4dvEXT)(GLuint , GLint , GLsizei , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x3dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix2x4dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x2dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix3x4dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x2dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glProgramUniformMatrix4x3dvEXT)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnableVertexArrayAttribEXT)(GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisableVertexArrayAttribEXT)(GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnableVertexArrayEXT)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisableVertexArrayEXT)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayColorOffsetEXT)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayEdgeFlagOffsetEXT)(GLuint , GLuint , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayFogCoordOffsetEXT)(GLuint , GLuint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayIndexOffsetEXT)(GLuint , GLuint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayMultiTexCoordOffsetEXT)(GLuint , GLuint , GLenum , GLint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayNormalOffsetEXT)(GLuint , GLuint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArraySecondaryColorOffsetEXT)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayTexCoordOffsetEXT)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayVertexOffsetEXT)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayVertexAttribIOffsetEXT)(GLuint , GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexArrayVertexAttribOffsetEXT)(GLuint , GLuint , GLuint , GLint , GLenum , GLboolean , GLsizei , GLintptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexArrayIntegervEXT)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexArrayPointervEXT)(GLuint , GLenum , GLvoid* *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexArrayIntegeri_vEXT)(GLuint , GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexArrayPointeri_vEXT)(GLuint , GLuint , GLenum , GLvoid* *) = NULL;

static int Load_EXT_direct_state_access()
{
	int numFailed = 0;
	_ptrc_glClientAttribDefaultEXT = (void (CODEGEN_FUNCPTR *)(GLbitfield ))IntGetProcAddress("glClientAttribDefaultEXT");
	/*if(!_ptrc_glClientAttribDefaultEXT) numFailed++;
	_ptrc_glPushClientAttribDefaultEXT = (void (CODEGEN_FUNCPTR *)(GLbitfield ))IntGetProcAddress("glPushClientAttribDefaultEXT");
	if(!_ptrc_glPushClientAttribDefaultEXT) numFailed++;
	_ptrc_glMatrixLoadfEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLfloat *))IntGetProcAddress("glMatrixLoadfEXT");
	if(!_ptrc_glMatrixLoadfEXT) numFailed++;
	_ptrc_glMatrixLoaddEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLdouble *))IntGetProcAddress("glMatrixLoaddEXT");
	if(!_ptrc_glMatrixLoaddEXT) numFailed++;
	_ptrc_glMatrixMultfEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLfloat *))IntGetProcAddress("glMatrixMultfEXT");
	if(!_ptrc_glMatrixMultfEXT) numFailed++;
	_ptrc_glMatrixMultdEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLdouble *))IntGetProcAddress("glMatrixMultdEXT");
	if(!_ptrc_glMatrixMultdEXT) numFailed++;
	_ptrc_glMatrixLoadIdentityEXT = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glMatrixLoadIdentityEXT");
	if(!_ptrc_glMatrixLoadIdentityEXT) numFailed++;
	_ptrc_glMatrixRotatefEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glMatrixRotatefEXT");
	if(!_ptrc_glMatrixRotatefEXT) numFailed++;
	_ptrc_glMatrixRotatedEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLdouble , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glMatrixRotatedEXT");
	if(!_ptrc_glMatrixRotatedEXT) numFailed++;
	_ptrc_glMatrixScalefEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glMatrixScalefEXT");
	if(!_ptrc_glMatrixScalefEXT) numFailed++;
	_ptrc_glMatrixScaledEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glMatrixScaledEXT");
	if(!_ptrc_glMatrixScaledEXT) numFailed++;
	_ptrc_glMatrixTranslatefEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glMatrixTranslatefEXT");
	if(!_ptrc_glMatrixTranslatefEXT) numFailed++;
	_ptrc_glMatrixTranslatedEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glMatrixTranslatedEXT");
	if(!_ptrc_glMatrixTranslatedEXT) numFailed++;
	_ptrc_glMatrixFrustumEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glMatrixFrustumEXT");
	if(!_ptrc_glMatrixFrustumEXT) numFailed++;
	_ptrc_glMatrixOrthoEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glMatrixOrthoEXT");
	if(!_ptrc_glMatrixOrthoEXT) numFailed++;
	_ptrc_glMatrixPopEXT = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glMatrixPopEXT");
	if(!_ptrc_glMatrixPopEXT) numFailed++;
	_ptrc_glMatrixPushEXT = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glMatrixPushEXT");
	if(!_ptrc_glMatrixPushEXT) numFailed++;
	_ptrc_glMatrixLoadTransposefEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLfloat *))IntGetProcAddress("glMatrixLoadTransposefEXT");
	if(!_ptrc_glMatrixLoadTransposefEXT) numFailed++;
	_ptrc_glMatrixLoadTransposedEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLdouble *))IntGetProcAddress("glMatrixLoadTransposedEXT");
	if(!_ptrc_glMatrixLoadTransposedEXT) numFailed++;
	_ptrc_glMatrixMultTransposefEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLfloat *))IntGetProcAddress("glMatrixMultTransposefEXT");
	if(!_ptrc_glMatrixMultTransposefEXT) numFailed++;
	_ptrc_glMatrixMultTransposedEXT = (void (CODEGEN_FUNCPTR *)(GLenum , const GLdouble *))IntGetProcAddress("glMatrixMultTransposedEXT");
	if(!_ptrc_glMatrixMultTransposedEXT) numFailed++;
	_ptrc_glTextureParameterfEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLfloat ))IntGetProcAddress("glTextureParameterfEXT");
	if(!_ptrc_glTextureParameterfEXT) numFailed++;
	_ptrc_glTextureParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , const GLfloat *))IntGetProcAddress("glTextureParameterfvEXT");
	if(!_ptrc_glTextureParameterfvEXT) numFailed++;
	_ptrc_glTextureParameteriEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLint ))IntGetProcAddress("glTextureParameteriEXT");
	if(!_ptrc_glTextureParameteriEXT) numFailed++;
	_ptrc_glTextureParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , const GLint *))IntGetProcAddress("glTextureParameterivEXT");
	if(!_ptrc_glTextureParameterivEXT) numFailed++;
	_ptrc_glTextureImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTextureImage1DEXT");
	if(!_ptrc_glTextureImage1DEXT) numFailed++;*/
	_ptrc_glTextureImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTextureImage2DEXT");
	if(!_ptrc_glTextureImage2DEXT) numFailed++;
	/*_ptrc_glTextureSubImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTextureSubImage1DEXT");
	if(!_ptrc_glTextureSubImage1DEXT) numFailed++;
	_ptrc_glTextureSubImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTextureSubImage2DEXT");
	if(!_ptrc_glTextureSubImage2DEXT) numFailed++;
	_ptrc_glCopyTextureImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLint ))IntGetProcAddress("glCopyTextureImage1DEXT");
	if(!_ptrc_glCopyTextureImage1DEXT) numFailed++;
	_ptrc_glCopyTextureImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLsizei , GLint ))IntGetProcAddress("glCopyTextureImage2DEXT");
	if(!_ptrc_glCopyTextureImage2DEXT) numFailed++;
	_ptrc_glCopyTextureSubImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLsizei ))IntGetProcAddress("glCopyTextureSubImage1DEXT");
	if(!_ptrc_glCopyTextureSubImage1DEXT) numFailed++;
	_ptrc_glCopyTextureSubImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glCopyTextureSubImage2DEXT");
	if(!_ptrc_glCopyTextureSubImage2DEXT) numFailed++;
	_ptrc_glGetTextureImageEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLenum , GLvoid *))IntGetProcAddress("glGetTextureImageEXT");
	if(!_ptrc_glGetTextureImageEXT) numFailed++;
	_ptrc_glGetTextureParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLfloat *))IntGetProcAddress("glGetTextureParameterfvEXT");
	if(!_ptrc_glGetTextureParameterfvEXT) numFailed++;
	_ptrc_glGetTextureParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLint *))IntGetProcAddress("glGetTextureParameterivEXT");
	if(!_ptrc_glGetTextureParameterivEXT) numFailed++;
	_ptrc_glGetTextureLevelParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLfloat *))IntGetProcAddress("glGetTextureLevelParameterfvEXT");
	if(!_ptrc_glGetTextureLevelParameterfvEXT) numFailed++;
	_ptrc_glGetTextureLevelParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLint *))IntGetProcAddress("glGetTextureLevelParameterivEXT");
	if(!_ptrc_glGetTextureLevelParameterivEXT) numFailed++;
	_ptrc_glTextureImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTextureImage3DEXT");
	if(!_ptrc_glTextureImage3DEXT) numFailed++;
	_ptrc_glTextureSubImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTextureSubImage3DEXT");
	if(!_ptrc_glTextureSubImage3DEXT) numFailed++;
	_ptrc_glCopyTextureSubImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glCopyTextureSubImage3DEXT");
	if(!_ptrc_glCopyTextureSubImage3DEXT) numFailed++;
	_ptrc_glMultiTexParameterfEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLfloat ))IntGetProcAddress("glMultiTexParameterfEXT");
	if(!_ptrc_glMultiTexParameterfEXT) numFailed++;
	_ptrc_glMultiTexParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLfloat *))IntGetProcAddress("glMultiTexParameterfvEXT");
	if(!_ptrc_glMultiTexParameterfvEXT) numFailed++;
	_ptrc_glMultiTexParameteriEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint ))IntGetProcAddress("glMultiTexParameteriEXT");
	if(!_ptrc_glMultiTexParameteriEXT) numFailed++;
	_ptrc_glMultiTexParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLint *))IntGetProcAddress("glMultiTexParameterivEXT");
	if(!_ptrc_glMultiTexParameterivEXT) numFailed++;
	_ptrc_glMultiTexImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glMultiTexImage1DEXT");
	if(!_ptrc_glMultiTexImage1DEXT) numFailed++;
	_ptrc_glMultiTexImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glMultiTexImage2DEXT");
	if(!_ptrc_glMultiTexImage2DEXT) numFailed++;
	_ptrc_glMultiTexSubImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glMultiTexSubImage1DEXT");
	if(!_ptrc_glMultiTexSubImage1DEXT) numFailed++;
	_ptrc_glMultiTexSubImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glMultiTexSubImage2DEXT");
	if(!_ptrc_glMultiTexSubImage2DEXT) numFailed++;
	_ptrc_glCopyMultiTexImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLint ))IntGetProcAddress("glCopyMultiTexImage1DEXT");
	if(!_ptrc_glCopyMultiTexImage1DEXT) numFailed++;
	_ptrc_glCopyMultiTexImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLsizei , GLint ))IntGetProcAddress("glCopyMultiTexImage2DEXT");
	if(!_ptrc_glCopyMultiTexImage2DEXT) numFailed++;
	_ptrc_glCopyMultiTexSubImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLsizei ))IntGetProcAddress("glCopyMultiTexSubImage1DEXT");
	if(!_ptrc_glCopyMultiTexSubImage1DEXT) numFailed++;
	_ptrc_glCopyMultiTexSubImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glCopyMultiTexSubImage2DEXT");
	if(!_ptrc_glCopyMultiTexSubImage2DEXT) numFailed++;
	_ptrc_glGetMultiTexImageEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLenum , GLvoid *))IntGetProcAddress("glGetMultiTexImageEXT");
	if(!_ptrc_glGetMultiTexImageEXT) numFailed++;
	_ptrc_glGetMultiTexParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLfloat *))IntGetProcAddress("glGetMultiTexParameterfvEXT");
	if(!_ptrc_glGetMultiTexParameterfvEXT) numFailed++;
	_ptrc_glGetMultiTexParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint *))IntGetProcAddress("glGetMultiTexParameterivEXT");
	if(!_ptrc_glGetMultiTexParameterivEXT) numFailed++;
	_ptrc_glGetMultiTexLevelParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLfloat *))IntGetProcAddress("glGetMultiTexLevelParameterfvEXT");
	if(!_ptrc_glGetMultiTexLevelParameterfvEXT) numFailed++;
	_ptrc_glGetMultiTexLevelParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLint *))IntGetProcAddress("glGetMultiTexLevelParameterivEXT");
	if(!_ptrc_glGetMultiTexLevelParameterivEXT) numFailed++;
	_ptrc_glMultiTexImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glMultiTexImage3DEXT");
	if(!_ptrc_glMultiTexImage3DEXT) numFailed++;
	_ptrc_glMultiTexSubImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glMultiTexSubImage3DEXT");
	if(!_ptrc_glMultiTexSubImage3DEXT) numFailed++;
	_ptrc_glCopyMultiTexSubImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glCopyMultiTexSubImage3DEXT");
	if(!_ptrc_glCopyMultiTexSubImage3DEXT) numFailed++;
	_ptrc_glBindMultiTextureEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glBindMultiTextureEXT");
	if(!_ptrc_glBindMultiTextureEXT) numFailed++;
	_ptrc_glEnableClientStateIndexedEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glEnableClientStateIndexedEXT");
	if(!_ptrc_glEnableClientStateIndexedEXT) numFailed++;
	_ptrc_glDisableClientStateIndexedEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glDisableClientStateIndexedEXT");
	if(!_ptrc_glDisableClientStateIndexedEXT) numFailed++;
	_ptrc_glEnableClientStateiEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glEnableClientStateiEXT");
	if(!_ptrc_glEnableClientStateiEXT) numFailed++;
	_ptrc_glDisableClientStateiEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glDisableClientStateiEXT");
	if(!_ptrc_glDisableClientStateiEXT) numFailed++;
	_ptrc_glMultiTexCoordPointerEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glMultiTexCoordPointerEXT");
	if(!_ptrc_glMultiTexCoordPointerEXT) numFailed++;
	_ptrc_glMultiTexEnvfEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLfloat ))IntGetProcAddress("glMultiTexEnvfEXT");
	if(!_ptrc_glMultiTexEnvfEXT) numFailed++;
	_ptrc_glMultiTexEnvfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLfloat *))IntGetProcAddress("glMultiTexEnvfvEXT");
	if(!_ptrc_glMultiTexEnvfvEXT) numFailed++;
	_ptrc_glMultiTexEnviEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint ))IntGetProcAddress("glMultiTexEnviEXT");
	if(!_ptrc_glMultiTexEnviEXT) numFailed++;
	_ptrc_glMultiTexEnvivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLint *))IntGetProcAddress("glMultiTexEnvivEXT");
	if(!_ptrc_glMultiTexEnvivEXT) numFailed++;
	_ptrc_glMultiTexGendEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLdouble ))IntGetProcAddress("glMultiTexGendEXT");
	if(!_ptrc_glMultiTexGendEXT) numFailed++;
	_ptrc_glMultiTexGendvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLdouble *))IntGetProcAddress("glMultiTexGendvEXT");
	if(!_ptrc_glMultiTexGendvEXT) numFailed++;
	_ptrc_glMultiTexGenfEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLfloat ))IntGetProcAddress("glMultiTexGenfEXT");
	if(!_ptrc_glMultiTexGenfEXT) numFailed++;
	_ptrc_glMultiTexGenfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLfloat *))IntGetProcAddress("glMultiTexGenfvEXT");
	if(!_ptrc_glMultiTexGenfvEXT) numFailed++;
	_ptrc_glMultiTexGeniEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint ))IntGetProcAddress("glMultiTexGeniEXT");
	if(!_ptrc_glMultiTexGeniEXT) numFailed++;
	_ptrc_glMultiTexGenivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLint *))IntGetProcAddress("glMultiTexGenivEXT");
	if(!_ptrc_glMultiTexGenivEXT) numFailed++;
	_ptrc_glGetMultiTexEnvfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLfloat *))IntGetProcAddress("glGetMultiTexEnvfvEXT");
	if(!_ptrc_glGetMultiTexEnvfvEXT) numFailed++;
	_ptrc_glGetMultiTexEnvivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint *))IntGetProcAddress("glGetMultiTexEnvivEXT");
	if(!_ptrc_glGetMultiTexEnvivEXT) numFailed++;
	_ptrc_glGetMultiTexGendvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLdouble *))IntGetProcAddress("glGetMultiTexGendvEXT");
	if(!_ptrc_glGetMultiTexGendvEXT) numFailed++;
	_ptrc_glGetMultiTexGenfvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLfloat *))IntGetProcAddress("glGetMultiTexGenfvEXT");
	if(!_ptrc_glGetMultiTexGenfvEXT) numFailed++;
	_ptrc_glGetMultiTexGenivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint *))IntGetProcAddress("glGetMultiTexGenivEXT");
	if(!_ptrc_glGetMultiTexGenivEXT) numFailed++;*/
	/*_ptrc_glGetFloatIndexedvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLfloat *))IntGetProcAddress("glGetFloatIndexedvEXT");
	if(!_ptrc_glGetFloatIndexedvEXT) numFailed++;*/
	/*_ptrc_glGetDoubleIndexedvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLdouble *))IntGetProcAddress("glGetDoubleIndexedvEXT");
	if(!_ptrc_glGetDoubleIndexedvEXT) numFailed++;*/
	/*_ptrc_glGetPointerIndexedvEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLvoid* *))IntGetProcAddress("glGetPointerIndexedvEXT");
	if(!_ptrc_glGetPointerIndexedvEXT) numFailed++;*/
	/*_ptrc_glGetFloati_vEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLfloat *))IntGetProcAddress("glGetFloati_vEXT");
	if(!_ptrc_glGetFloati_vEXT) numFailed++;
	_ptrc_glGetDoublei_vEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLdouble *))IntGetProcAddress("glGetDoublei_vEXT");
	if(!_ptrc_glGetDoublei_vEXT) numFailed++;
	_ptrc_glGetPointeri_vEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLvoid* *))IntGetProcAddress("glGetPointeri_vEXT");
	if(!_ptrc_glGetPointeri_vEXT) numFailed++;*/
	_ptrc_glCompressedTextureImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTextureImage3DEXT");
	if(!_ptrc_glCompressedTextureImage3DEXT) numFailed++;
	_ptrc_glCompressedTextureImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTextureImage2DEXT");
	if(!_ptrc_glCompressedTextureImage2DEXT) numFailed++;
	_ptrc_glCompressedTextureImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLenum , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTextureImage1DEXT");
	if(!_ptrc_glCompressedTextureImage1DEXT) numFailed++;
	_ptrc_glCompressedTextureSubImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTextureSubImage3DEXT");
	if(!_ptrc_glCompressedTextureSubImage3DEXT) numFailed++;
	_ptrc_glCompressedTextureSubImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTextureSubImage2DEXT");
	if(!_ptrc_glCompressedTextureSubImage2DEXT) numFailed++;
	_ptrc_glCompressedTextureSubImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLint , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTextureSubImage1DEXT");
	if(!_ptrc_glCompressedTextureSubImage1DEXT) numFailed++;
	_ptrc_glGetCompressedTextureImageEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint , GLvoid *))IntGetProcAddress("glGetCompressedTextureImageEXT");
	if(!_ptrc_glGetCompressedTextureImageEXT) numFailed++;
	_ptrc_glCompressedMultiTexImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedMultiTexImage3DEXT");
	if(!_ptrc_glCompressedMultiTexImage3DEXT) numFailed++;
	_ptrc_glCompressedMultiTexImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedMultiTexImage2DEXT");
	if(!_ptrc_glCompressedMultiTexImage2DEXT) numFailed++;
	_ptrc_glCompressedMultiTexImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLenum , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedMultiTexImage1DEXT");
	if(!_ptrc_glCompressedMultiTexImage1DEXT) numFailed++;
	_ptrc_glCompressedMultiTexSubImage3DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedMultiTexSubImage3DEXT");
	if(!_ptrc_glCompressedMultiTexSubImage3DEXT) numFailed++;
	_ptrc_glCompressedMultiTexSubImage2DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedMultiTexSubImage2DEXT");
	if(!_ptrc_glCompressedMultiTexSubImage2DEXT) numFailed++;
	_ptrc_glCompressedMultiTexSubImage1DEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLint , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedMultiTexSubImage1DEXT");
	if(!_ptrc_glCompressedMultiTexSubImage1DEXT) numFailed++;
	_ptrc_glGetCompressedMultiTexImageEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLvoid *))IntGetProcAddress("glGetCompressedMultiTexImageEXT");
	if(!_ptrc_glGetCompressedMultiTexImageEXT) numFailed++;
	/*_ptrc_glNamedProgramStringEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glNamedProgramStringEXT");
	if(!_ptrc_glNamedProgramStringEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameter4dEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLdouble , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glNamedProgramLocalParameter4dEXT");
	if(!_ptrc_glNamedProgramLocalParameter4dEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameter4dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , const GLdouble *))IntGetProcAddress("glNamedProgramLocalParameter4dvEXT");
	if(!_ptrc_glNamedProgramLocalParameter4dvEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameter4fEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glNamedProgramLocalParameter4fEXT");
	if(!_ptrc_glNamedProgramLocalParameter4fEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameter4fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , const GLfloat *))IntGetProcAddress("glNamedProgramLocalParameter4fvEXT");
	if(!_ptrc_glNamedProgramLocalParameter4fvEXT) numFailed++;
	_ptrc_glGetNamedProgramLocalParameterdvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLdouble *))IntGetProcAddress("glGetNamedProgramLocalParameterdvEXT");
	if(!_ptrc_glGetNamedProgramLocalParameterdvEXT) numFailed++;
	_ptrc_glGetNamedProgramLocalParameterfvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLfloat *))IntGetProcAddress("glGetNamedProgramLocalParameterfvEXT");
	if(!_ptrc_glGetNamedProgramLocalParameterfvEXT) numFailed++;
	_ptrc_glGetNamedProgramivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLint *))IntGetProcAddress("glGetNamedProgramivEXT");
	if(!_ptrc_glGetNamedProgramivEXT) numFailed++;
	_ptrc_glGetNamedProgramStringEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLvoid *))IntGetProcAddress("glGetNamedProgramStringEXT");
	if(!_ptrc_glGetNamedProgramStringEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameters4fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLsizei , const GLfloat *))IntGetProcAddress("glNamedProgramLocalParameters4fvEXT");
	if(!_ptrc_glNamedProgramLocalParameters4fvEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameterI4iEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glNamedProgramLocalParameterI4iEXT");
	if(!_ptrc_glNamedProgramLocalParameterI4iEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameterI4ivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , const GLint *))IntGetProcAddress("glNamedProgramLocalParameterI4ivEXT");
	if(!_ptrc_glNamedProgramLocalParameterI4ivEXT) numFailed++;
	_ptrc_glNamedProgramLocalParametersI4ivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLsizei , const GLint *))IntGetProcAddress("glNamedProgramLocalParametersI4ivEXT");
	if(!_ptrc_glNamedProgramLocalParametersI4ivEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameterI4uiEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLuint , GLuint , GLuint , GLuint ))IntGetProcAddress("glNamedProgramLocalParameterI4uiEXT");
	if(!_ptrc_glNamedProgramLocalParameterI4uiEXT) numFailed++;
	_ptrc_glNamedProgramLocalParameterI4uivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , const GLuint *))IntGetProcAddress("glNamedProgramLocalParameterI4uivEXT");
	if(!_ptrc_glNamedProgramLocalParameterI4uivEXT) numFailed++;
	_ptrc_glNamedProgramLocalParametersI4uivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLsizei , const GLuint *))IntGetProcAddress("glNamedProgramLocalParametersI4uivEXT");
	if(!_ptrc_glNamedProgramLocalParametersI4uivEXT) numFailed++;
	_ptrc_glGetNamedProgramLocalParameterIivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLint *))IntGetProcAddress("glGetNamedProgramLocalParameterIivEXT");
	if(!_ptrc_glGetNamedProgramLocalParameterIivEXT) numFailed++;
	_ptrc_glGetNamedProgramLocalParameterIuivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLuint *))IntGetProcAddress("glGetNamedProgramLocalParameterIuivEXT");
	if(!_ptrc_glGetNamedProgramLocalParameterIuivEXT) numFailed++;*/
	/*_ptrc_glTextureParameterIivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , const GLint *))IntGetProcAddress("glTextureParameterIivEXT");
	if(!_ptrc_glTextureParameterIivEXT) numFailed++;
	_ptrc_glTextureParameterIuivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , const GLuint *))IntGetProcAddress("glTextureParameterIuivEXT");
	if(!_ptrc_glTextureParameterIuivEXT) numFailed++;
	_ptrc_glGetTextureParameterIivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLint *))IntGetProcAddress("glGetTextureParameterIivEXT");
	if(!_ptrc_glGetTextureParameterIivEXT) numFailed++;
	_ptrc_glGetTextureParameterIuivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLuint *))IntGetProcAddress("glGetTextureParameterIuivEXT");
	if(!_ptrc_glGetTextureParameterIuivEXT) numFailed++;
	_ptrc_glMultiTexParameterIivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLint *))IntGetProcAddress("glMultiTexParameterIivEXT");
	if(!_ptrc_glMultiTexParameterIivEXT) numFailed++;
	_ptrc_glMultiTexParameterIuivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , const GLuint *))IntGetProcAddress("glMultiTexParameterIuivEXT");
	if(!_ptrc_glMultiTexParameterIuivEXT) numFailed++;
	_ptrc_glGetMultiTexParameterIivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint *))IntGetProcAddress("glGetMultiTexParameterIivEXT");
	if(!_ptrc_glGetMultiTexParameterIivEXT) numFailed++;
	_ptrc_glGetMultiTexParameterIuivEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLuint *))IntGetProcAddress("glGetMultiTexParameterIuivEXT");
	if(!_ptrc_glGetMultiTexParameterIuivEXT) numFailed++;*/
	/*_ptrc_glProgramUniform1fEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat ))IntGetProcAddress("glProgramUniform1fEXT");
	if(!_ptrc_glProgramUniform1fEXT) numFailed++;
	_ptrc_glProgramUniform2fEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat , GLfloat ))IntGetProcAddress("glProgramUniform2fEXT");
	if(!_ptrc_glProgramUniform2fEXT) numFailed++;
	_ptrc_glProgramUniform3fEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glProgramUniform3fEXT");
	if(!_ptrc_glProgramUniform3fEXT) numFailed++;
	_ptrc_glProgramUniform4fEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glProgramUniform4fEXT");
	if(!_ptrc_glProgramUniform4fEXT) numFailed++;
	_ptrc_glProgramUniform1iEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint ))IntGetProcAddress("glProgramUniform1iEXT");
	if(!_ptrc_glProgramUniform1iEXT) numFailed++;
	_ptrc_glProgramUniform2iEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint ))IntGetProcAddress("glProgramUniform2iEXT");
	if(!_ptrc_glProgramUniform2iEXT) numFailed++;
	_ptrc_glProgramUniform3iEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glProgramUniform3iEXT");
	if(!_ptrc_glProgramUniform3iEXT) numFailed++;
	_ptrc_glProgramUniform4iEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glProgramUniform4iEXT");
	if(!_ptrc_glProgramUniform4iEXT) numFailed++;
	_ptrc_glProgramUniform1fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform1fvEXT");
	if(!_ptrc_glProgramUniform1fvEXT) numFailed++;
	_ptrc_glProgramUniform2fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform2fvEXT");
	if(!_ptrc_glProgramUniform2fvEXT) numFailed++;
	_ptrc_glProgramUniform3fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform3fvEXT");
	if(!_ptrc_glProgramUniform3fvEXT) numFailed++;
	_ptrc_glProgramUniform4fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLfloat *))IntGetProcAddress("glProgramUniform4fvEXT");
	if(!_ptrc_glProgramUniform4fvEXT) numFailed++;
	_ptrc_glProgramUniform1ivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform1ivEXT");
	if(!_ptrc_glProgramUniform1ivEXT) numFailed++;
	_ptrc_glProgramUniform2ivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform2ivEXT");
	if(!_ptrc_glProgramUniform2ivEXT) numFailed++;
	_ptrc_glProgramUniform3ivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform3ivEXT");
	if(!_ptrc_glProgramUniform3ivEXT) numFailed++;
	_ptrc_glProgramUniform4ivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLint *))IntGetProcAddress("glProgramUniform4ivEXT");
	if(!_ptrc_glProgramUniform4ivEXT) numFailed++;
	_ptrc_glProgramUniformMatrix2fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix2fvEXT");
	if(!_ptrc_glProgramUniformMatrix2fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix3fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix3fvEXT");
	if(!_ptrc_glProgramUniformMatrix3fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix4fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix4fvEXT");
	if(!_ptrc_glProgramUniformMatrix4fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix2x3fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix2x3fvEXT");
	if(!_ptrc_glProgramUniformMatrix2x3fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix3x2fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix3x2fvEXT");
	if(!_ptrc_glProgramUniformMatrix3x2fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix2x4fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix2x4fvEXT");
	if(!_ptrc_glProgramUniformMatrix2x4fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix4x2fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix4x2fvEXT");
	if(!_ptrc_glProgramUniformMatrix4x2fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix3x4fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix3x4fvEXT");
	if(!_ptrc_glProgramUniformMatrix3x4fvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix4x3fvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glProgramUniformMatrix4x3fvEXT");
	if(!_ptrc_glProgramUniformMatrix4x3fvEXT) numFailed++;
	_ptrc_glProgramUniform1uiEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint ))IntGetProcAddress("glProgramUniform1uiEXT");
	if(!_ptrc_glProgramUniform1uiEXT) numFailed++;
	_ptrc_glProgramUniform2uiEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint , GLuint ))IntGetProcAddress("glProgramUniform2uiEXT");
	if(!_ptrc_glProgramUniform2uiEXT) numFailed++;
	_ptrc_glProgramUniform3uiEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint , GLuint , GLuint ))IntGetProcAddress("glProgramUniform3uiEXT");
	if(!_ptrc_glProgramUniform3uiEXT) numFailed++;
	_ptrc_glProgramUniform4uiEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint , GLuint , GLuint , GLuint ))IntGetProcAddress("glProgramUniform4uiEXT");
	if(!_ptrc_glProgramUniform4uiEXT) numFailed++;
	_ptrc_glProgramUniform1uivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform1uivEXT");
	if(!_ptrc_glProgramUniform1uivEXT) numFailed++;
	_ptrc_glProgramUniform2uivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform2uivEXT");
	if(!_ptrc_glProgramUniform2uivEXT) numFailed++;
	_ptrc_glProgramUniform3uivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform3uivEXT");
	if(!_ptrc_glProgramUniform3uivEXT) numFailed++;
	_ptrc_glProgramUniform4uivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLuint *))IntGetProcAddress("glProgramUniform4uivEXT");
	if(!_ptrc_glProgramUniform4uivEXT) numFailed++;*/
	_ptrc_glNamedBufferDataEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizeiptr , const GLvoid *, GLenum ))IntGetProcAddress("glNamedBufferDataEXT");
	if(!_ptrc_glNamedBufferDataEXT) numFailed++;
	_ptrc_glNamedBufferSubDataEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLintptr , GLsizeiptr , const GLvoid *))IntGetProcAddress("glNamedBufferSubDataEXT");
	if(!_ptrc_glNamedBufferSubDataEXT) numFailed++;
	_ptrc_glMapNamedBufferEXT = (GLvoid* (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glMapNamedBufferEXT");
	if(!_ptrc_glMapNamedBufferEXT) numFailed++;
	_ptrc_glUnmapNamedBufferEXT = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glUnmapNamedBufferEXT");
	if(!_ptrc_glUnmapNamedBufferEXT) numFailed++;
	_ptrc_glMapNamedBufferRangeEXT = (GLvoid* (CODEGEN_FUNCPTR *)(GLuint , GLintptr , GLsizeiptr , GLbitfield ))IntGetProcAddress("glMapNamedBufferRangeEXT");
	if(!_ptrc_glMapNamedBufferRangeEXT) numFailed++;
	_ptrc_glFlushMappedNamedBufferRangeEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLintptr , GLsizeiptr ))IntGetProcAddress("glFlushMappedNamedBufferRangeEXT");
	if(!_ptrc_glFlushMappedNamedBufferRangeEXT) numFailed++;
	_ptrc_glNamedCopyBufferSubDataEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLintptr , GLintptr , GLsizeiptr ))IntGetProcAddress("glNamedCopyBufferSubDataEXT");
	if(!_ptrc_glNamedCopyBufferSubDataEXT) numFailed++;
	_ptrc_glGetNamedBufferParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetNamedBufferParameterivEXT");
	if(!_ptrc_glGetNamedBufferParameterivEXT) numFailed++;
	_ptrc_glGetNamedBufferPointervEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLvoid* *))IntGetProcAddress("glGetNamedBufferPointervEXT");
	if(!_ptrc_glGetNamedBufferPointervEXT) numFailed++;
	_ptrc_glGetNamedBufferSubDataEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLintptr , GLsizeiptr , GLvoid *))IntGetProcAddress("glGetNamedBufferSubDataEXT");
	if(!_ptrc_glGetNamedBufferSubDataEXT) numFailed++;
	_ptrc_glTextureBufferEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLuint ))IntGetProcAddress("glTextureBufferEXT");
	if(!_ptrc_glTextureBufferEXT) numFailed++;
	_ptrc_glMultiTexBufferEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLuint ))IntGetProcAddress("glMultiTexBufferEXT");
	if(!_ptrc_glMultiTexBufferEXT) numFailed++;
	_ptrc_glNamedRenderbufferStorageEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLsizei , GLsizei ))IntGetProcAddress("glNamedRenderbufferStorageEXT");
	if(!_ptrc_glNamedRenderbufferStorageEXT) numFailed++;
	_ptrc_glGetNamedRenderbufferParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetNamedRenderbufferParameterivEXT");
	if(!_ptrc_glGetNamedRenderbufferParameterivEXT) numFailed++;
	_ptrc_glCheckNamedFramebufferStatusEXT = (GLenum (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glCheckNamedFramebufferStatusEXT");
	if(!_ptrc_glCheckNamedFramebufferStatusEXT) numFailed++;
	_ptrc_glNamedFramebufferTexture1DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLuint , GLint ))IntGetProcAddress("glNamedFramebufferTexture1DEXT");
	if(!_ptrc_glNamedFramebufferTexture1DEXT) numFailed++;
	_ptrc_glNamedFramebufferTexture2DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLuint , GLint ))IntGetProcAddress("glNamedFramebufferTexture2DEXT");
	if(!_ptrc_glNamedFramebufferTexture2DEXT) numFailed++;
	_ptrc_glNamedFramebufferTexture3DEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLuint , GLint , GLint ))IntGetProcAddress("glNamedFramebufferTexture3DEXT");
	if(!_ptrc_glNamedFramebufferTexture3DEXT) numFailed++;
	_ptrc_glNamedFramebufferRenderbufferEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLuint ))IntGetProcAddress("glNamedFramebufferRenderbufferEXT");
	if(!_ptrc_glNamedFramebufferRenderbufferEXT) numFailed++;
	_ptrc_glGetNamedFramebufferAttachmentParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLenum , GLint *))IntGetProcAddress("glGetNamedFramebufferAttachmentParameterivEXT");
	if(!_ptrc_glGetNamedFramebufferAttachmentParameterivEXT) numFailed++;
	_ptrc_glGenerateTextureMipmapEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glGenerateTextureMipmapEXT");
	if(!_ptrc_glGenerateTextureMipmapEXT) numFailed++;
	_ptrc_glGenerateMultiTexMipmapEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glGenerateMultiTexMipmapEXT");
	if(!_ptrc_glGenerateMultiTexMipmapEXT) numFailed++;
	_ptrc_glFramebufferDrawBufferEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glFramebufferDrawBufferEXT");
	if(!_ptrc_glFramebufferDrawBufferEXT) numFailed++;
	_ptrc_glFramebufferDrawBuffersEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , const GLenum *))IntGetProcAddress("glFramebufferDrawBuffersEXT");
	if(!_ptrc_glFramebufferDrawBuffersEXT) numFailed++;
	_ptrc_glFramebufferReadBufferEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glFramebufferReadBufferEXT");
	if(!_ptrc_glFramebufferReadBufferEXT) numFailed++;
	_ptrc_glGetFramebufferParameterivEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetFramebufferParameterivEXT");
	if(!_ptrc_glGetFramebufferParameterivEXT) numFailed++;
	/*_ptrc_glNamedRenderbufferStorageMultisampleEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLenum , GLsizei , GLsizei ))IntGetProcAddress("glNamedRenderbufferStorageMultisampleEXT");
	if(!_ptrc_glNamedRenderbufferStorageMultisampleEXT) numFailed++;*/
	/*_ptrc_glNamedRenderbufferStorageMultisampleCoverageEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLsizei , GLenum , GLsizei , GLsizei ))IntGetProcAddress("glNamedRenderbufferStorageMultisampleCoverageEXT");
	if(!_ptrc_glNamedRenderbufferStorageMultisampleCoverageEXT) numFailed++;*/
	_ptrc_glNamedFramebufferTextureEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLint ))IntGetProcAddress("glNamedFramebufferTextureEXT");
	if(!_ptrc_glNamedFramebufferTextureEXT) numFailed++;
	_ptrc_glNamedFramebufferTextureLayerEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLint , GLint ))IntGetProcAddress("glNamedFramebufferTextureLayerEXT");
	if(!_ptrc_glNamedFramebufferTextureLayerEXT) numFailed++;
	_ptrc_glNamedFramebufferTextureFaceEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint , GLint , GLenum ))IntGetProcAddress("glNamedFramebufferTextureFaceEXT");
	if(!_ptrc_glNamedFramebufferTextureFaceEXT) numFailed++;
	_ptrc_glTextureRenderbufferEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint ))IntGetProcAddress("glTextureRenderbufferEXT");
	if(!_ptrc_glTextureRenderbufferEXT) numFailed++;
	_ptrc_glMultiTexRenderbufferEXT = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glMultiTexRenderbufferEXT");
	if(!_ptrc_glMultiTexRenderbufferEXT) numFailed++;
	/*_ptrc_glProgramUniform1dEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble ))IntGetProcAddress("glProgramUniform1dEXT");
	if(!_ptrc_glProgramUniform1dEXT) numFailed++;
	_ptrc_glProgramUniform2dEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble , GLdouble ))IntGetProcAddress("glProgramUniform2dEXT");
	if(!_ptrc_glProgramUniform2dEXT) numFailed++;
	_ptrc_glProgramUniform3dEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glProgramUniform3dEXT");
	if(!_ptrc_glProgramUniform3dEXT) numFailed++;
	_ptrc_glProgramUniform4dEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLdouble , GLdouble , GLdouble , GLdouble ))IntGetProcAddress("glProgramUniform4dEXT");
	if(!_ptrc_glProgramUniform4dEXT) numFailed++;
	_ptrc_glProgramUniform1dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform1dvEXT");
	if(!_ptrc_glProgramUniform1dvEXT) numFailed++;
	_ptrc_glProgramUniform2dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform2dvEXT");
	if(!_ptrc_glProgramUniform2dvEXT) numFailed++;
	_ptrc_glProgramUniform3dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform3dvEXT");
	if(!_ptrc_glProgramUniform3dvEXT) numFailed++;
	_ptrc_glProgramUniform4dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , const GLdouble *))IntGetProcAddress("glProgramUniform4dvEXT");
	if(!_ptrc_glProgramUniform4dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix2dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix2dvEXT");
	if(!_ptrc_glProgramUniformMatrix2dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix3dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix3dvEXT");
	if(!_ptrc_glProgramUniformMatrix3dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix4dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix4dvEXT");
	if(!_ptrc_glProgramUniformMatrix4dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix2x3dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix2x3dvEXT");
	if(!_ptrc_glProgramUniformMatrix2x3dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix2x4dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix2x4dvEXT");
	if(!_ptrc_glProgramUniformMatrix2x4dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix3x2dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix3x2dvEXT");
	if(!_ptrc_glProgramUniformMatrix3x2dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix3x4dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix3x4dvEXT");
	if(!_ptrc_glProgramUniformMatrix3x4dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix4x2dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix4x2dvEXT");
	if(!_ptrc_glProgramUniformMatrix4x2dvEXT) numFailed++;
	_ptrc_glProgramUniformMatrix4x3dvEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLsizei , GLboolean , const GLdouble *))IntGetProcAddress("glProgramUniformMatrix4x3dvEXT");
	if(!_ptrc_glProgramUniformMatrix4x3dvEXT) numFailed++;*/
	/*_ptrc_glEnableVertexArrayAttribEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glEnableVertexArrayAttribEXT");
	if(!_ptrc_glEnableVertexArrayAttribEXT) numFailed++;
	_ptrc_glDisableVertexArrayAttribEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glDisableVertexArrayAttribEXT");
	if(!_ptrc_glDisableVertexArrayAttribEXT) numFailed++;
	_ptrc_glEnableVertexArrayEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glEnableVertexArrayEXT");
	if(!_ptrc_glEnableVertexArrayEXT) numFailed++;
	_ptrc_glDisableVertexArrayEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glDisableVertexArrayEXT");
	if(!_ptrc_glDisableVertexArrayEXT) numFailed++;
	_ptrc_glVertexArrayColorOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayColorOffsetEXT");
	if(!_ptrc_glVertexArrayColorOffsetEXT) numFailed++;
	_ptrc_glVertexArrayEdgeFlagOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayEdgeFlagOffsetEXT");
	if(!_ptrc_glVertexArrayEdgeFlagOffsetEXT) numFailed++;
	_ptrc_glVertexArrayFogCoordOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayFogCoordOffsetEXT");
	if(!_ptrc_glVertexArrayFogCoordOffsetEXT) numFailed++;
	_ptrc_glVertexArrayIndexOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayIndexOffsetEXT");
	if(!_ptrc_glVertexArrayIndexOffsetEXT) numFailed++;
	_ptrc_glVertexArrayMultiTexCoordOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayMultiTexCoordOffsetEXT");
	if(!_ptrc_glVertexArrayMultiTexCoordOffsetEXT) numFailed++;
	_ptrc_glVertexArrayNormalOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayNormalOffsetEXT");
	if(!_ptrc_glVertexArrayNormalOffsetEXT) numFailed++;
	_ptrc_glVertexArraySecondaryColorOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArraySecondaryColorOffsetEXT");
	if(!_ptrc_glVertexArraySecondaryColorOffsetEXT) numFailed++;
	_ptrc_glVertexArrayTexCoordOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayTexCoordOffsetEXT");
	if(!_ptrc_glVertexArrayTexCoordOffsetEXT) numFailed++;
	_ptrc_glVertexArrayVertexOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayVertexOffsetEXT");
	if(!_ptrc_glVertexArrayVertexOffsetEXT) numFailed++;
	_ptrc_glVertexArrayVertexAttribIOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint , GLint , GLenum , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayVertexAttribIOffsetEXT");
	if(!_ptrc_glVertexArrayVertexAttribIOffsetEXT) numFailed++;
	_ptrc_glVertexArrayVertexAttribOffsetEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint , GLint , GLenum , GLboolean , GLsizei , GLintptr ))IntGetProcAddress("glVertexArrayVertexAttribOffsetEXT");
	if(!_ptrc_glVertexArrayVertexAttribOffsetEXT) numFailed++;
	_ptrc_glGetVertexArrayIntegervEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetVertexArrayIntegervEXT");
	if(!_ptrc_glGetVertexArrayIntegervEXT) numFailed++;
	_ptrc_glGetVertexArrayPointervEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLvoid* *))IntGetProcAddress("glGetVertexArrayPointervEXT");
	if(!_ptrc_glGetVertexArrayPointervEXT) numFailed++;
	_ptrc_glGetVertexArrayIntegeri_vEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLint *))IntGetProcAddress("glGetVertexArrayIntegeri_vEXT");
	if(!_ptrc_glGetVertexArrayIntegeri_vEXT) numFailed++;
	_ptrc_glGetVertexArrayPointeri_vEXT = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLvoid* *))IntGetProcAddress("glGetVertexArrayPointeri_vEXT");
	if(!_ptrc_glGetVertexArrayPointeri_vEXT) numFailed++;*/
	return numFailed;
}

void (CODEGEN_FUNCPTR *_ptrc_glDebugMessageControlARB)(GLenum , GLenum , GLenum , GLsizei , const GLuint *, GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDebugMessageInsertARB)(GLenum , GLenum , GLuint , GLenum , GLsizei , const GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDebugMessageCallbackARB)(GLDEBUGPROCARB , const GLvoid *) = NULL;
GLuint (CODEGEN_FUNCPTR *_ptrc_glGetDebugMessageLogARB)(GLuint , GLsizei , GLenum *, GLenum *, GLuint *, GLenum *, GLsizei *, GLchar *) = NULL;

static int Load_ARB_debug_output()
{
	int numFailed = 0;
	_ptrc_glDebugMessageControlARB = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLsizei , const GLuint *, GLboolean ))IntGetProcAddress("glDebugMessageControlARB");
	if(!_ptrc_glDebugMessageControlARB) numFailed++;
	_ptrc_glDebugMessageInsertARB = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint , GLenum , GLsizei , const GLchar *))IntGetProcAddress("glDebugMessageInsertARB");
	if(!_ptrc_glDebugMessageInsertARB) numFailed++;
	_ptrc_glDebugMessageCallbackARB = (void (CODEGEN_FUNCPTR *)(GLDEBUGPROCARB , const GLvoid *))IntGetProcAddress("glDebugMessageCallbackARB");
	if(!_ptrc_glDebugMessageCallbackARB) numFailed++;
	_ptrc_glGetDebugMessageLogARB = (GLuint (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLenum *, GLenum *, GLuint *, GLenum *, GLsizei *, GLchar *))IntGetProcAddress("glGetDebugMessageLogARB");
	if(!_ptrc_glGetDebugMessageLogARB) numFailed++;
	return numFailed;
}

void (CODEGEN_FUNCPTR *_ptrc_glCullFace)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFrontFace)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glHint)(GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glLineWidth)(GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPointSize)(GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPolygonMode)(GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glScissor)(GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexParameterf)(GLenum , GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexParameterfv)(GLenum , GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexParameteri)(GLenum , GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexParameteriv)(GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexImage1D)(GLenum , GLint , GLint , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexImage2D)(GLenum , GLint , GLint , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawBuffer)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClear)(GLbitfield ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearColor)(GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearStencil)(GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearDepth)(GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glStencilMask)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glColorMask)(GLboolean , GLboolean , GLboolean , GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDepthMask)(GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisable)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnable)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFinish)() = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFlush)() = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBlendFunc)(GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glLogicOp)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glStencilFunc)(GLenum , GLint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glStencilOp)(GLenum , GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDepthFunc)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPixelStoref)(GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPixelStorei)(GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glReadBuffer)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glReadPixels)(GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetBooleanv)(GLenum , GLboolean *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetDoublev)(GLenum , GLdouble *) = NULL;
GLenum (CODEGEN_FUNCPTR *_ptrc_glGetError)() = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetFloatv)(GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetIntegerv)(GLenum , GLint *) = NULL;
const GLubyte * (CODEGEN_FUNCPTR *_ptrc_glGetString)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexImage)(GLenum , GLint , GLenum , GLenum , GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexParameterfv)(GLenum , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexParameteriv)(GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexLevelParameterfv)(GLenum , GLint , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexLevelParameteriv)(GLenum , GLint , GLenum , GLint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsEnabled)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDepthRange)(GLdouble , GLdouble ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glViewport)(GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawArrays)(GLenum , GLint , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawElements)(GLenum , GLsizei , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetPointerv)(GLenum , GLvoid* *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPolygonOffset)(GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTexImage1D)(GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTexImage2D)(GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTexSubImage1D)(GLenum , GLint , GLint , GLint , GLint , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTexSubImage2D)(GLenum , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexSubImage1D)(GLenum , GLint , GLint , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexSubImage2D)(GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindTexture)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteTextures)(GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenTextures)(GLsizei , GLuint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsTexture)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glIndexub)(GLubyte ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glIndexubv)(const GLubyte *) = NULL;


void (CODEGEN_FUNCPTR *_ptrc_glBlendColor)(GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBlendEquation)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawRangeElements)(GLenum , GLuint , GLuint , GLsizei , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexImage3D)(GLenum , GLint , GLint , GLsizei , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexSubImage3D)(GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCopyTexSubImage3D)(GLenum , GLint , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glActiveTexture)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSampleCoverage)(GLfloat , GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTexImage3D)(GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTexImage2D)(GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTexImage1D)(GLenum , GLint , GLenum , GLsizei , GLint , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTexSubImage3D)(GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTexSubImage2D)(GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompressedTexSubImage1D)(GLenum , GLint , GLint , GLsizei , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetCompressedTexImage)(GLenum , GLint , GLvoid *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glBlendFuncSeparate)(GLenum , GLenum , GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiDrawArrays)(GLenum , const GLint *, const GLsizei *, GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiDrawElements)(GLenum , const GLsizei *, GLenum , const GLvoid* const *, GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPointParameterf)(GLenum , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPointParameterfv)(GLenum , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPointParameteri)(GLenum , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPointParameteriv)(GLenum , const GLint *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glGenQueries)(GLsizei , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteQueries)(GLsizei , const GLuint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsQuery)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBeginQuery)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEndQuery)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetQueryiv)(GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetQueryObjectiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetQueryObjectuiv)(GLuint , GLenum , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindBuffer)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteBuffers)(GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenBuffers)(GLsizei , GLuint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsBuffer)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBufferData)(GLenum , GLsizeiptr , const GLvoid *, GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBufferSubData)(GLenum , GLintptr , GLsizeiptr , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetBufferSubData)(GLenum , GLintptr , GLsizeiptr , GLvoid *) = NULL;
GLvoid* (CODEGEN_FUNCPTR *_ptrc_glMapBuffer)(GLenum , GLenum ) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glUnmapBuffer)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetBufferParameteriv)(GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetBufferPointerv)(GLenum , GLenum , GLvoid* *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glBlendEquationSeparate)(GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawBuffers)(GLsizei , const GLenum *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glStencilOpSeparate)(GLenum , GLenum , GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glStencilFuncSeparate)(GLenum , GLenum , GLint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glStencilMaskSeparate)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glAttachShader)(GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindAttribLocation)(GLuint , GLuint , const GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glCompileShader)(GLuint ) = NULL;
GLuint (CODEGEN_FUNCPTR *_ptrc_glCreateProgram)() = NULL;
GLuint (CODEGEN_FUNCPTR *_ptrc_glCreateShader)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteProgram)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteShader)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDetachShader)(GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisableVertexAttribArray)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnableVertexAttribArray)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetActiveAttrib)(GLuint , GLuint , GLsizei , GLsizei *, GLint *, GLenum *, GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetActiveUniform)(GLuint , GLuint , GLsizei , GLsizei *, GLint *, GLenum *, GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetAttachedShaders)(GLuint , GLsizei , GLsizei *, GLuint *) = NULL;
GLint (CODEGEN_FUNCPTR *_ptrc_glGetAttribLocation)(GLuint , const GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetProgramiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetProgramInfoLog)(GLuint , GLsizei , GLsizei *, GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetShaderiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetShaderInfoLog)(GLuint , GLsizei , GLsizei *, GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetShaderSource)(GLuint , GLsizei , GLsizei *, GLchar *) = NULL;
GLint (CODEGEN_FUNCPTR *_ptrc_glGetUniformLocation)(GLuint , const GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetUniformfv)(GLuint , GLint , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetUniformiv)(GLuint , GLint , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexAttribdv)(GLuint , GLenum , GLdouble *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexAttribfv)(GLuint , GLenum , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexAttribiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexAttribPointerv)(GLuint , GLenum , GLvoid* *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsProgram)(GLuint ) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsShader)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glLinkProgram)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glShaderSource)(GLuint , GLsizei , const GLchar* const *, const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUseProgram)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform1f)(GLint , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform2f)(GLint , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform3f)(GLint , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform4f)(GLint , GLfloat , GLfloat , GLfloat , GLfloat ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform1i)(GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform2i)(GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform3i)(GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform4i)(GLint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform1fv)(GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform2fv)(GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform3fv)(GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform4fv)(GLint , GLsizei , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform1iv)(GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform2iv)(GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform3iv)(GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform4iv)(GLint , GLsizei , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix2fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix3fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix4fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glValidateProgram)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribPointer)(GLuint , GLint , GLenum , GLboolean , GLsizei , const GLvoid *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix2x3fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix3x2fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix2x4fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix4x2fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix3x4fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformMatrix4x3fv)(GLint , GLsizei , GLboolean , const GLfloat *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glBindVertexArray)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteVertexArrays)(GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenVertexArrays)(GLsizei , GLuint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsVertexArray)(GLuint ) = NULL;



GLvoid* (CODEGEN_FUNCPTR *_ptrc_glMapBufferRange)(GLenum , GLintptr , GLsizeiptr , GLbitfield ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFlushMappedBufferRange)(GLenum , GLintptr , GLsizeiptr ) = NULL;



GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsRenderbuffer)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindRenderbuffer)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteRenderbuffers)(GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenRenderbuffers)(GLsizei , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glRenderbufferStorage)(GLenum , GLenum , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetRenderbufferParameteriv)(GLenum , GLenum , GLint *) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsFramebuffer)(GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindFramebuffer)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteFramebuffers)(GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenFramebuffers)(GLsizei , GLuint *) = NULL;
GLenum (CODEGEN_FUNCPTR *_ptrc_glCheckFramebufferStatus)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferTexture1D)(GLenum , GLenum , GLenum , GLuint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferTexture2D)(GLenum , GLenum , GLenum , GLuint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferTexture3D)(GLenum , GLenum , GLenum , GLuint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferRenderbuffer)(GLenum , GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetFramebufferAttachmentParameteriv)(GLenum , GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGenerateMipmap)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBlitFramebuffer)(GLint , GLint , GLint , GLint , GLint , GLint , GLint , GLint , GLbitfield , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glRenderbufferStorageMultisample)(GLenum , GLsizei , GLenum , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferTextureLayer)(GLenum , GLenum , GLuint , GLint , GLint ) = NULL;


void (CODEGEN_FUNCPTR *_ptrc_glColorMaski)(GLuint , GLboolean , GLboolean , GLboolean , GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetBooleani_v)(GLenum , GLuint , GLboolean *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetIntegeri_v)(GLenum , GLuint , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEnablei)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDisablei)(GLenum , GLuint ) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsEnabledi)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBeginTransformFeedback)(GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEndTransformFeedback)() = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindBufferRange)(GLenum , GLuint , GLuint , GLintptr , GLsizeiptr ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindBufferBase)(GLenum , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTransformFeedbackVaryings)(GLuint , GLsizei , const GLchar* const *, GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTransformFeedbackVarying)(GLuint , GLuint , GLsizei , GLsizei *, GLsizei *, GLenum *, GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClampColor)(GLenum , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBeginConditionalRender)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glEndConditionalRender)() = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribIPointer)(GLuint , GLint , GLenum , GLsizei , const GLvoid *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexAttribIiv)(GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetVertexAttribIuiv)(GLuint , GLenum , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI1i)(GLuint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI2i)(GLuint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI3i)(GLuint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4i)(GLuint , GLint , GLint , GLint , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI1ui)(GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI2ui)(GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI3ui)(GLuint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4ui)(GLuint , GLuint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI1iv)(GLuint , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI2iv)(GLuint , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI3iv)(GLuint , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4iv)(GLuint , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI1uiv)(GLuint , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI2uiv)(GLuint , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI3uiv)(GLuint , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4uiv)(GLuint , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4bv)(GLuint , const GLbyte *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4sv)(GLuint , const GLshort *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4ubv)(GLuint , const GLubyte *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribI4usv)(GLuint , const GLushort *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetUniformuiv)(GLuint , GLint , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glBindFragDataLocation)(GLuint , GLuint , const GLchar *) = NULL;
GLint (CODEGEN_FUNCPTR *_ptrc_glGetFragDataLocation)(GLuint , const GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform1ui)(GLint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform2ui)(GLint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform3ui)(GLint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform4ui)(GLint , GLuint , GLuint , GLuint , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform1uiv)(GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform2uiv)(GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform3uiv)(GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniform4uiv)(GLint , GLsizei , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexParameterIiv)(GLenum , GLenum , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexParameterIuiv)(GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexParameterIiv)(GLenum , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetTexParameterIuiv)(GLenum , GLenum , GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearBufferiv)(GLenum , GLint , const GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearBufferuiv)(GLenum , GLint , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearBufferfv)(GLenum , GLint , const GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glClearBufferfi)(GLenum , GLint , GLfloat , GLint ) = NULL;
const GLubyte * (CODEGEN_FUNCPTR *_ptrc_glGetStringi)(GLenum , GLuint ) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glGetUniformIndices)(GLuint , GLsizei , const GLchar* const *, GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetActiveUniformsiv)(GLuint , GLsizei , const GLuint *, GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetActiveUniformName)(GLuint , GLuint , GLsizei , GLsizei *, GLchar *) = NULL;
GLuint (CODEGEN_FUNCPTR *_ptrc_glGetUniformBlockIndex)(GLuint , const GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetActiveUniformBlockiv)(GLuint , GLuint , GLenum , GLint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetActiveUniformBlockName)(GLuint , GLuint , GLsizei , GLsizei *, GLchar *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glUniformBlockBinding)(GLuint , GLuint , GLuint ) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glCopyBufferSubData)(GLenum , GLenum , GLintptr , GLintptr , GLsizeiptr ) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glDrawArraysInstanced)(GLenum , GLint , GLsizei , GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawElementsInstanced)(GLenum , GLsizei , GLenum , const GLvoid *, GLsizei ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexBuffer)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glPrimitiveRestartIndex)(GLuint ) = NULL;


void (CODEGEN_FUNCPTR *_ptrc_glDrawElementsBaseVertex)(GLenum , GLsizei , GLenum , const GLvoid *, GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawRangeElementsBaseVertex)(GLenum , GLuint , GLuint , GLsizei , GLenum , const GLvoid *, GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDrawElementsInstancedBaseVertex)(GLenum , GLsizei , GLenum , const GLvoid *, GLsizei , GLint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiDrawElementsBaseVertex)(GLenum , const GLsizei *, GLenum , const GLvoid* const *, GLsizei , const GLint *) = NULL;


void (CODEGEN_FUNCPTR *_ptrc_glProvokingVertex)(GLenum ) = NULL;


GLsync (CODEGEN_FUNCPTR *_ptrc_glFenceSync)(GLenum , GLbitfield ) = NULL;
GLboolean (CODEGEN_FUNCPTR *_ptrc_glIsSync)(GLsync ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glDeleteSync)(GLsync ) = NULL;
GLenum (CODEGEN_FUNCPTR *_ptrc_glClientWaitSync)(GLsync , GLbitfield , GLuint64 ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glWaitSync)(GLsync , GLbitfield , GLuint64 ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetInteger64v)(GLenum , GLint64 *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetSynciv)(GLsync , GLenum , GLsizei , GLsizei *, GLint *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glTexImage2DMultisample)(GLenum , GLsizei , GLint , GLsizei , GLsizei , GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexImage3DMultisample)(GLenum , GLsizei , GLint , GLsizei , GLsizei , GLsizei , GLboolean ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetMultisamplefv)(GLenum , GLuint , GLfloat *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSampleMaski)(GLuint , GLbitfield ) = NULL;


void (CODEGEN_FUNCPTR *_ptrc_glGetInteger64i_v)(GLenum , GLuint , GLint64 *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetBufferParameteri64v)(GLenum , GLenum , GLint64 *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glFramebufferTexture)(GLenum , GLenum , GLuint , GLint ) = NULL;



void (CODEGEN_FUNCPTR *_ptrc_glQueryCounter)(GLuint , GLenum ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetQueryObjecti64v)(GLuint , GLenum , GLint64 *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glGetQueryObjectui64v)(GLuint , GLenum , GLuint64 *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glVertexP2ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexP2uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexP3ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexP3uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexP4ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexP4uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP1ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP1uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP2ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP2uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP3ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP3uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP4ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glTexCoordP4uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP1ui)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP1uiv)(GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP2ui)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP2uiv)(GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP3ui)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP3uiv)(GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP4ui)(GLenum , GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glMultiTexCoordP4uiv)(GLenum , GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNormalP3ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glNormalP3uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glColorP3ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glColorP3uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glColorP4ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glColorP4uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSecondaryColorP3ui)(GLenum , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glSecondaryColorP3uiv)(GLenum , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP1ui)(GLuint , GLenum , GLboolean , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP1uiv)(GLuint , GLenum , GLboolean , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP2ui)(GLuint , GLenum , GLboolean , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP2uiv)(GLuint , GLenum , GLboolean , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP3ui)(GLuint , GLenum , GLboolean , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP3uiv)(GLuint , GLenum , GLboolean , const GLuint *) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP4ui)(GLuint , GLenum , GLboolean , GLuint ) = NULL;
void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribP4uiv)(GLuint , GLenum , GLboolean , const GLuint *) = NULL;

void (CODEGEN_FUNCPTR *_ptrc_glBindFragDataLocationIndexed)(GLuint , GLuint , GLuint , const GLchar *) = NULL;
GLint (CODEGEN_FUNCPTR *_ptrc_glGetFragDataIndex)(GLuint , const GLchar *) = NULL;


void (CODEGEN_FUNCPTR *_ptrc_glVertexAttribDivisor)(GLuint , GLuint ) = NULL;

static int Load_Version_3_3()
{
	int numFailed = 0;
	_ptrc_glCullFace = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glCullFace");
	if(!_ptrc_glCullFace) numFailed++;
	_ptrc_glFrontFace = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glFrontFace");
	if(!_ptrc_glFrontFace) numFailed++;
	_ptrc_glHint = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glHint");
	if(!_ptrc_glHint) numFailed++;
	_ptrc_glLineWidth = (void (CODEGEN_FUNCPTR *)(GLfloat ))IntGetProcAddress("glLineWidth");
	if(!_ptrc_glLineWidth) numFailed++;
	_ptrc_glPointSize = (void (CODEGEN_FUNCPTR *)(GLfloat ))IntGetProcAddress("glPointSize");
	if(!_ptrc_glPointSize) numFailed++;
	_ptrc_glPolygonMode = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glPolygonMode");
	if(!_ptrc_glPolygonMode) numFailed++;
	_ptrc_glScissor = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glScissor");
	if(!_ptrc_glScissor) numFailed++;
	_ptrc_glTexParameterf = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLfloat ))IntGetProcAddress("glTexParameterf");
	if(!_ptrc_glTexParameterf) numFailed++;
	_ptrc_glTexParameterfv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLfloat *))IntGetProcAddress("glTexParameterfv");
	if(!_ptrc_glTexParameterfv) numFailed++;
	_ptrc_glTexParameteri = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint ))IntGetProcAddress("glTexParameteri");
	if(!_ptrc_glTexParameteri) numFailed++;
	_ptrc_glTexParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLint *))IntGetProcAddress("glTexParameteriv");
	if(!_ptrc_glTexParameteriv) numFailed++;
	_ptrc_glTexImage1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTexImage1D");
	if(!_ptrc_glTexImage1D) numFailed++;
	_ptrc_glTexImage2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTexImage2D");
	if(!_ptrc_glTexImage2D) numFailed++;
	_ptrc_glDrawBuffer = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glDrawBuffer");
	if(!_ptrc_glDrawBuffer) numFailed++;
	_ptrc_glClear = (void (CODEGEN_FUNCPTR *)(GLbitfield ))IntGetProcAddress("glClear");
	if(!_ptrc_glClear) numFailed++;
	_ptrc_glClearColor = (void (CODEGEN_FUNCPTR *)(GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glClearColor");
	if(!_ptrc_glClearColor) numFailed++;
	_ptrc_glClearStencil = (void (CODEGEN_FUNCPTR *)(GLint ))IntGetProcAddress("glClearStencil");
	if(!_ptrc_glClearStencil) numFailed++;
	_ptrc_glClearDepth = (void (CODEGEN_FUNCPTR *)(GLdouble ))IntGetProcAddress("glClearDepth");
	if(!_ptrc_glClearDepth) numFailed++;
	_ptrc_glStencilMask = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glStencilMask");
	if(!_ptrc_glStencilMask) numFailed++;
	_ptrc_glColorMask = (void (CODEGEN_FUNCPTR *)(GLboolean , GLboolean , GLboolean , GLboolean ))IntGetProcAddress("glColorMask");
	if(!_ptrc_glColorMask) numFailed++;
	_ptrc_glDepthMask = (void (CODEGEN_FUNCPTR *)(GLboolean ))IntGetProcAddress("glDepthMask");
	if(!_ptrc_glDepthMask) numFailed++;
	_ptrc_glDisable = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glDisable");
	if(!_ptrc_glDisable) numFailed++;
	_ptrc_glEnable = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glEnable");
	if(!_ptrc_glEnable) numFailed++;
	_ptrc_glFinish = (void (CODEGEN_FUNCPTR *)())IntGetProcAddress("glFinish");
	if(!_ptrc_glFinish) numFailed++;
	_ptrc_glFlush = (void (CODEGEN_FUNCPTR *)())IntGetProcAddress("glFlush");
	if(!_ptrc_glFlush) numFailed++;
	_ptrc_glBlendFunc = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glBlendFunc");
	if(!_ptrc_glBlendFunc) numFailed++;
	_ptrc_glLogicOp = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glLogicOp");
	if(!_ptrc_glLogicOp) numFailed++;
	_ptrc_glStencilFunc = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLuint ))IntGetProcAddress("glStencilFunc");
	if(!_ptrc_glStencilFunc) numFailed++;
	_ptrc_glStencilOp = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum ))IntGetProcAddress("glStencilOp");
	if(!_ptrc_glStencilOp) numFailed++;
	_ptrc_glDepthFunc = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glDepthFunc");
	if(!_ptrc_glDepthFunc) numFailed++;
	_ptrc_glPixelStoref = (void (CODEGEN_FUNCPTR *)(GLenum , GLfloat ))IntGetProcAddress("glPixelStoref");
	if(!_ptrc_glPixelStoref) numFailed++;
	_ptrc_glPixelStorei = (void (CODEGEN_FUNCPTR *)(GLenum , GLint ))IntGetProcAddress("glPixelStorei");
	if(!_ptrc_glPixelStorei) numFailed++;
	_ptrc_glReadBuffer = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glReadBuffer");
	if(!_ptrc_glReadBuffer) numFailed++;
	_ptrc_glReadPixels = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , GLvoid *))IntGetProcAddress("glReadPixels");
	if(!_ptrc_glReadPixels) numFailed++;
	_ptrc_glGetBooleanv = (void (CODEGEN_FUNCPTR *)(GLenum , GLboolean *))IntGetProcAddress("glGetBooleanv");
	if(!_ptrc_glGetBooleanv) numFailed++;
	_ptrc_glGetDoublev = (void (CODEGEN_FUNCPTR *)(GLenum , GLdouble *))IntGetProcAddress("glGetDoublev");
	if(!_ptrc_glGetDoublev) numFailed++;
	_ptrc_glGetError = (GLenum (CODEGEN_FUNCPTR *)())IntGetProcAddress("glGetError");
	if(!_ptrc_glGetError) numFailed++;
	_ptrc_glGetFloatv = (void (CODEGEN_FUNCPTR *)(GLenum , GLfloat *))IntGetProcAddress("glGetFloatv");
	if(!_ptrc_glGetFloatv) numFailed++;
	_ptrc_glGetIntegerv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint *))IntGetProcAddress("glGetIntegerv");
	if(!_ptrc_glGetIntegerv) numFailed++;
	_ptrc_glGetString = (const GLubyte * (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glGetString");
	if(!_ptrc_glGetString) numFailed++;
	_ptrc_glGetTexImage = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLenum , GLvoid *))IntGetProcAddress("glGetTexImage");
	if(!_ptrc_glGetTexImage) numFailed++;
	_ptrc_glGetTexParameterfv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLfloat *))IntGetProcAddress("glGetTexParameterfv");
	if(!_ptrc_glGetTexParameterfv) numFailed++;
	_ptrc_glGetTexParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint *))IntGetProcAddress("glGetTexParameteriv");
	if(!_ptrc_glGetTexParameteriv) numFailed++;
	_ptrc_glGetTexLevelParameterfv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLfloat *))IntGetProcAddress("glGetTexLevelParameterfv");
	if(!_ptrc_glGetTexLevelParameterfv) numFailed++;
	_ptrc_glGetTexLevelParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLint *))IntGetProcAddress("glGetTexLevelParameteriv");
	if(!_ptrc_glGetTexLevelParameteriv) numFailed++;
	_ptrc_glIsEnabled = (GLboolean (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glIsEnabled");
	if(!_ptrc_glIsEnabled) numFailed++;
	_ptrc_glDepthRange = (void (CODEGEN_FUNCPTR *)(GLdouble , GLdouble ))IntGetProcAddress("glDepthRange");
	if(!_ptrc_glDepthRange) numFailed++;
	_ptrc_glViewport = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glViewport");
	if(!_ptrc_glViewport) numFailed++;
	_ptrc_glDrawArrays = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLsizei ))IntGetProcAddress("glDrawArrays");
	if(!_ptrc_glDrawArrays) numFailed++;
	_ptrc_glDrawElements = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLenum , const GLvoid *))IntGetProcAddress("glDrawElements");
	if(!_ptrc_glDrawElements) numFailed++;
	_ptrc_glGetPointerv = (void (CODEGEN_FUNCPTR *)(GLenum , GLvoid* *))IntGetProcAddress("glGetPointerv");
	if(!_ptrc_glGetPointerv) numFailed++;
	_ptrc_glPolygonOffset = (void (CODEGEN_FUNCPTR *)(GLfloat , GLfloat ))IntGetProcAddress("glPolygonOffset");
	if(!_ptrc_glPolygonOffset) numFailed++;
	_ptrc_glCopyTexImage1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLint ))IntGetProcAddress("glCopyTexImage1D");
	if(!_ptrc_glCopyTexImage1D) numFailed++;
	_ptrc_glCopyTexImage2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLint , GLint , GLsizei , GLsizei , GLint ))IntGetProcAddress("glCopyTexImage2D");
	if(!_ptrc_glCopyTexImage2D) numFailed++;
	_ptrc_glCopyTexSubImage1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLint , GLsizei ))IntGetProcAddress("glCopyTexSubImage1D");
	if(!_ptrc_glCopyTexSubImage1D) numFailed++;
	_ptrc_glCopyTexSubImage2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glCopyTexSubImage2D");
	if(!_ptrc_glCopyTexSubImage2D) numFailed++;
	_ptrc_glTexSubImage1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTexSubImage1D");
	if(!_ptrc_glTexSubImage1D) numFailed++;
	_ptrc_glTexSubImage2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTexSubImage2D");
	if(!_ptrc_glTexSubImage2D) numFailed++;
	_ptrc_glBindTexture = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glBindTexture");
	if(!_ptrc_glBindTexture) numFailed++;
	_ptrc_glDeleteTextures = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteTextures");
	if(!_ptrc_glDeleteTextures) numFailed++;
	_ptrc_glGenTextures = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenTextures");
	if(!_ptrc_glGenTextures) numFailed++;
	_ptrc_glIsTexture = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsTexture");
	if(!_ptrc_glIsTexture) numFailed++;
	_ptrc_glIndexub = (void (CODEGEN_FUNCPTR *)(GLubyte ))IntGetProcAddress("glIndexub");
	if(!_ptrc_glIndexub) numFailed++;
	_ptrc_glIndexubv = (void (CODEGEN_FUNCPTR *)(const GLubyte *))IntGetProcAddress("glIndexubv");
	if(!_ptrc_glIndexubv) numFailed++;
	_ptrc_glBlendColor = (void (CODEGEN_FUNCPTR *)(GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glBlendColor");
	if(!_ptrc_glBlendColor) numFailed++;
	_ptrc_glBlendEquation = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glBlendEquation");
	if(!_ptrc_glBlendEquation) numFailed++;
	_ptrc_glDrawRangeElements = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLuint , GLsizei , GLenum , const GLvoid *))IntGetProcAddress("glDrawRangeElements");
	if(!_ptrc_glDrawRangeElements) numFailed++;
	_ptrc_glTexImage3D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLsizei , GLsizei , GLsizei , GLint , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTexImage3D");
	if(!_ptrc_glTexImage3D) numFailed++;
	_ptrc_glTexSubImage3D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLenum , const GLvoid *))IntGetProcAddress("glTexSubImage3D");
	if(!_ptrc_glTexSubImage3D) numFailed++;
	_ptrc_glCopyTexSubImage3D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLint , GLint , GLint , GLsizei , GLsizei ))IntGetProcAddress("glCopyTexSubImage3D");
	if(!_ptrc_glCopyTexSubImage3D) numFailed++;
	_ptrc_glActiveTexture = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glActiveTexture");
	if(!_ptrc_glActiveTexture) numFailed++;
	_ptrc_glSampleCoverage = (void (CODEGEN_FUNCPTR *)(GLfloat , GLboolean ))IntGetProcAddress("glSampleCoverage");
	if(!_ptrc_glSampleCoverage) numFailed++;
	_ptrc_glCompressedTexImage3D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLsizei , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTexImage3D");
	if(!_ptrc_glCompressedTexImage3D) numFailed++;
	_ptrc_glCompressedTexImage2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLsizei , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTexImage2D");
	if(!_ptrc_glCompressedTexImage2D) numFailed++;
	_ptrc_glCompressedTexImage1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLenum , GLsizei , GLint , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTexImage1D");
	if(!_ptrc_glCompressedTexImage1D) numFailed++;
	_ptrc_glCompressedTexSubImage3D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLint , GLsizei , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTexSubImage3D");
	if(!_ptrc_glCompressedTexSubImage3D) numFailed++;
	_ptrc_glCompressedTexSubImage2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLint , GLsizei , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTexSubImage2D");
	if(!_ptrc_glCompressedTexSubImage2D) numFailed++;
	_ptrc_glCompressedTexSubImage1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLint , GLsizei , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glCompressedTexSubImage1D");
	if(!_ptrc_glCompressedTexSubImage1D) numFailed++;
	_ptrc_glGetCompressedTexImage = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLvoid *))IntGetProcAddress("glGetCompressedTexImage");
	if(!_ptrc_glGetCompressedTexImage) numFailed++;
	_ptrc_glBlendFuncSeparate = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLenum ))IntGetProcAddress("glBlendFuncSeparate");
	if(!_ptrc_glBlendFuncSeparate) numFailed++;
	_ptrc_glMultiDrawArrays = (void (CODEGEN_FUNCPTR *)(GLenum , const GLint *, const GLsizei *, GLsizei ))IntGetProcAddress("glMultiDrawArrays");
	if(!_ptrc_glMultiDrawArrays) numFailed++;
	_ptrc_glMultiDrawElements = (void (CODEGEN_FUNCPTR *)(GLenum , const GLsizei *, GLenum , const GLvoid* const *, GLsizei ))IntGetProcAddress("glMultiDrawElements");
	if(!_ptrc_glMultiDrawElements) numFailed++;
	_ptrc_glPointParameterf = (void (CODEGEN_FUNCPTR *)(GLenum , GLfloat ))IntGetProcAddress("glPointParameterf");
	if(!_ptrc_glPointParameterf) numFailed++;
	_ptrc_glPointParameterfv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLfloat *))IntGetProcAddress("glPointParameterfv");
	if(!_ptrc_glPointParameterfv) numFailed++;
	_ptrc_glPointParameteri = (void (CODEGEN_FUNCPTR *)(GLenum , GLint ))IntGetProcAddress("glPointParameteri");
	if(!_ptrc_glPointParameteri) numFailed++;
	_ptrc_glPointParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLint *))IntGetProcAddress("glPointParameteriv");
	if(!_ptrc_glPointParameteriv) numFailed++;
	_ptrc_glGenQueries = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenQueries");
	if(!_ptrc_glGenQueries) numFailed++;
	_ptrc_glDeleteQueries = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteQueries");
	if(!_ptrc_glDeleteQueries) numFailed++;
	_ptrc_glIsQuery = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsQuery");
	if(!_ptrc_glIsQuery) numFailed++;
	_ptrc_glBeginQuery = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glBeginQuery");
	if(!_ptrc_glBeginQuery) numFailed++;
	_ptrc_glEndQuery = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glEndQuery");
	if(!_ptrc_glEndQuery) numFailed++;
	_ptrc_glGetQueryiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint *))IntGetProcAddress("glGetQueryiv");
	if(!_ptrc_glGetQueryiv) numFailed++;
	_ptrc_glGetQueryObjectiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetQueryObjectiv");
	if(!_ptrc_glGetQueryObjectiv) numFailed++;
	_ptrc_glGetQueryObjectuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint *))IntGetProcAddress("glGetQueryObjectuiv");
	if(!_ptrc_glGetQueryObjectuiv) numFailed++;
	_ptrc_glBindBuffer = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glBindBuffer");
	if(!_ptrc_glBindBuffer) numFailed++;
	_ptrc_glDeleteBuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteBuffers");
	if(!_ptrc_glDeleteBuffers) numFailed++;
	_ptrc_glGenBuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenBuffers");
	if(!_ptrc_glGenBuffers) numFailed++;
	_ptrc_glIsBuffer = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsBuffer");
	if(!_ptrc_glIsBuffer) numFailed++;
	_ptrc_glBufferData = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizeiptr , const GLvoid *, GLenum ))IntGetProcAddress("glBufferData");
	if(!_ptrc_glBufferData) numFailed++;
	_ptrc_glBufferSubData = (void (CODEGEN_FUNCPTR *)(GLenum , GLintptr , GLsizeiptr , const GLvoid *))IntGetProcAddress("glBufferSubData");
	if(!_ptrc_glBufferSubData) numFailed++;
	_ptrc_glGetBufferSubData = (void (CODEGEN_FUNCPTR *)(GLenum , GLintptr , GLsizeiptr , GLvoid *))IntGetProcAddress("glGetBufferSubData");
	if(!_ptrc_glGetBufferSubData) numFailed++;
	_ptrc_glMapBuffer = (GLvoid* (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glMapBuffer");
	if(!_ptrc_glMapBuffer) numFailed++;
	_ptrc_glUnmapBuffer = (GLboolean (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glUnmapBuffer");
	if(!_ptrc_glUnmapBuffer) numFailed++;
	_ptrc_glGetBufferParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint *))IntGetProcAddress("glGetBufferParameteriv");
	if(!_ptrc_glGetBufferParameteriv) numFailed++;
	_ptrc_glGetBufferPointerv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLvoid* *))IntGetProcAddress("glGetBufferPointerv");
	if(!_ptrc_glGetBufferPointerv) numFailed++;
	_ptrc_glBlendEquationSeparate = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glBlendEquationSeparate");
	if(!_ptrc_glBlendEquationSeparate) numFailed++;
	_ptrc_glDrawBuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLenum *))IntGetProcAddress("glDrawBuffers");
	if(!_ptrc_glDrawBuffers) numFailed++;
	_ptrc_glStencilOpSeparate = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLenum ))IntGetProcAddress("glStencilOpSeparate");
	if(!_ptrc_glStencilOpSeparate) numFailed++;
	_ptrc_glStencilFuncSeparate = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint , GLuint ))IntGetProcAddress("glStencilFuncSeparate");
	if(!_ptrc_glStencilFuncSeparate) numFailed++;
	_ptrc_glStencilMaskSeparate = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glStencilMaskSeparate");
	if(!_ptrc_glStencilMaskSeparate) numFailed++;
	_ptrc_glAttachShader = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glAttachShader");
	if(!_ptrc_glAttachShader) numFailed++;
	_ptrc_glBindAttribLocation = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , const GLchar *))IntGetProcAddress("glBindAttribLocation");
	if(!_ptrc_glBindAttribLocation) numFailed++;
	_ptrc_glCompileShader = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glCompileShader");
	if(!_ptrc_glCompileShader) numFailed++;
	_ptrc_glCreateProgram = (GLuint (CODEGEN_FUNCPTR *)())IntGetProcAddress("glCreateProgram");
	if(!_ptrc_glCreateProgram) numFailed++;
	_ptrc_glCreateShader = (GLuint (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glCreateShader");
	if(!_ptrc_glCreateShader) numFailed++;
	_ptrc_glDeleteProgram = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glDeleteProgram");
	if(!_ptrc_glDeleteProgram) numFailed++;
	_ptrc_glDeleteShader = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glDeleteShader");
	if(!_ptrc_glDeleteShader) numFailed++;
	_ptrc_glDetachShader = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glDetachShader");
	if(!_ptrc_glDetachShader) numFailed++;
	_ptrc_glDisableVertexAttribArray = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glDisableVertexAttribArray");
	if(!_ptrc_glDisableVertexAttribArray) numFailed++;
	_ptrc_glEnableVertexAttribArray = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glEnableVertexAttribArray");
	if(!_ptrc_glEnableVertexAttribArray) numFailed++;
	_ptrc_glGetActiveAttrib = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLsizei , GLsizei *, GLint *, GLenum *, GLchar *))IntGetProcAddress("glGetActiveAttrib");
	if(!_ptrc_glGetActiveAttrib) numFailed++;
	_ptrc_glGetActiveUniform = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLsizei , GLsizei *, GLint *, GLenum *, GLchar *))IntGetProcAddress("glGetActiveUniform");
	if(!_ptrc_glGetActiveUniform) numFailed++;
	_ptrc_glGetAttachedShaders = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLsizei *, GLuint *))IntGetProcAddress("glGetAttachedShaders");
	if(!_ptrc_glGetAttachedShaders) numFailed++;
	_ptrc_glGetAttribLocation = (GLint (CODEGEN_FUNCPTR *)(GLuint , const GLchar *))IntGetProcAddress("glGetAttribLocation");
	if(!_ptrc_glGetAttribLocation) numFailed++;
	_ptrc_glGetProgramiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetProgramiv");
	if(!_ptrc_glGetProgramiv) numFailed++;
	_ptrc_glGetProgramInfoLog = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLsizei *, GLchar *))IntGetProcAddress("glGetProgramInfoLog");
	if(!_ptrc_glGetProgramInfoLog) numFailed++;
	_ptrc_glGetShaderiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetShaderiv");
	if(!_ptrc_glGetShaderiv) numFailed++;
	_ptrc_glGetShaderInfoLog = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLsizei *, GLchar *))IntGetProcAddress("glGetShaderInfoLog");
	if(!_ptrc_glGetShaderInfoLog) numFailed++;
	_ptrc_glGetShaderSource = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , GLsizei *, GLchar *))IntGetProcAddress("glGetShaderSource");
	if(!_ptrc_glGetShaderSource) numFailed++;
	_ptrc_glGetUniformLocation = (GLint (CODEGEN_FUNCPTR *)(GLuint , const GLchar *))IntGetProcAddress("glGetUniformLocation");
	if(!_ptrc_glGetUniformLocation) numFailed++;
	_ptrc_glGetUniformfv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLfloat *))IntGetProcAddress("glGetUniformfv");
	if(!_ptrc_glGetUniformfv) numFailed++;
	_ptrc_glGetUniformiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint *))IntGetProcAddress("glGetUniformiv");
	if(!_ptrc_glGetUniformiv) numFailed++;
	_ptrc_glGetVertexAttribdv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLdouble *))IntGetProcAddress("glGetVertexAttribdv");
	if(!_ptrc_glGetVertexAttribdv) numFailed++;
	_ptrc_glGetVertexAttribfv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLfloat *))IntGetProcAddress("glGetVertexAttribfv");
	if(!_ptrc_glGetVertexAttribfv) numFailed++;
	_ptrc_glGetVertexAttribiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetVertexAttribiv");
	if(!_ptrc_glGetVertexAttribiv) numFailed++;
	_ptrc_glGetVertexAttribPointerv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLvoid* *))IntGetProcAddress("glGetVertexAttribPointerv");
	if(!_ptrc_glGetVertexAttribPointerv) numFailed++;
	_ptrc_glIsProgram = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsProgram");
	if(!_ptrc_glIsProgram) numFailed++;
	_ptrc_glIsShader = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsShader");
	if(!_ptrc_glIsShader) numFailed++;
	_ptrc_glLinkProgram = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glLinkProgram");
	if(!_ptrc_glLinkProgram) numFailed++;
	_ptrc_glShaderSource = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , const GLchar* const *, const GLint *))IntGetProcAddress("glShaderSource");
	if(!_ptrc_glShaderSource) numFailed++;
	_ptrc_glUseProgram = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glUseProgram");
	if(!_ptrc_glUseProgram) numFailed++;
	_ptrc_glUniform1f = (void (CODEGEN_FUNCPTR *)(GLint , GLfloat ))IntGetProcAddress("glUniform1f");
	if(!_ptrc_glUniform1f) numFailed++;
	_ptrc_glUniform2f = (void (CODEGEN_FUNCPTR *)(GLint , GLfloat , GLfloat ))IntGetProcAddress("glUniform2f");
	if(!_ptrc_glUniform2f) numFailed++;
	_ptrc_glUniform3f = (void (CODEGEN_FUNCPTR *)(GLint , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glUniform3f");
	if(!_ptrc_glUniform3f) numFailed++;
	_ptrc_glUniform4f = (void (CODEGEN_FUNCPTR *)(GLint , GLfloat , GLfloat , GLfloat , GLfloat ))IntGetProcAddress("glUniform4f");
	if(!_ptrc_glUniform4f) numFailed++;
	_ptrc_glUniform1i = (void (CODEGEN_FUNCPTR *)(GLint , GLint ))IntGetProcAddress("glUniform1i");
	if(!_ptrc_glUniform1i) numFailed++;
	_ptrc_glUniform2i = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLint ))IntGetProcAddress("glUniform2i");
	if(!_ptrc_glUniform2i) numFailed++;
	_ptrc_glUniform3i = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLint , GLint ))IntGetProcAddress("glUniform3i");
	if(!_ptrc_glUniform3i) numFailed++;
	_ptrc_glUniform4i = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glUniform4i");
	if(!_ptrc_glUniform4i) numFailed++;
	_ptrc_glUniform1fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLfloat *))IntGetProcAddress("glUniform1fv");
	if(!_ptrc_glUniform1fv) numFailed++;
	_ptrc_glUniform2fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLfloat *))IntGetProcAddress("glUniform2fv");
	if(!_ptrc_glUniform2fv) numFailed++;
	_ptrc_glUniform3fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLfloat *))IntGetProcAddress("glUniform3fv");
	if(!_ptrc_glUniform3fv) numFailed++;
	_ptrc_glUniform4fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLfloat *))IntGetProcAddress("glUniform4fv");
	if(!_ptrc_glUniform4fv) numFailed++;
	_ptrc_glUniform1iv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLint *))IntGetProcAddress("glUniform1iv");
	if(!_ptrc_glUniform1iv) numFailed++;
	_ptrc_glUniform2iv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLint *))IntGetProcAddress("glUniform2iv");
	if(!_ptrc_glUniform2iv) numFailed++;
	_ptrc_glUniform3iv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLint *))IntGetProcAddress("glUniform3iv");
	if(!_ptrc_glUniform3iv) numFailed++;
	_ptrc_glUniform4iv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLint *))IntGetProcAddress("glUniform4iv");
	if(!_ptrc_glUniform4iv) numFailed++;
	_ptrc_glUniformMatrix2fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix2fv");
	if(!_ptrc_glUniformMatrix2fv) numFailed++;
	_ptrc_glUniformMatrix3fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix3fv");
	if(!_ptrc_glUniformMatrix3fv) numFailed++;
	_ptrc_glUniformMatrix4fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix4fv");
	if(!_ptrc_glUniformMatrix4fv) numFailed++;
	_ptrc_glValidateProgram = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glValidateProgram");
	if(!_ptrc_glValidateProgram) numFailed++;
	_ptrc_glVertexAttribPointer = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLenum , GLboolean , GLsizei , const GLvoid *))IntGetProcAddress("glVertexAttribPointer");
	if(!_ptrc_glVertexAttribPointer) numFailed++;
	_ptrc_glUniformMatrix2x3fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix2x3fv");
	if(!_ptrc_glUniformMatrix2x3fv) numFailed++;
	_ptrc_glUniformMatrix3x2fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix3x2fv");
	if(!_ptrc_glUniformMatrix3x2fv) numFailed++;
	_ptrc_glUniformMatrix2x4fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix2x4fv");
	if(!_ptrc_glUniformMatrix2x4fv) numFailed++;
	_ptrc_glUniformMatrix4x2fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix4x2fv");
	if(!_ptrc_glUniformMatrix4x2fv) numFailed++;
	_ptrc_glUniformMatrix3x4fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix3x4fv");
	if(!_ptrc_glUniformMatrix3x4fv) numFailed++;
	_ptrc_glUniformMatrix4x3fv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , GLboolean , const GLfloat *))IntGetProcAddress("glUniformMatrix4x3fv");
	if(!_ptrc_glUniformMatrix4x3fv) numFailed++;
	_ptrc_glBindVertexArray = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glBindVertexArray");
	if(!_ptrc_glBindVertexArray) numFailed++;
	_ptrc_glDeleteVertexArrays = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteVertexArrays");
	if(!_ptrc_glDeleteVertexArrays) numFailed++;
	_ptrc_glGenVertexArrays = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenVertexArrays");
	if(!_ptrc_glGenVertexArrays) numFailed++;
	_ptrc_glIsVertexArray = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsVertexArray");
	if(!_ptrc_glIsVertexArray) numFailed++;
	_ptrc_glMapBufferRange = (GLvoid* (CODEGEN_FUNCPTR *)(GLenum , GLintptr , GLsizeiptr , GLbitfield ))IntGetProcAddress("glMapBufferRange");
	if(!_ptrc_glMapBufferRange) numFailed++;
	_ptrc_glFlushMappedBufferRange = (void (CODEGEN_FUNCPTR *)(GLenum , GLintptr , GLsizeiptr ))IntGetProcAddress("glFlushMappedBufferRange");
	if(!_ptrc_glFlushMappedBufferRange) numFailed++;
	_ptrc_glIsRenderbuffer = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsRenderbuffer");
	if(!_ptrc_glIsRenderbuffer) numFailed++;
	_ptrc_glBindRenderbuffer = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glBindRenderbuffer");
	if(!_ptrc_glBindRenderbuffer) numFailed++;
	_ptrc_glDeleteRenderbuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteRenderbuffers");
	if(!_ptrc_glDeleteRenderbuffers) numFailed++;
	_ptrc_glGenRenderbuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenRenderbuffers");
	if(!_ptrc_glGenRenderbuffers) numFailed++;
	_ptrc_glRenderbufferStorage = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLsizei , GLsizei ))IntGetProcAddress("glRenderbufferStorage");
	if(!_ptrc_glRenderbufferStorage) numFailed++;
	_ptrc_glGetRenderbufferParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint *))IntGetProcAddress("glGetRenderbufferParameteriv");
	if(!_ptrc_glGetRenderbufferParameteriv) numFailed++;
	_ptrc_glIsFramebuffer = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsFramebuffer");
	if(!_ptrc_glIsFramebuffer) numFailed++;
	_ptrc_glBindFramebuffer = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glBindFramebuffer");
	if(!_ptrc_glBindFramebuffer) numFailed++;
	_ptrc_glDeleteFramebuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteFramebuffers");
	if(!_ptrc_glDeleteFramebuffers) numFailed++;
	_ptrc_glGenFramebuffers = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenFramebuffers");
	if(!_ptrc_glGenFramebuffers) numFailed++;
	_ptrc_glCheckFramebufferStatus = (GLenum (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glCheckFramebufferStatus");
	if(!_ptrc_glCheckFramebufferStatus) numFailed++;
	_ptrc_glFramebufferTexture1D = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLuint , GLint ))IntGetProcAddress("glFramebufferTexture1D");
	if(!_ptrc_glFramebufferTexture1D) numFailed++;
	_ptrc_glFramebufferTexture2D = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLuint , GLint ))IntGetProcAddress("glFramebufferTexture2D");
	if(!_ptrc_glFramebufferTexture2D) numFailed++;
	_ptrc_glFramebufferTexture3D = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLuint , GLint , GLint ))IntGetProcAddress("glFramebufferTexture3D");
	if(!_ptrc_glFramebufferTexture3D) numFailed++;
	_ptrc_glFramebufferRenderbuffer = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLuint ))IntGetProcAddress("glFramebufferRenderbuffer");
	if(!_ptrc_glFramebufferRenderbuffer) numFailed++;
	_ptrc_glGetFramebufferAttachmentParameteriv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLenum , GLint *))IntGetProcAddress("glGetFramebufferAttachmentParameteriv");
	if(!_ptrc_glGetFramebufferAttachmentParameteriv) numFailed++;
	_ptrc_glGenerateMipmap = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glGenerateMipmap");
	if(!_ptrc_glGenerateMipmap) numFailed++;
	_ptrc_glBlitFramebuffer = (void (CODEGEN_FUNCPTR *)(GLint , GLint , GLint , GLint , GLint , GLint , GLint , GLint , GLbitfield , GLenum ))IntGetProcAddress("glBlitFramebuffer");
	if(!_ptrc_glBlitFramebuffer) numFailed++;
	_ptrc_glRenderbufferStorageMultisample = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLenum , GLsizei , GLsizei ))IntGetProcAddress("glRenderbufferStorageMultisample");
	if(!_ptrc_glRenderbufferStorageMultisample) numFailed++;
	_ptrc_glFramebufferTextureLayer = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint , GLint , GLint ))IntGetProcAddress("glFramebufferTextureLayer");
	if(!_ptrc_glFramebufferTextureLayer) numFailed++;
	_ptrc_glColorMaski = (void (CODEGEN_FUNCPTR *)(GLuint , GLboolean , GLboolean , GLboolean , GLboolean ))IntGetProcAddress("glColorMaski");
	if(!_ptrc_glColorMaski) numFailed++;
	_ptrc_glGetBooleani_v = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLboolean *))IntGetProcAddress("glGetBooleani_v");
	if(!_ptrc_glGetBooleani_v) numFailed++;
	_ptrc_glGetIntegeri_v = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLint *))IntGetProcAddress("glGetIntegeri_v");
	if(!_ptrc_glGetIntegeri_v) numFailed++;
	_ptrc_glEnablei = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glEnablei");
	if(!_ptrc_glEnablei) numFailed++;
	_ptrc_glDisablei = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glDisablei");
	if(!_ptrc_glDisablei) numFailed++;
	_ptrc_glIsEnabledi = (GLboolean (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glIsEnabledi");
	if(!_ptrc_glIsEnabledi) numFailed++;
	_ptrc_glBeginTransformFeedback = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glBeginTransformFeedback");
	if(!_ptrc_glBeginTransformFeedback) numFailed++;
	_ptrc_glEndTransformFeedback = (void (CODEGEN_FUNCPTR *)())IntGetProcAddress("glEndTransformFeedback");
	if(!_ptrc_glEndTransformFeedback) numFailed++;
	_ptrc_glBindBufferRange = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLuint , GLintptr , GLsizeiptr ))IntGetProcAddress("glBindBufferRange");
	if(!_ptrc_glBindBufferRange) numFailed++;
	_ptrc_glBindBufferBase = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLuint ))IntGetProcAddress("glBindBufferBase");
	if(!_ptrc_glBindBufferBase) numFailed++;
	_ptrc_glTransformFeedbackVaryings = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , const GLchar* const *, GLenum ))IntGetProcAddress("glTransformFeedbackVaryings");
	if(!_ptrc_glTransformFeedbackVaryings) numFailed++;
	_ptrc_glGetTransformFeedbackVarying = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLsizei , GLsizei *, GLsizei *, GLenum *, GLchar *))IntGetProcAddress("glGetTransformFeedbackVarying");
	if(!_ptrc_glGetTransformFeedbackVarying) numFailed++;
	_ptrc_glClampColor = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum ))IntGetProcAddress("glClampColor");
	if(!_ptrc_glClampColor) numFailed++;
	_ptrc_glBeginConditionalRender = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glBeginConditionalRender");
	if(!_ptrc_glBeginConditionalRender) numFailed++;
	_ptrc_glEndConditionalRender = (void (CODEGEN_FUNCPTR *)())IntGetProcAddress("glEndConditionalRender");
	if(!_ptrc_glEndConditionalRender) numFailed++;
	_ptrc_glVertexAttribIPointer = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLenum , GLsizei , const GLvoid *))IntGetProcAddress("glVertexAttribIPointer");
	if(!_ptrc_glVertexAttribIPointer) numFailed++;
	_ptrc_glGetVertexAttribIiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetVertexAttribIiv");
	if(!_ptrc_glGetVertexAttribIiv) numFailed++;
	_ptrc_glGetVertexAttribIuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint *))IntGetProcAddress("glGetVertexAttribIuiv");
	if(!_ptrc_glGetVertexAttribIuiv) numFailed++;
	_ptrc_glVertexAttribI1i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint ))IntGetProcAddress("glVertexAttribI1i");
	if(!_ptrc_glVertexAttribI1i) numFailed++;
	_ptrc_glVertexAttribI2i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint ))IntGetProcAddress("glVertexAttribI2i");
	if(!_ptrc_glVertexAttribI2i) numFailed++;
	_ptrc_glVertexAttribI3i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint ))IntGetProcAddress("glVertexAttribI3i");
	if(!_ptrc_glVertexAttribI3i) numFailed++;
	_ptrc_glVertexAttribI4i = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLint , GLint , GLint ))IntGetProcAddress("glVertexAttribI4i");
	if(!_ptrc_glVertexAttribI4i) numFailed++;
	_ptrc_glVertexAttribI1ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glVertexAttribI1ui");
	if(!_ptrc_glVertexAttribI1ui) numFailed++;
	_ptrc_glVertexAttribI2ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint ))IntGetProcAddress("glVertexAttribI2ui");
	if(!_ptrc_glVertexAttribI2ui) numFailed++;
	_ptrc_glVertexAttribI3ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint , GLuint ))IntGetProcAddress("glVertexAttribI3ui");
	if(!_ptrc_glVertexAttribI3ui) numFailed++;
	_ptrc_glVertexAttribI4ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint , GLuint , GLuint ))IntGetProcAddress("glVertexAttribI4ui");
	if(!_ptrc_glVertexAttribI4ui) numFailed++;
	_ptrc_glVertexAttribI1iv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLint *))IntGetProcAddress("glVertexAttribI1iv");
	if(!_ptrc_glVertexAttribI1iv) numFailed++;
	_ptrc_glVertexAttribI2iv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLint *))IntGetProcAddress("glVertexAttribI2iv");
	if(!_ptrc_glVertexAttribI2iv) numFailed++;
	_ptrc_glVertexAttribI3iv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLint *))IntGetProcAddress("glVertexAttribI3iv");
	if(!_ptrc_glVertexAttribI3iv) numFailed++;
	_ptrc_glVertexAttribI4iv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLint *))IntGetProcAddress("glVertexAttribI4iv");
	if(!_ptrc_glVertexAttribI4iv) numFailed++;
	_ptrc_glVertexAttribI1uiv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLuint *))IntGetProcAddress("glVertexAttribI1uiv");
	if(!_ptrc_glVertexAttribI1uiv) numFailed++;
	_ptrc_glVertexAttribI2uiv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLuint *))IntGetProcAddress("glVertexAttribI2uiv");
	if(!_ptrc_glVertexAttribI2uiv) numFailed++;
	_ptrc_glVertexAttribI3uiv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLuint *))IntGetProcAddress("glVertexAttribI3uiv");
	if(!_ptrc_glVertexAttribI3uiv) numFailed++;
	_ptrc_glVertexAttribI4uiv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLuint *))IntGetProcAddress("glVertexAttribI4uiv");
	if(!_ptrc_glVertexAttribI4uiv) numFailed++;
	_ptrc_glVertexAttribI4bv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLbyte *))IntGetProcAddress("glVertexAttribI4bv");
	if(!_ptrc_glVertexAttribI4bv) numFailed++;
	_ptrc_glVertexAttribI4sv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLshort *))IntGetProcAddress("glVertexAttribI4sv");
	if(!_ptrc_glVertexAttribI4sv) numFailed++;
	_ptrc_glVertexAttribI4ubv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLubyte *))IntGetProcAddress("glVertexAttribI4ubv");
	if(!_ptrc_glVertexAttribI4ubv) numFailed++;
	_ptrc_glVertexAttribI4usv = (void (CODEGEN_FUNCPTR *)(GLuint , const GLushort *))IntGetProcAddress("glVertexAttribI4usv");
	if(!_ptrc_glVertexAttribI4usv) numFailed++;
	_ptrc_glGetUniformuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLint , GLuint *))IntGetProcAddress("glGetUniformuiv");
	if(!_ptrc_glGetUniformuiv) numFailed++;
	_ptrc_glBindFragDataLocation = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , const GLchar *))IntGetProcAddress("glBindFragDataLocation");
	if(!_ptrc_glBindFragDataLocation) numFailed++;
	_ptrc_glGetFragDataLocation = (GLint (CODEGEN_FUNCPTR *)(GLuint , const GLchar *))IntGetProcAddress("glGetFragDataLocation");
	if(!_ptrc_glGetFragDataLocation) numFailed++;
	_ptrc_glUniform1ui = (void (CODEGEN_FUNCPTR *)(GLint , GLuint ))IntGetProcAddress("glUniform1ui");
	if(!_ptrc_glUniform1ui) numFailed++;
	_ptrc_glUniform2ui = (void (CODEGEN_FUNCPTR *)(GLint , GLuint , GLuint ))IntGetProcAddress("glUniform2ui");
	if(!_ptrc_glUniform2ui) numFailed++;
	_ptrc_glUniform3ui = (void (CODEGEN_FUNCPTR *)(GLint , GLuint , GLuint , GLuint ))IntGetProcAddress("glUniform3ui");
	if(!_ptrc_glUniform3ui) numFailed++;
	_ptrc_glUniform4ui = (void (CODEGEN_FUNCPTR *)(GLint , GLuint , GLuint , GLuint , GLuint ))IntGetProcAddress("glUniform4ui");
	if(!_ptrc_glUniform4ui) numFailed++;
	_ptrc_glUniform1uiv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLuint *))IntGetProcAddress("glUniform1uiv");
	if(!_ptrc_glUniform1uiv) numFailed++;
	_ptrc_glUniform2uiv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLuint *))IntGetProcAddress("glUniform2uiv");
	if(!_ptrc_glUniform2uiv) numFailed++;
	_ptrc_glUniform3uiv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLuint *))IntGetProcAddress("glUniform3uiv");
	if(!_ptrc_glUniform3uiv) numFailed++;
	_ptrc_glUniform4uiv = (void (CODEGEN_FUNCPTR *)(GLint , GLsizei , const GLuint *))IntGetProcAddress("glUniform4uiv");
	if(!_ptrc_glUniform4uiv) numFailed++;
	_ptrc_glTexParameterIiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLint *))IntGetProcAddress("glTexParameterIiv");
	if(!_ptrc_glTexParameterIiv) numFailed++;
	_ptrc_glTexParameterIuiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLuint *))IntGetProcAddress("glTexParameterIuiv");
	if(!_ptrc_glTexParameterIuiv) numFailed++;
	_ptrc_glGetTexParameterIiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint *))IntGetProcAddress("glGetTexParameterIiv");
	if(!_ptrc_glGetTexParameterIiv) numFailed++;
	_ptrc_glGetTexParameterIuiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint *))IntGetProcAddress("glGetTexParameterIuiv");
	if(!_ptrc_glGetTexParameterIuiv) numFailed++;
	_ptrc_glClearBufferiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , const GLint *))IntGetProcAddress("glClearBufferiv");
	if(!_ptrc_glClearBufferiv) numFailed++;
	_ptrc_glClearBufferuiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , const GLuint *))IntGetProcAddress("glClearBufferuiv");
	if(!_ptrc_glClearBufferuiv) numFailed++;
	_ptrc_glClearBufferfv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , const GLfloat *))IntGetProcAddress("glClearBufferfv");
	if(!_ptrc_glClearBufferfv) numFailed++;
	_ptrc_glClearBufferfi = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLfloat , GLint ))IntGetProcAddress("glClearBufferfi");
	if(!_ptrc_glClearBufferfi) numFailed++;
	_ptrc_glGetStringi = (const GLubyte * (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glGetStringi");
	if(!_ptrc_glGetStringi) numFailed++;
	_ptrc_glGetUniformIndices = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , const GLchar* const *, GLuint *))IntGetProcAddress("glGetUniformIndices");
	if(!_ptrc_glGetUniformIndices) numFailed++;
	_ptrc_glGetActiveUniformsiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLsizei , const GLuint *, GLenum , GLint *))IntGetProcAddress("glGetActiveUniformsiv");
	if(!_ptrc_glGetActiveUniformsiv) numFailed++;
	_ptrc_glGetActiveUniformName = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLsizei , GLsizei *, GLchar *))IntGetProcAddress("glGetActiveUniformName");
	if(!_ptrc_glGetActiveUniformName) numFailed++;
	_ptrc_glGetUniformBlockIndex = (GLuint (CODEGEN_FUNCPTR *)(GLuint , const GLchar *))IntGetProcAddress("glGetUniformBlockIndex");
	if(!_ptrc_glGetUniformBlockIndex) numFailed++;
	_ptrc_glGetActiveUniformBlockiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLenum , GLint *))IntGetProcAddress("glGetActiveUniformBlockiv");
	if(!_ptrc_glGetActiveUniformBlockiv) numFailed++;
	_ptrc_glGetActiveUniformBlockName = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLsizei , GLsizei *, GLchar *))IntGetProcAddress("glGetActiveUniformBlockName");
	if(!_ptrc_glGetActiveUniformBlockName) numFailed++;
	_ptrc_glUniformBlockBinding = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint ))IntGetProcAddress("glUniformBlockBinding");
	if(!_ptrc_glUniformBlockBinding) numFailed++;
	_ptrc_glCopyBufferSubData = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLintptr , GLintptr , GLsizeiptr ))IntGetProcAddress("glCopyBufferSubData");
	if(!_ptrc_glCopyBufferSubData) numFailed++;
	_ptrc_glDrawArraysInstanced = (void (CODEGEN_FUNCPTR *)(GLenum , GLint , GLsizei , GLsizei ))IntGetProcAddress("glDrawArraysInstanced");
	if(!_ptrc_glDrawArraysInstanced) numFailed++;
	_ptrc_glDrawElementsInstanced = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLenum , const GLvoid *, GLsizei ))IntGetProcAddress("glDrawElementsInstanced");
	if(!_ptrc_glDrawElementsInstanced) numFailed++;
	_ptrc_glTexBuffer = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glTexBuffer");
	if(!_ptrc_glTexBuffer) numFailed++;
	_ptrc_glPrimitiveRestartIndex = (void (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glPrimitiveRestartIndex");
	if(!_ptrc_glPrimitiveRestartIndex) numFailed++;
	_ptrc_glDrawElementsBaseVertex = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLenum , const GLvoid *, GLint ))IntGetProcAddress("glDrawElementsBaseVertex");
	if(!_ptrc_glDrawElementsBaseVertex) numFailed++;
	_ptrc_glDrawRangeElementsBaseVertex = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLuint , GLsizei , GLenum , const GLvoid *, GLint ))IntGetProcAddress("glDrawRangeElementsBaseVertex");
	if(!_ptrc_glDrawRangeElementsBaseVertex) numFailed++;
	_ptrc_glDrawElementsInstancedBaseVertex = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLenum , const GLvoid *, GLsizei , GLint ))IntGetProcAddress("glDrawElementsInstancedBaseVertex");
	if(!_ptrc_glDrawElementsInstancedBaseVertex) numFailed++;
	_ptrc_glMultiDrawElementsBaseVertex = (void (CODEGEN_FUNCPTR *)(GLenum , const GLsizei *, GLenum , const GLvoid* const *, GLsizei , const GLint *))IntGetProcAddress("glMultiDrawElementsBaseVertex");
	if(!_ptrc_glMultiDrawElementsBaseVertex) numFailed++;
	_ptrc_glProvokingVertex = (void (CODEGEN_FUNCPTR *)(GLenum ))IntGetProcAddress("glProvokingVertex");
	if(!_ptrc_glProvokingVertex) numFailed++;
	_ptrc_glFenceSync = (GLsync (CODEGEN_FUNCPTR *)(GLenum , GLbitfield ))IntGetProcAddress("glFenceSync");
	if(!_ptrc_glFenceSync) numFailed++;
	_ptrc_glIsSync = (GLboolean (CODEGEN_FUNCPTR *)(GLsync ))IntGetProcAddress("glIsSync");
	if(!_ptrc_glIsSync) numFailed++;
	_ptrc_glDeleteSync = (void (CODEGEN_FUNCPTR *)(GLsync ))IntGetProcAddress("glDeleteSync");
	if(!_ptrc_glDeleteSync) numFailed++;
	_ptrc_glClientWaitSync = (GLenum (CODEGEN_FUNCPTR *)(GLsync , GLbitfield , GLuint64 ))IntGetProcAddress("glClientWaitSync");
	if(!_ptrc_glClientWaitSync) numFailed++;
	_ptrc_glWaitSync = (void (CODEGEN_FUNCPTR *)(GLsync , GLbitfield , GLuint64 ))IntGetProcAddress("glWaitSync");
	if(!_ptrc_glWaitSync) numFailed++;
	_ptrc_glGetInteger64v = (void (CODEGEN_FUNCPTR *)(GLenum , GLint64 *))IntGetProcAddress("glGetInteger64v");
	if(!_ptrc_glGetInteger64v) numFailed++;
	_ptrc_glGetSynciv = (void (CODEGEN_FUNCPTR *)(GLsync , GLenum , GLsizei , GLsizei *, GLint *))IntGetProcAddress("glGetSynciv");
	if(!_ptrc_glGetSynciv) numFailed++;
	_ptrc_glTexImage2DMultisample = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLint , GLsizei , GLsizei , GLboolean ))IntGetProcAddress("glTexImage2DMultisample");
	if(!_ptrc_glTexImage2DMultisample) numFailed++;
	_ptrc_glTexImage3DMultisample = (void (CODEGEN_FUNCPTR *)(GLenum , GLsizei , GLint , GLsizei , GLsizei , GLsizei , GLboolean ))IntGetProcAddress("glTexImage3DMultisample");
	if(!_ptrc_glTexImage3DMultisample) numFailed++;
	_ptrc_glGetMultisamplefv = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLfloat *))IntGetProcAddress("glGetMultisamplefv");
	if(!_ptrc_glGetMultisamplefv) numFailed++;
	_ptrc_glSampleMaski = (void (CODEGEN_FUNCPTR *)(GLuint , GLbitfield ))IntGetProcAddress("glSampleMaski");
	if(!_ptrc_glSampleMaski) numFailed++;
	_ptrc_glGetInteger64i_v = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint , GLint64 *))IntGetProcAddress("glGetInteger64i_v");
	if(!_ptrc_glGetInteger64i_v) numFailed++;
	_ptrc_glGetBufferParameteri64v = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLint64 *))IntGetProcAddress("glGetBufferParameteri64v");
	if(!_ptrc_glGetBufferParameteri64v) numFailed++;
	_ptrc_glFramebufferTexture = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint , GLint ))IntGetProcAddress("glFramebufferTexture");
	if(!_ptrc_glFramebufferTexture) numFailed++;
	_ptrc_glQueryCounter = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum ))IntGetProcAddress("glQueryCounter");
	if(!_ptrc_glQueryCounter) numFailed++;
	_ptrc_glGetQueryObjecti64v = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint64 *))IntGetProcAddress("glGetQueryObjecti64v");
	if(!_ptrc_glGetQueryObjecti64v) numFailed++;
	_ptrc_glGetQueryObjectui64v = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint64 *))IntGetProcAddress("glGetQueryObjectui64v");
	if(!_ptrc_glGetQueryObjectui64v) numFailed++;
	_ptrc_glVertexP2ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glVertexP2ui");
	if(!_ptrc_glVertexP2ui) numFailed++;
	_ptrc_glVertexP2uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glVertexP2uiv");
	if(!_ptrc_glVertexP2uiv) numFailed++;
	_ptrc_glVertexP3ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glVertexP3ui");
	if(!_ptrc_glVertexP3ui) numFailed++;
	_ptrc_glVertexP3uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glVertexP3uiv");
	if(!_ptrc_glVertexP3uiv) numFailed++;
	_ptrc_glVertexP4ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glVertexP4ui");
	if(!_ptrc_glVertexP4ui) numFailed++;
	_ptrc_glVertexP4uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glVertexP4uiv");
	if(!_ptrc_glVertexP4uiv) numFailed++;
	_ptrc_glTexCoordP1ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glTexCoordP1ui");
	if(!_ptrc_glTexCoordP1ui) numFailed++;
	_ptrc_glTexCoordP1uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glTexCoordP1uiv");
	if(!_ptrc_glTexCoordP1uiv) numFailed++;
	_ptrc_glTexCoordP2ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glTexCoordP2ui");
	if(!_ptrc_glTexCoordP2ui) numFailed++;
	_ptrc_glTexCoordP2uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glTexCoordP2uiv");
	if(!_ptrc_glTexCoordP2uiv) numFailed++;
	_ptrc_glTexCoordP3ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glTexCoordP3ui");
	if(!_ptrc_glTexCoordP3ui) numFailed++;
	_ptrc_glTexCoordP3uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glTexCoordP3uiv");
	if(!_ptrc_glTexCoordP3uiv) numFailed++;
	_ptrc_glTexCoordP4ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glTexCoordP4ui");
	if(!_ptrc_glTexCoordP4ui) numFailed++;
	_ptrc_glTexCoordP4uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glTexCoordP4uiv");
	if(!_ptrc_glTexCoordP4uiv) numFailed++;
	_ptrc_glMultiTexCoordP1ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glMultiTexCoordP1ui");
	if(!_ptrc_glMultiTexCoordP1ui) numFailed++;
	_ptrc_glMultiTexCoordP1uiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLuint *))IntGetProcAddress("glMultiTexCoordP1uiv");
	if(!_ptrc_glMultiTexCoordP1uiv) numFailed++;
	_ptrc_glMultiTexCoordP2ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glMultiTexCoordP2ui");
	if(!_ptrc_glMultiTexCoordP2ui) numFailed++;
	_ptrc_glMultiTexCoordP2uiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLuint *))IntGetProcAddress("glMultiTexCoordP2uiv");
	if(!_ptrc_glMultiTexCoordP2uiv) numFailed++;
	_ptrc_glMultiTexCoordP3ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glMultiTexCoordP3ui");
	if(!_ptrc_glMultiTexCoordP3ui) numFailed++;
	_ptrc_glMultiTexCoordP3uiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLuint *))IntGetProcAddress("glMultiTexCoordP3uiv");
	if(!_ptrc_glMultiTexCoordP3uiv) numFailed++;
	_ptrc_glMultiTexCoordP4ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , GLuint ))IntGetProcAddress("glMultiTexCoordP4ui");
	if(!_ptrc_glMultiTexCoordP4ui) numFailed++;
	_ptrc_glMultiTexCoordP4uiv = (void (CODEGEN_FUNCPTR *)(GLenum , GLenum , const GLuint *))IntGetProcAddress("glMultiTexCoordP4uiv");
	if(!_ptrc_glMultiTexCoordP4uiv) numFailed++;
	_ptrc_glNormalP3ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glNormalP3ui");
	if(!_ptrc_glNormalP3ui) numFailed++;
	_ptrc_glNormalP3uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glNormalP3uiv");
	if(!_ptrc_glNormalP3uiv) numFailed++;
	_ptrc_glColorP3ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glColorP3ui");
	if(!_ptrc_glColorP3ui) numFailed++;
	_ptrc_glColorP3uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glColorP3uiv");
	if(!_ptrc_glColorP3uiv) numFailed++;
	_ptrc_glColorP4ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glColorP4ui");
	if(!_ptrc_glColorP4ui) numFailed++;
	_ptrc_glColorP4uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glColorP4uiv");
	if(!_ptrc_glColorP4uiv) numFailed++;
	_ptrc_glSecondaryColorP3ui = (void (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glSecondaryColorP3ui");
	if(!_ptrc_glSecondaryColorP3ui) numFailed++;
	_ptrc_glSecondaryColorP3uiv = (void (CODEGEN_FUNCPTR *)(GLenum , const GLuint *))IntGetProcAddress("glSecondaryColorP3uiv");
	if(!_ptrc_glSecondaryColorP3uiv) numFailed++;
	_ptrc_glVertexAttribP1ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , GLuint ))IntGetProcAddress("glVertexAttribP1ui");
	if(!_ptrc_glVertexAttribP1ui) numFailed++;
	_ptrc_glVertexAttribP1uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , const GLuint *))IntGetProcAddress("glVertexAttribP1uiv");
	if(!_ptrc_glVertexAttribP1uiv) numFailed++;
	_ptrc_glVertexAttribP2ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , GLuint ))IntGetProcAddress("glVertexAttribP2ui");
	if(!_ptrc_glVertexAttribP2ui) numFailed++;
	_ptrc_glVertexAttribP2uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , const GLuint *))IntGetProcAddress("glVertexAttribP2uiv");
	if(!_ptrc_glVertexAttribP2uiv) numFailed++;
	_ptrc_glVertexAttribP3ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , GLuint ))IntGetProcAddress("glVertexAttribP3ui");
	if(!_ptrc_glVertexAttribP3ui) numFailed++;
	_ptrc_glVertexAttribP3uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , const GLuint *))IntGetProcAddress("glVertexAttribP3uiv");
	if(!_ptrc_glVertexAttribP3uiv) numFailed++;
	_ptrc_glVertexAttribP4ui = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , GLuint ))IntGetProcAddress("glVertexAttribP4ui");
	if(!_ptrc_glVertexAttribP4ui) numFailed++;
	_ptrc_glVertexAttribP4uiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLboolean , const GLuint *))IntGetProcAddress("glVertexAttribP4uiv");
	if(!_ptrc_glVertexAttribP4uiv) numFailed++;
	_ptrc_glBindFragDataLocationIndexed = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint , GLuint , const GLchar *))IntGetProcAddress("glBindFragDataLocationIndexed");
	if(!_ptrc_glBindFragDataLocationIndexed) numFailed++;
	_ptrc_glGetFragDataIndex = (GLint (CODEGEN_FUNCPTR *)(GLuint , const GLchar *))IntGetProcAddress("glGetFragDataIndex");
	if(!_ptrc_glGetFragDataIndex) numFailed++;
	_ptrc_glGenSamplers = (void (CODEGEN_FUNCPTR *)(GLsizei , GLuint *))IntGetProcAddress("glGenSamplers");
	if(!_ptrc_glGenSamplers) numFailed++;
	_ptrc_glDeleteSamplers = (void (CODEGEN_FUNCPTR *)(GLsizei , const GLuint *))IntGetProcAddress("glDeleteSamplers");
	if(!_ptrc_glDeleteSamplers) numFailed++;
	_ptrc_glIsSampler = (GLboolean (CODEGEN_FUNCPTR *)(GLuint ))IntGetProcAddress("glIsSampler");
	if(!_ptrc_glIsSampler) numFailed++;
	_ptrc_glBindSampler = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glBindSampler");
	if(!_ptrc_glBindSampler) numFailed++;
	_ptrc_glSamplerParameteri = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint ))IntGetProcAddress("glSamplerParameteri");
	if(!_ptrc_glSamplerParameteri) numFailed++;
	_ptrc_glSamplerParameteriv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLint *))IntGetProcAddress("glSamplerParameteriv");
	if(!_ptrc_glSamplerParameteriv) numFailed++;
	_ptrc_glSamplerParameterf = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLfloat ))IntGetProcAddress("glSamplerParameterf");
	if(!_ptrc_glSamplerParameterf) numFailed++;
	_ptrc_glSamplerParameterfv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLfloat *))IntGetProcAddress("glSamplerParameterfv");
	if(!_ptrc_glSamplerParameterfv) numFailed++;
	_ptrc_glSamplerParameterIiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLint *))IntGetProcAddress("glSamplerParameterIiv");
	if(!_ptrc_glSamplerParameterIiv) numFailed++;
	_ptrc_glSamplerParameterIuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , const GLuint *))IntGetProcAddress("glSamplerParameterIuiv");
	if(!_ptrc_glSamplerParameterIuiv) numFailed++;
	_ptrc_glGetSamplerParameteriv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetSamplerParameteriv");
	if(!_ptrc_glGetSamplerParameteriv) numFailed++;
	_ptrc_glGetSamplerParameterIiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLint *))IntGetProcAddress("glGetSamplerParameterIiv");
	if(!_ptrc_glGetSamplerParameterIiv) numFailed++;
	_ptrc_glGetSamplerParameterfv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLfloat *))IntGetProcAddress("glGetSamplerParameterfv");
	if(!_ptrc_glGetSamplerParameterfv) numFailed++;
	_ptrc_glGetSamplerParameterIuiv = (void (CODEGEN_FUNCPTR *)(GLuint , GLenum , GLuint *))IntGetProcAddress("glGetSamplerParameterIuiv");
	if(!_ptrc_glGetSamplerParameterIuiv) numFailed++;
	_ptrc_glVertexAttribDivisor = (void (CODEGEN_FUNCPTR *)(GLuint , GLuint ))IntGetProcAddress("glVertexAttribDivisor");
	if(!_ptrc_glVertexAttribDivisor) numFailed++;
	return numFailed;
}

typedef int (*PFN_LOADFUNCPOINTERS)();
typedef struct ogl_StrToExtMap_s
{
	char *extensionName;
	int *extensionVariable;
	PFN_LOADFUNCPOINTERS LoadExtension;
} ogl_StrToExtMap;

static ogl_StrToExtMap ExtensionMap[7] = {
	{"GL_ARB_sampler_objects", &ogl_ext_ARB_sampler_objects, Load_ARB_sampler_objects},
	{"GL_ARB_separate_shader_objects", &ogl_ext_ARB_separate_shader_objects, Load_ARB_separate_shader_objects},
	{"GL_ARB_shading_language_420pack", &ogl_ext_ARB_shading_language_420pack, NULL},
	{"GL_EXT_texture_filter_anisotropic", &ogl_ext_EXT_texture_filter_anisotropic, NULL},
	{"GL_EXT_texture_compression_s3tc", &ogl_ext_EXT_texture_compression_s3tc, NULL},
	{"GL_EXT_direct_state_access", &ogl_ext_EXT_direct_state_access, Load_EXT_direct_state_access},
	{"GL_ARB_debug_output", &ogl_ext_ARB_debug_output, Load_ARB_debug_output},
};

static int g_extensionMapSize = 7;

static ogl_StrToExtMap *FindExtEntry(const char *extensionName)
{
	int loop;
	ogl_StrToExtMap *currLoc = ExtensionMap;
	for(loop = 0; loop < g_extensionMapSize; ++loop, ++currLoc)
	{
		if(strcmp(extensionName, currLoc->extensionName) == 0)
			return currLoc;
	}
	
	return NULL;
}

static void ClearExtensionVars()
{
	ogl_ext_ARB_sampler_objects = ogl_LOAD_FAILED;
	ogl_ext_ARB_separate_shader_objects = ogl_LOAD_FAILED;
	ogl_ext_ARB_shading_language_420pack = ogl_LOAD_FAILED;
	ogl_ext_EXT_texture_filter_anisotropic = ogl_LOAD_FAILED;
	ogl_ext_EXT_texture_compression_s3tc = ogl_LOAD_FAILED;
	ogl_ext_EXT_direct_state_access = ogl_LOAD_FAILED;
	ogl_ext_ARB_debug_output = ogl_LOAD_FAILED;
}


static void LoadExtByName(const char *extensionName)
{
	ogl_StrToExtMap *entry = NULL;
	entry = FindExtEntry(extensionName);
	if(entry)
	{
		if(entry->LoadExtension)
		{
			int numFailed = entry->LoadExtension();
			if(numFailed == 0)
			{
				*(entry->extensionVariable) = ogl_LOAD_SUCCEEDED;
			}
			else
			{
				*(entry->extensionVariable) = ogl_LOAD_SUCCEEDED + numFailed;
			}
		}
		else
		{
			*(entry->extensionVariable) = ogl_LOAD_SUCCEEDED;
		}
	}
}


static void ProcExtsFromExtList()
{
	GLint iLoop;
	GLint iNumExtensions = 0;
	_ptrc_glGetIntegerv(GL_NUM_EXTENSIONS, &iNumExtensions);

	for(iLoop = 0; iLoop < iNumExtensions; iLoop++)
	{
		const char *strExtensionName = (const char *)_ptrc_glGetStringi(GL_EXTENSIONS, iLoop);
		LoadExtByName(strExtensionName);
	}
}

int ogl_LoadFunctions()
{
	int numFailed = 0;
	ClearExtensionVars();
	
	_ptrc_glGetIntegerv = (void (CODEGEN_FUNCPTR *)(GLenum , GLint *))IntGetProcAddress("glGetIntegerv");
	if(!_ptrc_glGetIntegerv) return ogl_LOAD_FAILED;
	_ptrc_glGetStringi = (const GLubyte * (CODEGEN_FUNCPTR *)(GLenum , GLuint ))IntGetProcAddress("glGetStringi");
	if(!_ptrc_glGetStringi) return ogl_LOAD_FAILED;
	
	ProcExtsFromExtList();
	numFailed = Load_Version_3_3();
	
	if(numFailed == 0)
		return ogl_LOAD_SUCCEEDED;
	else
		return ogl_LOAD_SUCCEEDED + numFailed;
}

static int g_major_version = 0;
static int g_minor_version = 0;

static void GetGLVersion()
{
	glGetIntegerv(GL_MAJOR_VERSION, &g_major_version);
	glGetIntegerv(GL_MINOR_VERSION, &g_minor_version);
}

int ogl_GetMajorVersion()
{
	if(g_major_version == 0)
		GetGLVersion();
	return g_major_version;
}

int ogl_GetMinorVersion()
{
	if(g_major_version == 0) //Yes, check the major version to get the minor one.
		GetGLVersion();
	return g_minor_version;
}

int ogl_IsVersionGEQ(int majorVersion, int minorVersion)
{
	if(g_major_version == 0)
		GetGLVersion();
		
	if(majorVersion > g_major_version) return 1;
	if(majorVersion < g_major_version) return 0;
	if(minorVersion >= g_minor_version) return 1;
	return 0;
}

#endif