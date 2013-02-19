using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Describes the format of a surface.
	/// </summary>
	public enum SurfaceFormat
	{
		/// <summary>
		///   Represents an uncompressed format with 8 bits of precision for the red, green, blue, and alpha channels.
		/// </summary>
		Rgba8 = 2101,

		/// <summary>
		///   Represents an uncompressed format with 8 bits of precision for the red, green, and blue channels.
		/// </summary>
		Rgb8 = 2102,

		/// <summary>
		///   Represents an uncompressed format with 8 bits of precision for the red and green channels.
		/// </summary>
		Rg8 = 2103,

		/// <summary>
		///   Represents an uncompressed format with 8 bits of precision for the red channel.
		/// </summary>
		R8 = 2104,

		/// <summary>
		///   Represents a compressed format with 5:6:5 bits of precision for the color channels, and 1 bit for the optional alpha
		///   channel.
		/// </summary>
		Bc1 = 2105,

		/// <summary>
		///   Represents a compressed format with 5:6:5 bits of precision for the color channels, and 4 bits for the alpha channel.
		/// </summary>
		Bc2 = 2106,

		/// <summary>
		///   Represents a compressed format with 5:6:5 bits of precision for the color channels, and 8 bits for the alpha channel.
		/// </summary>
		Bc3 = 2107,

		/// <summary>
		///   Represents a compressed format with 8 bits of precision for the red channel.
		/// </summary>
		Bc4 = 2108,

		/// <summary>
		///   Represents a compressed format with 8 bits of precision for the red and green channels.
		/// </summary>
		Bc5 = 2109,
	}
}