using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Rendering;

	/// <summary>
	///   An abstract base class for all entities in a level.
	/// </summary>
	/// <typeparam name="TEntity">The concrete entity type.</typeparam>
	public abstract class Entity<TEntity> : PooledObject<TEntity>, IEntity
		where TEntity : Entity<TEntity>, new()
	{
		/// <summary>
		///   Gets the game session the entity belongs to.
		/// </summary>
		protected GameSession Session { get; private set; }

		/// <summary>
		///   Gets the graphics device that is used to draw the entity.
		/// </summary>
		protected GraphicsDevice GraphicsDevice
		{
			get { return Session.GraphicsDevice; }
		}

		/// <summary>
		///   Gets the assets manager that manages all the assets of all entities.
		/// </summary>
		protected AssetsManager Assets
		{
			get { return Session.Assets; }
		}

		/// <summary>
		///   The render context that is used to draw the entity.
		/// </summary>
		protected RenderContext RenderContext
		{
			get { return Session.RenderContext; }
		}

		/// <summary>
		///   Gets or sets the entity's unique identifier.
		/// </summary>
		public Identifier Id { get; set; }

		/// <summary>
		///   Gets or sets the entity's position.
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		///   Gets or sets the entity's rotation.
		/// </summary>
		public float Rotation { get; set; }

		/// <summary>
		///   Gets or sets the entity's health.
		/// </summary>
		public float Health { get; set; }

		/// <summary>
		///   Gets or sets the player the entity belongs to.
		/// </summary>
		public Player Player { get; set; }

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
		///   Invoked when a collision occured between the entity and the given other entity.
		/// </summary>
		/// <param name="other">The other entity of the collision.</param>
		/// <param name="position">The position of the collision.</param>
		public virtual void OnCollision(IEntity other, Vector2 position)
		{
		}

		/// <summary>
		///   Invoked when the entity has been added to a level.
		/// </summary>
		protected virtual void Added()
		{
		}
	}
}