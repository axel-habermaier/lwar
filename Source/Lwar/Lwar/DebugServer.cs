namespace Lwar
{
	using System;
	using Gameplay.Server;
	using Network;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a debug server hosting a game session.
	/// </summary>
	public class DebugServer : Server
	{
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
		/// <param name="allocator">The allocator that should be used to allocate pooled objects.</param>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		public DebugServer(PoolAllocator allocator, ushort port = NetworkProtocol.DefaultServerPort)
			: base(port)
		{
			Assert.ArgumentNotNull(allocator);

			try
			{
				_gameSession = new GameSession(allocator);
				_serverLogic = new ServerLogic(allocator, _gameSession);

				_listener = new UdpListener(port, NetworkProtocol.MaxPacketSize);
				_listener.Start();

				_gameSession.InitializeServer(_serverLogic);
				_clients = new ClientCollection(allocator, _serverLogic, _listener);

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
			_serverLogic.RemoveInactivePlayers();
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

			base.OnDisposing();
			Log.Info("Server stopped.");
		}
	}
}