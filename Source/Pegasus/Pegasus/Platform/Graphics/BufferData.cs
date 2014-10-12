namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using Memory;

	/// <summary>
	///     Provides access to the data of a mapped buffer.
	/// </summary>
	public struct BufferData : IDisposable
	{
		/// <summary>
		///     The buffer that is mapped.
		/// </summary>
		private readonly Buffer _buffer;

		/// <summary>
		///     The pointer to the mapped buffer data.
		/// </summary>
		private IntPtr _pointer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer that should be mapped.</param>
		/// <param name="pointer">The pointer to the mapped buffer data.</param>
		internal BufferData(Buffer buffer, IntPtr pointer)
			: this()
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(pointer);

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
			_pointer = IntPtr.Zero;
		}

		/// <summary>
		///     Writes the given data to the mapped buffer.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be written.</typeparam>
		/// <param name="data">The data that should be written.</param>
		/// <param name="offsetInBytes">The offset in bytes into the buffer where the data should be written.</param>
		/// <param name="count">The number of elements that should be written.</param>
		public void Write<T>(T[] data, int offsetInBytes, int count)
			where T : struct
		{
			Assert.ArgumentNotNull(data);
			Assert.ArgumentSatisfies(offsetInBytes < _buffer.SizeInBytes, "Invalid offset.");
			Assert.ArgumentSatisfies(count >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(count <= data.Length, "Attempted to read outside the bounds of the data array.");
			Assert.ArgumentSatisfies(offsetInBytes + count * Marshal.SizeOf(typeof(T)) <= _buffer.SizeInBytes,
				"Attempted to write outside the bounds of the buffer.");

			using (var ptr = PinnedPointer.Create(data))
				Write(ptr, offsetInBytes, Marshal.SizeOf(typeof(T)) * count);
		}

		/// <summary>
		///     Writes the given data to the mapped buffer.
		/// </summary>
		/// <param name="data">The data that should be written.</param>
		/// <param name="offsetInBytes">The offset in bytes into the buffer where the data should be written.</param>
		/// <param name="count">The number of elements that should be written.</param>
		public void Write(float[] data, int offsetInBytes, int count)
		{
			Assert.ArgumentNotNull(data);
			Assert.ArgumentSatisfies(offsetInBytes < _buffer.SizeInBytes, "Invalid offset.");
			Assert.ArgumentSatisfies(count >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(count <= data.Length, "Attempted to read outside the bounds of the data array.");
			Assert.ArgumentSatisfies(offsetInBytes + count * sizeof(float) <= _buffer.SizeInBytes,
				"Attempted to write outside the bounds of the buffer.");

			Marshal.Copy(data, 0, _pointer + offsetInBytes, count);
		}

		/// <summary>
		///     Writes the given data to the mapped buffer.
		/// </summary>
		/// <param name="data">The data that should be written.</param>
		/// <param name="offsetInBytes">The offset into the buffer where the new data should be written to.</param>
		/// <param name="size">The size of the data that should be written in bytes.</param>
		public void Write(IntPtr data, int offsetInBytes, int size)
		{
			Assert.ArgumentNotNull(data);
			Assert.ArgumentSatisfies(size >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(offsetInBytes >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(size <= _buffer.SizeInBytes - offsetInBytes, "Attempted to write outside the bounds of the buffer.");
			Assert.NotNull(_pointer);

			NativeLibrary.Copy(_pointer + offsetInBytes, data, size);
		}
	}
}