using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a rocket.
	/// </summary>
	public partial class Rocket : Entity<Rocket>
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.Update, "Unsupported update type.");

			Position = message.Update.Position;
			Rotation = MathUtils.DegToRad(message.Update.Rotation);
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the rocket.</param>
		public static Rocket Create(Identifier id)
		{
			var rocket = GetInstance();
			rocket.Id = id;
			rocket.Template = Templates.Rocket;
			return rocket;
		}
	}
}