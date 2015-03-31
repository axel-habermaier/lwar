namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes a blend state of the output merger pipeline stage.
	/// </summary>
	public sealed unsafe class BlendState : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the blend state that should be created.</param>
		public BlendState(GraphicsDevice graphicsDevice, BlendDescription description)
			: base(graphicsDevice)
		{
			NativeObject = DeviceInterface->InitializeBlendState(&description);
		}

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetBlendStateName; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref DeviceState.BlendState, this);
			DeviceInterface->FreeBlendState(NativeObject);
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref DeviceState.BlendState, this))
				DeviceInterface->BindBlendState(NativeObject);
		}
	}
}