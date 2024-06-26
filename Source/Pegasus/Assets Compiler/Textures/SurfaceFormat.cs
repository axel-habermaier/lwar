﻿namespace Pegasus.AssetsCompiler.Textures
{
	using System;

	/// <summary>
	///     Describes the format of a surface.
	/// </summary>
	public enum SurfaceFormat
	{
		/// <summary>
		///     Represents an uncompressed format with 8 bits of precision for the red, green, blue, and alpha channels.
		/// </summary>
		Rgba8,

		/// <summary>
		///     Represents an uncompressed format with 8 bits of precision for the red channel.
		/// </summary>
		R8,

		/// <summary>
		///     Represents an uncompressed format with 24 bits of depth precision and 8 bits of stencil precision.
		/// </summary>
		Depth24Stencil8,

		/// <summary>
		///     Represents an uncompressed floating point format with 16 bits of precision for the red, green, blue, and alpha channels.
		/// </summary>
		Rgba16F,

		/// <summary>
		///     Represents a compressed format with 5:6:5 bits of precision for the color channels, and 1 bit for the optional alpha
		///     channel.
		/// </summary>
		Bc1,

		/// <summary>
		///     Represents a compressed format with 5:6:5 bits of precision for the color channels, and 4 bits for the alpha channel.
		/// </summary>
		Bc2,

		/// <summary>
		///     Represents a compressed format with 5:6:5 bits of precision for the color channels, and 8 bits for the alpha channel.
		/// </summary>
		Bc3,

		/// <summary>
		///     Represents a compressed format with 8 bits of precision for the red channel.
		/// </summary>
		Bc4,

		/// <summary>
		///     Represents a compressed format with 8 bits of precision for the red and green channels.
		/// </summary>
		Bc5
	}
}