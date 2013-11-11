namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Indicates which primitive type should be used for drawing.
	/// </summary>
	public enum PrimitiveType
	{
		/// <summary>
		///   Indicates that a list of triangles should be drawn.
		/// </summary>
		Triangles = 1801,

		/// <summary>
		///   Indicates that a triangle strip should be drawn.
		/// </summary>
		TriangleStrip = 1802,

		/// <summary>
		///   Indicates that a list of lines should be drawn.
		/// </summary>
		Lines = 1803,

		/// <summary>
		///   Indicates that a line strip should be drawn.
		/// </summary>
		LineStrip = 1804
	}
}