#include "Prelude.hpp"

static void LoadFunc(const char* entryPoint, void** func)
{
	void* procAddress = SDL_GL_GetProcAddress(entryPoint);

	// Stupid, but that's how it works... see also https://www.opengl.org/wiki/Load_OpenGL_Functions
	if (procAddress == reinterpret_cast<void*>(-1) ||
		procAddress == nullptr ||
		procAddress == reinterpret_cast<void*>(1) ||
		procAddress == reinterpret_cast<void*>(2) ||
		procAddress == reinterpret_cast<void*>(3))
	{
		PG_DIE("OpenGL 3.3 entry point '%s' is not supported by your graphics card.", entryPoint);
	}

	*func = procAddress;
}

void BindingsGL3::Initialize()
{
	LoadFunc("glActiveTexture", reinterpret_cast<void**>(&ActiveTexture));
	LoadFunc("glAttachShader", reinterpret_cast<void**>(&AttachShader));
	LoadFunc("glBindBuffer", reinterpret_cast<void**>(&BindBuffer));
	LoadFunc("glBindBufferBase", reinterpret_cast<void**>(&BindBufferBase));
	LoadFunc("glBindFramebuffer", reinterpret_cast<void**>(&BindFramebuffer));
	LoadFunc("glBindSampler", reinterpret_cast<void**>(&BindSampler));
	LoadFunc("glBindTexture", reinterpret_cast<void**>(&BindTexture));
	LoadFunc("glBindVertexArray", reinterpret_cast<void**>(&BindVertexArray));
	LoadFunc("glBlendEquationSeparate", reinterpret_cast<void**>(&BlendEquationSeparate));
	LoadFunc("glBlendFuncSeparate", reinterpret_cast<void**>(&BlendFuncSeparate));
	LoadFunc("glBufferData", reinterpret_cast<void**>(&BufferData));
	LoadFunc("glBufferSubData", reinterpret_cast<void**>(&BufferSubData));
	LoadFunc("glCheckFramebufferStatus", reinterpret_cast<void**>(&CheckFramebufferStatus));
	LoadFunc("glClear", reinterpret_cast<void**>(&Clear));
	LoadFunc("glClearColor", reinterpret_cast<void**>(&ClearColor));
	LoadFunc("glClearDepth", reinterpret_cast<void**>(&ClearDepth));
	LoadFunc("glClearStencil", reinterpret_cast<void**>(&ClearStencil));
	LoadFunc("glClientWaitSync", reinterpret_cast<void**>(&ClientWaitSync));
	LoadFunc("glColorMask", reinterpret_cast<void**>(&ColorMask));
	LoadFunc("glCompileShader", reinterpret_cast<void**>(&CompileShader));
	LoadFunc("glCompressedTexImage2D", reinterpret_cast<void**>(&CompressedTexImage2D));
	LoadFunc("glCreateProgram", reinterpret_cast<void**>(&CreateProgram));
	LoadFunc("glCreateShader", reinterpret_cast<void**>(&CreateShader));
	LoadFunc("glCullFace", reinterpret_cast<void**>(&CullFace));
	LoadFunc("glDeleteBuffers", reinterpret_cast<void**>(&DeleteBuffers));
	LoadFunc("glDeleteFramebuffers", reinterpret_cast<void**>(&DeleteFramebuffers));
	LoadFunc("glDeleteProgram", reinterpret_cast<void**>(&DeleteProgram));
	LoadFunc("glDeleteQueries", reinterpret_cast<void**>(&DeleteQueries));
	LoadFunc("glDeleteSamplers", reinterpret_cast<void**>(&DeleteSamplers));
	LoadFunc("glDeleteShader", reinterpret_cast<void**>(&DeleteShader));
	LoadFunc("glDeleteSync", reinterpret_cast<void**>(&DeleteSync));
	LoadFunc("glDeleteTextures", reinterpret_cast<void**>(&DeleteTextures));
	LoadFunc("glDeleteVertexArrays", reinterpret_cast<void**>(&DeleteVertexArrays));
	LoadFunc("glDepthFunc", reinterpret_cast<void**>(&DepthFunc));
	LoadFunc("glDepthMask", reinterpret_cast<void**>(&DepthMask));
	LoadFunc("glDetachShader", reinterpret_cast<void**>(&DetachShader));
	LoadFunc("glDisable", reinterpret_cast<void**>(&Disable));
	LoadFunc("glDrawArrays", reinterpret_cast<void**>(&DrawArrays));
	LoadFunc("glDrawArraysInstancedBaseInstance", reinterpret_cast<void**>(&DrawArraysInstancedBaseInstance));
	LoadFunc("glDrawBuffers", reinterpret_cast<void**>(&DrawBuffers));
	LoadFunc("glDrawElementsBaseVertex", reinterpret_cast<void**>(&DrawElementsBaseVertex));
	LoadFunc("glDrawElementsInstancedBaseVertexBaseInstance", reinterpret_cast<void**>(&DrawElementsInstancedBaseVertexBaseInstance));
	LoadFunc("glEnable", reinterpret_cast<void**>(&Enable));
	LoadFunc("glEnableVertexAttribArray", reinterpret_cast<void**>(&EnableVertexAttribArray));
	LoadFunc("glFenceSync", reinterpret_cast<void**>(&FenceSync));
	LoadFunc("glFramebufferTexture2D", reinterpret_cast<void**>(&FramebufferTexture2D));
	LoadFunc("glFrontFace", reinterpret_cast<void**>(&FrontFace));
	LoadFunc("glGenBuffers", reinterpret_cast<void**>(&GenBuffers));
	LoadFunc("glGenerateMipmap", reinterpret_cast<void**>(&GenerateMipmap));
	LoadFunc("glGenFramebuffers", reinterpret_cast<void**>(&GenFramebuffers));
	LoadFunc("glGenQueries", reinterpret_cast<void**>(&GenQueries));
	LoadFunc("glGenSamplers", reinterpret_cast<void**>(&GenSamplers));
	LoadFunc("glGenTextures", reinterpret_cast<void**>(&GenTextures));
	LoadFunc("glGenVertexArrays", reinterpret_cast<void**>(&GenVertexArrays));
	LoadFunc("glGetError", reinterpret_cast<void**>(&GetError));
	LoadFunc("glGetIntegerv", reinterpret_cast<void**>(&GetIntegerv));
	LoadFunc("glGetProgramInfoLog", reinterpret_cast<void**>(&GetProgramInfoLog));
	LoadFunc("glGetProgramiv", reinterpret_cast<void**>(&GetProgramiv));
	LoadFunc("glGetQueryObjectiv", reinterpret_cast<void**>(&GetQueryObjectiv));
	LoadFunc("glGetQueryObjectui64v", reinterpret_cast<void**>(&GetQueryObjectui64v));
	LoadFunc("glGetShaderInfoLog", reinterpret_cast<void**>(&GetShaderInfoLog));
	LoadFunc("glGetShaderiv", reinterpret_cast<void**>(&GetShaderiv));
	LoadFunc("glGetString", reinterpret_cast<void**>(&GetString));
	LoadFunc("glLinkProgram", reinterpret_cast<void**>(&LinkProgram));
	LoadFunc("glMapBuffer", reinterpret_cast<void**>(&MapBuffer));
	LoadFunc("glMapBufferRange", reinterpret_cast<void**>(&MapBufferRange));
	LoadFunc("glPolygonMode", reinterpret_cast<void**>(&PolygonMode));
	LoadFunc("glPolygonOffset", reinterpret_cast<void**>(&PolygonOffset));
	LoadFunc("glQueryCounter", reinterpret_cast<void**>(&QueryCounter));
	LoadFunc("glSamplerParameterf", reinterpret_cast<void**>(&SamplerParameterf));
	LoadFunc("glSamplerParameterfv", reinterpret_cast<void**>(&SamplerParameterfv));
	LoadFunc("glSamplerParameteri", reinterpret_cast<void**>(&SamplerParameteri));
	LoadFunc("glScissor", reinterpret_cast<void**>(&Scissor));
	LoadFunc("glShaderSource", reinterpret_cast<void**>(&ShaderSource));
	LoadFunc("glStencilFuncSeparate", reinterpret_cast<void**>(&StencilFuncSeparate));
	LoadFunc("glStencilOpSeparate", reinterpret_cast<void**>(&StencilOpSeparate));
	LoadFunc("glTexImage2D", reinterpret_cast<void**>(&TexImage2D));
	LoadFunc("glUnmapBuffer", reinterpret_cast<void**>(&UnmapBuffer));
	LoadFunc("glUseProgram", reinterpret_cast<void**>(&UseProgram));
	LoadFunc("glVertexAttribDivisor", reinterpret_cast<void**>(&VertexAttribDivisor));
	LoadFunc("glVertexAttribPointer", reinterpret_cast<void**>(&VertexAttribPointer));
	LoadFunc("glViewport", reinterpret_cast<void**>(&Viewport));
}