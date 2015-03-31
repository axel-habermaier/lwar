

	using glActiveTextureFunc = void(*)(uint32);
	using glAttachShaderFunc = void(*)(uint32, uint32);
	using glBindBufferFunc = void(*)(uint32, uint32);
	using glBindBufferBaseFunc = void(*)(uint32, uint32, uint32);
	using glBindFramebufferFunc = void(*)(uint32, uint32);
	using glBindSamplerFunc = void(*)(uint32, uint32);
	using glBindTextureFunc = void(*)(uint32, uint32);
	using glBindVertexArrayFunc = void(*)(uint32);
	using glBlendEquationSeparateFunc = void(*)(uint32, uint32);
	using glBlendFuncSeparateFunc = void(*)(uint32, uint32, uint32, uint32);
	using glBufferDataFunc = void(*)(uint32, void*, void*, uint32);
	using glBufferSubDataFunc = void(*)(uint32, void*, void*, void*);
	using glCheckFramebufferStatusFunc = uint32(*)(uint32);
	using glClearFunc = void(*)(uint32);
	using glClearColorFunc = void(*)(float32, float32, float32, float32);
	using glClearDepthFunc = void(*)(float64);
	using glClearStencilFunc = void(*)(int32);
	using glClientWaitSyncFunc = uint32(*)(void*, uint32, uint64);
	using glColorMaskFunc = void(*)(bool, bool, bool, bool);
	using glCompileShaderFunc = void(*)(uint32);
	using glCompressedTexImage2DFunc = void(*)(uint32, int32, uint32, int32, int32, int32, int32, void*);
	using glCreateProgramFunc = uint32(*)();
	using glCreateShaderFunc = uint32(*)(uint32);
	using glCullFaceFunc = void(*)(uint32);
	using glDeleteBuffersFunc = void(*)(int32, uint32*);
	using glDeleteFramebuffersFunc = void(*)(int32, uint32*);
	using glDeleteProgramFunc = void(*)(uint32);
	using glDeleteQueriesFunc = void(*)(int32, uint32*);
	using glDeleteSamplersFunc = void(*)(int32, uint32*);
	using glDeleteShaderFunc = void(*)(uint32);
	using glDeleteSyncFunc = void(*)(void*);
	using glDeleteTexturesFunc = void(*)(int32, uint32*);
	using glDeleteVertexArraysFunc = void(*)(int32, uint32*);
	using glDepthFuncFunc = void(*)(uint32);
	using glDepthMaskFunc = void(*)(bool);
	using glDetachShaderFunc = void(*)(uint32, uint32);
	using glDisableFunc = void(*)(uint32);
	using glDrawArraysFunc = void(*)(uint32, int32, int32);
	using glDrawArraysInstancedBaseInstanceFunc = void(*)(uint32, int32, int32, int32, uint32);
	using glDrawBuffersFunc = void(*)(int32, uint32*);
	using glDrawElementsBaseVertexFunc = void(*)(uint32, int32, uint32, void*, int32);
	using glDrawElementsInstancedBaseVertexBaseInstanceFunc = void(*)(uint32, int32, uint32, void*, int32, int32, uint32);
	using glEnableFunc = void(*)(uint32);
	using glEnableVertexAttribArrayFunc = void(*)(uint32);
	using glFenceSyncFunc = void*(*)(uint32, uint32);
	using glFramebufferTexture2DFunc = void(*)(uint32, uint32, uint32, uint32, int32);
	using glFrontFaceFunc = void(*)(uint32);
	using glGenBuffersFunc = void(*)(int32, uint32*);
	using glGenerateMipmapFunc = void(*)(uint32);
	using glGenFramebuffersFunc = void(*)(int32, uint32*);
	using glGenQueriesFunc = void(*)(int32, uint32*);
	using glGenSamplersFunc = void(*)(int32, uint32*);
	using glGenTexturesFunc = void(*)(int32, uint32*);
	using glGenVertexArraysFunc = void(*)(int32, uint32*);
	using glGetErrorFunc = uint32(*)();
	using glGetIntegervFunc = void(*)(uint32, int32*);
	using glGetProgramInfoLogFunc = void(*)(uint32, int32, int32*, uint8*);
	using glGetProgramivFunc = void(*)(uint32, uint32, int32*);
	using glGetQueryObjectivFunc = void(*)(uint32, uint32, int32*);
	using glGetQueryObjectui64vFunc = void(*)(uint32, uint32, uint64*);
	using glGetShaderInfoLogFunc = void(*)(uint32, int32, int32*, uint8*);
	using glGetShaderivFunc = void(*)(uint32, uint32, int32*);
	using glGetStringFunc = uint8*(*)(uint32);
	using glLinkProgramFunc = void(*)(uint32);
	using glMapBufferFunc = void*(*)(uint32, uint32);
	using glMapBufferRangeFunc = void*(*)(uint32, void*, void*, uint32);
	using glPolygonModeFunc = void(*)(uint32, uint32);
	using glPolygonOffsetFunc = void(*)(float32, float32);
	using glQueryCounterFunc = void(*)(uint32, uint32);
	using glSamplerParameterfFunc = void(*)(uint32, uint32, float32);
	using glSamplerParameterfvFunc = void(*)(uint32, uint32, float32*);
	using glSamplerParameteriFunc = void(*)(uint32, uint32, int32);
	using glScissorFunc = void(*)(int32, int32, int32, int32);
	using glShaderSourceFunc = void(*)(uint32, int32, uint8**, int32*);
	using glStencilFuncSeparateFunc = void(*)(uint32, uint32, int32, uint32);
	using glStencilOpSeparateFunc = void(*)(uint32, uint32, uint32, uint32);
	using glTexImage2DFunc = void(*)(uint32, int32, int32, int32, int32, int32, uint32, uint32, void*);
	using glUnmapBufferFunc = bool(*)(uint32);
	using glUseProgramFunc = void(*)(uint32);
	using glVertexAttribDivisorFunc = void(*)(uint32, uint32);
	using glVertexAttribPointerFunc = void(*)(uint32, int32, uint32, bool, int32, void*);
	using glViewportFunc = void(*)(int32, int32, int32, int32);

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

#define PG_DEL \
		void glActiveTexture(uint32 arg1) { _gl.ActiveTexture(arg1); PG_GL_CHECK_ERRORS(); } \
			void glAttachShader(uint32 arg1, uint32 arg2) { _gl.AttachShader(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glBindBuffer(uint32 arg1, uint32 arg2) { _gl.BindBuffer(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glBindBufferBase(uint32 arg1, uint32 arg2, uint32 arg3) { _gl.BindBufferBase(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glBindFramebuffer(uint32 arg1, uint32 arg2) { _gl.BindFramebuffer(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glBindSampler(uint32 arg1, uint32 arg2) { _gl.BindSampler(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glBindTexture(uint32 arg1, uint32 arg2) { _gl.BindTexture(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glBindVertexArray(uint32 arg1) { _gl.BindVertexArray(arg1); PG_GL_CHECK_ERRORS(); } \
			void glBlendEquationSeparate(uint32 arg1, uint32 arg2) { _gl.BlendEquationSeparate(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glBlendFuncSeparate(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4) { _gl.BlendFuncSeparate(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glBufferData(uint32 arg1, void* arg2, void* arg3, uint32 arg4) { _gl.BufferData(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glBufferSubData(uint32 arg1, void* arg2, void* arg3, void* arg4) { _gl.BufferSubData(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			uint32 glCheckFramebufferStatus(uint32 arg1) { auto result = _gl.CheckFramebufferStatus(arg1); PG_GL_CHECK_ERRORS(); return result; } \
			void glClear(uint32 arg1) { _gl.Clear(arg1); PG_GL_CHECK_ERRORS(); } \
			void glClearColor(float32 arg1, float32 arg2, float32 arg3, float32 arg4) { _gl.ClearColor(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glClearDepth(float64 arg1) { _gl.ClearDepth(arg1); PG_GL_CHECK_ERRORS(); } \
			void glClearStencil(int32 arg1) { _gl.ClearStencil(arg1); PG_GL_CHECK_ERRORS(); } \
			uint32 glClientWaitSync(void* arg1, uint32 arg2, uint64 arg3) { auto result = _gl.ClientWaitSync(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); return result; } \
			void glColorMask(bool arg1, bool arg2, bool arg3, bool arg4) { _gl.ColorMask(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glCompileShader(uint32 arg1) { _gl.CompileShader(arg1); PG_GL_CHECK_ERRORS(); } \
			void glCompressedTexImage2D(uint32 arg1, int32 arg2, uint32 arg3, int32 arg4, int32 arg5, int32 arg6, int32 arg7, void* arg8) { _gl.CompressedTexImage2D(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); PG_GL_CHECK_ERRORS(); } \
			uint32 glCreateProgram() { auto result = _gl.CreateProgram(); PG_GL_CHECK_ERRORS(); return result; } \
			uint32 glCreateShader(uint32 arg1) { auto result = _gl.CreateShader(arg1); PG_GL_CHECK_ERRORS(); return result; } \
			void glCullFace(uint32 arg1) { _gl.CullFace(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDeleteBuffers(int32 arg1, uint32* arg2) { _gl.DeleteBuffers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDeleteFramebuffers(int32 arg1, uint32* arg2) { _gl.DeleteFramebuffers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDeleteProgram(uint32 arg1) { _gl.DeleteProgram(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDeleteQueries(int32 arg1, uint32* arg2) { _gl.DeleteQueries(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDeleteSamplers(int32 arg1, uint32* arg2) { _gl.DeleteSamplers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDeleteShader(uint32 arg1) { _gl.DeleteShader(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDeleteSync(void* arg1) { _gl.DeleteSync(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDeleteTextures(int32 arg1, uint32* arg2) { _gl.DeleteTextures(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDeleteVertexArrays(int32 arg1, uint32* arg2) { _gl.DeleteVertexArrays(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDepthFunc(uint32 arg1) { _gl.DepthFunc(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDepthMask(bool arg1) { _gl.DepthMask(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDetachShader(uint32 arg1, uint32 arg2) { _gl.DetachShader(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDisable(uint32 arg1) { _gl.Disable(arg1); PG_GL_CHECK_ERRORS(); } \
			void glDrawArrays(uint32 arg1, int32 arg2, int32 arg3) { _gl.DrawArrays(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glDrawArraysInstancedBaseInstance(uint32 arg1, int32 arg2, int32 arg3, int32 arg4, uint32 arg5) { _gl.DrawArraysInstancedBaseInstance(arg1, arg2, arg3, arg4, arg5); PG_GL_CHECK_ERRORS(); } \
			void glDrawBuffers(int32 arg1, uint32* arg2) { _gl.DrawBuffers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glDrawElementsBaseVertex(uint32 arg1, int32 arg2, uint32 arg3, void* arg4, int32 arg5) { _gl.DrawElementsBaseVertex(arg1, arg2, arg3, arg4, arg5); PG_GL_CHECK_ERRORS(); } \
			void glDrawElementsInstancedBaseVertexBaseInstance(uint32 arg1, int32 arg2, uint32 arg3, void* arg4, int32 arg5, int32 arg6, uint32 arg7) { _gl.DrawElementsInstancedBaseVertexBaseInstance(arg1, arg2, arg3, arg4, arg5, arg6, arg7); PG_GL_CHECK_ERRORS(); } \
			void glEnable(uint32 arg1) { _gl.Enable(arg1); PG_GL_CHECK_ERRORS(); } \
			void glEnableVertexAttribArray(uint32 arg1) { _gl.EnableVertexAttribArray(arg1); PG_GL_CHECK_ERRORS(); } \
			void* glFenceSync(uint32 arg1, uint32 arg2) { auto result = _gl.FenceSync(arg1, arg2); PG_GL_CHECK_ERRORS(); return result; } \
			void glFramebufferTexture2D(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4, int32 arg5) { _gl.FramebufferTexture2D(arg1, arg2, arg3, arg4, arg5); PG_GL_CHECK_ERRORS(); } \
			void glFrontFace(uint32 arg1) { _gl.FrontFace(arg1); PG_GL_CHECK_ERRORS(); } \
			void glGenBuffers(int32 arg1, uint32* arg2) { _gl.GenBuffers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glGenerateMipmap(uint32 arg1) { _gl.GenerateMipmap(arg1); PG_GL_CHECK_ERRORS(); } \
			void glGenFramebuffers(int32 arg1, uint32* arg2) { _gl.GenFramebuffers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glGenQueries(int32 arg1, uint32* arg2) { _gl.GenQueries(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glGenSamplers(int32 arg1, uint32* arg2) { _gl.GenSamplers(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glGenTextures(int32 arg1, uint32* arg2) { _gl.GenTextures(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glGenVertexArrays(int32 arg1, uint32* arg2) { _gl.GenVertexArrays(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			uint32 glGetError() { auto result = _gl.GetError(); PG_GL_CHECK_ERRORS(); return result; } \
			void glGetIntegerv(uint32 arg1, int32* arg2) { _gl.GetIntegerv(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glGetProgramInfoLog(uint32 arg1, int32 arg2, int32* arg3, uint8* arg4) { _gl.GetProgramInfoLog(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glGetProgramiv(uint32 arg1, uint32 arg2, int32* arg3) { _gl.GetProgramiv(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glGetQueryObjectiv(uint32 arg1, uint32 arg2, int32* arg3) { _gl.GetQueryObjectiv(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glGetQueryObjectui64v(uint32 arg1, uint32 arg2, uint64* arg3) { _gl.GetQueryObjectui64v(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glGetShaderInfoLog(uint32 arg1, int32 arg2, int32* arg3, uint8* arg4) { _gl.GetShaderInfoLog(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glGetShaderiv(uint32 arg1, uint32 arg2, int32* arg3) { _gl.GetShaderiv(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			uint8* glGetString(uint32 arg1) { auto result = _gl.GetString(arg1); PG_GL_CHECK_ERRORS(); return result; } \
			void glLinkProgram(uint32 arg1) { _gl.LinkProgram(arg1); PG_GL_CHECK_ERRORS(); } \
			void* glMapBuffer(uint32 arg1, uint32 arg2) { auto result = _gl.MapBuffer(arg1, arg2); PG_GL_CHECK_ERRORS(); return result; } \
			void* glMapBufferRange(uint32 arg1, void* arg2, void* arg3, uint32 arg4) { auto result = _gl.MapBufferRange(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); return result; } \
			void glPolygonMode(uint32 arg1, uint32 arg2) { _gl.PolygonMode(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glPolygonOffset(float32 arg1, float32 arg2) { _gl.PolygonOffset(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glQueryCounter(uint32 arg1, uint32 arg2) { _gl.QueryCounter(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glSamplerParameterf(uint32 arg1, uint32 arg2, float32 arg3) { _gl.SamplerParameterf(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glSamplerParameterfv(uint32 arg1, uint32 arg2, float32* arg3) { _gl.SamplerParameterfv(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glSamplerParameteri(uint32 arg1, uint32 arg2, int32 arg3) { _gl.SamplerParameteri(arg1, arg2, arg3); PG_GL_CHECK_ERRORS(); } \
			void glScissor(int32 arg1, int32 arg2, int32 arg3, int32 arg4) { _gl.Scissor(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glShaderSource(uint32 arg1, int32 arg2, uint8** arg3, int32* arg4) { _gl.ShaderSource(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glStencilFuncSeparate(uint32 arg1, uint32 arg2, int32 arg3, uint32 arg4) { _gl.StencilFuncSeparate(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glStencilOpSeparate(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4) { _gl.StencilOpSeparate(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
			void glTexImage2D(uint32 arg1, int32 arg2, int32 arg3, int32 arg4, int32 arg5, int32 arg6, uint32 arg7, uint32 arg8, void* arg9) { _gl.TexImage2D(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9); PG_GL_CHECK_ERRORS(); } \
			bool glUnmapBuffer(uint32 arg1) { auto result = _gl.UnmapBuffer(arg1); PG_GL_CHECK_ERRORS(); return result; } \
			void glUseProgram(uint32 arg1) { _gl.UseProgram(arg1); PG_GL_CHECK_ERRORS(); } \
			void glVertexAttribDivisor(uint32 arg1, uint32 arg2) { _gl.VertexAttribDivisor(arg1, arg2); PG_GL_CHECK_ERRORS(); } \
			void glVertexAttribPointer(uint32 arg1, int32 arg2, uint32 arg3, bool arg4, int32 arg5, void* arg6) { _gl.VertexAttribPointer(arg1, arg2, arg3, arg4, arg5, arg6); PG_GL_CHECK_ERRORS(); } \
			void glViewport(int32 arg1, int32 arg2, int32 arg3, int32 arg4) { _gl.Viewport(arg1, arg2, arg3, arg4); PG_GL_CHECK_ERRORS(); } \
	

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

