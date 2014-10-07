namespace Lwar.Gameplay.Entities
{
	using System;
	using Actors;
	using Pegasus;
	using Pegasus.Math;

	/// <summary>
	///     Represents an entity, a special form of an actor that is synchronized with and under control of the server.
	/// </summary>
	/// <typeparam name="TEntity">The actual entity type.</typeparam>
	public abstract class Entity<TEntity> : Actor<TEntity>, IEntity
		where TEntity : Entity<TEntity>, new()
	{
		/// <summary>
		///     Gets the template defining some of the entity's properties.
		/// </summary>
		public EntityTemplate Template { get; protected set; }

		/// <summary>
		///     Gets or sets the entity's position relative to its parent.
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2(Transform.Position.X, Transform.Position.Z); }
			set { Transform.Position = new Vector3(value.X, 0, value.Y); }
		}

		/// <summary>
		///     Gets or sets the entity's rotation relative to its parent.
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
		///     Gets the generational identifier of the entity.
		/// </summary>
		public Identifier Identifier { get; protected set; }

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="position">The updated entity position.</param>
		public virtual void RemotePositionUpdate(Vector2 position)
		{
			Position = position;
		}

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="rotation">The updated entity rotation.</param>
		public virtual void RemoteRotationUpdate(float rotation)
		{
			Rotation = MathUtils.DegToRad(rotation);
		}

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="center">The updated circle center.</param>
		/// <param name="radius">The updated circle radius.</param>
		public virtual void RemoteCircleUpdate(Vector2 center, float radius)
		{
			Assert.That(false, "Circle updates are not supported by the entity.");
		}

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="origin">The updated ray origin.</param>
		/// <param name="direction">The updated ray direction.</param>
		/// <param name="length">The updated ray length.</param>
		/// <param name="target">The current ray target or null if no target is hit.</param>
		public virtual void RemoteRayUpdate(Vector2 origin, float direction, float length, IEntity target)
		{
			Assert.That(false, "Ray updates are not supported by the entity.");
		}

		/// <summary>
		///     Invoked when the entity collided another entity.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impact">The position of the impact.</param>
		public virtual void CollidedWith(IEntity other, Vector2 impact)
		{
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} {1}", GetType().Name, Identifier);
		}
	}
}