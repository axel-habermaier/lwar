using System;

namespace Lwar.Client.Network
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   Represents the server hosting a game session.
	/// </summary>
	public class Server
	{
		/// <summary>
		///   The log callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallbacks _logCallbacks = new NativeMethods.LogCallbacks
		{
			Die = s => Log.Die("(Server) {0}", RemoveTrailingNewline(s)),
			Error = s => Log.Error("(Server) {0}", RemoveTrailingNewline(s)),
			Warning = s => Log.Warn("(Server) {0}", RemoveTrailingNewline(s)),
			Info = s => Log.Info("(Server) {0}", RemoveTrailingNewline(s)),
			Debug = s => NetworkLog.DebugInfo("(Server) {0}", RemoveTrailingNewline(s))
		};

		/// <summary>
		///   Removes a trailing newline.
		/// </summary>
		private static string RemoveTrailingNewline(string s)
		{
			return s.EndsWith("\n") ? s.Substring(0, s.Length - 1) : s;
		}

		/// <summary>
		///   Initializes the server, runs it and shuts it down if an error occurs or cancellation is requested.
		/// </summary>
		/// <param name="context">The context in which the server process should be executed.</param>
		public async Task Run(ProcessContext context)
		{
			Log.Info("Initializing server...");

			NativeMethods.SetCallbacks(_logCallbacks);
			if (!NativeMethods.Initialize())
			{
				Log.Error("Failed to initialize the server.");
				return;
			}

			try
			{
				var watch = new Stopwatch();
				watch.Start();

				Func<int> updateServer = () => NativeMethods.Update((ulong)watch.ElapsedMilliseconds, true);
				while (!context.IsCanceled)
				{
					if (await context.WaitFor(updateServer) < 0)
					{
						Log.Error("Server stopped after error.");
						break;
					}
				}
			}
			finally
			{
				NativeMethods.Shutdown();
				Log.Info("Server has shut down.");
			}
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