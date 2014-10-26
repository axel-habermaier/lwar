namespace Lwar
{
	using System;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface;
	using Pegasus.Utilities;
	using Scripting;

	/// <summary>
	///     A base class for server implementations.
	/// </summary>
	public abstract class Server : DisposableObject
	{
		/// <summary>
		///     The currently running server instance.
		/// </summary>
		private static Server _server;

		/// <summary>
		///     Periodically sends server discovery messages.
		/// </summary>
		private readonly ServerDiscovery _serverDiscovery;

		/// <summary>
		///     The step timer that is used to update the server at a fixed rate.
		/// </summary>
		private readonly StepTimer _timer = new StepTimer { IsFixedTimeStep = true };

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		/// <param name="updateRate">The server update rate in frames per second.</param>
		protected Server(string serverName, ushort port, float updateRate = 1 / 60.0f)
		{
			_timer.TargetElapsedSeconds = updateRate;
			_timer.UpdateRequired += () => Update(_timer.ElapsedSeconds);
			_serverDiscovery = new ServerDiscovery(serverName, port);
		}

		/// <summary>
		///     Tries to start a native or a debug server.
		/// </summary>
		/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		public static bool TryStart(string serverName, ushort port)
		{
			Stop();

			try
			{
				if (Cvars.UseDebugServer)
					_server = new DebugServer(serverName, port);
				else
					_server = new NativeServer(serverName, port);

				return true;
			}
			catch (NetworkException e)
			{
				Log.Error("Unable to start the server: {0}", e.Message);
				MessageBox.Show("Server Failure", String.Format("Unable to start the server: {0}", e.Message));

				return false;
			}
		}

		/// <summary>
		///     Stops the currently running server, if any.
		/// </summary>
		public static void Stop()
		{
			_server.SafeDispose();
			_server = null;
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		public static void Update()
		{
			if (_server != null)
				_server._timer.Update();
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		protected virtual void Update(double elapsedSeconds)
		{
			_serverDiscovery.SendDiscoveryMessage(elapsedSeconds);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_serverDiscovery.SafeDispose();
		}
	}
}