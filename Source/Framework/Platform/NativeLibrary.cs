using System;

namespace Pegasus.Framework.Platform
{
	using System.Runtime.InteropServices;
	using System.Security;
	using Graphics;

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
		///   The log callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallbacks _logCallbacks = new NativeMethods.LogCallbacks
		{
			Die = s => Log.Die(EscapeBraces(s)),
			Error = s => Log.Error(EscapeBraces(s)),
			Warning = s => Log.Warn(EscapeBraces(s)),
			Info = s => Log.Info(EscapeBraces(s)),
			Debug = s => Log.DebugInfo(EscapeBraces(s))
		};

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public NativeLibrary()
		{
			Assert.That(!_isInitialized, "The library has already been initialized.");

			Log.Info("Initializing native platform library...");
			NativeMethods.Initialize(ref _logCallbacks);
			_isInitialized = true;
		}

		/// <summary>
		///   Escape curly braces that might be contained in a string in order to be able to pass it to the log functions.
		/// </summary>
		/// <param name="s">The string that should be escaped.</param>
		private static string EscapeBraces(string s)
		{
			return s.Replace("{", "{{").Replace("}", "}}");
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
		///   Provides access to the native platform types and functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			public delegate void LogCallback(string message);

			[DllImport(LibraryName, EntryPoint = "pgInitialize")]
			public static extern void Initialize(ref LogCallbacks callbacks);

			[DllImport(LibraryName, EntryPoint = "pgShutdown")]
			public static extern void Shutdown();

			[DllImport(LibraryName, EntryPoint = "pgGetGraphicsApi")]
			public static extern GraphicsApi GetGraphicsApi();

			[StructLayout(LayoutKind.Sequential)]
			public struct LogCallbacks
			{
				public LogCallback Die;
				public LogCallback Error;
				public LogCallback Warning;
				public LogCallback Info;
				public LogCallback Debug;
			}
		}
	}
}