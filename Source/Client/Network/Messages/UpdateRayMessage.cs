using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class UpdateRayMessage : UpdateMessage<UpdateRayMessage.Data, UpdateRayMessage>
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		/// <param name="entity">The entity that should be updated.</param>
		/// <param name="data">The updated data.</param>
		protected override void Process(GameSession session, IEntity entity, Data data)
		{
			// TODO
		}

		/// <summary>
		///   Extracts an update set from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the update set should be extracted.</param>
		protected override Data Extract(BufferReader buffer)
		{
			return new Data
			{
				Origin = new Vector2(buffer.ReadInt16(), buffer.ReadInt16()),
				Direction = buffer.ReadUInt16(),
				Length = buffer.ReadUInt16()
			};
		}

		public struct Data
		{
			public float Direction;
			public float Length;
			public Vector2 Origin;
		}
	}
}