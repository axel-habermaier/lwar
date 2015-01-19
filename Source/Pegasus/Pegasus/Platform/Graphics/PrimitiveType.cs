namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Indicates which primitive type should be used for drawing.
	/// </summary>
	public enum PrimitiveType
	{
		/// <summary>
		///     Indicates that a list of triangles should be drawn.
		/// </summary>
		TriangleList,

		/// <summary>
		///     Indicates that a triangle strip should be drawn.
		/// </summary>
		TriangleStrip,

		/// <summary>
		///     Indicates that a list of lines should be drawn.
		/// </summary>
		LineList,

		/// <summary>
		///     Indicates that a line strip should be drawn.
		/// </summary>
		LineStrip
	}
}