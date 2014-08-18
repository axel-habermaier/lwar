namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a pointer to a byte buffer.
	/// </summary>
	public unsafe class BufferPointer : IDisposable
	{
		/// <summary>
		///     The handle of the pinned byte array.
		/// </summary>
		private GCHandle _handle;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer the pointer should point to.</param>
		/// <param name="offset">The offset of the first byte the pointer should point to.</param>
		/// <param name="size">The size in bytes of the buffer.</param>
		public BufferPointer(byte[] buffer, int offset, int size)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentInRange(offset, 0, buffer.Length - 1);
			Assert.ArgumentInRange(size, 1, buffer.Length - offset);

			_handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			Pointer = (byte*)_handle.AddrOfPinnedObject() + offset;
			Size = size;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer the pointer should point to.</param>
		public BufferPointer(byte[] buffer)
			: this(buffer, 0, buffer.Length)
		{
		}

		/// <summary>
		///     The pointer to the beginning of the buffer.
		/// </summary>
		public byte* Pointer { get; private set; }

		/// <summary>
		///     The size of the buffer in bytes.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		public void Dispose()
		{
			if (_handle.IsAllocated)
				_handle.Free();
		}
	}
}