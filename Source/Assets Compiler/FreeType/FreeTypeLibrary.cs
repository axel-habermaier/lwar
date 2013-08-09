using System;

namespace Pegasus.AssetsCompiler.FreeType
{
	using System.Runtime.InteropServices;
	using Framework.Platform.Memory;

	/// <summary>
	///   Represents a freetype library object.
	/// </summary>
	internal class FreeTypeLibrary : DisposableObject
	{
#if Linux
	/// <summary>
	///   The name of the freetype dynamic link library.
	/// </summary>
		internal const string LibraryName = "libPlatform.so";
#else
		/// <summary>
		///   The name of the freetype dynamic link library.
		/// </summary>
		internal const string LibraryName = "FreeType/freetype250.x86.dll";
#endif

		/// <summary>
		///   The native freetype library object.
		/// </summary>
		private readonly IntPtr _library;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public FreeTypeLibrary()
		{
			NativeMethods.Initialize(out _library);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (_library != IntPtr.Zero)
				NativeMethods.Dispose(_library);
		}

		/// <summary>
		///   Provides access to native freetype functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Init_FreeType")]
			public static extern int Initialize(out IntPtr library);

			[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Library")]
			public static extern int Dispose(IntPtr library);
		}
	}
}