namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the data format of a vertex element.
	/// </summary>
	public enum VertexDataFormat
	{
		/// <summary>
		///     Indicates that the vertex data is a list of one-component float values.
		/// </summary>
		Single,

		/// <summary>
		///     Indicates that the vertex data is a list of two-component float values.
		/// </summary>
		Vector2,

		/// <summary>
		///     Indicates that the vertex data is a list of three-component float values.
		/// </summary>
		Vector3,

		/// <summary>
		///     Indicates that the vertex data is a list of four-component float values.
		/// </summary>
		Vector4,

		/// <summary>
		///     Indicates that the vertex data is a list of four-component color values.
		/// </summary>
		Color
	}
}