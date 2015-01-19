namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the address mode of texture lookups.
	/// </summary>
	public enum TextureAddressMode
	{
		/// <summary>
		///     Indicates the the address mode wraps the coordinations.
		/// </summary>
		Wrap,

		/// <summary>
		///     Indicates the the address mode clamps the coordinates.
		/// </summary>
		Clamp,

		/// <summary>
		///     Indicates the the address mode adds a border.
		/// </summary>
		Border
	}
}