using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a shader that controls a programmable stage of the graphics pipeline.
	/// </summary>
	public abstract class Shader : GraphicsObject
	{
		/// <summary>
		///   The type of the shader.
		/// </summary>
		private readonly ShaderType _type;

		/// <summary>
		///   The native shader instance.
		/// </summary>
		private IntPtr _shader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="type">The type of the shader.</param>
		/// <param name="shaderData">The shader source data.</param>
		protected Shader(GraphicsDevice graphicsDevice, ShaderType type, byte[] shaderData)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(shaderData, () => shaderData);
			Assert.ArgumentInRange(type, () => type);

			_type = type;
			Reinitialize(shaderData);
		}

		/// <summary>
		///   Reinitializes the shader.
		/// </summary>
		/// <param name="shaderData">The shader source data.</param>
		internal void Reinitialize(byte[] shaderData)
		{
			if (_shader != IntPtr.Zero)
			{
				NativeMethods.DestroyShader(_shader);
				_shader = IntPtr.Zero;
			}

			_shader = NativeMethods.CreateShader(GraphicsDevice.NativePtr, _type, shaderData);
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
		protected void BindShader()
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
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateShader")]
			public static extern IntPtr CreateShader(IntPtr device, ShaderType type, byte[] shaderData);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyShader")]
			public static extern void DestroyShader(IntPtr shader);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindShader")]
			public static extern void BindShader(IntPtr shader);
		}
	}
}