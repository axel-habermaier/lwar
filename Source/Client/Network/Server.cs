using System;

namespace Client.Network
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Processes;

	public class Server
	{
		/// <summary>
		///   The update frequency of the server in Hz.
		/// </summary>
		private const int ServerUpdateFrequency = 30;

		public async Task Run(ProcessContext context)
		{
			Log.Info("Initializing server...");

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
		}
	}
}