using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a phaser.
	/// </summary>
	public class Phaser : Entity<Phaser>
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.UpdateRay, "Unsupported update type.");
			Transform.Position = new Vector3(message.UpdateRay.Origin.X, 0, message.UpdateRay.Origin.Y);
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the phaser.</param>
		public static Phaser Create(Identifier id)
		{
			var phaser = GetInstance();
			phaser.Id = id;
			return phaser;
		}
	}
}