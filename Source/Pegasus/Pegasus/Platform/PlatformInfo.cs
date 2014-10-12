namespace Pegasus.Platform
{
	using System;
	using Graphics;
	using Memory;

	//using EndianessType = Memory.Endianess;

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
		public const ushort AssetFileVersion = 4;

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
		public const PlatformType Platform =
#if Windows
			PlatformType.Windows;
#elif Linux
			PlatformType.Linux;
#endif

		/// <summary>
		///     The scan code of the console key.
		/// </summary>
		internal const int ConsoleKey =
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
		public const GraphicsApi GraphicsApi =
#if Direct3D11
			Graphics.GraphicsApi.Direct3D11;
#else
			Graphics.GraphicsApi.OpenGL3;
#endif
	}
}