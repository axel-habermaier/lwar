using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   A vertex shader is a program that controls the vertex-shader stage.
	/// </summary>
	internal sealed class VertexShader : Shader
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		public VertexShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Reinitializes the shader.
		/// </summary>
		/// <param name="shaderCode">The shader source code.</param>
		/// <param name="length">The length of the shader code in bytes.</param>
		/// <param name="inputs">The shader input data layout.</param>
		internal unsafe void Reinitialize(byte* shaderCode, int length, ShaderInput[] inputs)
		{
			Assert.ArgumentNotNull(inputs, () => inputs);
			Assert.ArgumentSatisfies(inputs.Length > 0, () => inputs, "The shader must have at least one input.");

			DestroyShader();
			_shader = NativeMethods.CreateShader(GraphicsDevice.NativePtr, shaderCode, length, inputs, inputs.Length);
		}

		/// <summary>
		///   Provides access to the native shader functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateVertexShader")]
			public static extern unsafe IntPtr CreateShader(IntPtr device, byte* shaderData, int length, ShaderInput[] inputs,
															int inputCount);
		}
	}
}