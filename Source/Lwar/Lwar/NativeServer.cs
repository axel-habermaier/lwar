namespace Lwar
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Network;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;

	/// <summary>
	///     Represents a native server hosting a game session.
	/// </summary>
	public class NativeServer : Server
	{
		/// <summary>
		///     The log callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///     the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallbacks _logCallbacks;

		/// <summary>
		///     Indicates whether the server is currently running.
		/// </summary>
		private bool _isRunning;

		/// <summary>
		///     The number of seconds that have elapsed since the the server has been started.
		/// </summary>
		private double _time;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public NativeServer()
			: base(NetworkProtocol.DefaultServerPort)
		{
			_logCallbacks = new NativeMethods.LogCallbacks
			{
				Die = message => Log.Die("{0}", message.Trim()),
				Error = message => Log.Error("{0}", message.Trim()),
				Warning = message => Log.Warn("{0}", message.Trim()),
				Info = message => Log.Info("{0}", message.Trim()),
#if DEBUG
				Debug = message => Log.Debug("{0}", message.Trim()),
#endif
			};

			Log.Info("Initializing server...");

			NativeMethods.SetCallbacks(_logCallbacks);
			_isRunning = NativeMethods.Initialize();

			if (_isRunning)
				return;

			this.SafeDispose();
			throw new NetworkException("See the console for further details.");
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		protected override void Update(double elapsedSeconds)
		{
			base.Update(elapsedSeconds);

			if (!_isRunning)
				return;

			_time += elapsedSeconds;
			if (NativeMethods.Update((ulong)(_time * 1000), true) >= 0)
				return;

			_isRunning = false;
			Log.Error("Server stopped after error.");
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			base.OnDisposing();

			if (!_isRunning)
				return;

			_isRunning = false;
			NativeMethods.Shutdown();

			Log.Info("Server has shut down.");
		}

		/// <summary>
		///     Provides access to the native service types and functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
#if Windows
			private const string LibraryName = "Lwar Server.dll";
#else
			private const string LibraryName = "liblwar-server.so";
#endif

			[DllImport(LibraryName, EntryPoint = "server_init")]
			public static extern bool Initialize();

			[DllImport(LibraryName, EntryPoint = "server_update")]
			public static extern int Update(ulong time, bool force);

			[DllImport(LibraryName, EntryPoint = "server_shutdown")]
			public static extern void Shutdown();

			[DllImport(LibraryName, EntryPoint = "server_log_callbacks")]
			public static extern void SetCallbacks(LogCallbacks callbacks);

			[DllImport(LibraryName, EntryPoint = "server_performance_callbacks")]
			public static extern void SetCallbacks(PerformanceCallbacks callbacks);

			public delegate void LogCallback(string message);

			public delegate void TimerCallback(uint timer);

			public delegate void CounterCallback(uint counter, uint value);

			[StructLayout(LayoutKind.Sequential)]
			public struct LogCallbacks
			{
				public LogCallback Die;
				public LogCallback Error;
				public LogCallback Warning;
				public LogCallback Info;
				public LogCallback Debug;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct PerformanceCallbacks
			{
				public readonly TimerCallback start;
				public readonly TimerCallback stop;
				public readonly CounterCallback counted;
			}
		}
	}
}