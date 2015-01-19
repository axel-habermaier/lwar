namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Represents an Direct3D11-based combination of different shader programs that control the various pipeline
	///     stages of the GPU.
	/// </summary>
	internal class ShaderProgramD3D11 : GraphicsObjectD3D11, IShaderProgram
	{
		/// <summary>
		///     The graphics device the shader program belongs to.
		/// </summary>
		private readonly GraphicsDeviceD3D11 _graphicsDevice;

		/// <summary>
		///     The underlying Direct3D11 pixel shader.
		/// </summary>
		private readonly D3D11PixelShader _pixelShader;

		/// <summary>
		///     The underlying Direct3D11 vertex shader.
		/// </summary>
		private readonly D3D11VertexShader _vertexShader;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="vertexShader">The vertex shader that should be used by the shader program.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the shader program.</param>
		public ShaderProgramD3D11(GraphicsDeviceD3D11 graphicsDevice, VertexShader vertexShader, FragmentShader fragmentShader)
			: base(graphicsDevice)
		{
			_graphicsDevice = graphicsDevice;
			_vertexShader = ((VertexShaderD3D11)vertexShader.ShaderObject).Shader;
			_pixelShader = ((PixelShaderD3D11)fragmentShader.ShaderObject).Shader;
		}

		/// <summary>
		///     Binds the shaders of the shader program to the various pipeline stages.
		/// </summary>
		public unsafe void Bind()
		{
			// We cannot use DeviceState.Change here, as that creates garbage like crazy: D3D11Pixel/VertexShader
			// does not implement IEquatable

			if (_graphicsDevice.BoundVertexShader.Object != _vertexShader.Object)
			{
				_graphicsDevice.BoundVertexShader.Object = _vertexShader.Object;
				Context.VSSetShader(_vertexShader, null, 0);
			}

			if (_graphicsDevice.BoundPixelShader.Object != _pixelShader.Object)
			{
				_graphicsDevice.BoundPixelShader.Object = _pixelShader.Object;
				Context.PSSetShader(_pixelShader, null, 0);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Nothing to do here	
		}
	}
}