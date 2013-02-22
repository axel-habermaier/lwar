﻿using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class UpdateMessage : UpdateMessage<UpdateMessage.Data, UpdateMessage>
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		/// <param name="entity">The entity that should be updated.</param>
		/// <param name="data">The updated data.</param>
		protected override void Process(GameSession session, IEntity entity, Data data)
		{
			entity.Position = data.Position;
			entity.Rotation = MathUtils.DegToRad(data.Rotation);
			entity.Health = data.Health;
		}

		/// <summary>
		///   Extracts an update set from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the update set should be extracted.</param>
		protected override Data Extract(BufferReader buffer)
		{
			return new Data
			{
				Position = new Vector2(buffer.ReadInt16(), buffer.ReadInt16()),
				Rotation = buffer.ReadUInt16(),
				Health = buffer.ReadByte()
			};
		}

		public struct Data
		{
			public int Health;
			public Vector2 Position;
			public ushort Rotation;
		}
	}
}