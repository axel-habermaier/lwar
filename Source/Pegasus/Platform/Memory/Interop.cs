using System;

namespace Pegasus.Framework.Platform.Memory
{
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///   Provides access to certain native functions in a platform-independent way.
	/// </summary>
#if !DEBUG
	[SuppressUnmanagedCodeSecurity]
#endif
	internal static class Interop
	{
		/// <summary>
		///   Copies count bytes from the source to the destination.
		/// </summary>
		/// <param name="dest">The address of the first byte that should be written.</param>
		/// <param name="src">The address of the first byte that should be read.</param>
		/// <param name="count">The number of bytes that should be copied.</param>
#if Windows
		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
#elif Linux
		[DllImport("libc", EntryPoint = "memcpy")]
#endif
		public static extern IntPtr Copy(IntPtr dest, IntPtr src, int count);
	}
}