namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Indicates the color channels for which writes are enabled.
	/// </summary>
	[Flags]
	public enum ColorWriteChannels
	{
		/// <summary>
		///   Indicates that all writes are disabled.
		/// </summary>
		None = 0,

		/// <summary>
		///   Indicates that writes to the red channel are enabled.
		/// </summary>
		Red = 1,

		/// <summary>
		///   Indicates that writes to the green channel are enabled.
		/// </summary>
		Green = 2,

		/// <summary>
		///   Indicates that writes to the blue channel are enabled.
		/// </summary>
		Blue = 4,

		/// <summary>
		///   Indicates that writes to the alpha channel are enabled.
		/// </summary>
		Alpha = 8,

		/// <summary>
		///   Indicates that writes to all channels are enabled.
		/// </summary>
		All = Red | Green | Blue | Alpha
	}
}