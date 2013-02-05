using System;

namespace Lwar.Client.Network
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Represents a message that is used for the communication between the server and the clients.
	/// </summary>
	/// <typeparam name="TMessage">The concrete message type.</typeparam>
	public abstract class Message<TMessage> : PooledObject<TMessage>
		where TMessage : Message<TMessage>, new()
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public virtual void Process(GameSession session)
		{
			Assert.That(false, "The client cannot process this type of message.");
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public virtual bool Write(BufferWriter buffer)
		{
			Assert.That(false, "The client cannot send this type of message.");
			return true;
		}

		/// <summary>
		///   Tries to deserialize the message from the buffer, using the given deserializer. Returns null if the deserializer
		///   read past the end of the buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the message should be deserialized.</param>
		/// <param name="deserializer">The deserialized that should be used to deserialize the message.</param>
		protected static TMessage Deserialize(BufferReader buffer, Action<BufferReader, TMessage> deserializer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentNotNull(deserializer, () => deserializer);

			var message = GetInstance();
			if (buffer.TryRead(message, deserializer))
				return message;

			message.SafeDispose();
			return null;
		}
	}
}