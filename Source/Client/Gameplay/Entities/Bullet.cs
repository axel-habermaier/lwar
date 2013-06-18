using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a bullet.
	/// </summary>
	public class Bullet : Entity<Bullet>
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.UpdatePosition, "Unsupported update type.");

			Rotation = MathUtils.ComputeAngle(Position, message.UpdatePosition.Position, new Vector2(1, 0));
			Position = message.UpdatePosition.Position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the bullet.</param>
		public static Bullet Create(Identifier id)
		{
			var bullet = GetInstance();
			bullet.Id = id;
			bullet.Template = Templates.Bullet;
			return bullet;
		}
	}
}