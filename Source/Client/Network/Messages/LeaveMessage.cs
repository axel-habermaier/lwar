using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Rendering;

	public class LeaveMessage : Message<LeaveMessage>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the player that is added.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="gameSession">The game session that should be affected by the message.</param>
		/// <param name="renderContext">The render context that should be affected by the message.</param>
		public override void Process(GameSession gameSession, RenderContext renderContext)
		{
			//Assert.ArgumentNotNull(session, () => session);
			//Assert.That(_playerId != session.GameSession.LocalPlayer.Id, "Cannot remove the local player.");

			//var player = session.GameSession.PlayerMap[_playerId];
			//Assert.NotNull(player, "Server sent a remove message for an unknown player.");

			//session.GameSession.PlayerMap.Remove(player);
			//session.GameSession.Players.Remove(player);
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static LeaveMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) => m._playerId = b.ReadIdentifier());
		}
	}
}