namespace Lwar
{
	using System;
	using Gameplay.Server;
	using Network;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;

	/// <summary>
	///     Represents a debug server hosting a game session.
	/// </summary>
	public class DebugServer : Server
	{
		/// <summary>
		///     The allocator that is used to allocate server objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The clients connected to the server.
		/// </summary>
		private readonly ClientCollection _clients;

		/// <summary>
		///     The game session that manages the state of the entities.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     Listens for incoming UDP connections.
		/// </summary>
		private readonly UdpListener _listener;

		/// <summary>
		///     The server logic that handles the communication between the server and the clients.
		/// </summary>
		private readonly ServerLogic _serverLogic;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		public DebugServer(string serverName, ushort port)
			: base(serverName, port)
		{
			try
			{
				_allocator = new PoolAllocator();
				_gameSession = new GameSession(_allocator);
				_serverLogic = new ServerLogic(_allocator, _gameSession);

				_listener = new UdpListener(port, NetworkProtocol.MaxPacketSize);
				_listener.Start();

				_clients = new ClientCollection(_allocator, _serverLogic, _listener);
				_gameSession.InitializeServer(_serverLogic);

				Log.Info("Server started.");
			}
			catch (NetworkException)
			{
				this.SafeDispose();
				throw;
			}
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		protected override void Update(double elapsedSeconds)
		{
			base.Update(elapsedSeconds);

			_clients.DispatchClientMessages();
			_gameSession.Update((float)elapsedSeconds);
			_serverLogic.BroadcastEntityUpdates();
			_clients.SendQueuedMessages();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clients.SafeDispose();
			_gameSession.SafeDispose();
			_listener.SafeDispose();
			_allocator.SafeDispose();

			base.OnDisposing();
			Log.Info("Server stopped.");
		}
	}
}