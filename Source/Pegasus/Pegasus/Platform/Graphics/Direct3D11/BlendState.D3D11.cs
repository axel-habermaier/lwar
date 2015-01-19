namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes an Direct3D11-based blend state of the output merger pipeline stage.
	/// </summary>
	internal class BlendStateD3D11 : GraphicsObjectD3D11, IBlendState
	{
		/// <summary>
		///     The Direct3D11 blend state instance.
		/// </summary>
		private D3D11BlendState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the blend state that should be created.</param>
		public BlendStateD3D11(GraphicsDeviceD3D11 graphicsDevice, ref BlendDescription description)
			: base(graphicsDevice)
		{
			var desc = new D3D11BlendStateDescription
			{
				AlphaToCoverageEnable = false,
				IndependentBlendEnable = false,
				RenderTarget0 =
				{
					IsBlendEnabled = description.BlendEnabled,
					BlendOperation = description.BlendOperation.Map(),
					SourceBlend = description.SourceBlend.Map(),
					DestinationBlend = description.DestinationBlend.Map(),
					AlphaBlendOperation = description.BlendOperationAlpha.Map(),
					SourceAlphaBlend = description.SourceBlendAlpha.Map(),
					DestinationAlphaBlend = description.DestinationBlendAlpha.Map(),
					RenderTargetWriteMask = description.WriteMask.Map()
				}
			};

			Device.CreateBlendState(ref desc, out _state).CheckSuccess("Failed to create blend state.");
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public unsafe void Bind()
		{
			Context.OMSetBlendState(_state, null, UInt32.MaxValue);
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