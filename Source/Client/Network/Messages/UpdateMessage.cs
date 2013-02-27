using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Rendering;

	public class UpdateMessage : Message<UpdateMessage>, IUnreliableMessage
	{
		/// <summary>
		///   The updated entity data.
		/// </summary>
		private readonly List<UpdateRecord> _updates = new List<UpdateRecord>();

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="gameSession">The game session that should be affected by the message.</param>
		/// <param name="renderContext">The render context that should be affected by the message.</param>
		public override void Process(GameSession gameSession, RenderContext renderContext)
		{
			//Assert.ArgumentNotNull(session, () => session);

			//foreach (var update in _updates)
			//{
			//	var entity = session.GameSession.EntityMap[update.EntityId];

			//	// Entity generation mismatch
			//	if (entity == null)
			//		continue;

			//	entity.RemoteUpdate(update, Timestamp);
			//}
		}

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		/// <param name="type">The type of the update records contained in the message.</param>
		public static UpdateMessage Create(BufferReader buffer, UpdateRecordType type)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentInRange(type, () => type);

			return Deserialize(buffer, (b, m) =>
				{
					m._updates.Clear();

					var count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						var record = new UpdateRecord { EntityId = b.ReadIdentifier(), Type = type };
						switch (type)
						{
							case UpdateRecordType.Full:
								record.Full.Position = new Vector2(b.ReadInt16(), b.ReadInt16());
								record.Full.Rotation = b.ReadUInt16();
								record.Full.Health = b.ReadByte();
								break;
							case UpdateRecordType.Position:
								record.Position = new Vector2(b.ReadInt16(), b.ReadInt16());
								break;
							case UpdateRecordType.Ray:
								record.Ray.Origin = new Vector2(b.ReadInt16(), b.ReadInt16());
								record.Ray.Direction = b.ReadUInt16();
								record.Ray.Length = b.ReadUInt16();
								break;
							case UpdateRecordType.Circle:
								record.Circle.Center = new Vector2(b.ReadInt16(), b.ReadInt16());
								record.Circle.Radius = b.ReadUInt16();
								break;
							default:
								throw new InvalidOperationException("Unknown update record type.");
						}

						m._updates.Add(record);
					}
				});
		}
	}
}