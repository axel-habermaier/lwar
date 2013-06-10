using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a bullet.
	/// </summary>
	public partial class Bullet : Entity<Bullet>
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.Update, "Unsupported update type.");

			Rotation = MathUtils.ComputeAngle(Position, message.Update.Position, new Vector2(1, 0));
			Position = message.Update.Position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the bullet.</param>
		public static Bullet Create(Identifier id)
		{
			var bullet = GetInstance();
			bullet.Id = id;
			return bullet;
		}
	}
}