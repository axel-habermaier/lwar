namespace Pegasus.Platform
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.Security.Cryptography;
	using Logging;
	using Memory;

	/// <summary>
	///     Manages the initialization and shutdown of the native platform library.
	/// </summary>
	internal class NativeLibrary : DisposableObject
	{
#if Linux
		/// <summary>
		///   The name of the native Pegasus.Platform library.
		/// </summary>
		internal const string LibraryName = "libPlatform.so";
#else
		/// <summary>
		///     The name of the native Pegasus.Platform library.
		/// </summary>
		internal const string LibraryName = "Platform.dll";
#endif

		/// <summary>
		///     Indicates whether the library is already initialized.
		/// </summary>
		private static bool _isInitialized;

		/// <summary>
		///     The log callback that has been passed to the native code. We must keep a reference in order to prevent
		///     the garbage collector from freeing the delegate while it is still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallback _logCallback;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		public NativeLibrary(string appName)
		{
			Assert.ArgumentNotNullOrWhitespace(appName);
			Assert.That(!_isInitialized, "The library has already been initialized.");

			_logCallback = OnLoggedMessage;

			NativeMethods.Initialize(_logCallback, appName);
			_isInitialized = true;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.Shutdown();
			_isInitialized = false;
		}

		/// <summary>
		///     Shows a message box containing the given text. Only supported on Windows.
		/// </summary>
		/// <param name="caption">The caption of the message box.</param>
		/// <param name="text">The text that should be displayed in the message box.</param>
		/// <param name="arguments">The arguments for the text format string.</param>
		[Conditional("Windows"), StringFormatMethod("text")]
		public static void ShowMessageBox(string caption, string text, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(caption);
			Assert.ArgumentNotNullOrWhitespace(text);

			NativeMethods.ShowMessageBox(caption, String.Format(text, arguments));
		}

		/// <summary>
		///     Copies given number of bytes from the source to the destination.
		/// </summary>
		/// <param name="destination">The address of the first byte that should be written.</param>
		/// <param name="source">The address of the first byte that should be read.</param>
		/// <param name="byteCount">The number of bytes that should be copied.</param>
		public static void Copy(IntPtr destination, IntPtr source, int byteCount)
		{
			Assert.ArgumentNotNull(destination);
			Assert.ArgumentNotNull(source);

			NativeMethods.MemCopy(destination, source, byteCount);
		}

		/// <summary>
		///     Invoked when the native library generates a log entry.
		/// </summary>
		/// <param name="type">The type of the generated log entry.</param>
		/// <param name="message">The message that has been generated.</param>
		[DebuggerHidden]
		private static void OnLoggedMessage(LogType type, string message)
		{
			Assert.InRange(type);
			Assert.ArgumentNotNullOrWhitespace(message);

			var logEntry = new LogEntry(type, message);
			logEntry.RaiseLogEvent();
		}

		/// <summary>
		///     Provides access to the native platform types and functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			public delegate void LogCallback(LogType type, string message);

			[DllImport(LibraryName, EntryPoint = "pgInitialize")]
			public static extern void Initialize(LogCallback callback, [MarshalAs(UnmanagedType.LPStr)] string appName);

			[DllImport(LibraryName, EntryPoint = "pgMemCopy")]
			public static extern void MemCopy(IntPtr destination, IntPtr source, int byteCount);

			[DllImport(LibraryName, EntryPoint = "pgShutdown")]
			public static extern void Shutdown();

			[DllImport(LibraryName, EntryPoint = "pgShowMessageBox")]
			public static extern void ShowMessageBox(string caption, string message);
		}
	}
}