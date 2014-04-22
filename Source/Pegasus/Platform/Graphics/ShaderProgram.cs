﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///     Represents a combination of different shader programs that control the various pipeline stages of the GPU.
	/// </summary>
	internal sealed class ShaderProgram : GraphicsObject
	{
		/// <summary>
		///     The fragment shader used by the shader program.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///     The vertex shader used by the shader program.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///     The native shader program instance.
		/// </summary>
		private IntPtr _shaderProgram;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that the shader program should use.</param>
		/// <param name="fragmentShader">The fragment shader that the shader program should use.</param>
		public ShaderProgram(VertexShader vertexShader, FragmentShader fragmentShader)
		{
			Assert.ArgumentNotNull(vertexShader);
			Assert.ArgumentNotNull(fragmentShader);

			_vertexShader = vertexShader;
			_fragmentShader = fragmentShader;

			_vertexShader.Reinitialized += Reinitialize;
			_fragmentShader.Reinitialized += Reinitialize;

			Reinitialize();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Destroy();

			_vertexShader.Reinitialized -= Reinitialize;
			_fragmentShader.Reinitialized -= Reinitialize;
		}

		/// <summary>
		///     Destroys the shader program.
		/// </summary>
		private void Destroy()
		{
			NativeMethods.DestroyProgram(_shaderProgram);
			_shaderProgram = IntPtr.Zero;
		}

		/// <summary>
		///     Binds the shaders of the shader program to the various pipeline stages.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);
			NativeMethods.BindProgram(_shaderProgram);
		}

		/// <summary>
		///     Reinitializes the shader program.
		/// </summary>
		private void Reinitialize()
		{
			Assert.NotDisposed(this);

			Destroy();
			_shaderProgram = NativeMethods.CreateProgram(GraphicsDevice.Current.NativePtr, _vertexShader.NativePtr, _fragmentShader.NativePtr);
		}

		/// <summary>
		///     Provides access to the native shader program functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateProgram")]
			public static extern IntPtr CreateProgram(IntPtr device, IntPtr vertexShader, IntPtr fragmentShader);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyProgram")]
			public static extern IntPtr DestroyProgram(IntPtr program);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindProgram")]
			public static extern IntPtr BindProgram(IntPtr program);
		}
	}
}