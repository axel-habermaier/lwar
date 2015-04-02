namespace Lwar.Network
{
	using System;
	using Messages;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Deserializes network messages from a buffer reader.
	/// </summary>
	/// <remarks>
	///     This class is not implemented as an iterator in order to avoid the memory allocations required by the use of
	///     iterators.
	/// </remarks>
	internal sealed class MessageDeserializer : PooledObject
	{
		/// <summary>
		///     If tracing is enabled, the contents of all sent packets are shown in the debug output.
		/// </summary>
		private const bool EnableTracing = false;

		/// <summary>
		///     The object pool that is used to allocate message objects.
		/// </summary>
		private PoolAllocator _allocator;

		/// <summary>
		///     The number of batched messages that must still be deserialized.
		/// </summary>
		private int _batchedMessageCount;

		/// <summary>
		///     The type of the batched message that is being deserialized.
		/// </summary>
		private MessageType? _batchedMessageType;

		/// <summary>
		///     The sequence number of the batched messages that are being deserialized.
		/// </summary>
		private uint _batchedSequenceNumber;

		/// <summary>
		///     A cached delegate to the deserialization function in order to avoid unnecessary memory allocations.
		/// </summary>
		private BufferReader.Deserializer<SequencedMessage?> _cachedDeserializer;

		/// <summary>
		///     The delivery manager that is used to decide whether the packet should be delivered.
		/// </summary>
		private DeliveryManager _deliveryManager;

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_batchedMessageCount = 0;
			_batchedMessageType = null;
			_batchedSequenceNumber = 0;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate message objects.</param>
		/// <param name="deliveryManager">The delivery manager that should be used to decide whether the packet should be delivered.</param>
		public static MessageDeserializer Create(PoolAllocator poolAllocator, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentNotNull(deliveryManager);

			var deserializer = poolAllocator.Allocate<MessageDeserializer>();
			deserializer._deliveryManager = deliveryManager;
			deserializer._allocator = poolAllocator;
			deserializer._cachedDeserializer = deserializer.DeserializeMessage;
			return deserializer;
		}

		/// <summary>
		///     Tries to deserialize a message from the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize a message.</param>
		/// <param name="message">Stores the deserialized message.</param>
		public bool TryDeserialize(ref BufferReader reader, out SequencedMessage message)
		{
			// Each iteration of the loop reads a single message (batched or non-batched). If, however, we encounter
			// a message that we don't want to deliver, the message is skipped (for batched messages, all messages
			// contained therein are skipped as well). Therefore, the loop reads from the buffer until it finds the
			// first element that actually should be delivered.
			while (true)
			{
				// There's nothing to do if we've already reached the end of the buffer.
				if (reader.EndOfBuffer)
				{
					message = default(SequencedMessage);
					return false;
				}

				// If we're done deserializing an batched message, reset the batched message type flag.
				var continueWithOptimizedMessage = _batchedMessageType != null && _batchedMessageCount > 0;
				if (!continueWithOptimizedMessage)
					_batchedMessageType = null;

				// Read a message from the buffer, either an batched or a non-batched one. If deserialization fails,
				// we're done.
				SequencedMessage? deserializedMessage;
				if (!reader.TryRead(out deserializedMessage, _cachedDeserializer))
				{
					// If we can't read the expected number of batched messages, something must be wrong with the packet.
					if (continueWithOptimizedMessage)
						Assert.That(false, "Received an invalid packet. Parts of the packet have been ignored.");

					message = default(SequencedMessage);
					return false;
				}

				// Check if an batched message with a count of zero was sent.
				if (deserializedMessage == null)
					continue;

				// Delivery is always allowed for unreliable messages.
				message = deserializedMessage.Value;
				if (message.Message.IsUnreliable)
					return true;

				// Delivery might not be allowed for reliable messages, though.
				Assert.That(message.Message.IsReliable, "A message of type '{0}' is neither reliable nor unreliable?", message.Message.MessageType);
				if (_deliveryManager.AllowReliableDelivery(message.SequenceNumber))
					return true;

				// Otherwise, we have to dispose the current message.
				message.Message.SafeDispose();
			}
		}

		/// <summary>
		///     Deserializes a message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		private SequencedMessage? DeserializeMessage(ref BufferReader reader)
		{
			var messageType = _batchedMessageType ?? (MessageType)reader.ReadByte();
			var sequenceNumber = _batchedMessageType == null ? reader.ReadUInt32() : _batchedSequenceNumber;

			var message = Message.Allocate(_allocator, messageType);

			if (message.UseBatchedTransmission)
			{
				if (_batchedMessageType == null)
					_batchedMessageCount = reader.ReadByte();

				--_batchedMessageCount;
				_batchedMessageType = messageType;
				_batchedSequenceNumber = sequenceNumber;
			}

			// If the remote peer sent a batched message with a message count of 0, there's no message to return
			if (_batchedMessageCount < 0)
			{
				message.SafeDispose();
				return null;
			}

			message.Deserialize(ref reader);

			if (message.IsReliable)
				Log.DebugIf(EnableTracing, "(Client) {0}: {1}", sequenceNumber, message);

			return new SequencedMessage(message, sequenceNumber);
		}
	}
}