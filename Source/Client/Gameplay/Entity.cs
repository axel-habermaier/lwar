using System;

namespace Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Gameplay;

	/// <summary>
	///   An abstract base class for all entities in a level.
	/// </summary>
	/// <typeparam name="TEntity">The concrete entity type.</typeparam>
	public abstract class Entity<TEntity> : PooledObject<TEntity>, IEntity
		where TEntity : Entity<TEntity>, new()
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected Entity()
		{
			Id = EntityIdentifier.Create();
		}

		/// <summary>
		///   Gets the game session the entity belongs to.
		/// </summary>
		protected GameSession Session { get; private set; }

		/// <summary>
		///   Gets or sets the entity's unique identifier.
		/// </summary>
		public EntityIdentifier Id { get; set; }

		/// <summary>
		///   Gets or sets the entity's position.
		/// </summary>
		public Vector2f8 Position { get; set; }

		#region Lifecycle 

		/// <summary>
		///   Invoked when the entity should update its internal state.
		/// </summary>
		public virtual void Update()
		{
		}

		/// <summary>
		///   Invoked when the entity should draw itself.
		/// </summary>
		public virtual void Draw()
		{
		}

		/// <summary>
		///   Invoked when the entity has been removed from a level.
		/// </summary>
		public virtual void Removed()
		{
		}

		/// <summary>
		///   Invoked when the entity has been added to a level.
		/// </summary>
		/// <param name="session">The game session the entity belongs to.</param>
		public void Added(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			Session = session;
			Added();
		}

		/// <summary>
		///   Invoked when the entity has been added to a level.
		/// </summary>
		protected virtual void Added()
		{
		}

		#endregion
	}
}