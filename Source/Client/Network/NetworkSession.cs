using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
	using System.Net;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Memory;

	/// <summary>
	///   Represents a network session that implements the lwar network protocol.
	/// </summary>
	public class NetworkSession : DisposableObject
	{
		/// <summary>
		///   The connection to the remote server.
		/// </summary>
		private readonly ServerConnection _connection;

		/// <summary>
		///   The delivery manager is responsible for the delivery guarantees of all incoming and outgoing messages.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///   The message queue is responsible for packing all queued outgoing messages into a packet. Reliable messages will be
		///   resent until their reception has been acknowledged by the remote peer.
		/// </summary>
		private readonly MessageQueue _outgoingMessages;

		/// <summary>
		///   A cached queue that is used to retreive the received messages from the server connection.
		/// </summary>
		private readonly Queue<Message> _receivedMessages = new Queue<Message>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public NetworkSession(IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint);

			_deliveryManager = new DeliveryManager();
			var packetFactory = new LwarPacketFactory();
			_connection = new ServerConnection(serverEndPoint, packetFactory);

			_outgoingMessages = new MessageQueue(packetFactory, _deliveryManager);
			Send(new Message { Type = MessageType.Connect });
		}

		/// <summary>
		///   Gets the endpoint of the server.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return _connection.ServerEndPoint; }
		}

		/// <summary>
		///   Gets the remaining time in milliseconds before the network session will be dropped.
		/// </summary>
		public double TimeToDrop
		{
			get { return _connection.TimeToDrop; }
		}

		/// <summary>
		///   Gets a value indicating whether a connection to the server is established.
		/// </summary>
		public bool IsConnected
		{
			get { return _connection.State == ConnectionState.Connected; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server has been established and the game state is currently
		///   being synced.
		/// </summary>
		public bool IsSyncing
		{
			get { return _connection.State == ConnectionState.Syncing; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _connection.IsLagging; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server is faulted.
		/// </summary>
		public bool IsFaulted
		{
			get { return _connection.State == ConnectionState.Faulted; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server has been dropped.
		/// </summary>
		public bool IsDropped
		{
			get { return _connection.State == ConnectionState.Dropped; }
		}

		/// <summary>
		///   Gets a value indicating whether the server is full and cannot accept any further clients.
		/// </summary>
		public bool ServerIsFull
		{
			get { return _connection.State == ConnectionState.Full; }
		}

		/// <summary>
		///   Updates the state of the network session.
		/// </summary>
		/// <param name="dispatcher">The message dispatcher that should be used to dispatch the received server messages.</param>
		public void Update(MessageDispatcher dispatcher)
		{
			Assert.ArgumentNotNull(dispatcher);

			_connection.Receive(_receivedMessages, _deliveryManager);
			_connection.Send(_outgoingMessages);
			_connection.Update();

			while (_receivedMessages.Count != 0)
				dispatcher.Dispatch(_receivedMessages.Dequeue());
		}

		/// <summary>
		///   Sends the given reliable message to the server.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(Message message)
		{
			_outgoingMessages.Enqueue(ref message);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_connection.SafeDispose();
			_deliveryManager.SafeDispose();
		}
	}
}