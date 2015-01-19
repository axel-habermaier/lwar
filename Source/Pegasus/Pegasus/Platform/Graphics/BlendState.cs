namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Memory;
	using Utilities;

	/// <summary>
	///     Describes a blend state of the output merger pipeline stage.
	/// </summary>
	public sealed class BlendState : GraphicsObject
	{
		/// <summary>
		///     The underlying blend state object.
		/// </summary>
		private readonly IBlendState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the blend state that should be created.</param>
		public BlendState(GraphicsDevice graphicsDevice, ref BlendDescription description)
			: base(graphicsDevice)
		{
			_state = graphicsDevice.CreateBlendState(ref description);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref GraphicsDevice.State.BlendState, this);
			_state.SafeDispose();
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref GraphicsDevice.State.BlendState, this))
				_state.Bind();
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