using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;

	/// <summary>
	///   Represents a planet.
	/// </summary>
	public class Planet : Entity<Planet>
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.Update, "Unsupported update type.");
			Position = message.Update.Position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the planet.</param>
		public static Planet Create(Identifier id)
		{
			var planet = GetInstance();
			planet.Id = id;
			return planet;
		}
	}
}