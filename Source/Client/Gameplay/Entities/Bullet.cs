using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a bullet.
	/// </summary>
	public class Bullet : Entity<Bullet>
	{
		// TODO: Remove this hack
		/// <summary>
		///   Indicates how many update message have been received for the bullet.
		/// </summary>
		private int _updateCount;

		// TODO: Remove this hack
		/// <summary>
		///   Gets a value indicating whether the bullet is valid and should be drawn. This flag is used to filter out bullets for
		///   which less than two update messages have been received, which would cause the bullet to be drawn oriented
		///   incorrectly.
		/// </summary>
		public bool IsValid
		{
			get { return _updateCount >= 2; }
		}

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.UpdatePosition, "Unsupported update type.");

			Rotation = MathUtils.ComputeAngle(Position, message.UpdatePosition.Position, new Vector2(1, 0));
			Position = message.UpdatePosition.Position;

			// TODO: Remove this hack
			++_updateCount;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the bullet.</param>
		public static Bullet Create(Identifier id)
		{
			var bullet = GetInstance();
			bullet.Id = id;
			bullet._updateCount = 0;
			bullet.Template = Templates.Bullet;
			return bullet;
		}
	}
}