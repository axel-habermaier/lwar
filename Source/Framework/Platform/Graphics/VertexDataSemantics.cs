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
		Position = 2500,

		/// <summary>
		///   Indicates that the vertex data describes colors.
		/// </summary>
		Color = 2501,

		/// <summary>
		///   Indicates that the vertex data describes texture coordinates.
		/// </summary>
		TextureCoordinate = 2502,

		/// <summary>
		///   Indicates that the vertex data describes normals.
		/// </summary>
		Normal = 2503,

		/// <summary>
		///   Indicates that the vertex data describes binormals.
		/// </summary>
		Binormal = 2504,

		/// <summary>
		///   Indicates that the vertex data describes tangents.
		/// </summary>
		Tangent = 2505,

		/// <summary>
		///   Indicates that the vertex data describes blend indices.
		/// </summary>
		BlendIndices = 2506,

		/// <summary>
		///   Indicates that the vertex data describes blend weights.
		/// </summary>
		BlendWeight = 2507,

		/// <summary>
		///   Indicates that the vertex data describes depth values.
		/// </summary>
		Depth = 2508,
	}
}