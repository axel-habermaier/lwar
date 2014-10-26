namespace Lwar.Network.Messages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a message that is used for the communication between the server and the client.
	/// </summary>
	public abstract class Message : SharedPooledObject
	{
		/// <summary>
		///     Maps a message type to its transmission information.
		/// </summary>
		private static readonly Dictionary<Type, TransmissionInfo> TransmissionInfos = new Dictionary<Type, TransmissionInfo>();

		/// <summary>
		///     Maps a message type to a message instance allocator.
		/// </summary>
		private static readonly Dictionary<MessageType, Func<PoolAllocator, Message>> MessageConstructors =
			new Dictionary<MessageType, Func<PoolAllocator, Message>>();

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Message()
		{
			var allocatorParameter = Expression.Parameter(typeof(PoolAllocator));
			var allocateMethod = typeof(PoolAllocator).GetMethod("Allocate", BindingFlags.Public | BindingFlags.Instance);

			var messageTypes = Assembly
				.GetExecutingAssembly()
				.GetTypes().Where(type => type.IsClass && !type.IsAbstract && typeof(Message).IsAssignableFrom(type));

			foreach (var messageType in messageTypes)
			{
				var reliable = (ReliableTransmissionAttribute)messageType
					.GetCustomAttributes(typeof(ReliableTransmissionAttribute), false).FirstOrDefault();
				var unreliable = (UnreliableTransmissionAttribute)messageType
					.GetCustomAttributes(typeof(UnreliableTransmissionAttribute), false).FirstOrDefault();

				Assert.That(reliable == null || unreliable == null,
					"Cannot use both reliable and unreliable transmission for messages of type '{0}'.", messageType.FullName);
				Assert.That(reliable != null || unreliable != null,
					"No transmission type has been specified for messages of type '{0}'.", messageType.FullName);

				var typedAllocateMethod = allocateMethod.MakeGenericMethod(messageType);
				var callAllocateMethod = Expression.Call(allocatorParameter, typedAllocateMethod);
				var constructMessage = Expression.Lambda<Func<PoolAllocator, Message>>(callAllocateMethod, allocatorParameter).Compile();

				if (reliable != null)
				{
					Assert.That((int)reliable.MessageType < 100, "Invalid reliable transmission message type.");

					MessageConstructors.Add(reliable.MessageType, constructMessage);
					TransmissionInfos.Add(messageType, new TransmissionInfo
					{
						BatchedTransmission = false,
						MessageType = reliable.MessageType,
						ReliableTransmission = true,
					});
				}

				if (unreliable != null)
				{
					Assert.That((int)unreliable.MessageType > 100, "Invalid unreliable transmission message type.");

					MessageConstructors.Add(unreliable.MessageType, constructMessage);
					TransmissionInfos.Add(messageType, new TransmissionInfo
					{
						BatchedTransmission = unreliable.EnableBatching,
						MessageType = unreliable.MessageType,
						ReliableTransmission = false,
					});
				}

				RuntimeHelpers.RunClassConstructor(messageType.TypeHandle);
			}
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected Message()
		{
			var transmissionInfo = TransmissionInfos[GetType()];

			MessageType = transmissionInfo.MessageType;
			IsReliable = transmissionInfo.ReliableTransmission;
			UseBatchedTransmission = transmissionInfo.BatchedTransmission;
		}

		/// <summary>
		///     Gets the type of the message.
		/// </summary>
		public MessageType MessageType { get; private set; }

		/// <summary>
		///     Gets a value indicating whether as many messages of this type as possible are batched together into a
		///     single network transmission.
		/// </summary>
		public bool UseBatchedTransmission { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the message is reliable.
		/// </summary>
		public bool IsReliable { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the message is unreliable.
		/// </summary>
		public bool IsUnreliable
		{
			get { return !IsReliable; }
		}

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public abstract void Serialize(BufferWriter writer);

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public abstract void Deserialize(BufferReader reader);

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public abstract void Dispatch(IMessageHandler handler, uint sequenceNumber);

		/// <summary>
		///     Creates a message instance for a message of the given message transmission type.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the message.</param>
		/// <param name="messageType">The message transmission type a message instance should be created for.</param>
		public static Message Create(PoolAllocator allocator, MessageType messageType)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentInRange(messageType);

			Func<PoolAllocator, Message> constructor;
			if (!MessageConstructors.TryGetValue(messageType, out constructor))
				throw new InvalidOperationException("Unsupported message type.");

			return constructor(allocator);
		}

		/// <summary>
		///     Provides transmission information about a message.
		/// </summary>
		private struct TransmissionInfo
		{
			/// <summary>
			///     Indicates whether as many messages as possible should be batched together for optimized transmission.
			/// </summary>
			public bool BatchedTransmission;

			/// <summary>
			///     The transmission type of the message.
			/// </summary>
			public MessageType MessageType;

			/// <summary>
			///     Indicates whether reliable or unreliable transmission should be used.
			/// </summary>
			public bool ReliableTransmission;
		}
	}
}