using System;

namespace Lwar.Gameplay.Entities
{
	using Actors;
	using Network;
	using Network.Messages;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents an entity, a special form of an actor that is synchronized with and under control of the server.
	/// </summary>
	/// <typeparam name="TEntity">The actual entity type.</typeparam>
	public abstract class Entity<TEntity> : Actor<TEntity>, IEntity
		where TEntity : Entity<TEntity>, new()
	{
		/// <summary>
		///   Gets the template defining some of the entity's properties.
		/// </summary>
		public Template Template { get; protected set; }

		/// <summary>
		///   Gets or sets the entity's position relative to its parent.
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2(Transform.Position.X, Transform.Position.Z); }
			set { Transform.Position = new Vector3(value.X, 0, value.Y); }
		}

		/// <summary>
		///   Gets or sets the entity's rotation relative to its parent.
		/// </summary>
		public float Rotation
		{
			get { return Transform.Rotation.Y; }
			set
			{
				var rotation = Transform.Rotation;
				rotation.Y = value;
				Transform.Rotation = rotation;
			}
		}

		/// <summary>
		///   Gets or sets the generational identifier of the entity.
		/// </summary>
		public Identifier Id { get; protected set; }

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public abstract void RemoteUpdate(ref Message message);

		/// <summary>
		///   Invoked when the entity collided another entity.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impact">The position of the impact.</param>
		public virtual void CollidedWith(IEntity other, Vector2 impact)
		{
		}
	}
}