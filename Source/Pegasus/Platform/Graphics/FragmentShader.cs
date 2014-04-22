﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///     A fragment shader is a program that controls the fragment-shader stage.
	/// </summary>
	public sealed class FragmentShader : Shader
	{
		/// <summary>
		///     Reinitializes the shader.
		/// </summary>
		/// <param name="shaderCode">The shader source code.</param>
		/// <param name="length">The length of the shader code in bytes.</param>
		internal unsafe void Reinitialize(byte* shaderCode, int length)
		{
			Assert.ArgumentNotNull(new IntPtr(shaderCode));
			Assert.ArgumentSatisfies(length > 0, "Invalid shader code length.");

			DestroyShader();
			_shader = NativeMethods.CreateShader(GraphicsDevice.Current.NativePtr, shaderCode, length);

			OnReinitialized();
		}

		/// <summary>
		///     Provides access to the native shader functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateFragmentShader")]
			public static extern unsafe IntPtr CreateShader(IntPtr device, byte* shaderData, int length);
		}
	}
}