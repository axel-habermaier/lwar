namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Actors;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;

	/// <summary>
	///     Represents an entity, a special form of an actor that is synchronized with and under control of the server.
	/// </summary>
	/// <typeparam name="TEntity">The actual entity type.</typeparam>
	public abstract class Entity<TEntity> : Actor<TEntity>, IEntity
		where TEntity : Entity<TEntity>, new()
	{
		/// <summary>
		///     The sequence number of the last remote update of the entity.
		/// </summary>
		private uint _lastUpdateSequenceNumber;

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
		///     Gets or sets the entity's orientation relative to its parent.
		/// </summary>
		public float Orientation
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
		///     Gets the generational identity of the entity.
		/// </summary>
		public Identity Identifier { get; protected set; }

		/// <summary>
		///     Applies a remote position update to the entity.
		/// </summary>
		/// <param name="position">The updated entity position.</param>
		public virtual void RemotePositionUpdate(Vector2 position)
		{
			Position = position;
		}

		/// <summary>
		///     Applies a remote orientation update to the entity.
		/// </summary>
		/// <param name="rotation">The updated entity rotation.</param>
		public virtual void RemoteOrientationUpdate(float rotation)
		{
			Orientation = MathUtils.DegToRad(rotation);
		}

		/// <summary>
		///     Handles a collision between this entity and another entity at the given impact position.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impactPosition">The position of the impact.</param>
		public virtual void OnCollision(IEntity other, Vector2 impactPosition)
		{
		}

		/// <summary>
		///     Checks whether the entity accepts an update with the given sequence number. All following entity updates are only
		///     accepted when their sequence number exceeds the given one.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		public bool AcceptUpdate(uint sequenceNumber)
		{
			if (_lastUpdateSequenceNumber >= sequenceNumber)
			{
				Log.Debug("Entity rejected outdated update.");
				return false;
			}

			_lastUpdateSequenceNumber = sequenceNumber;
			return true;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_lastUpdateSequenceNumber = 0;
			base.OnReturning();
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