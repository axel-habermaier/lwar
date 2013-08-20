using System;

namespace Pegasus.Framework.Platform.Memory
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a pointer to a byte buffer.
	/// </summary>
	public unsafe class BufferPointer : PooledObject<BufferPointer>
	{
		/// <summary>
		///   The handle of the pinned byte array.
		/// </summary>
		private GCHandle _handle;

		/// <summary>
		///   The pointer to the beginning of the buffer.
		/// </summary>
		public byte* Pointer { get; private set; }

		/// <summary>
		///   The size of the buffer in bytes.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer the pointer should point to.</param>
		public static BufferPointer Create(byte[] buffer)
		{
			return Create(buffer, 0, buffer.Length);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The buffer the pointer should point to.</param>
		/// <param name="offset">The offset of the first byte the pointer should point to.</param>
		/// <param name="size">The size in bytes of the buffer.</param>
		public static BufferPointer Create(byte[] buffer, int offset, int size)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentInRange(offset, 0, buffer.Length - 1);
			Assert.ArgumentInRange(size, 1, buffer.Length - offset);

			var bufferPointer = GetInstance();
			bufferPointer._handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			bufferPointer.Pointer = (byte*)bufferPointer._handle.AddrOfPinnedObject() + offset;
			bufferPointer.Size = size;
			return bufferPointer;
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			if (_handle.IsAllocated)
				_handle.Free();
		}
	}
}