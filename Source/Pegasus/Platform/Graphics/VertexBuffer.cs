namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using Memory;

	/// <summary>
	///     A vertex buffer holds vertex data such as position, texture coordinates, or color information for
	///     a list of vertices.
	/// </summary>
	public sealed class VertexBuffer : Buffer
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		/// <param name="data">An optional pointer to a data array that is copied to the vertex buffer.</param>
		/// <param name="size">The size in bytes of the vertex buffer.</param>
		private VertexBuffer(ResourceUsage usage, IntPtr data, int size)
			: base(BufferType.VertexBuffer, usage, data, size)
		{
		}

		/// <summary>
		///     Creates a vertex buffer that is large enough to hold the given data.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be stored in the vertex buffer.</typeparam>
		/// <param name="data">The data that should be copied into the buffer.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		public static VertexBuffer Create<T>(T[] data,
											 ResourceUsage usage = ResourceUsage.Static)
			where T : struct
		{
			Assert.ArgumentNotNull(data);
			Assert.ArgumentSatisfies(data.Length > 0, "The data array must not be empty.");

			return data.UsePointer(ptr => new VertexBuffer(usage, ptr, data.Size()));
		}

		/// <summary>
		///     Creates a vertex buffer that is large enough to hold the given data.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be stored in the vertex buffer.</typeparam>
		/// <param name="elementCount">The number of elements of type T that the buffer should be able to store.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		public static VertexBuffer Create<T>(int elementCount, ResourceUsage usage = ResourceUsage.Static)
			where T : struct
		{
			Assert.ArgumentInRange(elementCount, 0, Int32.MaxValue);
			return new VertexBuffer(usage, IntPtr.Zero, Marshal.SizeOf(typeof(T)) * elementCount);
		}
	}
}