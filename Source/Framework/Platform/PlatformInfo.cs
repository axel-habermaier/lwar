using System;

namespace Pegasus.Framework.Platform
{
	using Graphics;
	using EndianessType = Memory.Endianess;

	/// <summary>
	///   Provides further information about the platform the application is running on.
	/// </summary>
	public static class PlatformInfo
	{
		/// <summary>
		///   The file extension used for compiled asset files.
		/// </summary>
		public const string AssetExtension = ".pca"; // Pegasus Compiled Asset

#if BigEndian
	/// <summary>
	///   Indicates whether the platform is a big or little endian architecture.
	/// </summary>
		public const EndianessType Endianess = EndianessType.Big;
#else
		/// <summary>
		///   Indicates whether the platform is a big or little endian architecture.
		/// </summary>
		public const EndianessType Endianess = EndianessType.Little;
#endif

#if Windows
		/// <summary>
		///   The type of the platform the application is running on.
		/// </summary>
		public static readonly PlatformType Platform = PlatformType.Windows;

		/// <summary>
		///   The scancode of the console key.
		/// </summary>
		public const int ConsoleKey = 41;
#elif Linux
	/// <summary>
	///   The type of the platform the application is running on.
	/// </summary>
        public const PlatformType Platform = PlatformType.Linux;

		/// <summary>
		///    The scancode of the console key.
		/// </summary>
		public const int ConsoleKey = 49;
#endif

#if DEBUG
	/// <summary>
	/// Indicates whether the application was built in debug mode.
	/// </summary>
	    public const bool IsDebug = true;
#else
		/// <summary>
		///   Indicates whether the application was built in debug mode.
		/// </summary>
		public const bool IsDebug = false;
#endif

		/// <summary>
		///   Gets the type of graphics API that is used for rendering.
		/// </summary>
		public static GraphicsApi GraphicsApi
		{
			get { return NativeLibrary.GraphicsApi; }
		}
	}
}