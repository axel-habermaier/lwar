namespace Pegasus.Platform.Network
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///     Raised when a network error occurred.
	/// </summary>
	public class NetworkException : Exception
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public NetworkException()
			: base(LastNetworkError)
		{
		}

		/// <summary>
		///     Gets a string describing the last network error that occurred.
		/// </summary>
		private static string LastNetworkError
		{
			get
			{
				var error = NativeMethods.GetLastNetworkError();
				if (error == IntPtr.Zero)
					return "An unknown network error occurred.";

				return Marshal.PtrToStringAnsi(error);
			}
		}

		/// <summary>
		///     Provides access to the native network functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetLastNetworkError")]
			public static extern IntPtr GetLastNetworkError();
		}
	}
}