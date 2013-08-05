using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;
	using Graphics;
	using Logging;
	using Memory;

	/// <summary>
	///   Manages the initialization and shutdown of the native platform library.
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
		///   The name of the native Pegasus.Platform library.
		/// </summary>
		internal const string LibraryName = "Pegasus.Platform.dll";
#endif

		/// <summary>
		///   Indicates whether the library is already initialized.
		/// </summary>
		private static bool _isInitialized;

		/// <summary>
		///   Gets the graphics API that is used by the library.
		/// </summary>
		public static GraphicsApi GraphicsApi
		{
			get { return NativeMethods.GetGraphicsApi(); }
		}

		/// <summary>
		///   The log callback that has been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegate while it is still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallback _logCallback;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public NativeLibrary()
		{
			Assert.That(!_isInitialized, "The library has already been initialized.");

			_logCallback = OnLoggedMessage;

			Log.Info("Initializing native platform library...");
			NativeMethods.Initialize(_logCallback);

			_isInitialized = true;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Log.Info("Shutting down native platform library...");
			NativeMethods.Shutdown();
			_isInitialized = false;
		}

		/// <summary>
		///   Invoked when the native library generates a log entry.
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
		///   Provides access to the native platform types and functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			public delegate void LogCallback(LogType type, string message);

			[DllImport(LibraryName, EntryPoint = "pgInitialize")]
			public static extern void Initialize(LogCallback callback);

			[DllImport(LibraryName, EntryPoint = "pgShutdown")]
			public static extern void Shutdown();

			[DllImport(LibraryName, EntryPoint = "pgGetGraphicsApi")]
			public static extern GraphicsApi GetGraphicsApi();
		}
	}
}