namespace Pegasus.Platform
{
	using System;
	using Memory;

	/// <summary>
	///     Provides further information about the platform the application is running on.
	/// </summary>
	public static class PlatformInfo
	{
		/// <summary>
		///     Indicates whether the platform is a big or little endian architecture.
		/// </summary>
		public const Endianess Endianess = 
#if BigEndian
			Memory.Endianess.Big;
#else
			Memory.Endianess.Little;
#endif

		/// <summary>
		///     The type of the platform the application is running on.
		/// </summary>
		public static readonly PlatformType Platform =
			Environment.OSVersion.Platform == PlatformID.Win32NT
				? PlatformType.Windows
				: PlatformType.Linux;

		/// <summary>
		///     Indicates whether the application was built in debug mode.
		/// </summary>
		public const bool IsDebug =
#if DEBUG
			true;
#else
			false;
#endif
	}
}