namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes an OpenGL3-based depth stencil state of the output merger pipeline stage.
	/// </summary>
	internal class DepthStencilStateGL3 : GraphicsObjectGL3, IDepthStencilState
	{
		/// <summary>
		///     The description of the depth stencil state.
		/// </summary>
		private DepthStencilDescription _desc;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the depth stencil state that should be created.</param>
		public DepthStencilStateGL3(GraphicsDeviceGL3 graphicsDevice, ref DepthStencilDescription description)
			: base(graphicsDevice)
		{
			_desc = description;
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			if (_desc.DepthEnabled)
			{
				GraphicsDevice.EnableDepthTest(true);
				GraphicsDevice.SetDepthFunc(_desc.DepthFunction.Map());
			}
			else
				GraphicsDevice.EnableDepthTest(false);

			if (_desc.StencilEnabled)
			{
				GraphicsDevice.EnableStencilTest(true);

				SetupStencilState(ref _desc.FrontFace, GL.Front);
				SetupStencilState(ref _desc.BackFace, GL.Back);
			}
			else
				GraphicsDevice.EnableStencilTest(false);

			GraphicsDevice.EnableDepthWrites(_desc.DepthWriteEnabled);
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
		///     Sets up the stencil state for the given face.
		/// </summary>
		/// <param name="face">The face that should be configured.</param>
		/// <param name="stencilDesc">The description of the stencil state.</param>
		private void SetupStencilState(ref StencilOperationDescription stencilDesc, uint face)
		{
			_gl.StencilFuncSeparate(face, stencilDesc.StencilFunction.Map(), _desc.StencilReadMask, _desc.StencilWriteMask);
			_gl.StencilOpSeparate(face, stencilDesc.FailOperation.Map(), stencilDesc.DepthFailOperation.Map(), stencilDesc.PassOperation.Map());
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