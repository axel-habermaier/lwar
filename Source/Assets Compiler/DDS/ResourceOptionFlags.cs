using System;

namespace Pegasus.AssetsCompiler.DDS
{
	/// <summary>
	///   The resource option flags.
	/// </summary>
	[Flags]
	public enum ResourceOptionFlags
	{
		None = 0,
		GenerateMipMaps = 1,
		Shared = 2,
		TextureCube = 4,
		DrawIndirectArguments = 16,
		BufferAllowRawViews = 32,
		BufferStructured = 64,
		ResourceClamp = 128,
		SharedKeyedmutex = 256,
		GdiCompatible = 512,
	}
}