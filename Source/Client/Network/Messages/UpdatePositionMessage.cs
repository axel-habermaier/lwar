using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class UpdatePositionMessage : UpdateMessage<Vector2, UpdatePositionMessage>
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		/// <param name="entity">The entity that should be updated.</param>
		/// <param name="position">The updated position.</param>
		protected override void Process(GameSession session, IEntity entity, Vector2 position)
		{
			entity.Position = position;
		}

		/// <summary>
		///   Extracts an update set from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the update set should be extracted.</param>
		protected override Vector2 Extract(BufferReader buffer)
		{
			return new Vector2(buffer.ReadInt16(), buffer.ReadInt16());
		}
	}
}