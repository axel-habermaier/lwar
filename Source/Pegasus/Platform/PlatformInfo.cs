namespace Pegasus.Platform
{
	using System;
	using Graphics;
	using EndianessType = Memory.Endianess;

	/// <summary>
	///     Provides further information about the platform the application is running on.
	/// </summary>
	public static class PlatformInfo
	{
		/// <summary>
		///     The file extension used for compiled asset files.
		/// </summary>
		public const string AssetExtension = ".pg";

		/// <summary>
		///     The compiled asset file version supported by the application.
		/// </summary>
		public const ushort AssetFileVersion = 2;

		/// <summary>
		///     Indicates whether the platform is a big or little endian architecture.
		/// </summary>
		public const EndianessType Endianess = 
#if BigEndian
			EndianessType.Big;
#else
			EndianessType.Little;
#endif

		/// <summary>
		///     The type of the platform the application is running on.
		/// </summary>
		public static readonly PlatformType Platform =
#if Windows
			PlatformType.Windows;
#elif Linux
			PlatformType.Linux;
#endif

		/// <summary>
		///     The scan code of the console key.
		/// </summary>
		public const int ConsoleKey =
#if Windows
			41;
#elif Linux
			49;
#endif

		/// <summary>
		///     Indicates whether the application was built in debug mode.
		/// </summary>
		public const bool IsDebug =
#if DEBUG
			true;
#else
			false;
#endif

		/// <summary>
		///     Gets the type of graphics API that is used for rendering.
		/// </summary>
		public static GraphicsApi GraphicsApi
		{
			get { return NativeLibrary.GraphicsApi; }
		}
	}
}