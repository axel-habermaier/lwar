using System;

namespace Pegasus.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Binds a vertex buffer to be used by the input-assembler stage.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexInputBinding
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="vertexBuffer">The vertex buffer that stores the data.</param>
		/// <param name="format">The format of the vertex data.</param>
		/// <param name="semantics">A value indicating the usage scenario of the vertex data.</param>
		/// <param name="stride">The stride in bytes between each data element.</param>
		/// <param name="offset">
		///   The offset in bytes. The offset should point to the first byte of
		///   the first data value in the associated vertex buffer.
		/// </param>
		public VertexInputBinding(VertexBuffer vertexBuffer, VertexDataFormat format, DataSemantics semantics,
								  int stride, int offset)
			: this()
		{
			Format = format;
			Semantics = semantics;
			Stride = stride;
			Offset = offset;
			VertexBuffer = vertexBuffer.NativePtr;
		}

		/// <summary>
		///   The format of the data.
		/// </summary>
		internal VertexDataFormat Format;

		/// <summary>
		///   A value indicating the usage scenario of the vertex data.
		/// </summary>
		internal DataSemantics Semantics;

		/// <summary>
		///   The stride in bytes between each data element.
		/// </summary>
		internal int Stride;

		/// <summary>
		///   The offset in bytes. The offset should point to the first byte of
		///   the first data value in the associated vertex buffer.
		/// </summary>
		internal int Offset;

		/// <summary>
		///   The vertex buffer that stores the data.
		/// </summary>
		internal IntPtr VertexBuffer;
	}
}