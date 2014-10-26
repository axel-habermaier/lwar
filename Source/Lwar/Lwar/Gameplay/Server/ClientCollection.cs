namespace Lwar.Gameplay.Server
{
	using System;
	using System.Collections.Generic;
	using Network;
	using Network.Messages;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a collection of clients.
	/// </summary>
	public class ClientCollection : DisposableObject
	{
		/// <summary>
		///     The allocator that is used to allocate pooled objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The clients of the game session.
		/// </summary>
		private readonly List<Client> _clients = new List<Client>(NetworkProtocol.MaxPlayers);

		/// <summary>
		///     Listens for incoming UDP connections.
		/// </summary>
		private readonly UdpListener _listener;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate pooled objects.</param>
		/// <param name="serverLogic">The server logic that handles the communication between the server and the clients.</param>
		/// <param name="port">The port that should be used to listen for connecting clients.</param>
		public ClientCollection(PoolAllocator allocator, ServerLogic serverLogic, ushort port = NetworkProtocol.DefaultServerPort)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(serverLogic);

			serverLogic.Broadcast += Broadcast;

			_allocator = allocator;
			_listener = new UdpListener(port, NetworkProtocol.MaxPacketSize);
			_listener.ChannelCreated += channel => _clients.Add(Client.Create(_allocator, Connection.Create(_allocator, channel), serverLogic));
		}

		/// <summary>
		///     Dispatches all messages received from the clients.
		/// </summary>
		public void DispatchClientMessages()
		{
			try
			{
				_listener.Update();
			}
			catch (NetworkException)
			{
				// We don't care about receive problems at this point. Might be the wrong thing to do here...
				// What if the socket is really completely broken for some reason?
			}

			RemoveDisconnectedClients();

			foreach (var client in _clients)
				client.DispatchReceivedMessages();
		}

		/// <summary>
		///     Sends all queued messages to the clients.
		/// </summary>
		public void SendQueuedMessages()
		{
			foreach (var client in _clients)
				client.SendQueuedMessages();
		}

		/// <summary>
		///     Sends the given message to all connected clients.
		/// </summary>
		/// <param name="message">The message that should be sent to all clients.</param>
		private void Broadcast(Message message)
		{
			Assert.ArgumentNotNull(message);

			using (message.AcquireOwnership())
			{
				foreach (var client in _clients)
				{
					if (!client.IsDisconnected && client.IsSynced)
						client.Send(message);
				}
			}
		}

		/// <summary>
		///     Removes all disconnected clients.
		/// </summary>
		private void RemoveDisconnectedClients()
		{
			for (var i = 0; i < _clients.Count; ++i)
			{
				if (!_clients[i].IsDisconnected)
					continue;

				_clients[i].SafeDispose();
				_clients[i] = _clients[_clients.Count - 1];
				_clients.RemoveAt(_clients.Count - 1);
				--i;
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			foreach (var client in _clients)
				client.Disconnect();

			_clients.SafeDisposeAll();
			_listener.SafeDispose();
		}
	}
}