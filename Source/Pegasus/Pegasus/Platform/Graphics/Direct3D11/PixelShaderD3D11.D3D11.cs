namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;

	/// <summary>
	///     Represents an Direct3D11-based pixel shader.
	/// </summary>
	internal class PixelShaderD3D11 : GraphicsObjectD3D11, IShader
	{
		/// <summary>
		///     The underlying Direct3D11 pixel shader.
		/// </summary>
		internal D3D11PixelShader Shader;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="byteCode">The shader byte code.</param>
		/// <param name="byteCodeLength">The length of the byte code in bytes.</param>
		public unsafe PixelShaderD3D11(GraphicsDeviceD3D11 graphicsDevice, IntPtr byteCode, int byteCodeLength)
			: base(graphicsDevice)
		{
			var bytes = new byte[byteCodeLength];
			Marshal.Copy(byteCode, bytes, 0, byteCodeLength);

			Device.CreatePixelShader((void*)byteCode, byteCodeLength, new D3D11ClassLinkage(), out Shader)
				  .CheckSuccess("Failed to create pixel shader.");
		}

		/// <summary>
		///     Sets the debug name of the shader.
		/// </summary>
		/// <param name="name">The debug name of the shader.</param>
		public void SetName(string name)
		{
			Shader.SetDebugName(name);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Shader.Release();
		}
	}
}