namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;

	/// <summary>
	///     Binds a vertex buffer to be used by the input-assembler stage.
	/// </summary>
	public struct VertexBinding
	{
		/// <summary>
		///     The format of the data.
		/// </summary>
		internal readonly VertexDataFormat Format;

		/// <summary>
		///     The offset in bytes. The offset should point to the first byte of
		///     the first data value in the associated vertex buffer.
		/// </summary>
		internal readonly int Offset;

		/// <summary>
		///     A value indicating the usage scenario of the vertex data.
		/// </summary>
		internal readonly DataSemantics Semantics;

		/// <summary>
		///     The number of instances to draw using the same per-instance data before advancing in the buffer by one element.
		/// </summary>
		internal readonly uint StepRate;

		/// <summary>
		///     The stride in bytes between each data element.
		/// </summary>
		internal readonly int Stride;

		/// <summary>
		///     The vertex buffer that stores the data.
		/// </summary>
		internal readonly VertexBuffer VertexBuffer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="buffer">The vertex buffer that stores the data.</param>
		/// <param name="format">The format of the vertex data.</param>
		/// <param name="semantics">A value indicating the usage scenario of the vertex data.</param>
		/// <param name="stride">The stride in bytes between each data element.</param>
		/// <param name="offset">
		///     The offset in bytes. The offset should point to the first byte of
		///     the first data value in the associated vertex buffer.
		/// </param>
		/// <param name="stepRate">
		///     The number of instances to draw using the same per-instance data before advancing in the
		///     buffer by one element.
		/// </param>
		public VertexBinding(VertexBuffer buffer, VertexDataFormat format, DataSemantics semantics, int stride, int offset, uint stepRate = 0)
			: this()
		{
			Format = format;
			Semantics = semantics;
			Stride = stride;
			Offset = offset;
			StepRate = stepRate;
			VertexBuffer = buffer;
		}
	}
}