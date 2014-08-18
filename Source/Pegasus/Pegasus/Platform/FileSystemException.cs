namespace Pegasus.Platform
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///     Raised when a network error occurred.
	/// </summary>
	public class FileSystemException : Exception
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FileSystemException()
			: base(LastError)
		{
		}

		/// <summary>
		///     Gets a string describing the last file system error that occurred.
		/// </summary>
		private static string LastError
		{
			get
			{
				var error = NativeMethods.GetLastError();
				if (error == IntPtr.Zero)
					return "An unknown file system error occurred.";

				return Marshal.PtrToStringAnsi(error);
			}
		}

		/// <summary>
		///     Provides access to the native file system functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetLastFileError")]
			public static extern IntPtr GetLastError();
		}
	}
}