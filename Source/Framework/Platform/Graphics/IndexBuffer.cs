using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;
	using Memory;

	/// <summary>
	///   An index buffer holds indices that are used to index into vertex buffers.
	/// </summary>
	public sealed class IndexBuffer : Buffer
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		/// <param name="indices">An optional pointer to a data array that is copied to the index buffer.</param>
		/// <param name="size">The size in bytes of the index buffer.</param>
		/// <param name="indexSize">The size of the indices.</param>
		private IndexBuffer(GraphicsDevice graphicsDevice, ResourceUsage usage, IntPtr indices, int size,
							IndexSize indexSize)
			: base(graphicsDevice, BufferType.IndexBuffer, usage, indices, size)
		{
			IndexSize = indexSize;
		}

		/// <summary>
		///   Gets the size of the indices.
		/// </summary>
		public IndexSize IndexSize { get; private set; }

		/// <summary>
		///   Creates an index buffer that is large enough to hold the given indices.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be stored in the index buffer.</typeparam>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="indices">The indices that should be copied into the buffer.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		public static IndexBuffer Create<T>(GraphicsDevice graphicsDevice, T[] indices,
											ResourceUsage usage = ResourceUsage.Static)
			where T : struct
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(indices, () => indices);
			Assert.ArgumentSatisfies(indices.Length > 0, () => indices, "The index array must not be empty.");
			Assert.That(typeof(T) == typeof(uint) || typeof(T) == typeof(ushort),
						"Unsupported index type '{0}'. Indices must be of type uint or ushort.", typeof(T).FullName);

			return
				indices.UsePointer(ptr => new IndexBuffer(graphicsDevice, usage, ptr, indices.Size(), GetIndexSize<T>()));
		}

		/// <summary>
		///   Creates an index buffer that is large enough to hold the given data.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be stored in the index buffer.</typeparam>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="indexCount">The number of indices of type T that the buffer should be able to store.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		public static IndexBuffer Create<T>(GraphicsDevice graphicsDevice, int indexCount,
											ResourceUsage usage = ResourceUsage.Static)
			where T : struct
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentInRange(indexCount, () => indexCount, 0, typeof(T) == typeof(uint) ? Int32.MaxValue : UInt16.MaxValue);
			Assert.That(typeof(T) == typeof(uint) || typeof(T) == typeof(ushort),
						"Unsupported index type '{0}'. Indices must be of type uint or ushort.", typeof(T).FullName);

			return new IndexBuffer(graphicsDevice, usage, IntPtr.Zero, Marshal.SizeOf(typeof(T)) * indexCount,
								   GetIndexSize<T>());
		}

		/// <summary>
		///   Gets the index size from the given type.
		/// </summary>
		private static IndexSize GetIndexSize<T>()
		{
			if (typeof(T) == typeof(uint))
				return IndexSize.ThirtyTwoBits;

			return IndexSize.SixteenBits;
		}
	}
}