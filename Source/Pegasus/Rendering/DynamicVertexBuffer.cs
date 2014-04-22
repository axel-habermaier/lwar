namespace Pegasus.Rendering
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Represents a dynamic vertex buffer that is split into several chunks. Each chunk is of the requested size; therefore,
	///     the amount of memory allocated on the GPU is the product of the chunk size and the number of chunks. The chunks are
	///     updated in a round-robin fashion, updating only those parts of the vertex buffer that the GPU is currently not using.
	///     This frees the GPU driver from copying the buffer, which sometimes causes noticeable hiccups.
	///     When the dynamic vertex buffer runs out of chunks that it can safely update, it introduces a CPU/GPU synchronization
	///     point.
	/// </summary>
	public class DynamicVertexBuffer : DisposableObject
	{
		/// <summary>
		///     The number of chunks that the dynamic vertex buffer uses.
		/// </summary>
		private int _chunkCount;

		/// <summary>
		///     The size of a single chunk in bytes.
		/// </summary>
		private int _chunkSize;

		/// <summary>
		///     The index of the current chunk that the dynamic vertex buffer uses for mapping and drawing operations.
		/// </summary>
		private int _currentChunk = -1;

		/// <summary>
		///     The number of elements stored in each chunk.
		/// </summary>
		private int _elementCount;

		/// <summary>
		///     Gets the underlying vertex buffer. The returned buffer should only be used to initialize vertex layouts;
		///     mapping the buffer will most likely result in some unexpected behavior.
		/// </summary>
		public VertexBuffer Buffer { get; private set; }

		/// <summary>
		///     Gets the vertex offset that must be applied to a drawing operation when the data of the last buffer mapping operation
		///     should be drawn.
		/// </summary>
		public int VertexOffset
		{
			get { return _currentChunk * _elementCount; }
		}

		/// <summary>
		///     Sets the debug name of the dynamic vertex buffer.
		/// </summary>
		/// <param name="name">The debug name of the vertex buffer.</param>
		[Conditional("DEBUG")]
		public void SetName(string name)
		{
			Buffer.SetName(name);
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <typeparam name="T">The type of the data stored in the vertex buffer.</typeparam>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="elementCount">The number of elements of type T that the buffer should be able to store.</param>
		/// <param name="chunkCount">The number of chunks that should be allocated.</param>
		public static DynamicVertexBuffer Create<T>(GraphicsDevice graphicsDevice, int elementCount, int chunkCount)
			where T : struct
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentInRange(elementCount, 1, UInt16.MaxValue);
			Assert.ArgumentInRange(chunkCount, 2, Byte.MaxValue);

			return new DynamicVertexBuffer
			{
				Buffer = VertexBuffer.Create<T>(graphicsDevice, elementCount * chunkCount, ResourceUsage.Dynamic),
				_chunkCount = chunkCount,
				_elementCount = elementCount,
				_chunkSize = elementCount * Marshal.SizeOf(typeof(T))
			};
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Buffer.SafeDispose();
		}

		/// <summary>
		///     Maps the next chunk of the buffer and returns a pointer to the first byte of the chunk.
		/// </summary>
		public IntPtr Map()
		{
			_currentChunk = (_currentChunk + 1) % _chunkCount;
			return Buffer.MapRange(MapMode.WriteNoOverwrite, _currentChunk * _chunkSize, _chunkSize);
		}

		/// <summary>
		///     Unmaps the buffer.
		/// </summary>
		public void Unmap()
		{
			Buffer.Unmap();
		}
	}
}