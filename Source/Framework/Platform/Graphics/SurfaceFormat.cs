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
		///   Represents an uncompressed format with 8 bits of precision for the red channel.
		/// </summary>
		R8 = 2102,

		/// <summary>
		///   Represents an uncompressed format with 24 bits of depth precision and 8 bits of stencil precision.
		/// </summary>
		Depth24Stencil8 = 2103,

		/// <summary>
		///   Represents an uncompressed floating point format with 16 bits of precision for the red, green, blue, and alpha
		///   channels.
		/// </summary>
		Rgba16F = 2121,

		/// <summary>
		///   Represents a compressed format with 5:6:5 bits of precision for the color channels, and 1 bit for the optional alpha
		///   channel.
		/// </summary>
		Bc1 = 2151,

		/// <summary>
		///   Represents a compressed format with 5:6:5 bits of precision for the color channels, and 4 bits for the alpha channel.
		/// </summary>
		Bc2 = 2152,

		/// <summary>
		///   Represents a compressed format with 5:6:5 bits of precision for the color channels, and 8 bits for the alpha channel.
		/// </summary>
		Bc3 = 2153,

		/// <summary>
		///   Represents a compressed format with 8 bits of precision for the red channel.
		/// </summary>
		Bc4 = 2154,

		/// <summary>
		///   Represents a compressed format with 8 bits of precision for the red and green channels.
		/// </summary>
		Bc5 = 2155
	}
}