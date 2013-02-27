using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;

	/// <summary>
	///   Handles the reception of a message.
	/// </summary>
	/// <param name="message">The message that should be handled.</param>
	public delegate void MessageHandler(ref Message message);

	/// <summary>
	///   Dispatches messages received from the server by raising message-specific events.
	/// </summary>
	public sealed class MessageDispatcher
	{
		public event MessageHandler PlayerJoined;

		/// <summary>
		///   Dispatches the messages contained in the queue.
		/// </summary>
		/// <param name="messages">The messages that should be dispatched.</param>
		public void Dispatch(Queue<Message> messages)
		{
			while (messages.Count != 0)
			{
				var message = messages.Dequeue();
				switch (message.Type)
				{
					case MessageType.Join:
						PlayerJoined(ref message);
						break;
					case MessageType.Leave:
					case MessageType.Selection:
					case MessageType.Name:
					case MessageType.Stats:

					default:
						throw new InvalidOperationException("Unexpected message type.");
				}
			}
		}
	}
}