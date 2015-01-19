namespace Pegasus.AssetsCompiler.Effects
{
	using System;

	/// <summary>
	///     Describes the format vertex data.
	/// </summary>
	public enum VertexDataFormat
	{
		/// <summary>
		///     Indicates that the vertex data is a list of one-component float values.
		/// </summary>
		Float,

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