namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes a depth stencil state of the output merger pipeline stage.
	/// </summary>
	public sealed unsafe class DepthStencilState : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the depth stencil state that should be created.</param>
		public DepthStencilState(GraphicsDevice graphicsDevice, DepthStencilDescription description)
			: base(graphicsDevice)
		{
			NativeObject = DeviceInterface->InitializeDepthStencilState(&description);
		}

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetDepthStencilStateName; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref DeviceState.DepthStencilState, this);
			DeviceInterface->FreeDepthStencilState(NativeObject);
		}

		/// <summary>
		///     Sets the state on the pipeline. The state can no longer be changed afterwards.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref DeviceState.DepthStencilState, this))
				DeviceInterface->BindDepthStencilState(NativeObject);
		}
	}
}