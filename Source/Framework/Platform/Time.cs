using System;

namespace Pegasus.Framework.Platform
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a time value.
	/// </summary>
	public struct Time
	{
		/// <summary>
		///   Gets or sets the offset in seconds that is applied to the time.
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		///   Gets the current time in seconds.
		/// </summary>
		public double Seconds
		{
			get { return NativeMethods.GetTime() + Offset; }
		}

		/// <summary>
		///   Gets the current time in milliseconds.
		/// </summary>
		public double Milliseconds
		{
			get { return Seconds * 1000; }
		}

		/// <summary>
		///   Provides access to the native function.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetTime")]
			public static extern double GetTime();
		}
	}
}