using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class JoinMessage : Message<JoinMessage>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the player that is added.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   Gets or sets a value that indicates whether this message corresponds to the local player.
		/// </summary>
		public bool IsLocalPlayer { get; set; }

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSessionOld session)
		{
			Assert.ArgumentNotNull(session, () => session);

			var player = new Player(_playerId);
			session.GameSession.Players.Add(player);
			session.GameSession.PlayerMap.Add(player);

			if (IsLocalPlayer)
				session.GameSession.LocalPlayer = player;
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static JoinMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m.IsLocalPlayer = false;
					m._playerId = b.ReadIdentifier();
				});
		}
	}
}