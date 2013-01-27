using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Describes the format vertex data.
	/// </summary>
	public enum VertexDataFormat
	{
		/// <summary>
		///   Indicates that the vertex data is a list of one-component float values.
		/// </summary>
		Single = 2400,

		/// <summary>
		///   Indicates that the vertex data is a list of two-component float values.
		/// </summary>
		Vector2 = 2401,

		/// <summary>
		///   Indicates that the vertex data is a list of three-component float values.
		/// </summary>
		Vector3 = 2402,

		/// <summary>
		///   Indicates that the vertex data is a list of four-component float values.
		/// </summary>
		Vector4 = 2403,

		/// <summary>
		///   Indicates that the vertex data is a list of color values.
		/// </summary>
		Color = 2404
	}
}