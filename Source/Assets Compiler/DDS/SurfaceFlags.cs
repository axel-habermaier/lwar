using System;

namespace Pegasus.AssetsCompiler.DDS
{
	/// <summary>
	///   The surface flags.
	/// </summary>
	[Flags]
	public enum SurfaceFlags
	{
		Texture = 0x00001000,
		Mipmap = 0x00400008,
		Cubemap = 0x00000008,
	}
}