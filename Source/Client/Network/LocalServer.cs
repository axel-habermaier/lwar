﻿using System;

namespace Lwar.Client.Network
{
	using System.Collections.Concurrent;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Scripting;

	/// <summary>
	///   Represents a local server hosting a game session.
	/// </summary>
	public class LocalServer : DisposableObject
	{
		/// <summary>
		///   The update frequency of the server in Hz.
		/// </summary>
		private const int UpdateFrequency = 55;// 30;

		/// <summary>
		///   The command registry that manages the application commands.
		/// </summary>
		private readonly CommandRegistry _commands;

		/// <summary>
		///   The log callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallbacks _logCallbacks;

		/// <summary>
		///   Stores the log entires that have been generated by the server on the server thread.
		/// </summary>
		private readonly ConcurrentQueue<Action> _logs = new ConcurrentQueue<Action>();

		/// <summary>
		///   Can be used to cancel the server update process.
		/// </summary>
		private CancellationTokenSource _cancellation;

		/// <summary>
		///   The task executing the server update process.
		/// </summary>
		private Task _task;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="commands">The command registry that manages the application commands.</param>
		public LocalServer(CommandRegistry commands)
		{
			Assert.ArgumentNotNull(commands, () => commands);

			_logCallbacks = new NativeMethods.LogCallbacks
			{
				Die = s => _logs.Enqueue(() => Log.Die("(Server) {0}", RemoveTrailingNewlines(s))),
				Error = s => _logs.Enqueue(() => Log.Error("(Server) {0}", RemoveTrailingNewlines(s))),
				Warning = s => _logs.Enqueue(() => Log.Warn("(Server) {0}", RemoveTrailingNewlines(s))),
				Info = s => _logs.Enqueue(() => Log.Info("(Server) {0}", RemoveTrailingNewlines(s))),
				Debug = s => _logs.Enqueue(() => Log.DebugInfo("(Server) {0}", RemoveTrailingNewlines(s)))
			};

			_commands = commands;
			commands.OnStartServer += Run;
			commands.OnStopServer += Shutdown;
		}

		/// <summary>
		///   Gets a value indicating whether the server is currently initialized.
		/// </summary>
		public bool IsInitialized
		{
			get { return _cancellation != null && _task != null; }
		}

		/// <summary>
		///   Gets a value indicating whether the server is currently running.
		/// </summary>
		public bool IsRunning
		{
			get { return IsInitialized && !_cancellation.IsCancellationRequested; }
		}

		/// <summary>
		///   Removes a trailing newline.
		/// </summary>
		private static string RemoveTrailingNewlines(string s)
		{
			while (s.EndsWith("\n") || s.EndsWith("\r"))
				s = s.Substring(0, s.Length - 1);
			return s;
		}

		/// <summary>
		///   Runs the server update process on a separate thread.
		/// </summary>
		private void Run()
		{
			Shutdown();
			Log.Info("Initializing server...");

			NativeMethods.SetCallbacks(_logCallbacks);
			var isInitialized = NativeMethods.Initialize();

			if (!isInitialized)
			{
				HandleServerLogs();
				Log.Error("Failed to initialize the server.");
				return;
			}

			_cancellation = new CancellationTokenSource();
			var token = _cancellation.Token;
			_task = Task.Factory.StartNew(() =>
				{
					using (var clock = Clock.Create())
					{
						while (!token.IsCancellationRequested)
						{
							var updateStart = clock.Milliseconds;
							if (NativeMethods.Update((ulong)clock.Milliseconds, true) < 0)
							{
								_logs.Enqueue(() => Log.Error("Server stopped after error."));
								break;
							}

							var timeTillNextUpdate = (1000 / UpdateFrequency) - (clock.Milliseconds - updateStart);
							if (timeTillNextUpdate > 0)
								Thread.Sleep((int)timeTillNextUpdate);
						}
					}
				}, token);
		}

		/// <summary>
		///   Handles the queued server log entries.
		/// </summary>
		private void HandleServerLogs()
		{
			Action log;
			while (_logs.TryDequeue(out log))
				log();
		}

		/// <summary>
		///   Updates the server state.
		/// </summary>
		public void Update()
		{
			HandleServerLogs();
		}

		/// <summary>
		///   Shuts down the server.
		/// </summary>
		private void Shutdown()
		{
			if (!IsInitialized)
				return;

			if (IsRunning)
				_cancellation.Cancel();

			_task.Wait();
			NativeMethods.Shutdown();
			Log.Info("Server has shut down.");

			_task.SafeDispose();
			_cancellation.SafeDispose();
			_task = null;
			_cancellation = null;

			HandleServerLogs();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Shutdown();

			_commands.OnStartServer -= Run;
			_commands.OnStopServer -= Shutdown;
		}

		/// <summary>
		///   Provides access to the native service types and functions.
		/// </summary>
		private static class NativeMethods
		{
#if Windows
			private const string LibraryName = "lwar-server.dll";
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