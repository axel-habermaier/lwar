namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Memory;
	using Utilities;

	/// <summary>
	///     Describes a depth stencil state of the output merger pipeline stage.
	/// </summary>
	public sealed class DepthStencilState : GraphicsObject
	{
		/// <summary>
		///     The underlying depth stencil state object.
		/// </summary>
		private readonly IDepthStencilState _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the depth stencil state that should be created.</param>
		public DepthStencilState(GraphicsDevice graphicsDevice, ref DepthStencilDescription description)
			: base(graphicsDevice)
		{
			_state = graphicsDevice.CreateDepthStencilState(ref description);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref GraphicsDevice.State.DepthStencilState, this);
			_state.SafeDispose();
		}

		/// <summary>
		///     Sets the state on the pipeline. The state can no longer be changed afterwards.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref GraphicsDevice.State.DepthStencilState, this))
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