using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class CollisionMessage : Message<CollisionMessage>, IUnreliableMessage
	{
		/// <summary>
		///   The identifier of the first entity involved in the collision.
		/// </summary>
		private Identifier _entity1;

		/// <summary>
		///   The identifier of the second entity involved in the collision.
		/// </summary>
		private Identifier _entity2;

		/// <summary>
		///   The position of the impact.
		/// </summary>
		private Vector2 _impactPosition;

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSessionOld session)
		{
			Assert.ArgumentNotNull(session, () => session);

			//var entity1 = session.Entities.Find(_entity1);
			//var entity2 = session.Entities.Find(_entity2);

			//// There's nothing we can do if either of the entities has already been removed
			//if (entity1 == null || entity2 == null)
			//	return;

			//entity1.OnCollision(entity2, _impactPosition);
			//entity2.OnCollision(entity1, _impactPosition);
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static CollisionMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._entity1 = b.ReadIdentifier();
					m._entity2 = b.ReadIdentifier();
					m._impactPosition = new Vector2(b.ReadInt16(), b.ReadInt16());
				});
		}
	}
}