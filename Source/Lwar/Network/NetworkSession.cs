namespace Lwar.Network
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Messages;
	using Pegasus;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Scripting;

	/// <summary>
	///     Represents a network session that implements the lwar network protocol.
	/// </summary>
	public class NetworkSession : DisposableObject
	{
		/// <summary>
		///     The number of disconnect messages that are sent to the server.
		/// </summary>
		private const int DisconnectMessageCount = 3;

		/// <summary>
		///     The time (in milliseconds) to wait between sending two disconnect messages to server.
		/// </summary>
		private const int DisconnectSendInterval = 60;

		/// <summary>
		///     The connection to the remote server.
		/// </summary>
		private readonly ServerConnection _connection;

		/// <summary>
		///     The delivery manager is responsible for the delivery guarantees of all incoming and outgoing messages.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///     The message dispatcher that is used to dispatch incoming server messages.
		/// </summary>
		private readonly MessageDispatcher _messageDispatcher;

		/// <summary>
		///     The message queue is responsible for packing all queued outgoing messages into a packet. Reliable messages will be
		///     resent until their reception has been acknowledged by the remote peer.
		/// </summary>
		private readonly MessageQueue _outgoingMessages;

		/// <summary>
		///     A cached queue that is used to retrieve the received messages from the server connection.
		/// </summary>
		private readonly Queue<Message> _receivedMessages = new Queue<Message>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		/// <param name="messageDispatcher">The message dispatcher that should be used to dispatch incoming server messages.</param>
		public NetworkSession(IPEndPoint serverEndPoint, MessageDispatcher messageDispatcher)
		{
			Assert.ArgumentNotNull(messageDispatcher);

			_messageDispatcher = messageDispatcher;
			_deliveryManager = new DeliveryManager();
			_connection = new ServerConnection(serverEndPoint);

			_outgoingMessages = new MessageQueue(_deliveryManager);
			Send(ConnectMessage.Create(Cvars.PlayerName));
		}

		/// <summary>
		///     Gets a value indicating whether the game state has been synced with the server.
		/// </summary>
		public bool IsSynced
		{
			get { return _connection.IsSynced && !_connection.IsFaulted; }
		}

		/// <summary>
		///     Gets the endpoint of the server.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return _connection.ServerEndPoint; }
		}

		/// <summary>
		///     Gets the remaining time in milliseconds before the network session will be dropped.
		/// </summary>
		public double TimeToDrop
		{
			get { return _connection.TimeToDrop; }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _connection.IsLagging; }
		}

		/// <summary>
		///     Updates the state of the network session.
		/// </summary>
		public void Update()
		{
			_connection.Send(_outgoingMessages);
			_connection.Receive(_receivedMessages, _deliveryManager);
			_connection.Update();

			while (_receivedMessages.Count != 0)
				_messageDispatcher.Dispatch(_receivedMessages.Dequeue());
		}

		/// <summary>
		///     Sends the given reliable message to the server.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(Message message)
		{
			_outgoingMessages.Enqueue(ref message);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Send a couple of Disconnect messages to the server
			for (var i = 0; i < DisconnectMessageCount && !_connection.IsFaulted; ++i)
			{
				Send(new Message { Type = MessageType.Disconnect });

				_connection.Send(_outgoingMessages);
				_connection.Update();
				Thread.Sleep(DisconnectSendInterval);
			}

			_connection.SafeDispose();
		}
	}
}