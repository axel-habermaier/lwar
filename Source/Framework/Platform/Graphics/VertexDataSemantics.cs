using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Describes the usage scenario of an input element.
	/// </summary>
	public enum VertexDataSemantics
	{
		/// <summary>
		///   Indicates that the vertex data describes positions.
		/// </summary>
		Position = 200,

		/// <summary>
		///   Indicates that the vertex data describes colors.
		/// </summary>
		Color = 201,

		/// <summary>
		///   Indicates that the vertex data describes texture coordinates.
		/// </summary>
		TextureCoordinate = 202,

		/// <summary>
		///   Indicates that the vertex data describes normals.
		/// </summary>
		Normal = 203,

		/// <summary>
		///   Indicates that the vertex data describes binormals.
		/// </summary>
		Binormal = 204,

		/// <summary>
		///   Indicates that the vertex data describes tangents.
		/// </summary>
		Tangent = 205,

		/// <summary>
		///   Indicates that the vertex data describes blend indices.
		/// </summary>
		BlendIndices = 206,

		/// <summary>
		///   Indicates that the vertex data describes blend weights.
		/// </summary>
		BlendWeight = 207,

		/// <summary>
		///   Indicates that the vertex data describes depth values.
		/// </summary>
		Depth = 208,
	}
}