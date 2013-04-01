using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   A fragment shader is a program that controls the fragment-shader stage.
	/// </summary>
	public sealed class FragmentShader : GraphicsObject
	{
		/// <summary>
		///   The native shader instance.
		/// </summary>
		private IntPtr _shader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		public FragmentShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Reinitializes the shader.
		/// </summary>
		/// <param name="shaderCode">The shader source code.</param>
		/// <param name="length">The length of the shader code in bytes.</param>
		internal unsafe void Reinitialize(byte* shaderCode, int length)
		{
			NativeMethods.DestroyShader(_shader);
			_shader = IntPtr.Zero;

			_shader = NativeMethods.CreateShader(GraphicsDevice.NativePtr, shaderCode, length);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyShader(_shader);
		}

		/// <summary>
		///   Binds the shader to the pipeline.
		/// </summary>
		public void Bind()
		{
			NativeMethods.BindShader(_shader);
		}

		/// <summary>
		///   Provides access to the native shader functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateFragmentShader")]
			public static extern unsafe IntPtr CreateShader(IntPtr device, byte* shaderData, int length);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyShader")]
			public static extern void DestroyShader(IntPtr shader);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindShader")]
			public static extern void BindShader(IntPtr shader);
		}
	}
}