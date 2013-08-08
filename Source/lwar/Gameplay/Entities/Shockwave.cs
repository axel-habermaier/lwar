using System;

namespace Lwar.Gameplay.Entities
{
	using Network;
	using Network.Messages;
	using Pegasus.Framework;

	/// <summary>
	///   Represents a shockwave.
	/// </summary>
	public class Shockwave : Entity<Shockwave>
	{
		/// <summary>
		///   Gets the current radius of the shockwave.
		/// </summary>
		public float Radius { get; private set; }

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.UpdateCircle, "Unsupported update type.");

			Position = message.UpdateCircle.Center;
			Radius = message.UpdateCircle.Radius;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the shockwave.</param>
		public static Shockwave Create(Identifier id)
		{
			var shockwave = GetInstance();
			shockwave.Id = id;
			return shockwave;
		}
	}
}