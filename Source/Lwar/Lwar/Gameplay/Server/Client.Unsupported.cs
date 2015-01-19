namespace Lwar.Gameplay.Server
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Utilities;

	internal partial class Client
	{
		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerKill(PlayerKillMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnEntityAdded(EntityAddMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerJoin(PlayerJoinMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerLeave(PlayerLeaveMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnReject(ClientRejectedMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnEntityRemove(EntityRemoveMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void IMessageHandler.OnPlayerStats(PlayerStatsMessage message, uint sequenceNumber)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnSynced(ClientSyncedMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void IMessageHandler.OnUpdateCircle(UpdateCircleMessage message, uint sequenceNumber)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void IMessageHandler.OnUpdateTransform(UpdateTransformMessage message, uint sequenceNumber)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void IMessageHandler.OnUpdatePosition(UpdatePositionMessage message, uint sequenceNumber)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void IMessageHandler.OnUpdateRay(UpdateRayMessage message, uint sequenceNumber)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void IMessageHandler.OnUpdateShip(UpdateShipMessage message, uint sequenceNumber)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnEntityCollision(EntityCollisionMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles an unsupported message.
		/// </summary>
		/// <param name="message">The unsupported message that should be handled.</param>
		private void HandleUnsupportedMessage(Message message)
		{
			Assert.NotReached("Received an unexpected message of type '{0}' from client at '{1}'.", message.MessageType, _connection.RemoteEndPoint);
		}
	}
}