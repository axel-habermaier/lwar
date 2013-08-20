using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Indicates the fill mode to be used when rendering polygons.
	/// </summary>
	public enum FillMode
	{
		/// <summary>
		///   Indicates that only the wireframe should be drawn.
		/// </summary>
		Wireframe = 1501,

		/// <summary>
		///   Indicates that a solid polygon should be drawn.
		/// </summary>
		Solid = 1502
	}
}