namespace Lwar.Network
{
	using System;
	using Messages;

	/// <summary>
	///     Handles incoming messages from a remote peer, updating the game state of the client or server while possibly also
	///     sending messages back to the remote peer.
	/// </summary>
	public interface IMessageHandler
	{
		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnEntityAdded(EntityAddMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerChatMessage(PlayerChatMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnEntityCollision(EntityCollisionMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnConnect(ClientConnectMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnDisconnect(DisconnectMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerInput(PlayerInputMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerJoin(PlayerJoinMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerKill(PlayerKillMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerLeave(PlayerLeaveMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerName(PlayerNameMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnReject(ClientRejectedMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnEntityRemove(EntityRemoveMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnPlayerSelection(PlayerLoadoutMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void OnPlayerStats(PlayerStatsMessage message, uint sequenceNumber);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void OnSynced(ClientSyncedMessage message);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void OnUpdateCircle(UpdateCircleMessage message, uint sequenceNumber);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void OnUpdateTransform(UpdateTransformMessage message, uint sequenceNumber);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void OnUpdatePosition(UpdatePositionMessage message, uint sequenceNumber);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void OnUpdateRay(UpdateRayMessage message, uint sequenceNumber);

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		void OnUpdateShip(UpdateShipMessage message, uint sequenceNumber);
	}
}