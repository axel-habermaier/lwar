namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the properties of a vertex layout.
	/// </summary>
	internal struct VertexLayoutDescription
	{
		/// <summary>
		///     The vertex bindings of the vertex layout.
		/// </summary>
		public VertexBinding[] Bindings;

		/// <summary>
		///     The (optional) index buffer of the vertex layout.
		/// </summary>
		public IndexBuffer IndexBuffer;

		/// <summary>
		///     The offset into the index buffer.
		/// </summary>
		public int IndexOffset;

		/// <summary>
		///     The signature shader (optional depending on the underlying graphics API).
		/// </summary>
		public byte[] ShaderSignature;
	}
}