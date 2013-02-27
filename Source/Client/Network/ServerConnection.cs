using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
	using System.Net;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Represents a proxy of an lwar server that a client can use to communicate with the server.
	/// </summary>
	public class ServerConnection : DisposableObject
	{
		/// <summary>
		///   The duration in milliseconds that the proxy waits for a new packet from the server before the connection is
		///   considered to be dropped.
		/// </summary>
		private const int DroppedTimeout = 15000;

		/// <summary>
		///   The duration in milliseconds that the proxy waits for a new packet from the server before the connection is
		///   considered to be lagging.
		/// </summary>
		private const int LaggingTimeout = 500;

		/// <summary>
		///   Provides the time that is used to check whether a connection is lagging or dropped.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   The Udp socket that is used for the communication with the server.
		/// </summary>
		private readonly UdpSocket _socket;

		/// <summary>
		///   The time when the last packet has been received from the server.
		/// </summary>
		private double _lastPacketTimestamp;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The endpoint of the server.</param>
		/// <param name="packetFactory">The packet factory that should be used to create incoming packets.</param>
		public ServerConnection(IPEndPoint serverEndPoint, IPacketFactory packetFactory)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);

			_socket = new UdpSocket(packetFactory);
			ServerEndPoint = serverEndPoint;
			State = ConnectionState.Connecting;
		}

		/// <summary>
		///   Gets the current state of the virtual connection to the server.
		/// </summary>
		public ConnectionState State { get; private set; }

		/// <summary>
		///   Gets the endpoint of the server.
		/// </summary>
		public IPEndPoint ServerEndPoint { get; private set; }

		/// <summary>
		///   Gets the remaining time in milliseconds before the connection will be dropped.
		/// </summary>
		public double TimeToDrop
		{
			get { return DroppedTimeout - (_clock.Milliseconds - _lastPacketTimestamp); }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _clock.Milliseconds - _lastPacketTimestamp > LaggingTimeout; }
		}

		/// <summary>
		///   Sends the queued messages to the server.
		/// </summary>
		/// <param name="messageQueue">The queued messages that should be sent.</param>
		public void Send(MessageQueue messageQueue)
		{
			try
			{
				// While we're connecting or syncing, send as many packets as possible (even if the payload is empty)
				if (State == ConnectionState.Connecting || State == ConnectionState.Syncing)
					_socket.Send(messageQueue.CreatePacket(), ServerEndPoint);

				// Once we're connected, only send a packet if we actually have some payload to send
				if (State == ConnectionState.Connected && messageQueue.HasPendingData)
					_socket.Send(messageQueue.CreatePacket(), ServerEndPoint);
			}
			catch (SocketOperationException e)
			{
				NetworkLog.ClientError("The connection to the server has been terminated due to an error: {0}", e.Message);
				State = ConnectionState.Faulted;
			}
		}

		/// <summary>
		///   Handles incoming packets from the server.
		/// </summary>
		/// <param name="messageQueue">The message queue the received messages should be added to.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		public void Receive(Queue<Message> messageQueue, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(messageQueue, () => messageQueue);
			Assert.ArgumentNotNull(deliveryManager, () => deliveryManager);

			if (State != ConnectionState.Connecting && State != ConnectionState.Connected && State != ConnectionState.Syncing)
				return;

			try
			{
				var sender = new IPEndPoint(IPAddress.Any, 0);
				IncomingPacket packet;

				while (_socket.TryReceive(ref sender, out packet))
				{
					using (packet)
					{
						if (sender.SameEndPoint(ServerEndPoint))
							HandlePacket(packet, messageQueue, deliveryManager);
						else
						{
							NetworkLog.ClientWarn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.",
												  sender, ServerEndPoint);
						}
					}
				}
			}
			catch (SocketOperationException e)
			{
				NetworkLog.ClientError("The connection to the server has been terminated due to an error: {0}", e.Message);
				State = ConnectionState.Faulted;
			}
		}

		/// <summary>
		///   Handles the incoming packet.
		/// </summary>
		/// <param name="packet">The packet that should be handled.</param>
		/// <param name="messageQueue">The message queue the received messages should be added to.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		private void HandlePacket(IncomingPacket packet, Queue<Message> messageQueue, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(packet, () => packet);

			var buffer = packet.Reader;
			var header = PacketHeader.Create(buffer);

			if (header == null)
				return;

			_lastPacketTimestamp = _clock.Milliseconds;
			deliveryManager.UpdateLastAckedSequenceNumber(header.Value.Acknowledgement);
			var allowUnreliableDelivery = deliveryManager.AllowUnreliableDelivery(header.Value.Timestamp);

			while (!buffer.EndOfBuffer)
			{
				List<Message> messages;
				if (!buffer.TryRead(out messages, MessageSerialization.Deserialize))
					continue;

				for (var i = 0; i < messages.Count; ++i)
				{
					var message = messages[i];

					if (message.Type.IsReliable() && !deliveryManager.AllowReliableDelivery(message.SequenceNumber))
						continue;

					if (message.Type.IsUnreliable() && !allowUnreliableDelivery)
						continue;

					if (message.Type.IsUnreliable())
						message.Timestamp = header.Value.Timestamp;

					HandleMessage(ref message, messageQueue);
				}
			}
		}

		/// <summary>
		///   Handles internal messages and adds external messages to the queue.
		/// </summary>
		/// <param name="message">The message that should be handled.</param>
		/// <param name="messageQueue">The message queue an external message should be added to.</param>
		private void HandleMessage(ref Message message, Queue<Message> messageQueue)
		{
			switch (message.Type)
			{
				case MessageType.Join:
					// If the first message we get from the server is a join message, we know that the server has
					// accepted the connection and starts syncing the game state.
					// The first join sent by the server is the join for the local player.
					if (message.SequenceNumber == 1)
					{
						message.Join.IsLocalPlayer = true;
						State = ConnectionState.Syncing;
					}
					messageQueue.Enqueue(message);
					break;
				case MessageType.Synced:
					// We should only receive a sync packet if we're actually syncing
					if (State != ConnectionState.Syncing)
						NetworkLog.ClientWarn("Ignored an unexpected Synced message.");
					else
						State = ConnectionState.Connected;
					break;
				case MessageType.Full:
					// Only the first message can be a server full message
					if (State != ConnectionState.Connecting)
						NetworkLog.ClientWarn("Ignored an unexpected server full message.");
					else
						State = ConnectionState.Full;
					break;
				default:
					messageQueue.Enqueue(message);
					break;
			}
		}

		/// <summary>
		///   Updates the connection state, dropping the connection if no more packets are received for a specific amount of time.
		/// </summary>
		public void Update()
		{
			if (State != ConnectionState.Syncing && State != ConnectionState.Connected)
				return;

			var delta = _clock.Milliseconds - _lastPacketTimestamp;
			if (delta > DroppedTimeout)
				State = ConnectionState.Dropped;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
			_socket.SafeDispose();
		}
	}
}