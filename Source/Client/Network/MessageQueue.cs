using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   The message queue is responsible for packing all queued messages into a packet and sending it to the remote
	///   peer. Reliable messages will be resent until their reception has been acknowledged by the remote peer.
	/// </summary>
	public class MessageQueue
	{
		/// <summary>
		///   Cached delegate of the message serialization function.
		/// </summary>
		private static readonly Action<BufferWriter, Message> MessageSerializer = MessageSerialization.Serialize;

		/// <summary>
		///   The delivery manager that is used to enforce the message delivery constraints.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///   The packet factory that is used to create incoming and outgoing packets.
		/// </summary>
		private readonly IPacketFactory _packetFactory;

		/// <summary>
		///   The queued reliable messages that have not yet been sent or that have not yet been acknowledged.
		/// </summary>
		private readonly Queue<Message> _reliableMessages = new Queue<Message>();

		/// <summary>
		///   The queued unreliable messages that have not yet been sent.
		/// </summary>
		private readonly Queue<Message> _unreliableMessages = new Queue<Message>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="packetFactory">The packet factory that should be used to create incoming and outgoing packets.</param>
		/// <param name="deliveryManager">The delivery manager that is used to enforce the message delivery constraints.</param>
		public MessageQueue(IPacketFactory packetFactory, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);
			Assert.ArgumentNotNull(deliveryManager, () => deliveryManager);

			_packetFactory = packetFactory;
			_deliveryManager = deliveryManager;
		}

		/// <summary>
		///   Gets a value indicating whether any messages are waiting in the queue.
		/// </summary>
		public bool HasPendingData
		{
			get { return _reliableMessages.Count != 0 || _unreliableMessages.Count != 0; }
		}

		/// <summary>
		///   Enqueues the reliable message.
		/// </summary>
		/// <param name="message">The message that should be enqueued.</param>
		public void Enqueue(ref Message message)
		{
			if (message.Type.IsReliable())
			{
				_deliveryManager.AssignSequenceNumber(ref message);
				_reliableMessages.Enqueue(message);
			}
			else
				_unreliableMessages.Enqueue(message);
		}

		/// <summary>
		///   Sends all queued messages, resending all reliable messages that have previously been sent but not yet acknowledged.
		/// </summary>
		public OutgoingPacket CreatePacket()
		{
			RemoveAckedMessages();

			var packet = _packetFactory.CreateOutgoingPacket();
			_deliveryManager.WriteHeader(packet.Writer);

			AddMessages(_reliableMessages, packet.Writer);
			AddMessages(_unreliableMessages, packet.Writer);
			_unreliableMessages.Clear();

			return packet;
		}

		/// <summary>
		///   Removes all acknowledged reliable messages from the queue.
		/// </summary>
		private void RemoveAckedMessages()
		{
			while (_reliableMessages.Count != 0)
			{
				var message = _reliableMessages.Peek();
				if (_deliveryManager.IsAcknowledged(ref message))
					_reliableMessages.Dequeue();
				else
					break;
			}
		}

		/// <summary>
		///   Adds all queued messages to the buffer that fit into the remaining space.
		/// </summary>
		/// <param name="messages">The messages that should be written to the buffer.</param>
		/// <param name="buffer">The buffer the messages should be written into.</param>
		private static void AddMessages(IEnumerable<Message> messages, BufferWriter buffer)
		{
			foreach (var message in messages)
			{
				if (!buffer.TryWrite(message, MessageSerializer))
					return;
			}
		}
	}
}