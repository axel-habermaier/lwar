using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
	using Gameplay;

	/// <summary>
	///   Dispatches messages received from the server.
	/// </summary>
	public sealed class MessageDispatcher
	{
		/// <summary>
		///   The game session the messages are dispatched to.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the messages should be dispatched to.</param>
		public MessageDispatcher(GameSession gameSession)
		{
			_gameSession = gameSession;
		}

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
					case MessageType.Chat:
						break;
					case MessageType.Name:
						break;
					case MessageType.Selection:
						break;
					case MessageType.Add:
						break;
					case MessageType.Collision:
						break;
					case MessageType.Join:
						_gameSession.Players.Add(message.Join.Player, message.Join.IsLocalPlayer);
						break;
					case MessageType.Leave:
						_gameSession.Players.Remove(message.Remove);
						break;
					case MessageType.Remove:
						break;
					case MessageType.Stats:
						break;
					case MessageType.Update:
						break;
					case MessageType.UpdatePosition:
						break;
					case MessageType.UpdateRay:
						break;
					case MessageType.UpdateCircle:
						break;
					default:
						throw new InvalidOperationException("Unexpected message type.");
				}
			}
		}
	}
}