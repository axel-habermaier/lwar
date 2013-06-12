using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a ray.
	/// </summary>
	public class Ray : Entity<Ray>
	{
		/// <summary>
		///   Gets the length of the ray.
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.UpdateRay, "Unsupported update type.");
			Position = message.UpdateRay.Origin;

			Rotation = MathUtils.DegToRad(message.UpdateRay.Direction);
			Length = message.UpdateRay.Length;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the ray.</param>
		public static Ray Create(Identifier id)
		{
			var ray = GetInstance();
			ray.Id = id;
			ray.Template = Templates.Ray;
			return ray;
		}
	}
}