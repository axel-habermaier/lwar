using System;

namespace Pegasus.AssetsCompiler.DDS
{
	/// <summary>
	///   The pixel format flags.
	/// </summary>
	[Flags]
	public enum PixelFormatFlags
	{
		FourCC = 0x00000004,
		Rgb = 0x00000040,
		Rgba = 0x00000041,
		Luminance = 0x00020000,
		LuminanceAlpha = 0x00020001,
		Alpha = 0x00000002,
		Pal8 = 0x00000020,
	}
}