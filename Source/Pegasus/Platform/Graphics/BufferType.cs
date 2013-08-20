using System;

namespace Pegasus.Platform.Graphics
{
	/// <summary>
	///   Describes the type of a buffer.
	/// </summary>
	public enum BufferType
	{
		/// <summary>
		///   Indicates that a buffer is a constant buffer.
		/// </summary>
		ConstantBuffer = 2701,

		/// <summary>
		///   Indicates that a buffer is a vertex buffer.
		/// </summary>
		VertexBuffer = 2702,

		/// <summary>
		///   Indicates that a buffer is an index buffer.
		/// </summary>
		IndexBuffer = 2703
	}
}