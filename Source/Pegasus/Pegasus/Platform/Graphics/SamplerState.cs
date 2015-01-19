namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Memory;
	using Utilities;

	/// <summary>
	///     Describes a sampler state of a shader pipeline stage.
	/// </summary>
	public sealed class SamplerState : GraphicsObject
	{
		/// <summary>
		///     The underlying sampler state object.
		/// </summary>
		private readonly ISamplerState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the sampler state that should be created.</param>
		public SamplerState(GraphicsDevice graphicsDevice, ref SamplerDescription description)
			: base(graphicsDevice)
		{
			IsMipmapped = description.Filter != TextureFilter.NearestNoMipmaps && description.Filter != TextureFilter.BilinearNoMipmaps;
			_state = graphicsDevice.CreateSamplerState(ref description);
		}

		/// <summary>
		///     Gets a value indicating whether the use of the sampler state requires a mipmapped texture.
		/// </summary>
		public bool IsMipmapped { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(GraphicsDevice.State.SamplerStates, this);
			_state.SafeDispose();
		}

		/// <summary>
		///     Binds the state on the given slot.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotDisposed(this);
			Assert.That(slot < GraphicsDevice.TextureSlotCount, "Invalid sampler slot.");

			if (DeviceState.Change(GraphicsDevice.State.SamplerStates, slot, this))
				_state.Bind(slot);
		}

		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only invoked in debug builds.
		/// </summary>
		/// <param name="name">The new name of the graphics object.</param>
		protected override void OnRenamed(string name)
		{
			_state.SetName(name);
		}
	}
}