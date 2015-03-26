namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes a sampler state of a shader pipeline stage.
	/// </summary>
	public sealed unsafe class SamplerState : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the sampler state that should be created.</param>
		public SamplerState(GraphicsDevice graphicsDevice, SamplerDescription description)
			: base(graphicsDevice)
		{
			IsMipmapped = description.Filter != TextureFilter.NearestNoMipmaps && description.Filter != TextureFilter.BilinearNoMipmaps;
			NativeObject = DeviceInterface->InitializeSamplerState(&description);
		}

		/// <summary>
		///     Gets a value indicating whether the use of the sampler state requires a mipmapped texture.
		/// </summary>
		public bool IsMipmapped { get; private set; }

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetSamplerStateName; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(DeviceState.SamplerStates, this);
			DeviceInterface->FreeSamplerState(NativeObject);
		}

		/// <summary>
		///     Binds the state on the given slot.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotDisposed(this);
			Assert.That(slot < GraphicsDevice.TextureSlotCount, "Invalid sampler slot.");

			if (DeviceState.Change(DeviceState.SamplerStates, slot, this))
				DeviceInterface->BindSamplerState(NativeObject, slot);
		}
	}
}