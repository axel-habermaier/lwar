using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Rendering;

	public class RemoveMessage : Message<RemoveMessage>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the entity that is removed.
		/// </summary>
		private Identifier _entityId;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="gameSession">The game session that should be affected by the message.</param>
		/// <param name="renderContext">The render context that should be affected by the message.</param>
		public override void Process(GameSession gameSession, RenderContext renderContext)
		{
			//Assert.ArgumentNotNull(session, () => session);

			//var entity = session.GameSession.EntityMap[_entityId];
			//Assert.NotNull(entity, "Server sent a remove message for an unknown entity.");

			//session.GameSession.EntityMap.Remove(entity);
			//session.GameSession.Entities.Remove(entity);
			//entity.Removed(session.GameSession, session.RenderContext);
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static RemoveMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) => m._entityId = b.ReadIdentifier());
		}
	}
}