namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Describes the properties of a vertex layout.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct VertexLayoutDescription
	{
		/// <summary>
		///     The vertex bindings of the vertex layout.
		/// </summary>
		internal VertexBinding* Bindings;

		/// <summary>
		///     The number of vertex bindings of the vertex layout.
		/// </summary>
		internal int BindingsCount;

		/// <summary>
		///     The (optional) index buffer of the vertex layout.
		/// </summary>
		internal void* IndexBuffer;

		/// <summary>
		///     The offset into the index buffer.
		/// </summary>
		internal int IndexOffset;

		/// <summary>
		///     The size of the indices.
		/// </summary>
		internal IndexSize IndexSize;

		/// <summary>
		///     The vertex layout signature.
		/// </summary>
		internal byte* Signature;

		/// <summary>
		///     The length of the vertex layout signature.
		/// </summary>
		internal int SignatureLength;
	}
}