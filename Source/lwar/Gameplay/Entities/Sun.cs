using System;

namespace Lwar.Gameplay.Entities
{
	using Network;
	using Network.Messages;
	using Pegasus.Framework;

	/// <summary>
	///   Represents a sun.
	/// </summary>
	public class Sun : Entity<Sun>
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.UpdatePosition, "Unsupported update type.");
			Position = message.UpdatePosition.Position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the sun.</param>
		public static Sun Create(Identifier id)
		{
			var sun = GetInstance();
			sun.Id = id;
			sun.Template = Templates.Sun;
			return sun;
		}
	}
}