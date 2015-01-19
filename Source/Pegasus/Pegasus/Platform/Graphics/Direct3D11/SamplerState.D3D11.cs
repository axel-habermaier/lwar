namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes an Direct3D11-based sampler state of a shader pipeline stage.
	/// </summary>
	internal class SamplerStateD3D11 : GraphicsObjectD3D11, ISamplerState
	{
		/// <summary>
		///     The underlying Direct3D11 sampler state.
		/// </summary>
		private D3D11SamplerState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the sampler state that should be created.</param>
		public SamplerStateD3D11(GraphicsDeviceD3D11 graphicsDevice, ref SamplerDescription description)
			: base(graphicsDevice)
		{
			var desc = new D3D11SamplerStateDescription
			{
				AddressU = description.AddressU.Map(),
				AddressV = description.AddressV.Map(),
				AddressW = description.AddressW.Map(),
				ComparisonFunction = description.Comparison.Map(),
				Filter = description.Filter.Map(),
				MaximumAnisotropy = description.MaximumAnisotropy,
				MaximumLod = description.MaximumLod,
				MinimumLod = description.MinimumLod,
				MipLodBias = description.MipLodBias,
				BorderColor = description.BorderColor.Map()
			};

			if (description.Filter == TextureFilter.NearestNoMipmaps || description.Filter == TextureFilter.BilinearNoMipmaps)
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (desc.MaximumLod == Single.MaxValue)
					desc.MaximumLod = 0;
				if (desc.MinimumLod == Single.MinValue)
					desc.MinimumLod = 0;
			}

			Device.CreateSamplerState(ref desc, out _state).CheckSuccess("Failed to create sampler state.");
		}

		/// <summary>
		///     Binds the state on the given slot.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		public unsafe void Bind(int slot)
		{
			var state = _state;
			Context.VSSetSamplers(slot, 1, &state);
			Context.PSSetSamplers(slot, 1, &state);
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