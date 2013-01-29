using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   The message queue that is responsible for packing all queued messages into a packet and sending it to the remote
	///   peer. Reliable messages will be resent until their reception has been acknowledged by the remote peer.
	/// </summary>
	public class MessageQueue
	{
		/// <summary>
		///   The delivery manager that is used to enforce the message delivery constraints.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///   The queued reliable messages that have not yet been sent or that have not yet been acknowledged.
		/// </summary>
		private readonly Queue<IReliableMessage> _reliableMessages = new Queue<IReliableMessage>();

		/// <summary>
		///   The queued unreliable messages that have not yet been sent.
		/// </summary>
		private readonly Queue<IUnreliableMessage> _unreliableMessages = new Queue<IUnreliableMessage>();

		/// <summary>
		///   Determines the current time.
		/// </summary>
		private Time _time;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="deliveryManager">The delivery manager that is used to enforce the message delivery constraints.</param>
		public MessageQueue(DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(deliveryManager, () => deliveryManager);

			_time = new Time();
			_time.Offset = _time.Seconds;
			_deliveryManager = deliveryManager;
		}

		/// <summary>
		///   Enqueues the reliable message.
		/// </summary>
		/// <param name="message">The reliable message that should be enqueued.</param>
		public void Enqueue(IReliableMessage message)
		{
			_deliveryManager.AssignSequenceNumber(message);
			_reliableMessages.Enqueue(message);
		}

		/// <summary>
		///   Enqueues the unreliable message.
		/// </summary>
		/// <param name="message">The unreliable message that should be enqueued.</param>
		public void Enqueue(IUnreliableMessage message)
		{
			_unreliableMessages.Enqueue(message);
		}

		/// <summary>
		///   Sends all queued messages, resending all reliable messages that have previously been sent but not yet acknowledged.
		/// </summary>
		public OutgoingPacket CreatePacket()
		{
			RemoveAckedMessages();

			var packet = OutgoingPacket.Create();
			var header = new Header(_deliveryManager.LastReceivedSequenceNumber, (uint)_time.Milliseconds);
			header.Write(packet.Writer);

			AddMessages(_reliableMessages, packet.Writer);
			AddMessages(_unreliableMessages, packet.Writer);
			return packet;
		}

		/// <summary>
		///   Removes all acknowledged reliable messages from the queue.
		/// </summary>
		private void RemoveAckedMessages()
		{
			while (_reliableMessages.Count != 0)
			{
				if (_deliveryManager.IsAcknowledged(_reliableMessages.Peek()))
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
		private static void AddMessages(IEnumerable<IMessage> messages, BufferWriter buffer)
		{
			foreach (var message in messages)
			{
				if (!message.Serialize(buffer))
					break;
			}
		}
	}
}