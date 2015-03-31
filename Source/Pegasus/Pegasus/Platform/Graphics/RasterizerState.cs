namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes the state of the rasterizer pipeline stage.
	/// </summary>
	public sealed unsafe class RasterizerState : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the rasterizer state that should be created.</param>
		public RasterizerState(GraphicsDevice graphicsDevice, RasterizerDescription description)
			: base(graphicsDevice)
		{
			NativeObject = DeviceInterface->InitializeRasterizerState(&description);
		}

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetRasterizerStateName; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref DeviceState.RasterizerState, this);
			DeviceInterface->FreeRasterizerState(NativeObject);
		}

		/// <summary>
		///     Sets the state on the pipeline. The state can no longer be changed afterwards.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref DeviceState.RasterizerState, this))
				DeviceInterface->BindRasterizerState(NativeObject);
		}
	}
}