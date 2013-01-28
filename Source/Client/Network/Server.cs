﻿using System;

namespace Lwar.Client.Network
{
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;

	/// <summary>
	///   Represents the server hosting a game session.
	/// </summary>
	public class Server : DisposableObject
	{
		/// <summary>
		///   The update frequency of the server in Hz.
		/// </summary>
		private const int UpdateFrequency = 30;

		/// <summary>
		///   Can be used to cancel the server update process.
		/// </summary>
		private CancellationTokenSource _cancellation;

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
		///   The task executing the server update process.
		/// </summary>
		private Task _task;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Server()
		{
			_logCallbacks = new NativeMethods.LogCallbacks
			{
				Die = s => _logs.Enqueue(() => Log.Die("(Server) {0}", RemoveTrailingNewline(s))),
				Error = s => _logs.Enqueue(() => Log.Error("(Server) {0}", RemoveTrailingNewline(s))),
				Warning = s => _logs.Enqueue(() => Log.Warn("(Server) {0}", RemoveTrailingNewline(s))),
				Info = s => _logs.Enqueue(() => Log.Info("(Server) {0}", RemoveTrailingNewline(s))),
				Debug = s => _logs.Enqueue(() => NetworkLog.DebugInfo("(Server) {0}", RemoveTrailingNewline(s)))
			};

			LwarCommands.StartServer.Invoked += Run;
			LwarCommands.StopServer.Invoked += Shutdown;
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
		///   Raised when the server terminated unexpectedly.
		/// </summary>
		public event Action Faulted;

		/// <summary>
		///   Removes a trailing newline.
		/// </summary>
		private static string RemoveTrailingNewline(string s)
		{
			return s.EndsWith("\n") ? s.Substring(0, s.Length - 1) : s;
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
				Log.Error("Failed to initialize the server.");
				return;
			}

			_cancellation = new CancellationTokenSource();
			var token = _cancellation.Token;
			_task = Task.Factory.StartNew(() =>
				{
					var watch = new Stopwatch();
					watch.Start();

					while (!token.IsCancellationRequested)
					{
						var updateStart = watch.ElapsedMilliseconds;
						if (NativeMethods.Update((ulong)watch.ElapsedMilliseconds, true) < 0)
						{
							_logs.Enqueue(() => Log.Error("Server stopped after error."));
							break;
						}

						var timeTillNextUpdate = (1000 / UpdateFrequency) - (watch.ElapsedMilliseconds - updateStart);
						if (timeTillNextUpdate > 0)
							Thread.Sleep((int)timeTillNextUpdate);
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
			if (!IsRunning)
				return;

			HandleServerLogs();

			if (!IsRunning && Faulted != null)
				Faulted();
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
		}

		/// <summary>
		///   Provides access to the native service types and functions.
		/// </summary>
		private static class NativeMethods
		{
#if Windows
			private const string LibraryName = "lwar-server.dll";
#else
			private const string LibraryName = "libserver.so";
#endif

			[DllImport(LibraryName, EntryPoint = "server_init")]
			public static extern bool Initialize();

			[DllImport(LibraryName, EntryPoint = "server_update")]
			public static extern int Update(ulong time, bool force);

			[DllImport(LibraryName, EntryPoint = "server_shutdown")]
			public static extern void Shutdown();

			[DllImport(LibraryName, EntryPoint = "server_callbacks")]
			public static extern void SetCallbacks(LogCallbacks callbacks);

			public delegate void LogCallback(string message);

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