namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     A constant buffer holds constant data for shader programs.
	/// </summary>
	public sealed unsafe class ConstantBuffer : Buffer
	{
		/// <summary>
		///     The slot the constant buffer is bound to.
		/// </summary>
		private readonly int _slot;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="size">The size of the constant buffer's contents in bytes.</param>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		public ConstantBuffer(GraphicsDevice graphicsDevice, int size, int slot)
			: base(graphicsDevice, BufferType.ConstantBuffer, ResourceUsage.Dynamic, IntPtr.Zero, size)
		{
			_slot = slot;
		}

		/// <summary>
		///     Copies the given data to the buffer, overwriting all previous data.
		/// </summary>
		/// <param name="data">The data that should be copied.</param>
		public void CopyData(void* data)
		{
			Assert.ArgumentNotNull(new IntPtr(data));
			Assert.NotDisposed(this);

			DeviceInterface->CopyBuffer(NativeObject, data);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(DeviceState.ConstantBuffers, this);
			base.OnDisposing();
		}

		/// <summary>
		///     Binds the constant buffer to the given slot without uploading any possible changes of the buffer to the GPU.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(DeviceState.ConstantBuffers, _slot, this))
				DeviceInterface->BindBuffer(NativeObject, _slot);
		}
	}
}