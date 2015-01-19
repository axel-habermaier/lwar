namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents SDL2 version information.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct VersionInfo
	{
		/// <summary>
		///     The major version number.
		/// </summary>
		private byte _major;

		/// <summary>
		///     The minor version number.
		/// </summary>
		private byte _minor;

		/// <summary>
		///     The patch level.
		/// </summary>
		private byte _patch;

		/// <summary>
		///     The minimum version of the SDL2 library required by the application.
		/// </summary>
		public static readonly VersionInfo Required = new VersionInfo { _major = 2, _minor = 0, _patch = 2 };

		/// <summary>
		///     Gets the actual version of the SDL2 library the application is using.
		/// </summary>
		public static VersionInfo Actual
		{
			get
			{
				VersionInfo version;
				SDL_GetVersion(out version);
				return version;
			}
		}

		/// <summary>
		///     Gets the integer representation of the given version.
		/// </summary>
		/// <param name="version">The version that should be converted to an integer.</param>
		public static implicit operator int(VersionInfo version)
		{
			return (version._major * 1000) + (version._minor * 100) + version._patch;
		}

		/// <summary>
		///     Gets a string representation of the version.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}.{1}.{2}", _major, _minor, _patch);
		}

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_GetVersion(out VersionInfo ver);
	}
}