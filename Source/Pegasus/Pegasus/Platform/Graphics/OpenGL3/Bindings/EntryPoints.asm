
#include "Debug.asm"

.class private auto ansi sealed beforefieldinit Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints
	extends System.Object
{
	.field private class [mscorlib]System.Action _checkErrors
	.field private class [mscorlib]System.Func`2<string, native int> _loader

	.method public hidebysig specialname rtspecialname instance void .ctor (
			class [mscorlib]System.Action checkErrors,
			class [mscorlib]System.Func`2<string, native int> loader
		) cil managed 
	{
		.maxstack 3

		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()

		ldarg.0
		ldarg.1
		stfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors

		ldarg.0
		ldarg.2
		stfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		
		ldarg.0
		ldarg.2
		ldstr "glGetError"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getError

		ldarg.0
		ldarg.2
		ldstr "glGetIntegerv"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getIntegerv

		ret
	}

	.method public hidebysig void Load() cil managed 
	{
		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenSamplers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genSamplers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteSamplers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteSamplers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBindSampler"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindSampler

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glSamplerParameteri"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_samplerParameteri

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glSamplerParameterf"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_samplerParameterf

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glSamplerParameterfv"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_samplerParameterfv

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDrawArraysInstancedBaseInstance"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawArraysInstancedBaseInstance

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDrawElementsInstancedBaseVertexBaseInstance"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawElementsInstancedBaseVertexBaseInstance

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glCullFace"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_cullFace

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glFrontFace"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_frontFace

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glPolygonMode"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_polygonMode

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glScissor"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_scissor

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glTexImage2D"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_texImage2D

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glClear"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clear

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glClearColor"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clearColor

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glClearStencil"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clearStencil

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glClearDepth"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clearDepth

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glColorMask"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_colorMask

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDepthMask"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_depthMask

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDisable"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_disable

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glEnable"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_enable

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDepthFunc"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_depthFunc

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetError"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getError

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetIntegerv"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getIntegerv

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetString"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getString

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glViewport"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_viewport

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDrawArrays"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawArrays

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glPolygonOffset"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_polygonOffset

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBindTexture"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindTexture

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteTextures"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteTextures

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenTextures"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genTextures

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glActiveTexture"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_activeTexture

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glCompressedTexImage2D"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_compressedTexImage2D

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBlendFuncSeparate"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_blendFuncSeparate

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenQueries"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genQueries

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteQueries"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteQueries

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetQueryObjectiv"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getQueryObjectiv

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBindBuffer"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindBuffer

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteBuffers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteBuffers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenBuffers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genBuffers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBufferData"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bufferData

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBufferSubData"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bufferSubData

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glMapBuffer"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_mapBuffer

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glUnmapBuffer"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_unmapBuffer

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBlendEquationSeparate"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_blendEquationSeparate

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDrawBuffers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawBuffers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glStencilOpSeparate"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_stencilOpSeparate

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glStencilFuncSeparate"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_stencilFuncSeparate

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glAttachShader"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_attachShader

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glCompileShader"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_compileShader

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glCreateProgram"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_createProgram

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glCreateShader"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_createShader

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteProgram"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteProgram

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteShader"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteShader

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDetachShader"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_detachShader

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glEnableVertexAttribArray"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_enableVertexAttribArray

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetProgramiv"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getProgramiv

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetProgramInfoLog"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getProgramInfoLog

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetShaderiv"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getShaderiv

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetShaderInfoLog"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getShaderInfoLog

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glLinkProgram"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_linkProgram

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glShaderSource"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_shaderSource

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glUseProgram"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_useProgram

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glVertexAttribPointer"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_vertexAttribPointer

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBindVertexArray"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindVertexArray

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteVertexArrays"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteVertexArrays

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenVertexArrays"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genVertexArrays

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glMapBufferRange"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_mapBufferRange

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBindFramebuffer"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindFramebuffer

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteFramebuffers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteFramebuffers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenFramebuffers"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genFramebuffers

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glCheckFramebufferStatus"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkFramebufferStatus

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glFramebufferTexture2D"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_framebufferTexture2D

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGenerateMipmap"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_generateMipmap

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glBindBufferBase"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindBufferBase

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDrawElementsBaseVertex"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawElementsBaseVertex

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glFenceSync"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_fenceSync

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glDeleteSync"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteSync

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glClientWaitSync"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clientWaitSync

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glQueryCounter"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_queryCounter

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glGetQueryObjectui64v"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getQueryObjectui64v

		ldarg.0
		ldarg.0
		ldfld class [mscorlib]System.Func`2<string, native int> Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_loader
		ldstr "glVertexAttribDivisor"
		callvirt instance !1 class [mscorlib]System.Func`2<string, native int>::Invoke(!0)
		stfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_vertexAttribDivisor

		ret
	}

	.method public hidebysig void GenSamplers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genSamplers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteSamplers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteSamplers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BindSampler(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindSampler
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void SamplerParameteri(uint32 arg1, uint32 arg2, int32 arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_samplerParameteri
		calli unmanaged stdcall void(uint32, uint32, int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void SamplerParameterf(uint32 arg1, uint32 arg2, float32 arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_samplerParameterf
		calli unmanaged stdcall void(uint32, uint32, float32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void SamplerParameterfv(uint32 arg1, uint32 arg2, float32* arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_samplerParameterfv
		calli unmanaged stdcall void(uint32, uint32, float32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DrawArraysInstancedBaseInstance(uint32 arg1, int32 arg2, int32 arg3, int32 arg4, uint32 arg5) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 7

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawArraysInstancedBaseInstance
		calli unmanaged stdcall void(uint32, int32, int32, int32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DrawElementsInstancedBaseVertexBaseInstance(uint32 arg1, int32 arg2, uint32 arg3, void* arg4, int32 arg5, int32 arg6, uint32 arg7) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 9

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.s 6
		ldarg.s 7
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawElementsInstancedBaseVertexBaseInstance
		calli unmanaged stdcall void(uint32, int32, uint32, void*, int32, int32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void CullFace(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_cullFace
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void FrontFace(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_frontFace
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void PolygonMode(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_polygonMode
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void Scissor(int32 arg1, int32 arg2, int32 arg3, int32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_scissor
		calli unmanaged stdcall void(int32, int32, int32, int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void TexImage2D(uint32 arg1, int32 arg2, int32 arg3, int32 arg4, int32 arg5, int32 arg6, uint32 arg7, uint32 arg8, void* arg9) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 11

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.s 6
		ldarg.s 7
		ldarg.s 8
		ldarg.s 9
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_texImage2D
		calli unmanaged stdcall void(uint32, int32, int32, int32, int32, int32, uint32, uint32, void*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void Clear(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clear
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void ClearColor(float32 arg1, float32 arg2, float32 arg3, float32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clearColor
		calli unmanaged stdcall void(float32, float32, float32, float32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void ClearStencil(int32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clearStencil
		calli unmanaged stdcall void(int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void ClearDepth(float64 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clearDepth
		calli unmanaged stdcall void(float64)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void ColorMask(bool arg1, bool arg2, bool arg3, bool arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_colorMask
		calli unmanaged stdcall void(bool, bool, bool, bool)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DepthMask(bool arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_depthMask
		calli unmanaged stdcall void(bool)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void Disable(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_disable
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void Enable(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_enable
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DepthFunc(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_depthFunc
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig uint32 GetError() cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 2

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getError
		calli unmanaged stdcall uint32()
		ret
	}

	.method public hidebysig void GetIntegerv(uint32 arg1, int32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getIntegerv
		calli unmanaged stdcall void(uint32, int32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig uint8* GetString(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getString
		calli unmanaged stdcall uint8*(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void Viewport(int32 arg1, int32 arg2, int32 arg3, int32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_viewport
		calli unmanaged stdcall void(int32, int32, int32, int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DrawArrays(uint32 arg1, int32 arg2, int32 arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawArrays
		calli unmanaged stdcall void(uint32, int32, int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void PolygonOffset(float32 arg1, float32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_polygonOffset
		calli unmanaged stdcall void(float32, float32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BindTexture(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindTexture
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteTextures(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteTextures
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GenTextures(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genTextures
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void ActiveTexture(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_activeTexture
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void CompressedTexImage2D(uint32 arg1, int32 arg2, uint32 arg3, int32 arg4, int32 arg5, int32 arg6, int32 arg7, void* arg8) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 10

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.s 6
		ldarg.s 7
		ldarg.s 8
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_compressedTexImage2D
		calli unmanaged stdcall void(uint32, int32, uint32, int32, int32, int32, int32, void*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BlendFuncSeparate(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_blendFuncSeparate
		calli unmanaged stdcall void(uint32, uint32, uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GenQueries(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genQueries
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteQueries(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteQueries
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GetQueryObjectiv(uint32 arg1, uint32 arg2, int32* arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getQueryObjectiv
		calli unmanaged stdcall void(uint32, uint32, int32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BindBuffer(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindBuffer
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteBuffers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteBuffers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GenBuffers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genBuffers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BufferData(uint32 arg1, void* arg2, void* arg3, uint32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bufferData
		calli unmanaged stdcall void(uint32, void*, void*, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BufferSubData(uint32 arg1, void* arg2, void* arg3, void* arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bufferSubData
		calli unmanaged stdcall void(uint32, void*, void*, void*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void* MapBuffer(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_mapBuffer
		calli unmanaged stdcall void*(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig bool UnmapBuffer(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_unmapBuffer
		calli unmanaged stdcall bool(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BlendEquationSeparate(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_blendEquationSeparate
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DrawBuffers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawBuffers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void StencilOpSeparate(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_stencilOpSeparate
		calli unmanaged stdcall void(uint32, uint32, uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void StencilFuncSeparate(uint32 arg1, uint32 arg2, int32 arg3, uint32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_stencilFuncSeparate
		calli unmanaged stdcall void(uint32, uint32, int32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void AttachShader(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_attachShader
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void CompileShader(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_compileShader
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig uint32 CreateProgram() cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 2

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_createProgram
		calli unmanaged stdcall uint32()
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig uint32 CreateShader(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_createShader
		calli unmanaged stdcall uint32(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteProgram(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteProgram
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteShader(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteShader
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DetachShader(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_detachShader
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void EnableVertexAttribArray(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_enableVertexAttribArray
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GetProgramiv(uint32 arg1, uint32 arg2, int32* arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getProgramiv
		calli unmanaged stdcall void(uint32, uint32, int32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GetProgramInfoLog(uint32 arg1, int32 arg2, int32* arg3, uint8* arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getProgramInfoLog
		calli unmanaged stdcall void(uint32, int32, int32*, uint8*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GetShaderiv(uint32 arg1, uint32 arg2, int32* arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getShaderiv
		calli unmanaged stdcall void(uint32, uint32, int32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GetShaderInfoLog(uint32 arg1, int32 arg2, int32* arg3, uint8* arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getShaderInfoLog
		calli unmanaged stdcall void(uint32, int32, int32*, uint8*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void LinkProgram(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_linkProgram
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void ShaderSource(uint32 arg1, int32 arg2, uint8** arg3, int32* arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_shaderSource
		calli unmanaged stdcall void(uint32, int32, uint8**, int32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void UseProgram(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_useProgram
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void VertexAttribPointer(uint32 arg1, int32 arg2, uint32 arg3, bool arg4, int32 arg5, void* arg6) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 8

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.s 6
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_vertexAttribPointer
		calli unmanaged stdcall void(uint32, int32, uint32, bool, int32, void*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BindVertexArray(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindVertexArray
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteVertexArrays(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteVertexArrays
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GenVertexArrays(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genVertexArrays
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void* MapBufferRange(uint32 arg1, void* arg2, void* arg3, uint32 arg4) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 6

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_mapBufferRange
		calli unmanaged stdcall void*(uint32, void*, void*, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BindFramebuffer(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindFramebuffer
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteFramebuffers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteFramebuffers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GenFramebuffers(int32 arg1, uint32* arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_genFramebuffers
		calli unmanaged stdcall void(int32, uint32*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig uint32 CheckFramebufferStatus(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkFramebufferStatus
		calli unmanaged stdcall uint32(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void FramebufferTexture2D(uint32 arg1, uint32 arg2, uint32 arg3, uint32 arg4, int32 arg5) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 7

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_framebufferTexture2D
		calli unmanaged stdcall void(uint32, uint32, uint32, uint32, int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GenerateMipmap(uint32 arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_generateMipmap
		calli unmanaged stdcall void(uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void BindBufferBase(uint32 arg1, uint32 arg2, uint32 arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_bindBufferBase
		calli unmanaged stdcall void(uint32, uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DrawElementsBaseVertex(uint32 arg1, int32 arg2, uint32 arg3, void* arg4, int32 arg5) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 7

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.s 4
		ldarg.s 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_drawElementsBaseVertex
		calli unmanaged stdcall void(uint32, int32, uint32, void*, int32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void* FenceSync(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_fenceSync
		calli unmanaged stdcall void*(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void DeleteSync(void* arg1) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 3

		ldarg.s 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_deleteSync
		calli unmanaged stdcall void(void*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig uint32 ClientWaitSync(void* arg1, uint32 arg2, uint64 arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_clientWaitSync
		calli unmanaged stdcall uint32(void*, uint32, uint64)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void QueryCounter(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_queryCounter
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void GetQueryObjectui64v(uint32 arg1, uint32 arg2, uint64* arg3) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 5

		ldarg.s 1
		ldarg.s 2
		ldarg.s 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_getQueryObjectui64v
		calli unmanaged stdcall void(uint32, uint32, uint64*)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.method public hidebysig void VertexAttribDivisor(uint32 arg1, uint32 arg2) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 4

		ldarg.s 1
		ldarg.s 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_vertexAttribDivisor
		calli unmanaged stdcall void(uint32, uint32)
		#ifdef DEBUG
			ldarg.0
			ldfld class [mscorlib]System.Action Pegasus.Platform.Graphics.OpenGL3.Bindings.EntryPoints::_checkErrors
			callvirt instance void class [mscorlib]System.Action::Invoke()
		#endif
		ret
	}

	.field private native int _genSamplers
	.field private native int _deleteSamplers
	.field private native int _bindSampler
	.field private native int _samplerParameteri
	.field private native int _samplerParameterf
	.field private native int _samplerParameterfv
	.field private native int _drawArraysInstancedBaseInstance
	.field private native int _drawElementsInstancedBaseVertexBaseInstance
	.field private native int _cullFace
	.field private native int _frontFace
	.field private native int _polygonMode
	.field private native int _scissor
	.field private native int _texImage2D
	.field private native int _clear
	.field private native int _clearColor
	.field private native int _clearStencil
	.field private native int _clearDepth
	.field private native int _colorMask
	.field private native int _depthMask
	.field private native int _disable
	.field private native int _enable
	.field private native int _depthFunc
	.field private native int _getError
	.field private native int _getIntegerv
	.field private native int _getString
	.field private native int _viewport
	.field private native int _drawArrays
	.field private native int _polygonOffset
	.field private native int _bindTexture
	.field private native int _deleteTextures
	.field private native int _genTextures
	.field private native int _activeTexture
	.field private native int _compressedTexImage2D
	.field private native int _blendFuncSeparate
	.field private native int _genQueries
	.field private native int _deleteQueries
	.field private native int _getQueryObjectiv
	.field private native int _bindBuffer
	.field private native int _deleteBuffers
	.field private native int _genBuffers
	.field private native int _bufferData
	.field private native int _bufferSubData
	.field private native int _mapBuffer
	.field private native int _unmapBuffer
	.field private native int _blendEquationSeparate
	.field private native int _drawBuffers
	.field private native int _stencilOpSeparate
	.field private native int _stencilFuncSeparate
	.field private native int _attachShader
	.field private native int _compileShader
	.field private native int _createProgram
	.field private native int _createShader
	.field private native int _deleteProgram
	.field private native int _deleteShader
	.field private native int _detachShader
	.field private native int _enableVertexAttribArray
	.field private native int _getProgramiv
	.field private native int _getProgramInfoLog
	.field private native int _getShaderiv
	.field private native int _getShaderInfoLog
	.field private native int _linkProgram
	.field private native int _shaderSource
	.field private native int _useProgram
	.field private native int _vertexAttribPointer
	.field private native int _bindVertexArray
	.field private native int _deleteVertexArrays
	.field private native int _genVertexArrays
	.field private native int _mapBufferRange
	.field private native int _bindFramebuffer
	.field private native int _deleteFramebuffers
	.field private native int _genFramebuffers
	.field private native int _checkFramebufferStatus
	.field private native int _framebufferTexture2D
	.field private native int _generateMipmap
	.field private native int _bindBufferBase
	.field private native int _drawElementsBaseVertex
	.field private native int _fenceSync
	.field private native int _deleteSync
	.field private native int _clientWaitSync
	.field private native int _queryCounter
	.field private native int _getQueryObjectui64v
	.field private native int _vertexAttribDivisor
}
