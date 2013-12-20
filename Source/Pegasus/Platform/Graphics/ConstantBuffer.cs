namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   A constant buffer holds constant data for shader programs.
	/// </summary>
	public sealed class ConstantBuffer : Buffer
	{
		/// <summary>
		///   The slot the constant buffer is bound to.
		/// </summary>
		private readonly int _slot;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="size">The size of the constant buffer's contents in bytes.</param>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		internal ConstantBuffer(GraphicsDevice graphicsDevice, int size, int slot)
			: base(graphicsDevice, BufferType.ConstantBuffer, ResourceUsage.Dynamic, IntPtr.Zero, size)
		{
			_slot = slot;
		}

		/// <summary>
		///   Binds the constant buffer to the given slot without uploading any possible changes of the buffer to the GPU.
		/// </summary>
		internal void Bind()
		{
			Assert.NotDisposed(this);

			BindBuffer(_slot);
		}

		/// <summary>
		///   Copies the given data to the buffer. The size of the data is determined by the parameter that has been passed to the
		///   constructor of this instance.
		/// </summary>
		/// <param name="data">The data that should be copied into the buffer.</param>
		internal unsafe void CopyData(void* data)
		{
			Assert.That(data != null, "A valid data pointer must be specified.");
			Assert.NotDisposed(this);

			UpdateConstantBuffer(new IntPtr(data));
		}
	}
}