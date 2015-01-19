namespace Pegasus.Platform.Graphics
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     Provides access to the data of a mapped buffer.
	/// </summary>
	public unsafe struct BufferData : IDisposable
	{
		/// <summary>
		///     The buffer that is mapped.
		/// </summary>
		private readonly Buffer _buffer;

		/// <summary>
		///     The pointer to the mapped buffer data.
		/// </summary>
		private void* _pointer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer that should be mapped.</param>
		/// <param name="pointer">The pointer to the mapped buffer data.</param>
		internal BufferData(Buffer buffer, void* pointer)
			: this()
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(new IntPtr(pointer));

			_buffer = buffer;
			_pointer = pointer;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Assert.NotNull(_buffer);

			_buffer.Unmap();
			_pointer = null;
		}

		/// <summary>
		///     Writes the given data to the mapped buffer.
		/// </summary>
		/// <param name="data">The data that should be written.</param>
		/// <param name="offsetInBytes">The offset into the buffer where the new data should be written to.</param>
		/// <param name="sizeInBytes">The size of the data that should be written in bytes.</param>
		public void Write(IntPtr data, int offsetInBytes, int sizeInBytes)
		{
			Write((void*)data, offsetInBytes, sizeInBytes);
		}

		/// <summary>
		///     Writes the given data to the mapped buffer.
		/// </summary>
		/// <param name="data">The data that should be written.</param>
		/// <param name="offsetInBytes">The offset into the buffer where the new data should be written to.</param>
		/// <param name="sizeInBytes">The size of the data that should be written in bytes.</param>
		public void Write(void* data, int offsetInBytes, int sizeInBytes)
		{
			Assert.ArgumentNotNull(new IntPtr(data));
			Assert.ArgumentSatisfies(sizeInBytes >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(offsetInBytes >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(sizeInBytes <= _buffer.SizeInBytes - offsetInBytes, "Attempted to write outside the bounds of the buffer.");

			MemCopy.Copy((byte*)_pointer + offsetInBytes, data, sizeInBytes);
		}
	}
}