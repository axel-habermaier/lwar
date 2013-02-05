using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   The message queue that is responsible for packing all queued messages into a packet and sending it to the remote
	///   peer. Reliable messages will be resent until their reception has been acknowledged by the remote peer.
	/// </summary>
	public class MessageQueue : DisposableObject
	{
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
		private readonly Queue<IReliableMessage> _reliableMessages = new Queue<IReliableMessage>();

		/// <summary>
		///   The queued unreliable messages that have not yet been sent.
		/// </summary>
		private readonly Queue<IUnreliableMessage> _unreliableMessages = new Queue<IUnreliableMessage>();

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
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_reliableMessages.SafeDisposeAll();
			_unreliableMessages.SafeDisposeAll();
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

			var packet = _packetFactory.CreateOutgoingPacket();
			_deliveryManager.WriteHeader(packet.Writer);

			AddMessages(_reliableMessages, packet.Writer);
			AddMessages(_unreliableMessages, packet.Writer);

			_unreliableMessages.SafeDisposeAll();
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
				if (_deliveryManager.IsAcknowledged(_reliableMessages.Peek()))
				{
					var message = _reliableMessages.Dequeue();
					message.Dispose();
				}
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
				if (!message.Write(buffer))
					break;
			}
		}
	}
}