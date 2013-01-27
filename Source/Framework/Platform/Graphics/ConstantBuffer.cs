using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   A constant buffer holds constant data for shader programs.
	/// </summary>
	public sealed class ConstantBuffer : Buffer
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="data">The data that should be copied into the constant buffer.</param>
		/// <param name="size">The size of the constant buffer.</param>
		private ConstantBuffer(GraphicsDevice graphicsDevice, IntPtr data, int size)
			: base(graphicsDevice, BufferType.ConstantBuffer, ResourceUsage.Dynamic, data, size)
		{
		}

		/// <summary>
		///   Binds the buffer to the given slot.
		/// </summary>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		public void Bind(int slot)
		{
			Assert.NotDisposed(this);
			Assert.InRange(slot, 0, GraphicsDevice.ConstantBufferSlotCount);

			if (DeviceState.ConstantBuffers[slot] == this)
				return;

			DeviceState.ConstantBuffers[slot] = this;
			BindBuffer(slot);
		}

		/// <summary>
		///   Copies the given data to the buffer.
		/// </summary>
		/// <param name="data">The data that should be copied into the buffer.</param>
		/// <param name="size">The size of the data that should be copied in bytes.</param>
		public void SetData(IntPtr data, int size)
		{
			CopyData(data, size);
		}

		/// <summary>
		///   Creates a constant buffer sized to and initialized with the given data.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be stored in the constant buffer.</typeparam>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="data">The data that should be copied into the buffer.</param>
		public static ConstantBuffer Create<T>(GraphicsDevice graphicsDevice, T data)
			where T : struct
		{
			return data.UsePointer(ptr => new ConstantBuffer(graphicsDevice, ptr, data.Size()));
		}
	}
}