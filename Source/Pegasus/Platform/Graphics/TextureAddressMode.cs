using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Describes the address mode of texture lookups.
	/// </summary>
	public enum TextureAddressMode
	{
		/// <summary>
		///   Indicates the the address mode clamps the coordinates.
		/// </summary>
		Clamp = 2201,

		/// <summary>
		///   Indicates the the address mode wraps the coordinations.
		/// </summary>
		Wrap = 2202,

		/// <summary>
		///   Indicates the the address mode adds a border.
		/// </summary>
		Border = 2203
	}
}