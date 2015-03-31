namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the type of a buffer.
	/// </summary>
	public enum BufferType
	{
		/// <summary>
		///     Indicates that a buffer is a constant buffer.
		/// </summary>
		ConstantBuffer,

		/// <summary>
		///     Indicates that a buffer is a vertex buffer.
		/// </summary>
		VertexBuffer,

		/// <summary>
		///     Indicates that a buffer is an index buffer.
		/// </summary>
		IndexBuffer
	}
}