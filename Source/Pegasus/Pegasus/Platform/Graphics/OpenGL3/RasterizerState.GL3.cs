namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes the OpenGL3-based state of the rasterizer pipeline stage.
	/// </summary>
	internal class RasterizerStateGL3 : GraphicsObjectGL3, IRasterizerState
	{
		/// <summary>
		///     The description of the rasterizer state.
		/// </summary>
		private readonly RasterizerDescription _desc;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the rasterizer state that should be created.</param>
		public RasterizerStateGL3(GraphicsDeviceGL3 graphicsDevice, ref RasterizerDescription description)
			: base(graphicsDevice)
		{
			_desc = description;
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			// TODO: What about depth bias?
			GraphicsDevice.SetPolygonMode(_desc.FillMode.Map());
			GraphicsDevice.SetPolygonOffset(_desc.SlopeScaledDepthBias, _desc.DepthBiasClamp);
			GraphicsDevice.SetFrontFace(_desc.FrontIsCounterClockwise ? GL.Ccw : GL.Cw);

			switch (_desc.CullMode)
			{
				case CullMode.Front:
					GraphicsDevice.EnableCullFace(true);
					GraphicsDevice.SetCullFace(GL.Front);
					break;
				case CullMode.Back:
					GraphicsDevice.EnableCullFace(true);
					GraphicsDevice.SetCullFace(GL.Back);
					break;
				case CullMode.None:
					GraphicsDevice.EnableCullFace(false);
					break;
			}

			GraphicsDevice.EnableScissor(_desc.ScissorEnabled);
			GraphicsDevice.EnableDepthClamp(_desc.DepthClipEnabled);
			GraphicsDevice.EnableMultisample(_desc.MultisampleEnabled);
			GraphicsDevice.EnableAntialiasedLine(_desc.AntialiasedLineEnabled);
		}

		/// <summary>
		///     Sets the debug name of the state.
		/// </summary>
		/// <param name="name">The debug name of the state.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
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