namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes an Direct3D11-based depth stencil state of the output merger pipeline stage.
	/// </summary>
	internal class DepthStencilStateD3D11 : GraphicsObjectD3D11, IDepthStencilState
	{
		/// <summary>
		///     The underlying Direct3D11 depth stencil state.
		/// </summary>
		private D3D11DepthStencilState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the depth stencil state that should be created.</param>
		public DepthStencilStateD3D11(GraphicsDeviceD3D11 graphicsDevice, ref DepthStencilDescription description)
			: base(graphicsDevice)
		{
			var desc = new D3D11DepthStencilStateDescription
			{
				BackFace =
				{
					Comparison = description.BackFace.StencilFunction.Map(),
					DepthFailOperation = description.BackFace.DepthFailOperation.Map(),
					FailOperation = description.BackFace.FailOperation.Map(),
					PassOperation = description.BackFace.PassOperation.Map()
				},
				FrontFace =
				{
					Comparison = description.FrontFace.StencilFunction.Map(),
					DepthFailOperation = description.FrontFace.DepthFailOperation.Map(),
					FailOperation = description.FrontFace.FailOperation.Map(),
					PassOperation = description.FrontFace.PassOperation.Map()
				},
				DepthComparison = description.DepthFunction.Map(),
				DepthWriteMask = description.DepthWriteEnabled ? D3D11DepthWriteMask.All : D3D11DepthWriteMask.Zero,
				IsDepthEnabled = description.DepthEnabled,
				IsStencilEnabled = description.StencilEnabled,
				StencilReadMask = description.StencilReadMask,
				StencilWriteMask = description.StencilWriteMask
			};

			Device.CreateDepthStencilState(ref desc, out _state).CheckSuccess("Failed to create depth stencil state.");
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			Context.OMSetDepthStencilState(_state, 0);
		}

		/// <summary>
		///     Sets the debug name of the state.
		/// </summary>
		/// <param name="name">The debug name of the state.</param>
		public void SetName(string name)
		{
			_state.SetDebugName(name);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_state.Release();
		}
	}
}