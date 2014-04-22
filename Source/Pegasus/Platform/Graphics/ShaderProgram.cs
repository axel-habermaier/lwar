namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Framework;

	/// <summary>
	///     Represents a combination of different shader programs that control the various pipeline stages of the GPU.
	/// </summary>
	public sealed class ShaderProgram : GraphicsObject
	{
		/// <summary>
		///     The native shader program instance.
		/// </summary>
		private IntPtr _shaderProgram;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="vertexShader">The vertex shader that the shader program should use.</param>
		/// <param name="fragmentShader">The fragment shader that the shader program should use.</param>
		public ShaderProgram(GraphicsDevice graphicsDevice, VertexShader vertexShader, FragmentShader fragmentShader)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(vertexShader);
			Assert.ArgumentNotNull(fragmentShader);

			VertexShader = vertexShader;
			FragmentShader = fragmentShader;

			Reinitialize();
		}

		/// <summary>
		///     Gets the fragment shader used by the shader program.
		/// </summary>
		public FragmentShader FragmentShader { get; private set; }

		/// <summary>
		///     Gets the vertex shader used by the shader program.
		/// </summary>
		public VertexShader VertexShader { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Destroy();
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
		internal void Reinitialize()
		{
			Assert.NotDisposed(this);

			Destroy();

			if (VertexShader.NativePtr != IntPtr.Zero && FragmentShader.NativePtr != IntPtr.Zero)
				_shaderProgram = NativeMethods.CreateProgram(Application.Current.GraphicsDevice.NativePtr, VertexShader.NativePtr, FragmentShader.NativePtr);
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