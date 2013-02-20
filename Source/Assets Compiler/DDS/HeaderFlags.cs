using System;

namespace Pegasus.AssetsCompiler.DDS
{
	/// <summary>
	///   The header flags.
	/// </summary>
	[Flags]
	public enum HeaderFlags
	{
		Texture = 0x00001007,
		Mipmap = 0x00020000,
		Volume = 0x00800000,
		Pitch = 0x00000008,
		LinearSize = 0x00080000,
		Height = 0x00000002,
		Width = 0x00000004,
	};
}