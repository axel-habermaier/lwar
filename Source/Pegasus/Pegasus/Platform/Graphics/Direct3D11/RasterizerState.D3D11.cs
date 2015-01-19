namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes the Direct3D11-based state of the rasterizer pipeline stage.
	/// </summary>
	internal class RasterizerStateD3D11 : GraphicsObjectD3D11, IRasterizerState
	{
		/// <summary>
		///     The underlying Direct3D11 rasterizer state.
		/// </summary>
		private D3D11RasterizerState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the rasterizer state that should be created.</param>
		public RasterizerStateD3D11(GraphicsDeviceD3D11 graphicsDevice, ref RasterizerDescription description)
			: base(graphicsDevice)
		{
			var desc = new D3D11RasterizerStateDescription
			{
				CullMode = description.CullMode.Map(),
				FillMode = description.FillMode.Map(),
				DepthBias = description.DepthBias,
				DepthBiasClamp = description.DepthBiasClamp,
				IsAntialiasedLineEnabled = description.AntialiasedLineEnabled,
				IsDepthClipEnabled = description.DepthClipEnabled,
				IsFrontCounterClockwise = description.FrontIsCounterClockwise,
				IsMultisampleEnabled = description.MultisampleEnabled,
				IsScissorEnabled = description.ScissorEnabled,
				SlopeScaledDepthBias = description.SlopeScaledDepthBias
			};

			Device.CreateRasterizerState(ref desc, out _state).CheckSuccess("Failed to create rasterizer state.");
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			Context.RSSetState(_state);
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