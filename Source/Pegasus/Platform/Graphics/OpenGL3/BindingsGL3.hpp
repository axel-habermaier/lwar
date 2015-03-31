#pragma once

#ifdef PG_SYSTEM_WINDOWS
	#define GL_APIENTRY __stdcall
#else
	#define GL_APIENTRY
#endif

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Typedefs
//-------------------------------------------------------------------------------------------------------------------------------------------------------
using glActiveTextureFunc = void(GL_APIENTRY*)(uint32);
using glAttachShaderFunc = void(GL_APIENTRY*)(uint32, uint32);
using glBindBufferFunc = void(GL_APIENTRY*)(uint32, uint32);
using glBindBufferBaseFunc = void(GL_APIENTRY*)(uint32, uint32, uint32);
using glBindFramebufferFunc = void(GL_APIENTRY*)(uint32, uint32);
using glBindSamplerFunc = void(GL_APIENTRY*)(uint32, uint32);
using glBindTextureFunc = void(GL_APIENTRY*)(uint32, uint32);
using glBindVertexArrayFunc = void(GL_APIENTRY*)(uint32);
using glBlendEquationSeparateFunc = void(GL_APIENTRY*)(uint32, uint32);
using glBlendFuncSeparateFunc = void(GL_APIENTRY*)(uint32, uint32, uint32, uint32);
using glBufferDataFunc = void(GL_APIENTRY*)(uint32, void*, void*, uint32);
using glBufferSubDataFunc = void(GL_APIENTRY*)(uint32, void*, void*, void*);
using glCheckFramebufferStatusFunc = uint32(GL_APIENTRY*)(uint32);
using glClearFunc = void(GL_APIENTRY*)(uint32);
using glClearColorFunc = void(GL_APIENTRY*)(float32, float32, float32, float32);
using glClearDepthFunc = void(GL_APIENTRY*)(float64);
using glClearStencilFunc = void(GL_APIENTRY*)(int32);
using glClientWaitSyncFunc = uint32(GL_APIENTRY*)(void*, uint32, uint64);
using glColorMaskFunc = void(GL_APIENTRY*)(int32, int32, int32, int32);
using glCompileShaderFunc = void(GL_APIENTRY*)(uint32);
using glCompressedTexImage2DFunc = void(GL_APIENTRY*)(uint32, int32, uint32, int32, int32, int32, int32, void*);
using glCreateProgramFunc = uint32(GL_APIENTRY*)();
using glCreateShaderFunc = uint32(GL_APIENTRY*)(uint32);
using glCullFaceFunc = void(GL_APIENTRY*)(uint32);
using glDeleteBuffersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDeleteFramebuffersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDeleteProgramFunc = void(GL_APIENTRY*)(uint32);
using glDeleteQueriesFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDeleteSamplersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDeleteShaderFunc = void(GL_APIENTRY*)(uint32);
using glDeleteSyncFunc = void(GL_APIENTRY*)(void*);
using glDeleteTexturesFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDeleteVertexArraysFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDepthFuncFunc = void(GL_APIENTRY*)(uint32);
using glDepthMaskFunc = void(GL_APIENTRY*)(int32);
using glDetachShaderFunc = void(GL_APIENTRY*)(uint32, uint32);
using glDisableFunc = void(GL_APIENTRY*)(uint32);
using glDrawArraysFunc = void(GL_APIENTRY*)(uint32, int32, int32);
using glDrawArraysInstancedBaseInstanceFunc = void(GL_APIENTRY*)(uint32, int32, int32, int32, uint32);
using glDrawBuffersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glDrawElementsBaseVertexFunc = void(GL_APIENTRY*)(uint32, int32, uint32, void*, int32);
using glDrawElementsInstancedBaseVertexBaseInstanceFunc = void(GL_APIENTRY*)(uint32, int32, uint32, void*, int32, int32, uint32);
using glEnableFunc = void(GL_APIENTRY*)(uint32);
using glEnableVertexAttribArrayFunc = void(GL_APIENTRY*)(uint32);
using glFenceSyncFunc = void*(GL_APIENTRY*)(uint32, uint32);
using glFramebufferTexture2DFunc = void(GL_APIENTRY*)(uint32, uint32, uint32, uint32, int32);
using glFrontFaceFunc = void(GL_APIENTRY*)(uint32);
using glGenBuffersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glGenerateMipmapFunc = void(GL_APIENTRY*)(uint32);
using glGenFramebuffersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glGenQueriesFunc = void(GL_APIENTRY*)(int32, uint32*);
using glGenSamplersFunc = void(GL_APIENTRY*)(int32, uint32*);
using glGenTexturesFunc = void(GL_APIENTRY*)(int32, uint32*);
using glGenVertexArraysFunc = void(GL_APIENTRY*)(int32, uint32*);
using glGetErrorFunc = uint32(GL_APIENTRY*)();
using glGetIntegervFunc = void(GL_APIENTRY*)(uint32, int32*);
using glGetProgramInfoLogFunc = void(GL_APIENTRY*)(uint32, int32, int32*, char*);
using glGetProgramivFunc = void(GL_APIENTRY*)(uint32, uint32, int32*);
using glGetQueryObjectivFunc = void(GL_APIENTRY*)(uint32, uint32, int32*);
using glGetQueryObjectui64vFunc = void(GL_APIENTRY*)(uint32, uint32, uint64*);
using glGetShaderInfoLogFunc = void(GL_APIENTRY*)(uint32, int32, int32*, char*);
using glGetShaderivFunc = void(GL_APIENTRY*)(uint32, uint32, int32*);
using glGetStringFunc = byte*(GL_APIENTRY*)(uint32);
using glLinkProgramFunc = void(GL_APIENTRY*)(uint32);
using glMapBufferFunc = void*(GL_APIENTRY*)(uint32, uint32);
using glMapBufferRangeFunc = void*(GL_APIENTRY*)(uint32, void*, void*, uint32);
using glPolygonModeFunc = void(GL_APIENTRY*)(uint32, uint32);
using glPolygonOffsetFunc = void(GL_APIENTRY*)(float32, float32);
using glQueryCounterFunc = void(GL_APIENTRY*)(uint32, uint32);
using glSamplerParameterfFunc = void(GL_APIENTRY*)(uint32, uint32, float32);
using glSamplerParameterfvFunc = void(GL_APIENTRY*)(uint32, uint32, float32*);
using glSamplerParameteriFunc = void(GL_APIENTRY*)(uint32, uint32, int32);
using glScissorFunc = void(GL_APIENTRY*)(int32, int32, int32, int32);
using glShaderSourceFunc = void(GL_APIENTRY*)(uint32, int32, byte**, int32*);
using glStencilFuncSeparateFunc = void(GL_APIENTRY*)(uint32, uint32, int32, uint32);
using glStencilOpSeparateFunc = void(GL_APIENTRY*)(uint32, uint32, uint32, uint32);
using glTexImage2DFunc = void(GL_APIENTRY*)(uint32, int32, int32, int32, int32, int32, uint32, uint32, void*);
using glUnmapBufferFunc = int32(GL_APIENTRY*)(uint32);
using glUseProgramFunc = void(GL_APIENTRY*)(uint32);
using glVertexAttribDivisorFunc = void(GL_APIENTRY*)(uint32, uint32);
using glVertexAttribPointerFunc = void(GL_APIENTRY*)(uint32, int32, uint32, int32, int32, void*);
using glViewportFunc = void(GL_APIENTRY*)(int32, int32, int32, int32);

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// BindingsGL3
//-------------------------------------------------------------------------------------------------------------------------------------------------------
struct BindingsGL3
{
	void Initialize();

	glActiveTextureFunc ActiveTexture;
	glAttachShaderFunc AttachShader;
	glBindBufferFunc BindBuffer;
	glBindBufferBaseFunc BindBufferBase;
	glBindFramebufferFunc BindFramebuffer;
	glBindSamplerFunc BindSampler;
	glBindTextureFunc BindTexture;
	glBindVertexArrayFunc BindVertexArray;
	glBlendEquationSeparateFunc BlendEquationSeparate;
	glBlendFuncSeparateFunc BlendFuncSeparate;
	glBufferDataFunc BufferData;
	glBufferSubDataFunc BufferSubData;
	glCheckFramebufferStatusFunc CheckFramebufferStatus;
	glClearFunc Clear;
	glClearColorFunc ClearColor;
	glClearDepthFunc ClearDepth;
	glClearStencilFunc ClearStencil;
	glClientWaitSyncFunc ClientWaitSync;
	glColorMaskFunc ColorMask;
	glCompileShaderFunc CompileShader;
	glCompressedTexImage2DFunc CompressedTexImage2D;
	glCreateProgramFunc CreateProgram;
	glCreateShaderFunc CreateShader;
	glCullFaceFunc CullFace;
	glDeleteBuffersFunc DeleteBuffers;
	glDeleteFramebuffersFunc DeleteFramebuffers;
	glDeleteProgramFunc DeleteProgram;
	glDeleteQueriesFunc DeleteQueries;
	glDeleteSamplersFunc DeleteSamplers;
	glDeleteShaderFunc DeleteShader;
	glDeleteSyncFunc DeleteSync;
	glDeleteTexturesFunc DeleteTextures;
	glDeleteVertexArraysFunc DeleteVertexArrays;
	glDepthFuncFunc DepthFunc;
	glDepthMaskFunc DepthMask;
	glDetachShaderFunc DetachShader;
	glDisableFunc Disable;
	glDrawArraysFunc DrawArrays;
	glDrawArraysInstancedBaseInstanceFunc DrawArraysInstancedBaseInstance;
	glDrawBuffersFunc DrawBuffers;
	glDrawElementsBaseVertexFunc DrawElementsBaseVertex;
	glDrawElementsInstancedBaseVertexBaseInstanceFunc DrawElementsInstancedBaseVertexBaseInstance;
	glEnableFunc Enable;
	glEnableVertexAttribArrayFunc EnableVertexAttribArray;
	glFenceSyncFunc FenceSync;
	glFramebufferTexture2DFunc FramebufferTexture2D;
	glFrontFaceFunc FrontFace;
	glGenBuffersFunc GenBuffers;
	glGenerateMipmapFunc GenerateMipmap;
	glGenFramebuffersFunc GenFramebuffers;
	glGenQueriesFunc GenQueries;
	glGenSamplersFunc GenSamplers;
	glGenTexturesFunc GenTextures;
	glGenVertexArraysFunc GenVertexArrays;
	glGetErrorFunc GetError;
	glGetIntegervFunc GetIntegerv;
	glGetProgramInfoLogFunc GetProgramInfoLog;
	glGetProgramivFunc GetProgramiv;
	glGetQueryObjectivFunc GetQueryObjectiv;
	glGetQueryObjectui64vFunc GetQueryObjectui64v;
	glGetShaderInfoLogFunc GetShaderInfoLog;
	glGetShaderivFunc GetShaderiv;
	glGetStringFunc GetString;
	glLinkProgramFunc LinkProgram;
	glMapBufferFunc MapBuffer;
	glMapBufferRangeFunc MapBufferRange;
	glPolygonModeFunc PolygonMode;
	glPolygonOffsetFunc PolygonOffset;
	glQueryCounterFunc QueryCounter;
	glSamplerParameterfFunc SamplerParameterf;
	glSamplerParameterfvFunc SamplerParameterfv;
	glSamplerParameteriFunc SamplerParameteri;
	glScissorFunc Scissor;
	glShaderSourceFunc ShaderSource;
	glStencilFuncSeparateFunc StencilFuncSeparate;
	glStencilOpSeparateFunc StencilOpSeparate;
	glTexImage2DFunc TexImage2D;
	glUnmapBufferFunc UnmapBuffer;
	glUseProgramFunc UseProgram;
	glVertexAttribDivisorFunc VertexAttribDivisor;
	glVertexAttribPointerFunc VertexAttribPointer;
	glViewportFunc Viewport;
};

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Macro
//-------------------------------------------------------------------------------------------------------------------------------------------------------

#define PG_DECLARE_OPENGL3_FUNCS \
	void glActiveTexture(uint32 arg1) { _bindings.ActiveTexture(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glAttachShader(uint32 arg1, uint32 arg2) { _bindings.AttachShader(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBindBuffer(uint32 arg1, uint32 arg2) { _bindings.BindBuffer(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBindBufferBase(uint32 arg1, uint32 arg2, uint32 arg3) { _bindings.BindBufferBase(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBindFramebuffer(uint32 arg1, uint32 arg2) { _bindings.BindFramebuffer(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBindSampler(uint32 arg1, uint32 arg2) { _bindings.BindSampler(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBindTexture(uint32 arg1, uint32 arg2) { _bindings.BindTexture(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBindVertexArray(uint32 arg1) { _bindings.BindVertexArray(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBlendEquationSeparate(uint32 arg1, uint32 arg2) { _bindings.BlendEquationSeparate(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBlendFuncSeparate(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4) { _bindings.BlendFuncSeparate(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBufferData(uint32 arg1, void* arg2, void* arg3, uint32 arg4) { _bindings.BufferData(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glBufferSubData(uint32 arg1, void* arg2, void* arg3, void* arg4) { _bindings.BufferSubData(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	uint32 glCheckFramebufferStatus(uint32 arg1) { auto result = _bindings.CheckFramebufferStatus(arg1); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glClear(uint32 arg1) { _bindings.Clear(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glClearColor(float32 arg1, float32 arg2, float32 arg3, float32 arg4) { _bindings.ClearColor(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glClearDepth(float64 arg1) { _bindings.ClearDepth(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glClearStencil(int32 arg1) { _bindings.ClearStencil(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	uint32 glClientWaitSync(void* arg1, uint32 arg2, uint64 arg3) { auto result = _bindings.ClientWaitSync(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glColorMask(int32 arg1, int32 arg2, int32 arg3, int32 arg4) { _bindings.ColorMask(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glCompileShader(uint32 arg1) { _bindings.CompileShader(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glCompressedTexImage2D(uint32 arg1, int32 arg2, uint32 arg3, int32 arg4, int32 arg5, int32 arg6, int32 arg7, void* arg8) { _bindings.CompressedTexImage2D(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); PG_DEBUG_ONLY(CheckErrors()); } \
	uint32 glCreateProgram() { auto result = _bindings.CreateProgram(); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	uint32 glCreateShader(uint32 arg1) { auto result = _bindings.CreateShader(arg1); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glCullFace(uint32 arg1) { _bindings.CullFace(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteBuffers(int32 arg1, uint32* arg2) { _bindings.DeleteBuffers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteFramebuffers(int32 arg1, uint32* arg2) { _bindings.DeleteFramebuffers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteProgram(uint32 arg1) { _bindings.DeleteProgram(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteQueries(int32 arg1, uint32* arg2) { _bindings.DeleteQueries(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteSamplers(int32 arg1, uint32* arg2) { _bindings.DeleteSamplers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteShader(uint32 arg1) { _bindings.DeleteShader(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteSync(void* arg1) { _bindings.DeleteSync(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteTextures(int32 arg1, uint32* arg2) { _bindings.DeleteTextures(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDeleteVertexArrays(int32 arg1, uint32* arg2) { _bindings.DeleteVertexArrays(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDepthFunc(uint32 arg1) { _bindings.DepthFunc(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDepthMask(int32 arg1) { _bindings.DepthMask(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDetachShader(uint32 arg1, uint32 arg2) { _bindings.DetachShader(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDisable(uint32 arg1) { _bindings.Disable(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDrawArrays(uint32 arg1, int32 arg2, int32 arg3) { _bindings.DrawArrays(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDrawArraysInstancedBaseInstance(uint32 arg1, int32 arg2, int32 arg3, int32 arg4, uint32 arg5) { _bindings.DrawArraysInstancedBaseInstance(arg1, arg2, arg3, arg4, arg5); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDrawBuffers(int32 arg1, uint32* arg2) { _bindings.DrawBuffers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDrawElementsBaseVertex(uint32 arg1, int32 arg2, uint32 arg3, void* arg4, int32 arg5) { _bindings.DrawElementsBaseVertex(arg1, arg2, arg3, arg4, arg5); PG_DEBUG_ONLY(CheckErrors()); } \
	void glDrawElementsInstancedBaseVertexBaseInstance(uint32 arg1, int32 arg2, uint32 arg3, void* arg4, int32 arg5, int32 arg6, uint32 arg7) { _bindings.DrawElementsInstancedBaseVertexBaseInstance(arg1, arg2, arg3, arg4, arg5, arg6, arg7); PG_DEBUG_ONLY(CheckErrors()); } \
	void glEnable(uint32 arg1) { _bindings.Enable(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glEnableVertexAttribArray(uint32 arg1) { _bindings.EnableVertexAttribArray(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void* glFenceSync(uint32 arg1, uint32 arg2) { auto result = _bindings.FenceSync(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glFramebufferTexture2D(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4, int32 arg5) { _bindings.FramebufferTexture2D(arg1, arg2, arg3, arg4, arg5); PG_DEBUG_ONLY(CheckErrors()); } \
	void glFrontFace(uint32 arg1) { _bindings.FrontFace(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenBuffers(int32 arg1, uint32* arg2) { _bindings.GenBuffers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenerateMipmap(uint32 arg1) { _bindings.GenerateMipmap(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenFramebuffers(int32 arg1, uint32* arg2) { _bindings.GenFramebuffers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenQueries(int32 arg1, uint32* arg2) { _bindings.GenQueries(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenSamplers(int32 arg1, uint32* arg2) { _bindings.GenSamplers(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenTextures(int32 arg1, uint32* arg2) { _bindings.GenTextures(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGenVertexArrays(int32 arg1, uint32* arg2) { _bindings.GenVertexArrays(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	uint32 glGetError() { return _bindings.GetError(); } \
	void glGetIntegerv(uint32 arg1, int32* arg2) { _bindings.GetIntegerv(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGetProgramInfoLog(uint32 arg1, int32 arg2, int32* arg3, char* arg4) { _bindings.GetProgramInfoLog(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGetProgramiv(uint32 arg1, uint32 arg2, int32* arg3) { _bindings.GetProgramiv(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGetQueryObjectiv(uint32 arg1, uint32 arg2, int32* arg3) { _bindings.GetQueryObjectiv(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGetQueryObjectui64v(uint32 arg1, uint32 arg2, uint64* arg3) { _bindings.GetQueryObjectui64v(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGetShaderInfoLog(uint32 arg1, int32 arg2, int32* arg3, char* arg4) { _bindings.GetShaderInfoLog(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glGetShaderiv(uint32 arg1, uint32 arg2, int32* arg3) { _bindings.GetShaderiv(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	byte* glGetString(uint32 arg1) { auto result = _bindings.GetString(arg1); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glLinkProgram(uint32 arg1) { _bindings.LinkProgram(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void* glMapBuffer(uint32 arg1, uint32 arg2) { auto result = _bindings.MapBuffer(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void* glMapBufferRange(uint32 arg1, void* arg2, void* arg3, uint32 arg4) { auto result = _bindings.MapBufferRange(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glPolygonMode(uint32 arg1, uint32 arg2) { _bindings.PolygonMode(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glPolygonOffset(float32 arg1, float32 arg2) { _bindings.PolygonOffset(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glQueryCounter(uint32 arg1, uint32 arg2) { _bindings.QueryCounter(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glSamplerParameterf(uint32 arg1, uint32 arg2, float32 arg3) { _bindings.SamplerParameterf(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glSamplerParameterfv(uint32 arg1, uint32 arg2, float32* arg3) { _bindings.SamplerParameterfv(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glSamplerParameteri(uint32 arg1, uint32 arg2, int32 arg3) { _bindings.SamplerParameteri(arg1, arg2, arg3); PG_DEBUG_ONLY(CheckErrors()); } \
	void glScissor(int32 arg1, int32 arg2, int32 arg3, int32 arg4) { _bindings.Scissor(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glShaderSource(uint32 arg1, int32 arg2, byte** arg3, int32* arg4) { _bindings.ShaderSource(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glStencilFuncSeparate(uint32 arg1, uint32 arg2, int32 arg3, uint32 arg4) { _bindings.StencilFuncSeparate(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glStencilOpSeparate(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4) { _bindings.StencilOpSeparate(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \
	void glTexImage2D(uint32 arg1, int32 arg2, int32 arg3, int32 arg4, int32 arg5, int32 arg6, uint32 arg7, uint32 arg8, void* arg9) { _bindings.TexImage2D(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9); PG_DEBUG_ONLY(CheckErrors()); } \
	int32 glUnmapBuffer(uint32 arg1) { auto result = _bindings.UnmapBuffer(arg1); PG_DEBUG_ONLY(CheckErrors()); return result; } \
	void glUseProgram(uint32 arg1) { _bindings.UseProgram(arg1); PG_DEBUG_ONLY(CheckErrors()); } \
	void glVertexAttribDivisor(uint32 arg1, uint32 arg2) { _bindings.VertexAttribDivisor(arg1, arg2); PG_DEBUG_ONLY(CheckErrors()); } \
	void glVertexAttribPointer(uint32 arg1, int32 arg2, uint32 arg3, int32 arg4, int32 arg5, void* arg6) { _bindings.VertexAttribPointer(arg1, arg2, arg3, arg4, arg5, arg6); PG_DEBUG_ONLY(CheckErrors()); } \
	void glViewport(int32 arg1, int32 arg2, int32 arg3, int32 arg4) { _bindings.Viewport(arg1, arg2, arg3, arg4); PG_DEBUG_ONLY(CheckErrors()); } \

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Constants
//-------------------------------------------------------------------------------------------------------------------------------------------------------
#define GL_SAMPLER_BINDING 0x8919
#define GL_VERTEX_SHADER_BIT 0x00000001
#define GL_FRAGMENT_SHADER_BIT 0x00000002
#define GL_GEOMETRY_SHADER_BIT 0x00000004
#define GL_TESS_CONTROL_SHADER_BIT 0x00000008
#define GL_TESS_EVALUATION_SHADER_BIT 0x00000010
#define GL_ALL_SHADER_BITS 0xFFFFFFFF
#define GL_PROGRAM_SEPARABLE 0x8258
#define GL_ACTIVE_PROGRAM 0x8259
#define GL_PROGRAM_PIPELINE_BINDING 0x825A
#define GL_TEXTURE_MAX_ANISOTROPY_EXT 0x84FE
#define GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT 0x84FF
#define GL_COMPRESSED_RGB_S3TC_DXT1_EXT 0x83F0
#define GL_COMPRESSED_RGBA_S3TC_DXT1_EXT 0x83F1
#define GL_COMPRESSED_RGBA_S3TC_DXT3_EXT 0x83F2
#define GL_COMPRESSED_RGBA_S3TC_DXT5_EXT 0x83F3
#define GL_PROGRAM_MATRIX_EXT 0x8E2D
#define GL_TRANSPOSE_PROGRAM_MATRIX_EXT 0x8E2E
#define GL_PROGRAM_MATRIX_STACK_DEPTH_EXT 0x8E2F
#define GL_DEBUG_OUTPUT_SYNCHRONOUS_ARB 0x8242
#define GL_DEBUG_NEXT_LOGGED_MESSAGE_LENGTH_ARB 0x8243
#define GL_DEBUG_CALLBACK_FUNCTION_ARB 0x8244
#define GL_DEBUG_CALLBACK_USER_PARAM_ARB 0x8245
#define GL_DEBUG_SOURCE_API_ARB 0x8246
#define GL_DEBUG_SOURCE_WINDOW_SYSTEM_ARB 0x8247
#define GL_DEBUG_SOURCE_SHADER_COMPILER_ARB 0x8248
#define GL_DEBUG_SOURCE_THIRD_PARTY_ARB 0x8249
#define GL_DEBUG_SOURCE_APPLICATION_ARB 0x824A
#define GL_DEBUG_SOURCE_OTHER_ARB 0x824B
#define GL_DEBUG_TYPE_ERROR_ARB 0x824C
#define GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR_ARB 0x824D
#define GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR_ARB 0x824E
#define GL_DEBUG_TYPE_PORTABILITY_ARB 0x824F
#define GL_DEBUG_TYPE_PERFORMANCE_ARB 0x8250
#define GL_DEBUG_TYPE_OTHER_ARB 0x8251
#define GL_MAX_DEBUG_MESSAGE_LENGTH_ARB 0x9143
#define GL_MAX_DEBUG_LOGGED_MESSAGES_ARB 0x9144
#define GL_DEBUG_LOGGED_MESSAGES_ARB 0x9145
#define GL_DEBUG_SEVERITY_HIGH_ARB 0x9146
#define GL_DEBUG_SEVERITY_MEDIUM_ARB 0x9147
#define GL_DEBUG_SEVERITY_LOW_ARB 0x9148
#define GL_DEPTH_BUFFER_BIT 0x00000100
#define GL_STENCIL_BUFFER_BIT 0x00000400
#define GL_COLOR_BUFFER_BIT 0x00004000
#define GL_FALSE 0
#define GL_TRUE 1
#define GL_POINTS 0x0000
#define GL_LINES 0x0001
#define GL_LINE_LOOP 0x0002
#define GL_LINE_STRIP 0x0003
#define GL_TRIANGLES 0x0004
#define GL_TRIANGLE_STRIP 0x0005
#define GL_TRIANGLE_FAN 0x0006
#define GL_QUADS 0x0007
#define GL_NEVER 0x0200
#define GL_LESS 0x0201
#define GL_EQUAL 0x0202
#define GL_LEQUAL 0x0203
#define GL_GREATER 0x0204
#define GL_NOTEQUAL 0x0205
#define GL_GEQUAL 0x0206
#define GL_ALWAYS 0x0207
#define GL_ZERO 0
#define GL_ONE 1
#define GL_SRC_COLOR 0x0300
#define GL_ONE_MINUS_SRC_COLOR 0x0301
#define GL_SRC_ALPHA 0x0302
#define GL_ONE_MINUS_SRC_ALPHA 0x0303
#define GL_DST_ALPHA 0x0304
#define GL_ONE_MINUS_DST_ALPHA 0x0305
#define GL_DST_COLOR 0x0306
#define GL_ONE_MINUS_DST_COLOR 0x0307
#define GL_SRC_ALPHA_SATURATE 0x0308
#define GL_NONE 0
#define GL_FRONT_LEFT 0x0400
#define GL_FRONT_RIGHT 0x0401
#define GL_BACK_LEFT 0x0402
#define GL_BACK_RIGHT 0x0403
#define GL_FRONT 0x0404
#define GL_BACK 0x0405
#define GL_LEFT 0x0406
#define GL_RIGHT 0x0407
#define GL_FRONT_AND_BACK 0x0408
#define GL_NO_ERROR 0
#define GL_INVALID_ENUM 0x0500
#define GL_INVALID_VALUE 0x0501
#define GL_INVALID_OPERATION 0x0502
#define GL_OUT_OF_MEMORY 0x0505
#define GL_CW 0x0900
#define GL_CCW 0x0901
#define GL_POINT_SIZE 0x0B11
#define GL_POINT_SIZE_RANGE 0x0B12
#define GL_POINT_SIZE_GRANULARITY 0x0B13
#define GL_LINE_SMOOTH 0x0B20
#define GL_LINE_WIDTH 0x0B21
#define GL_LINE_WIDTH_RANGE 0x0B22
#define GL_LINE_WIDTH_GRANULARITY 0x0B23
#define GL_POLYGON_MODE 0x0B40
#define GL_POLYGON_SMOOTH 0x0B41
#define GL_CULL_FACE 0x0B44
#define GL_CULL_FACE_MODE 0x0B45
#define GL_FRONT_FACE 0x0B46
#define GL_DEPTH_RANGE 0x0B70
#define GL_DEPTH_TEST 0x0B71
#define GL_DEPTH_WRITEMASK 0x0B72
#define GL_DEPTH_CLEAR_VALUE 0x0B73
#define GL_DEPTH_FUNC 0x0B74
#define GL_STENCIL_TEST 0x0B90
#define GL_STENCIL_CLEAR_VALUE 0x0B91
#define GL_STENCIL_FUNC 0x0B92
#define GL_STENCIL_VALUE_MASK 0x0B93
#define GL_STENCIL_FAIL 0x0B94
#define GL_STENCIL_PASS_DEPTH_FAIL 0x0B95
#define GL_STENCIL_PASS_DEPTH_PASS 0x0B96
#define GL_STENCIL_REF 0x0B97
#define GL_STENCIL_WRITEMASK 0x0B98
#define GL_VIEWPORT 0x0BA2
#define GL_DITHER 0x0BD0
#define GL_BLEND_DST 0x0BE0
#define GL_BLEND_SRC 0x0BE1
#define GL_BLEND 0x0BE2
#define GL_LOGIC_OP_MODE 0x0BF0
#define GL_COLOR_LOGIC_OP 0x0BF2
#define GL_DRAW_BUFFER 0x0C01
#define GL_READ_BUFFER 0x0C02
#define GL_SCISSOR_BOX 0x0C10
#define GL_SCISSOR_TEST 0x0C11
#define GL_COLOR_CLEAR_VALUE 0x0C22
#define GL_COLOR_WRITEMASK 0x0C23
#define GL_DOUBLEBUFFER 0x0C32
#define GL_STEREO 0x0C33
#define GL_LINE_SMOOTH_HINT 0x0C52
#define GL_POLYGON_SMOOTH_HINT 0x0C53
#define GL_UNPACK_SWAP_BYTES 0x0CF0
#define GL_UNPACK_LSB_FIRST 0x0CF1
#define GL_UNPACK_ROW_LENGTH 0x0CF2
#define GL_UNPACK_SKIP_ROWS 0x0CF3
#define GL_UNPACK_SKIP_PIXELS 0x0CF4
#define GL_UNPACK_ALIGNMENT 0x0CF5
#define GL_PACK_SWAP_BYTES 0x0D00
#define GL_PACK_LSB_FIRST 0x0D01
#define GL_PACK_ROW_LENGTH 0x0D02
#define GL_PACK_SKIP_ROWS 0x0D03
#define GL_PACK_SKIP_PIXELS 0x0D04
#define GL_PACK_ALIGNMENT 0x0D05
#define GL_MAX_TEXTURE_SIZE 0x0D33
#define GL_MAX_VIEWPORT_DIMS 0x0D3A
#define GL_SUBPIXEL_BITS 0x0D50
#define GL_TEXTURE_1D 0x0DE0
#define GL_TEXTURE_2D 0x0DE1
#define GL_POLYGON_OFFSET_UNITS 0x2A00
#define GL_POLYGON_OFFSET_POINT 0x2A01
#define GL_POLYGON_OFFSET_LINE 0x2A02
#define GL_POLYGON_OFFSET_FILL 0x8037
#define GL_POLYGON_OFFSET_FACTOR 0x8038
#define GL_TEXTURE_BINDING_1D 0x8068
#define GL_TEXTURE_BINDING_2D 0x8069
#define GL_TEXTURE_WIDTH 0x1000
#define GL_TEXTURE_HEIGHT 0x1001
#define GL_TEXTURE_INTERNAL_FORMAT 0x1003
#define GL_TEXTURE_BORDER_COLOR 0x1004
#define GL_TEXTURE_RED_SIZE 0x805C
#define GL_TEXTURE_GREEN_SIZE 0x805D
#define GL_TEXTURE_BLUE_SIZE 0x805E
#define GL_TEXTURE_ALPHA_SIZE 0x805F
#define GL_DONT_CARE 0x1100
#define GL_FASTEST 0x1101
#define GL_NICEST 0x1102
#define GL_BYTE 0x1400
#define GL_UNSIGNED_BYTE 0x1401
#define GL_SHORT 0x1402
#define GL_UNSIGNED_SHORT 0x1403
#define GL_INT 0x1404
#define GL_UNSIGNED_INT 0x1405
#define GL_FLOAT 0x1406
#define GL_DOUBLE 0x140A
#define GL_CLEAR 0x1500
#define GL_AND 0x1501
#define GL_AND_REVERSE 0x1502
#define GL_COPY 0x1503
#define GL_AND_INVERTED 0x1504
#define GL_NOOP 0x1505
#define GL_XOR 0x1506
#define GL_OR 0x1507
#define GL_NOR 0x1508
#define GL_EQUIV 0x1509
#define GL_INVERT 0x150A
#define GL_OR_REVERSE 0x150B
#define GL_COPY_INVERTED 0x150C
#define GL_OR_INVERTED 0x150D
#define GL_NAND 0x150E
#define GL_SET 0x150F
#define GL_TEXTURE 0x1702
#define GL_COLOR 0x1800
#define GL_DEPTH 0x1801
#define GL_STENCIL 0x1802
#define GL_STENCIL_INDEX 0x1901
#define GL_DEPTH_COMPONENT 0x1902
#define GL_RED 0x1903
#define GL_GREEN 0x1904
#define GL_BLUE 0x1905
#define GL_ALPHA 0x1906
#define GL_RGB 0x1907
#define GL_RGBA 0x1908
#define GL_POINT 0x1B00
#define GL_LINE 0x1B01
#define GL_FILL 0x1B02
#define GL_KEEP 0x1E00
#define GL_REPLACE 0x1E01
#define GL_INCR 0x1E02
#define GL_DECR 0x1E03
#define GL_VENDOR 0x1F00
#define GL_RENDERER 0x1F01
#define GL_VERSION 0x1F02
#define GL_EXTENSIONS 0x1F03
#define GL_NEAREST 0x2600
#define GL_LINEAR 0x2601
#define GL_NEAREST_MIPMAP_NEAREST 0x2700
#define GL_LINEAR_MIPMAP_NEAREST 0x2701
#define GL_NEAREST_MIPMAP_LINEAR 0x2702
#define GL_LINEAR_MIPMAP_LINEAR 0x2703
#define GL_TEXTURE_MAG_FILTER 0x2800
#define GL_TEXTURE_MIN_FILTER 0x2801
#define GL_TEXTURE_WRAP_S 0x2802
#define GL_TEXTURE_WRAP_T 0x2803
#define GL_PROXY_TEXTURE_1D 0x8063
#define GL_PROXY_TEXTURE_2D 0x8064
#define GL_REPEAT 0x2901
#define GL_R3_G3_B2 0x2A10
#define GL_RGB4 0x804F
#define GL_RGB5 0x8050
#define GL_RGB8 0x8051
#define GL_RGB10 0x8052
#define GL_RGB12 0x8053
#define GL_RGB16 0x8054
#define GL_RGBA2 0x8055
#define GL_RGBA4 0x8056
#define GL_RGB5_A1 0x8057
#define GL_RGBA8 0x8058
#define GL_RGB10_A2 0x8059
#define GL_RGBA12 0x805A
#define GL_RGBA16 0x805B
#define GL_CONSTANT_COLOR 0x8001
#define GL_ONE_MINUS_CONSTANT_COLOR 0x8002
#define GL_CONSTANT_ALPHA 0x8003
#define GL_ONE_MINUS_CONSTANT_ALPHA 0x8004
#define GL_BLEND_COLOR 0x8005
#define GL_FUNC_ADD 0x8006
#define GL_MIN 0x8007
#define GL_MAX 0x8008
#define GL_BLEND_EQUATION 0x8009
#define GL_FUNC_SUBTRACT 0x800A
#define GL_FUNC_REVERSE_SUBTRACT 0x800B
#define GL_CONVOLUTION_1D 0x8010
#define GL_CONVOLUTION_2D 0x8011
#define GL_SEPARABLE_2D 0x8012
#define GL_CONVOLUTION_BORDER_MODE 0x8013
#define GL_CONVOLUTION_FILTER_SCALE 0x8014
#define GL_CONVOLUTION_FILTER_BIAS 0x8015
#define GL_REDUCE 0x8016
#define GL_CONVOLUTION_FORMAT 0x8017
#define GL_CONVOLUTION_WIDTH 0x8018
#define GL_CONVOLUTION_HEIGHT 0x8019
#define GL_MAX_CONVOLUTION_WIDTH 0x801A
#define GL_MAX_CONVOLUTION_HEIGHT 0x801B
#define GL_POST_CONVOLUTION_RED_SCALE 0x801C
#define GL_POST_CONVOLUTION_GREEN_SCALE 0x801D
#define GL_POST_CONVOLUTION_BLUE_SCALE 0x801E
#define GL_POST_CONVOLUTION_ALPHA_SCALE 0x801F
#define GL_POST_CONVOLUTION_RED_BIAS 0x8020
#define GL_POST_CONVOLUTION_GREEN_BIAS 0x8021
#define GL_POST_CONVOLUTION_BLUE_BIAS 0x8022
#define GL_POST_CONVOLUTION_ALPHA_BIAS 0x8023
#define GL_HISTOGRAM 0x8024
#define GL_PROXY_HISTOGRAM 0x8025
#define GL_HISTOGRAM_WIDTH 0x8026
#define GL_HISTOGRAM_FORMAT 0x8027
#define GL_HISTOGRAM_RED_SIZE 0x8028
#define GL_HISTOGRAM_GREEN_SIZE 0x8029
#define GL_HISTOGRAM_BLUE_SIZE 0x802A
#define GL_HISTOGRAM_ALPHA_SIZE 0x802B
#define GL_HISTOGRAM_LUMINANCE_SIZE 0x802C
#define GL_HISTOGRAM_SINK 0x802D
#define GL_MINMAX 0x802E
#define GL_MINMAX_FORMAT 0x802F
#define GL_MINMAX_SINK 0x8030
#define GL_TABLE_TOO_LARGE 0x8031
#define GL_COLOR_MATRIX 0x80B1
#define GL_COLOR_MATRIX_STACK_DEPTH 0x80B2
#define GL_MAX_COLOR_MATRIX_STACK_DEPTH 0x80B3
#define GL_POST_COLOR_MATRIX_RED_SCALE 0x80B4
#define GL_POST_COLOR_MATRIX_GREEN_SCALE 0x80B5
#define GL_POST_COLOR_MATRIX_BLUE_SCALE 0x80B6
#define GL_POST_COLOR_MATRIX_ALPHA_SCALE 0x80B7
#define GL_POST_COLOR_MATRIX_RED_BIAS 0x80B8
#define GL_POST_COLOR_MATRIX_GREEN_BIAS 0x80B9
#define GL_POST_COLOR_MATRIX_BLUE_BIAS 0x80BA
#define GL_POST_COLOR_MATRIX_ALPHA_BIAS 0x80BB
#define GL_COLOR_TABLE 0x80D0
#define GL_POST_CONVOLUTION_COLOR_TABLE 0x80D1
#define GL_POST_COLOR_MATRIX_COLOR_TABLE 0x80D2
#define GL_PROXY_COLOR_TABLE 0x80D3
#define GL_PROXY_POST_CONVOLUTION_COLOR_TABLE 0x80D4
#define GL_PROXY_POST_COLOR_MATRIX_COLOR_TABLE 0x80D5
#define GL_COLOR_TABLE_SCALE 0x80D6
#define GL_COLOR_TABLE_BIAS 0x80D7
#define GL_COLOR_TABLE_FORMAT 0x80D8
#define GL_COLOR_TABLE_WIDTH 0x80D9
#define GL_COLOR_TABLE_RED_SIZE 0x80DA
#define GL_COLOR_TABLE_GREEN_SIZE 0x80DB
#define GL_COLOR_TABLE_BLUE_SIZE 0x80DC
#define GL_COLOR_TABLE_ALPHA_SIZE 0x80DD
#define GL_COLOR_TABLE_LUMINANCE_SIZE 0x80DE
#define GL_COLOR_TABLE_INTENSITY_SIZE 0x80DF
#define GL_CONSTANT_BORDER 0x8151
#define GL_REPLICATE_BORDER 0x8153
#define GL_CONVOLUTION_BORDER_COLOR 0x8154
#define GL_UNSIGNED_BYTE_3_3_2 0x8032
#define GL_UNSIGNED_SHORT_4_4_4_4 0x8033
#define GL_UNSIGNED_SHORT_5_5_5_1 0x8034
#define GL_UNSIGNED_INT_8_8_8_8 0x8035
#define GL_UNSIGNED_INT_10_10_10_2 0x8036
#define GL_TEXTURE_BINDING_3D 0x806A
#define GL_PACK_SKIP_IMAGES 0x806B
#define GL_PACK_IMAGE_HEIGHT 0x806C
#define GL_UNPACK_SKIP_IMAGES 0x806D
#define GL_UNPACK_IMAGE_HEIGHT 0x806E
#define GL_TEXTURE_3D 0x806F
#define GL_PROXY_TEXTURE_3D 0x8070
#define GL_TEXTURE_DEPTH 0x8071
#define GL_TEXTURE_WRAP_R 0x8072
#define GL_MAX_3D_TEXTURE_SIZE 0x8073
#define GL_UNSIGNED_BYTE_2_3_3_REV 0x8362
#define GL_UNSIGNED_SHORT_5_6_5 0x8363
#define GL_UNSIGNED_SHORT_5_6_5_REV 0x8364
#define GL_UNSIGNED_SHORT_4_4_4_4_REV 0x8365
#define GL_UNSIGNED_SHORT_1_5_5_5_REV 0x8366
#define GL_UNSIGNED_INT_8_8_8_8_REV 0x8367
#define GL_BGR 0x80E0
#define GL_MAX_ELEMENTS_VERTICES 0x80E8
#define GL_MAX_ELEMENTS_INDICES 0x80E9
#define GL_CLAMP_TO_EDGE 0x812F
#define GL_TEXTURE_MIN_LOD 0x813A
#define GL_TEXTURE_MAX_LOD 0x813B
#define GL_TEXTURE_BASE_LEVEL 0x813C
#define GL_TEXTURE_MAX_LEVEL 0x813D
#define GL_SMOOTH_POINT_SIZE_RANGE 0x0B12
#define GL_SMOOTH_POINT_SIZE_GRANULARITY 0x0B13
#define GL_SMOOTH_LINE_WIDTH_RANGE 0x0B22
#define GL_SMOOTH_LINE_WIDTH_GRANULARITY 0x0B23
#define GL_ALIASED_LINE_WIDTH_RANGE 0x846E
#define GL_TEXTURE0 0x84C0
#define GL_TEXTURE1 0x84C1
#define GL_TEXTURE2 0x84C2
#define GL_TEXTURE3 0x84C3
#define GL_TEXTURE4 0x84C4
#define GL_TEXTURE5 0x84C5
#define GL_TEXTURE6 0x84C6
#define GL_TEXTURE7 0x84C7
#define GL_TEXTURE8 0x84C8
#define GL_TEXTURE9 0x84C9
#define GL_TEXTURE10 0x84CA
#define GL_TEXTURE11 0x84CB
#define GL_TEXTURE12 0x84CC
#define GL_TEXTURE13 0x84CD
#define GL_TEXTURE14 0x84CE
#define GL_TEXTURE15 0x84CF
#define GL_TEXTURE16 0x84D0
#define GL_TEXTURE17 0x84D1
#define GL_TEXTURE18 0x84D2
#define GL_TEXTURE19 0x84D3
#define GL_TEXTURE20 0x84D4
#define GL_TEXTURE21 0x84D5
#define GL_TEXTURE22 0x84D6
#define GL_TEXTURE23 0x84D7
#define GL_TEXTURE24 0x84D8
#define GL_TEXTURE25 0x84D9
#define GL_TEXTURE26 0x84DA
#define GL_TEXTURE27 0x84DB
#define GL_TEXTURE28 0x84DC
#define GL_TEXTURE29 0x84DD
#define GL_TEXTURE30 0x84DE
#define GL_TEXTURE31 0x84DF
#define GL_ACTIVE_TEXTURE 0x84E0
#define GL_MULTISAMPLE 0x809D
#define GL_SAMPLE_ALPHA_TO_COVERAGE 0x809E
#define GL_SAMPLE_ALPHA_TO_ONE 0x809F
#define GL_SAMPLE_COVERAGE 0x80A0
#define GL_SAMPLE_BUFFERS 0x80A8
#define GL_SAMPLES 0x80A9
#define GL_SAMPLE_COVERAGE_VALUE 0x80AA
#define GL_SAMPLE_COVERAGE_INVERT 0x80AB
#define GL_TEXTURE_CUBE_MAP 0x8513
#define GL_TEXTURE_BINDING_CUBE_MAP 0x8514
#define GL_TEXTURE_CUBE_MAP_POSITIVE_X 0x8515
#define GL_TEXTURE_CUBE_MAP_NEGATIVE_X 0x8516
#define GL_TEXTURE_CUBE_MAP_POSITIVE_Y 0x8517
#define GL_TEXTURE_CUBE_MAP_NEGATIVE_Y 0x8518
#define GL_TEXTURE_CUBE_MAP_POSITIVE_Z 0x8519
#define GL_TEXTURE_CUBE_MAP_NEGATIVE_Z 0x851A
#define GL_PROXY_TEXTURE_CUBE_MAP 0x851B
#define GL_MAX_CUBE_MAP_TEXTURE_SIZE 0x851C
#define GL_COMPRESSED_RGB 0x84ED
#define GL_COMPRESSED_RGBA 0x84EE
#define GL_TEXTURE_COMPRESSION_HINT 0x84EF
#define GL_TEXTURE_COMPRESSED_IMAGE_SIZE 0x86A0
#define GL_TEXTURE_COMPRESSED 0x86A1
#define GL_NUM_COMPRESSED_TEXTURE_FORMATS 0x86A2
#define GL_COMPRESSED_TEXTURE_FORMATS 0x86A3
#define GL_CLAMP_TO_BORDER 0x812D
#define GL_BLEND_DST_RGB 0x80C8
#define GL_BLEND_SRC_RGB 0x80C9
#define GL_BLEND_DST_ALPHA 0x80CA
#define GL_BLEND_SRC_ALPHA 0x80CB
#define GL_POINT_FADE_THRESHOLD_SIZE 0x8128
#define GL_DEPTH_COMPONENT16 0x81A5
#define GL_DEPTH_COMPONENT24 0x81A6
#define GL_DEPTH_COMPONENT32 0x81A7
#define GL_MIRRORED_REPEAT 0x8370
#define GL_MAX_TEXTURE_LOD_BIAS 0x84FD
#define GL_TEXTURE_LOD_BIAS 0x8501
#define GL_INCR_WRAP 0x8507
#define GL_DECR_WRAP 0x8508
#define GL_TEXTURE_DEPTH_SIZE 0x884A
#define GL_TEXTURE_COMPARE_MODE 0x884C
#define GL_TEXTURE_COMPARE_FUNC 0x884D
#define GL_BUFFER_SIZE 0x8764
#define GL_BUFFER_USAGE 0x8765
#define GL_QUERY_COUNTER_BITS 0x8864
#define GL_CURRENT_QUERY 0x8865
#define GL_QUERY_RESULT 0x8866
#define GL_QUERY_RESULT_AVAILABLE 0x8867
#define GL_ARRAY_BUFFER 0x8892
#define GL_ELEMENT_ARRAY_BUFFER 0x8893
#define GL_ARRAY_BUFFER_BINDING 0x8894
#define GL_ELEMENT_ARRAY_BUFFER_BINDING 0x8895
#define GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING 0x889F
#define GL_READ_ONLY 0x88B8
#define GL_WRITE_ONLY 0x88B9
#define GL_READ_WRITE 0x88BA
#define GL_BUFFER_ACCESS 0x88BB
#define GL_BUFFER_MAPPED 0x88BC
#define GL_BUFFER_MAP_POINTER 0x88BD
#define GL_STREAM_DRAW 0x88E0
#define GL_STREAM_READ 0x88E1
#define GL_STREAM_COPY 0x88E2
#define GL_STATIC_DRAW 0x88E4
#define GL_STATIC_READ 0x88E5
#define GL_STATIC_COPY 0x88E6
#define GL_DYNAMIC_DRAW 0x88E8
#define GL_DYNAMIC_READ 0x88E9
#define GL_DYNAMIC_COPY 0x88EA
#define GL_SAMPLES_PASSED 0x8914
#define GL_BLEND_EQUATION_RGB 0x8009
#define GL_VERTEX_ATTRIB_ARRAY_ENABLED 0x8622
#define GL_VERTEX_ATTRIB_ARRAY_SIZE 0x8623
#define GL_VERTEX_ATTRIB_ARRAY_STRIDE 0x8624
#define GL_VERTEX_ATTRIB_ARRAY_TYPE 0x8625
#define GL_CURRENT_VERTEX_ATTRIB 0x8626
#define GL_VERTEX_PROGRAM_POINT_SIZE 0x8642
#define GL_VERTEX_ATTRIB_ARRAY_POINTER 0x8645
#define GL_STENCIL_BACK_FUNC 0x8800
#define GL_STENCIL_BACK_FAIL 0x8801
#define GL_STENCIL_BACK_PASS_DEPTH_FAIL 0x8802
#define GL_STENCIL_BACK_PASS_DEPTH_PASS 0x8803
#define GL_MAX_DRAW_BUFFERS 0x8824
#define GL_DRAW_BUFFER0 0x8825
#define GL_DRAW_BUFFER1 0x8826
#define GL_DRAW_BUFFER2 0x8827
#define GL_DRAW_BUFFER3 0x8828
#define GL_DRAW_BUFFER4 0x8829
#define GL_DRAW_BUFFER5 0x882A
#define GL_DRAW_BUFFER6 0x882B
#define GL_DRAW_BUFFER7 0x882C
#define GL_DRAW_BUFFER8 0x882D
#define GL_DRAW_BUFFER9 0x882E
#define GL_DRAW_BUFFER10 0x882F
#define GL_DRAW_BUFFER11 0x8830
#define GL_DRAW_BUFFER12 0x8831
#define GL_DRAW_BUFFER13 0x8832
#define GL_DRAW_BUFFER14 0x8833
#define GL_DRAW_BUFFER15 0x8834
#define GL_BLEND_EQUATION_ALPHA 0x883D
#define GL_MAX_VERTEX_ATTRIBS 0x8869
#define GL_VERTEX_ATTRIB_ARRAY_NORMALIZED 0x886A
#define GL_MAX_TEXTURE_IMAGE_UNITS 0x8872
#define GL_FRAGMENT_SHADER 0x8B30
#define GL_VERTEX_SHADER 0x8B31
#define GL_MAX_FRAGMENT_UNIFORM_COMPONENTS 0x8B49
#define GL_MAX_VERTEX_UNIFORM_COMPONENTS 0x8B4A
#define GL_MAX_VARYING_FLOATS 0x8B4B
#define GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS 0x8B4C
#define GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS 0x8B4D
#define GL_SHADER_TYPE 0x8B4F
#define GL_FLOAT_VEC2 0x8B50
#define GL_FLOAT_VEC3 0x8B51
#define GL_FLOAT_VEC4 0x8B52
#define GL_INT_VEC2 0x8B53
#define GL_INT_VEC3 0x8B54
#define GL_INT_VEC4 0x8B55
#define GL_int32 0x8B56
#define GL_int32_VEC2 0x8B57
#define GL_int32_VEC3 0x8B58
#define GL_int32_VEC4 0x8B59
#define GL_FLOAT_MAT2 0x8B5A
#define GL_FLOAT_MAT3 0x8B5B
#define GL_FLOAT_MAT4 0x8B5C
#define GL_SAMPLER_1D 0x8B5D
#define GL_SAMPLER_2D 0x8B5E
#define GL_SAMPLER_3D 0x8B5F
#define GL_SAMPLER_CUBE 0x8B60
#define GL_SAMPLER_1D_SHADOW 0x8B61
#define GL_SAMPLER_2D_SHADOW 0x8B62
#define GL_DELETE_STATUS 0x8B80
#define GL_COMPILE_STATUS 0x8B81
#define GL_LINK_STATUS 0x8B82
#define GL_VALIDATE_STATUS 0x8B83
#define GL_INFO_LOG_LENGTH 0x8B84
#define GL_ATTACHED_SHADERS 0x8B85
#define GL_ACTIVE_UNIFORMS 0x8B86
#define GL_ACTIVE_UNIFORM_MAX_LENGTH 0x8B87
#define GL_SHADER_SOURCE_LENGTH 0x8B88
#define GL_ACTIVE_ATTRIBUTES 0x8B89
#define GL_ACTIVE_ATTRIBUTE_MAX_LENGTH 0x8B8A
#define GL_FRAGMENT_SHADER_DERIVATIVE_HINT 0x8B8B
#define GL_SHADING_LANGUAGE_VERSION 0x8B8C
#define GL_CURRENT_PROGRAM 0x8B8D
#define GL_POINT_SPRITE_COORD_ORIGIN 0x8CA0
#define GL_LOWER_LEFT 0x8CA1
#define GL_UPPER_LEFT 0x8CA2
#define GL_STENCIL_BACK_REF 0x8CA3
#define GL_STENCIL_BACK_VALUE_MASK 0x8CA4
#define GL_STENCIL_BACK_WRITEMASK 0x8CA5
#define GL_PIXEL_PACK_BUFFER 0x88EB
#define GL_PIXEL_UNPACK_BUFFER 0x88EC
#define GL_PIXEL_PACK_BUFFER_BINDING 0x88ED
#define GL_PIXEL_UNPACK_BUFFER_BINDING 0x88EF
#define GL_FLOAT_MAT2x3 0x8B65
#define GL_FLOAT_MAT2x4 0x8B66
#define GL_FLOAT_MAT3x2 0x8B67
#define GL_FLOAT_MAT3x4 0x8B68
#define GL_FLOAT_MAT4x2 0x8B69
#define GL_FLOAT_MAT4x3 0x8B6A
#define GL_SRGB 0x8C40
#define GL_SRGB8 0x8C41
#define GL_SRGB_ALPHA 0x8C42
#define GL_SRGB8_ALPHA8 0x8C43
#define GL_COMPRESSED_SRGB 0x8C48
#define GL_COMPRESSED_SRGB_ALPHA 0x8C49
#define GL_VERTEX_ARRAY_BINDING 0x85B5
#define GL_RG 0x8227
#define GL_RG_INTEGER 0x8228
#define GL_R8 0x8229
#define GL_R16 0x822A
#define GL_RG8 0x822B
#define GL_RG16 0x822C
#define GL_R16F 0x822D
#define GL_R32F 0x822E
#define GL_RG16F 0x822F
#define GL_RG32F 0x8230
#define GL_R8I 0x8231
#define GL_R8UI 0x8232
#define GL_R16I 0x8233
#define GL_R16UI 0x8234
#define GL_R32I 0x8235
#define GL_R32UI 0x8236
#define GL_RG8I 0x8237
#define GL_RG8UI 0x8238
#define GL_RG16I 0x8239
#define GL_RG16UI 0x823A
#define GL_RG32I 0x823B
#define GL_RG32UI 0x823C
#define GL_COMPRESSED_RED_RGTC1 0x8DBB
#define GL_COMPRESSED_SIGNED_RED_RGTC1 0x8DBC
#define GL_COMPRESSED_RG_RGTC2 0x8DBD
#define GL_COMPRESSED_SIGNED_RG_RGTC2 0x8DBE
#define GL_MAP_READ_BIT 0x0001
#define GL_MAP_WRITE_BIT 0x0002
#define GL_MAP_INVALIDATE_RANGE_BIT 0x0004
#define GL_MAP_INVALIDATE_BUFFER_BIT 0x0008
#define GL_MAP_FLUSH_EXPLICIT_BIT 0x0010
#define GL_MAP_UNSYNCHRONIZED_BIT 0x0020
#define GL_HALF_FLOAT 0x140B
#define GL_FRAMEBUFFER_SRGB 0x8DB9
#define GL_INVALID_FRAMEBUFFER_OPERATION 0x0506
#define GL_FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING 0x8210
#define GL_FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE 0x8211
#define GL_FRAMEBUFFER_ATTACHMENT_RED_SIZE 0x8212
#define GL_FRAMEBUFFER_ATTACHMENT_GREEN_SIZE 0x8213
#define GL_FRAMEBUFFER_ATTACHMENT_BLUE_SIZE 0x8214
#define GL_FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE 0x8215
#define GL_FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE 0x8216
#define GL_FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE 0x8217
#define GL_FRAMEBUFFER_DEFAULT 0x8218
#define GL_FRAMEBUFFER_UNDEFINED 0x8219
#define GL_DEPTH_STENCIL_ATTACHMENT 0x821A
#define GL_INDEX 0x8222
#define GL_MAX_RENDERBUFFER_SIZE 0x84E8
#define GL_DEPTH_STENCIL 0x84F9
#define GL_UNSIGNED_INT_24_8 0x84FA
#define GL_DEPTH24_STENCIL8 0x88F0
#define GL_TEXTURE_STENCIL_SIZE 0x88F1
#define GL_TEXTURE_RED_TYPE 0x8C10
#define GL_TEXTURE_GREEN_TYPE 0x8C11
#define GL_TEXTURE_BLUE_TYPE 0x8C12
#define GL_TEXTURE_ALPHA_TYPE 0x8C13
#define GL_TEXTURE_DEPTH_TYPE 0x8C16
#define GL_UNSIGNED_NORMALIZED 0x8C17
#define GL_FRAMEBUFFER_BINDING 0x8CA6
#define GL_DRAW_FRAMEBUFFER_BINDING 0x8CA6
#define GL_RENDERBUFFER_BINDING 0x8CA7
#define GL_READ_FRAMEBUFFER 0x8CA8
#define GL_DRAW_FRAMEBUFFER 0x8CA9
#define GL_READ_FRAMEBUFFER_BINDING 0x8CAA
#define GL_RENDERBUFFER_SAMPLES 0x8CAB
#define GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE 0x8CD0
#define GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME 0x8CD1
#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL 0x8CD2
#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE 0x8CD3
#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER 0x8CD4
#define GL_FRAMEBUFFER_COMPLETE 0x8CD5
#define GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT 0x8CD6
#define GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT 0x8CD7
#define GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER 0x8CDB
#define GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER 0x8CDC
#define GL_FRAMEBUFFER_UNSUPPORTED 0x8CDD
#define GL_MAX_COLOR_ATTACHMENTS 0x8CDF
#define GL_COLOR_ATTACHMENT0 0x8CE0
#define GL_COLOR_ATTACHMENT1 0x8CE1
#define GL_COLOR_ATTACHMENT2 0x8CE2
#define GL_COLOR_ATTACHMENT3 0x8CE3
#define GL_COLOR_ATTACHMENT4 0x8CE4
#define GL_COLOR_ATTACHMENT5 0x8CE5
#define GL_COLOR_ATTACHMENT6 0x8CE6
#define GL_COLOR_ATTACHMENT7 0x8CE7
#define GL_COLOR_ATTACHMENT8 0x8CE8
#define GL_COLOR_ATTACHMENT9 0x8CE9
#define GL_COLOR_ATTACHMENT10 0x8CEA
#define GL_COLOR_ATTACHMENT11 0x8CEB
#define GL_COLOR_ATTACHMENT12 0x8CEC
#define GL_COLOR_ATTACHMENT13 0x8CED
#define GL_COLOR_ATTACHMENT14 0x8CEE
#define GL_COLOR_ATTACHMENT15 0x8CEF
#define GL_DEPTH_ATTACHMENT 0x8D00
#define GL_STENCIL_ATTACHMENT 0x8D20
#define GL_FRAMEBUFFER 0x8D40
#define GL_RENDERBUFFER 0x8D41
#define GL_RENDERBUFFER_WIDTH 0x8D42
#define GL_RENDERBUFFER_HEIGHT 0x8D43
#define GL_RENDERBUFFER_INTERNAL_FORMAT 0x8D44
#define GL_STENCIL_INDEX1 0x8D46
#define GL_STENCIL_INDEX4 0x8D47
#define GL_STENCIL_INDEX8 0x8D48
#define GL_STENCIL_INDEX16 0x8D49
#define GL_RENDERBUFFER_RED_SIZE 0x8D50
#define GL_RENDERBUFFER_GREEN_SIZE 0x8D51
#define GL_RENDERBUFFER_BLUE_SIZE 0x8D52
#define GL_RENDERBUFFER_ALPHA_SIZE 0x8D53
#define GL_RENDERBUFFER_DEPTH_SIZE 0x8D54
#define GL_RENDERBUFFER_STENCIL_SIZE 0x8D55
#define GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE 0x8D56
#define GL_MAX_SAMPLES 0x8D57
#define GL_TEXTURE_LUMINANCE_TYPE 0x8C14
#define GL_TEXTURE_INTENSITY_TYPE 0x8C15
#define GL_DEPTH_COMPONENT32F 0x8CAC
#define GL_DEPTH32F_STENCIL8 0x8CAD
#define GL_FLOAT_32_UNSIGNED_INT_24_8_REV 0x8DAD
#define GL_COMPARE_REF_TO_TEXTURE 0x884E
#define GL_CLIP_DISTANCE0 0x3000
#define GL_CLIP_DISTANCE1 0x3001
#define GL_CLIP_DISTANCE2 0x3002
#define GL_CLIP_DISTANCE3 0x3003
#define GL_CLIP_DISTANCE4 0x3004
#define GL_CLIP_DISTANCE5 0x3005
#define GL_CLIP_DISTANCE6 0x3006
#define GL_CLIP_DISTANCE7 0x3007
#define GL_MAX_CLIP_DISTANCES 0x0D32
#define GL_MAJOR_VERSION 0x821B
#define GL_MINOR_VERSION 0x821C
#define GL_NUM_EXTENSIONS 0x821D
#define GL_CONTEXT_FLAGS 0x821E
#define GL_COMPRESSED_RED 0x8225
#define GL_COMPRESSED_RG 0x8226
#define GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT 0x00000001
#define GL_RGBA32F 0x8814
#define GL_RGB32F 0x8815
#define GL_RGBA16F 0x881A
#define GL_RGB16F 0x881B
#define GL_VERTEX_ATTRIB_ARRAY_INTEGER 0x88FD
#define GL_MAX_ARRAY_TEXTURE_LAYERS 0x88FF
#define GL_MIN_PROGRAM_TEXEL_OFFSET 0x8904
#define GL_MAX_PROGRAM_TEXEL_OFFSET 0x8905
#define GL_CLAMP_READ_COLOR 0x891C
#define GL_FIXED_ONLY 0x891D
#define GL_MAX_VARYING_COMPONENTS 0x8B4B
#define GL_TEXTURE_1D_ARRAY 0x8C18
#define GL_PROXY_TEXTURE_1D_ARRAY 0x8C19
#define GL_TEXTURE_2D_ARRAY 0x8C1A
#define GL_PROXY_TEXTURE_2D_ARRAY 0x8C1B
#define GL_TEXTURE_BINDING_1D_ARRAY 0x8C1C
#define GL_TEXTURE_BINDING_2D_ARRAY 0x8C1D
#define GL_R11F_G11F_B10F 0x8C3A
#define GL_UNSIGNED_INT_10F_11F_11F_REV 0x8C3B
#define GL_RGB9_E5 0x8C3D
#define GL_UNSIGNED_INT_5_9_9_9_REV 0x8C3E
#define GL_TEXTURE_SHARED_SIZE 0x8C3F
#define GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH 0x8C76
#define GL_TRANSFORM_FEEDBACK_BUFFER_MODE 0x8C7F
#define GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS 0x8C80
#define GL_TRANSFORM_FEEDBACK_VARYINGS 0x8C83
#define GL_TRANSFORM_FEEDBACK_BUFFER_START 0x8C84
#define GL_TRANSFORM_FEEDBACK_BUFFER_SIZE 0x8C85
#define GL_PRIMITIVES_GENERATED 0x8C87
#define GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN 0x8C88
#define GL_RASTERIZER_DISCARD 0x8C89
#define GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS 0x8C8A
#define GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS 0x8C8B
#define GL_INTERLEAVED_ATTRIBS 0x8C8C
#define GL_SEPARATE_ATTRIBS 0x8C8D
#define GL_TRANSFORM_FEEDBACK_BUFFER 0x8C8E
#define GL_TRANSFORM_FEEDBACK_BUFFER_BINDING 0x8C8F
#define GL_RGBA32UI 0x8D70
#define GL_RGB32UI 0x8D71
#define GL_RGBA16UI 0x8D76
#define GL_RGB16UI 0x8D77
#define GL_RGBA8UI 0x8D7C
#define GL_RGB8UI 0x8D7D
#define GL_RGBA32I 0x8D82
#define GL_RGB32I 0x8D83
#define GL_RGBA16I 0x8D88
#define GL_RGB16I 0x8D89
#define GL_RGBA8I 0x8D8E
#define GL_RGB8I 0x8D8F
#define GL_RED_INTEGER 0x8D94
#define GL_GREEN_INTEGER 0x8D95
#define GL_BLUE_INTEGER 0x8D96
#define GL_RGB_INTEGER 0x8D98
#define GL_RGBA_INTEGER 0x8D99
#define GL_BGR_INTEGER 0x8D9A
#define GL_BGRA_INTEGER 0x8D9B
#define GL_SAMPLER_1D_ARRAY 0x8DC0
#define GL_SAMPLER_2D_ARRAY 0x8DC1
#define GL_SAMPLER_1D_ARRAY_SHADOW 0x8DC3
#define GL_SAMPLER_2D_ARRAY_SHADOW 0x8DC4
#define GL_SAMPLER_CUBE_SHADOW 0x8DC5
#define GL_UNSIGNED_INT_VEC2 0x8DC6
#define GL_UNSIGNED_INT_VEC3 0x8DC7
#define GL_UNSIGNED_INT_VEC4 0x8DC8
#define GL_INT_SAMPLER_1D 0x8DC9
#define GL_INT_SAMPLER_2D 0x8DCA
#define GL_INT_SAMPLER_3D 0x8DCB
#define GL_INT_SAMPLER_CUBE 0x8DCC
#define GL_INT_SAMPLER_1D_ARRAY 0x8DCE
#define GL_INT_SAMPLER_2D_ARRAY 0x8DCF
#define GL_UNSIGNED_INT_SAMPLER_1D 0x8DD1
#define GL_UNSIGNED_INT_SAMPLER_2D 0x8DD2
#define GL_UNSIGNED_INT_SAMPLER_3D 0x8DD3
#define GL_UNSIGNED_INT_SAMPLER_CUBE 0x8DD4
#define GL_UNSIGNED_INT_SAMPLER_1D_ARRAY 0x8DD6
#define GL_UNSIGNED_INT_SAMPLER_2D_ARRAY 0x8DD7
#define GL_QUERY_WAIT 0x8E13
#define GL_QUERY_NO_WAIT 0x8E14
#define GL_QUERY_BY_REGION_WAIT 0x8E15
#define GL_QUERY_BY_REGION_NO_WAIT 0x8E16
#define GL_BUFFER_ACCESS_FLAGS 0x911F
#define GL_BUFFER_MAP_LENGTH 0x9120
#define GL_BUFFER_MAP_OFFSET 0x9121
#define GL_UNIFORM_BUFFER 0x8A11
#define GL_UNIFORM_BUFFER_BINDING 0x8A28
#define GL_UNIFORM_BUFFER_START 0x8A29
#define GL_UNIFORM_BUFFER_SIZE 0x8A2A
#define GL_MAX_VERTEX_UNIFORM_BLOCKS 0x8A2B
#define GL_MAX_FRAGMENT_UNIFORM_BLOCKS 0x8A2D
#define GL_MAX_COMBINED_UNIFORM_BLOCKS 0x8A2E
#define GL_MAX_UNIFORM_BUFFER_BINDINGS 0x8A2F
#define GL_MAX_UNIFORM_BLOCK_SIZE 0x8A30
#define GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS 0x8A31
#define GL_MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS 0x8A33
#define GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT 0x8A34
#define GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH 0x8A35
#define GL_ACTIVE_UNIFORM_BLOCKS 0x8A36
#define GL_UNIFORM_TYPE 0x8A37
#define GL_UNIFORM_SIZE 0x8A38
#define GL_UNIFORM_NAME_LENGTH 0x8A39
#define GL_UNIFORM_BLOCK_INDEX 0x8A3A
#define GL_UNIFORM_OFFSET 0x8A3B
#define GL_UNIFORM_ARRAY_STRIDE 0x8A3C
#define GL_UNIFORM_MATRIX_STRIDE 0x8A3D
#define GL_UNIFORM_IS_ROW_MAJOR 0x8A3E
#define GL_UNIFORM_BLOCK_BINDING 0x8A3F
#define GL_UNIFORM_BLOCK_DATA_SIZE 0x8A40
#define GL_UNIFORM_BLOCK_NAME_LENGTH 0x8A41
#define GL_UNIFORM_BLOCK_ACTIVE_UNIFORMS 0x8A42
#define GL_UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES 0x8A43
#define GL_UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER 0x8A44
#define GL_UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER 0x8A46
#define GL_INVALID_INDEX 0xFFFFFFFF
#define GL_MAX_GEOMETRY_UNIFORM_BLOCKS 0x8A2C
#define GL_MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS 0x8A32
#define GL_UNIFORM_BLOCK_REFERENCED_BY_GEOMETRY_SHADER 0x8A45
#define GL_COPY_READ_BUFFER 0x8F36
#define GL_COPY_WRITE_BUFFER 0x8F37
#define GL_COPY_READ_BUFFER_BINDING 0x8F36
#define GL_COPY_WRITE_BUFFER_BINDING 0x8F37
#define GL_SAMPLER_2D_RECT 0x8B63
#define GL_SAMPLER_2D_RECT_SHADOW 0x8B64
#define GL_SAMPLER_BUFFER 0x8DC2
#define GL_INT_SAMPLER_2D_RECT 0x8DCD
#define GL_INT_SAMPLER_BUFFER 0x8DD0
#define GL_UNSIGNED_INT_SAMPLER_2D_RECT 0x8DD5
#define GL_UNSIGNED_INT_SAMPLER_BUFFER 0x8DD8
#define GL_TEXTURE_BUFFER 0x8C2A
#define GL_MAX_TEXTURE_BUFFER_SIZE 0x8C2B
#define GL_TEXTURE_BINDING_BUFFER 0x8C2C
#define GL_TEXTURE_BUFFER_DATA_STORE_BINDING 0x8C2D
#define GL_TEXTURE_RECTANGLE 0x84F5
#define GL_TEXTURE_BINDING_RECTANGLE 0x84F6
#define GL_PROXY_TEXTURE_RECTANGLE 0x84F7
#define GL_MAX_RECTANGLE_TEXTURE_SIZE 0x84F8
#define GL_RED_SNORM 0x8F90
#define GL_RG_SNORM 0x8F91
#define GL_RGB_SNORM 0x8F92
#define GL_RGBA_SNORM 0x8F93
#define GL_R8_SNORM 0x8F94
#define GL_RG8_SNORM 0x8F95
#define GL_RGB8_SNORM 0x8F96
#define GL_RGBA8_SNORM 0x8F97
#define GL_R16_SNORM 0x8F98
#define GL_RG16_SNORM 0x8F99
#define GL_RGB16_SNORM 0x8F9A
#define GL_RGBA16_SNORM 0x8F9B
#define GL_SIGNED_NORMALIZED 0x8F9C
#define GL_PRIMITIVE_RESTART 0x8F9D
#define GL_PRIMITIVE_RESTART_INDEX 0x8F9E
#define GL_DEPTH_CLAMP 0x864F
#define GL_QUADS_FOLLOW_PROVOKING_VERTEX_CONVENTION 0x8E4C
#define GL_FIRST_VERTEX_CONVENTION 0x8E4D
#define GL_LAST_VERTEX_CONVENTION 0x8E4E
#define GL_PROVOKING_VERTEX 0x8E4F
#define GL_TEXTURE_CUBE_MAP_SEAMLESS 0x884F
#define GL_MAX_SERVER_WAIT_TIMEOUT 0x9111
#define GL_OBJECT_TYPE 0x9112
#define GL_SYNC_CONDITION 0x9113
#define GL_SYNC_STATUS 0x9114
#define GL_SYNC_FLAGS 0x9115
#define GL_SYNC_FENCE 0x9116
#define GL_SYNC_GPU_COMMANDS_COMPLETE 0x9117
#define GL_UNSIGNALED 0x9118
#define GL_SIGNALED 0x9119
#define GL_ALREADY_SIGNALED 0x911A
#define GL_TIMEOUT_EXPIRED 0x911B
#define GL_CONDITION_SATISFIED 0x911C
#define GL_WAIT_FAILED 0x911D
#define GL_TIMEOUT_IGNORED 0xFFFFFFFFFFFFFFFF
#define GL_SYNC_FLUSH_COMMANDS_BIT 0x00000001
#define GL_SAMPLE_POSITION 0x8E50
#define GL_SAMPLE_MASK 0x8E51
#define GL_SAMPLE_MASK_VALUE 0x8E52
#define GL_MAX_SAMPLE_MASK_WORDS 0x8E59
#define GL_TEXTURE_2D_MULTISAMPLE 0x9100
#define GL_PROXY_TEXTURE_2D_MULTISAMPLE 0x9101
#define GL_TEXTURE_2D_MULTISAMPLE_ARRAY 0x9102
#define GL_PROXY_TEXTURE_2D_MULTISAMPLE_ARRAY 0x9103
#define GL_TEXTURE_BINDING_2D_MULTISAMPLE 0x9104
#define GL_TEXTURE_BINDING_2D_MULTISAMPLE_ARRAY 0x9105
#define GL_TEXTURE_SAMPLES 0x9106
#define GL_TEXTURE_FIXED_SAMPLE_LOCATIONS 0x9107
#define GL_SAMPLER_2D_MULTISAMPLE 0x9108
#define GL_INT_SAMPLER_2D_MULTISAMPLE 0x9109
#define GL_UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE 0x910A
#define GL_SAMPLER_2D_MULTISAMPLE_ARRAY 0x910B
#define GL_INT_SAMPLER_2D_MULTISAMPLE_ARRAY 0x910C
#define GL_UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY 0x910D
#define GL_MAX_COLOR_TEXTURE_SAMPLES 0x910E
#define GL_MAX_DEPTH_TEXTURE_SAMPLES 0x910F
#define GL_MAX_INTEGER_SAMPLES 0x9110
#define GL_BGRA 0x80E1
#define GL_CONTEXT_CORE_PROFILE_BIT 0x00000001
#define GL_CONTEXT_COMPATIBILITY_PROFILE_BIT 0x00000002
#define GL_LINES_ADJACENCY 0x000A
#define GL_LINE_STRIP_ADJACENCY 0x000B
#define GL_TRIANGLES_ADJACENCY 0x000C
#define GL_TRIANGLE_STRIP_ADJACENCY 0x000D
#define GL_PROGRAM_POINT_SIZE 0x8642
#define GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS 0x8C29
#define GL_FRAMEBUFFER_ATTACHMENT_LAYERED 0x8DA7
#define GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS 0x8DA8
#define GL_GEOMETRY_SHADER 0x8DD9
#define GL_GEOMETRY_VERTICES_OUT 0x8916
#define GL_GEOMETRY_INPUT_TYPE 0x8917
#define GL_GEOMETRY_OUTPUT_TYPE 0x8918
#define GL_MAX_GEOMETRY_UNIFORM_COMPONENTS 0x8DDF
#define GL_MAX_GEOMETRY_OUTPUT_VERTICES 0x8DE0
#define GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS 0x8DE1
#define GL_MAX_VERTEX_OUTPUT_COMPONENTS 0x9122
#define GL_MAX_GEOMETRY_INPUT_COMPONENTS 0x9123
#define GL_MAX_GEOMETRY_OUTPUT_COMPONENTS 0x9124
#define GL_MAX_FRAGMENT_INPUT_COMPONENTS 0x9125
#define GL_CONTEXT_PROFILE_MASK 0x9126
#define GL_RGB10_A2UI 0x906F
#define GL_TEXTURE_SWIZZLE_R 0x8E42
#define GL_TEXTURE_SWIZZLE_G 0x8E43
#define GL_TEXTURE_SWIZZLE_B 0x8E44
#define GL_TEXTURE_SWIZZLE_A 0x8E45
#define GL_TEXTURE_SWIZZLE_RGBA 0x8E46
#define GL_TIME_ELAPSED 0x88BF
#define GL_TIMESTAMP 0x8E28
#define GL_UNSIGNED_INT_2_10_10_10_REV 0x8368
#define GL_INT_2_10_10_10_REV 0x8D9F
#define GL_SRC1_ALPHA 0x8589
#define GL_SRC1_COLOR 0x88F9
#define GL_ONE_MINUS_SRC1_COLOR 0x88FA
#define GL_ONE_MINUS_SRC1_ALPHA 0x88FB
#define GL_MAX_DUAL_SOURCE_DRAW_BUFFERS 0x88FC
#define GL_ANY_SAMPLES_PASSED 0x8C2F
#define GL_VERTEX_ATTRIB_ARRAY_DIVISOR 0x88FE