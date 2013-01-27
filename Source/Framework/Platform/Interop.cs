using System;

namespace Pegasus.Framework.Platform
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Provides access to certain native functions in a platform-independent way.
	/// </summary>
#if !DEBUG
	[System.Security.SuppressUnmanagedCodeSecurity]
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

		/// <summary>
		///   Converts the given function pointer into a delegate instance of the given type.
		/// </summary>
		/// <typeparam name="TDelegate">The type of the function.</typeparam>
		/// <param name="function">The function pointer that should be converted to a delegate.</param>
		/// <returns>Returns a delegate to the function.</returns>
		public static TDelegate ConvertToDelegate<TDelegate>(IntPtr function)
			where TDelegate : class
		{
			return Marshal.GetDelegateForFunctionPointer(function, typeof(TDelegate)) as TDelegate;
		}
	}
}