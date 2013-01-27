using System;

namespace Client.Network
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Processes;

	public class Server
	{
		/// <summary>
		///   The update frequency of the server in Hz.
		/// </summary>
		private const int ServerUpdateFrequency = 30;

		/// <summary>
		///   The log callbacks that have been passed to the native code. We must keep a reference in order to prevent
		///   the garbage collector from freeing the delegates while they are still being used by native code.
		/// </summary>
		private readonly NativeMethods.LogCallbacks _logCallbacks = new NativeMethods.LogCallbacks
		{
			Die = s => Log.Die("(Server) {0}", s),
			Error = s => Log.Error("(Server) {0}", s),
			Warning = s => Log.Warn("(Server) {0}", s),
			Info = s => Log.Info("(Server) {0}", s),
			Debug = s => NetworkLog.DebugInfo("(Server) {0}", s)
		};

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

				while (!context.IsCanceled)
				{
					if (NativeMethods.Update((ulong)watch.ElapsedMilliseconds, true) < 0)
					{
						Log.Error("Server stopped after error.");
						break;
					}

					await context.Delay(1000 / ServerUpdateFrequency);
				}
			}
			finally
			{
				NativeMethods.Shutdown();
				Log.Info("Server has shut down.");
			}
		}

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