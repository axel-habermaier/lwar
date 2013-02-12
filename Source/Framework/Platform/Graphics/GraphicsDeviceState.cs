using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using Math;

	/// <summary>
	///   Tracks the state of the graphics device, i.e., the resources, states, shaders, and other objects
	///   currently bound to the pipeline stages of the graphics device.
	/// </summary>
	internal sealed class GraphicsDeviceState
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public GraphicsDeviceState()
		{
			Samplers = new SamplerState[GraphicsDevice.SamplerSlotCount];
			ConstantBuffers = new ConstantBuffer[GraphicsDevice.ConstantBufferSlotCount];
			Textures = new Texture[GraphicsDevice.TextureSlotCount];
		}

		/// <summary>
		///   Gets the currently bound samplers.
		/// </summary>
		public SamplerState[] Samplers { get; private set; }

		/// <summary>
		///   Gets the currently bound constant buffers.
		/// </summary>
		public ConstantBuffer[] ConstantBuffers { get; private set; }

		/// <summary>
		///   Gets the currently bound textures.
		/// </summary>
		public Texture[] Textures { get; private set; }

		/// <summary>
		///   Gets or sets the currently bound vertex shader.
		/// </summary>
		public VertexShader VertexShader { get; set; }

		/// <summary>
		///   Gets or sets the currently bound fragment shader.
		/// </summary>
		public FragmentShader FragmentShader { get; set; }

		/// <summary>
		///   Gets or sets the currently bound primitive type.
		/// </summary>
		public PrimitiveType PrimitiveType { get; set; }

		/// <summary>
		///   Gets or sets the currently bound depth stencil state.
		/// </summary>
		public DepthStencilState DepthStencilState { get; set; }

		/// <summary>
		///   Gets or sets the currently bound blend state.
		/// </summary>
		public BlendState BlendState { get; set; }

		/// <summary>
		///   Gets or sets the currently bound rasterizer state.
		/// </summary>
		public RasterizerState RasterizerState { get; set; }

		/// <summary>
		///   Gets or sets the currently bound viewport.
		/// </summary>
		public Rectangle Viewport { get; set; }

		/// <summary>
		///   Gets or sets the currently set scissor rectangle.
		/// </summary>
		public Rectangle ScissorRectangle { get; set; }

		/// <summary>
		///   Gets or sets the currently bound render target.
		/// </summary>
		public RenderTarget RenderTarget { get; set; }

		/// <summary>
		///   Gets or sets the currently bound input layout.
		/// </summary>
		public VertexInputLayout VertexInputLayout { get; set; }
	}
}