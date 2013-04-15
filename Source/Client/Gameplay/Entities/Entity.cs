using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Rendering;

	/// <summary>
	///   Represents an entity.
	/// </summary>
	/// <typeparam name="TEntity">The actual entity type.</typeparam>
	public abstract class Entity<TEntity> : PooledObject<TEntity>, IEntity
		where TEntity : Entity<TEntity>, new()
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected Entity()
		{
			Transform = new Transformation();
		}

		/// <summary>
		///   Gets the game session the entity belongs to.
		/// </summary>
		protected GameSession GameSession { get; private set; }

		/// <summary>
		///   Gets the render context that draws the entity.
		/// </summary>
		protected RenderContext RenderContext { get; private set; }

		/// <summary>
		///   Gets the transformation of the entity.
		/// </summary>
		public Transformation Transform { get; private set; }

		/// <summary>
		///   Gets or sets the entity's position relative to its parent.
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2(Transform.Position.X, Transform.Position.Z); }
			set { Transform.Position = new Vector3(value.X, 0, value.Y); }
		}

		/// <summary>
		///   Gets or sets the generational identifier of the entity.
		/// </summary>
		public Identifier Id { get; protected set; }

		/// <summary>
		///   Adds the entity to the game session and the render context.
		/// </summary>
		/// <param name="gameSession">The game session the entity should be added to.</param>
		/// <param name="renderContext">The render context the entity should be added to.</param>
		public void Added(GameSession gameSession, RenderContext renderContext)
		{
			Assert.ArgumentNotNull(gameSession, () => gameSession);
			Assert.ArgumentNotNull(renderContext, () => renderContext);

			GameSession = gameSession;
			RenderContext = renderContext;

			Transform.Reset();
			Transform.Attach(gameSession.RootTransform);

			RenderContext.Add(this as TEntity);
			OnAdded();
		}

		/// <summary>
		///   Removes the entity from the game session and the render context.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		public void Removed()
		{
			Transform.Detach();
			RenderContext.Remove(this as TEntity);

			OnRemoved();
		}

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		public virtual void Update()
		{
		}

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public abstract void RemoteUpdate(ref Message message);

		/// <summary>
		///   Invoked when the entity is added to the game session.
		/// </summary>
		protected virtual void OnAdded()
		{
		}

		/// <summary>
		///   Invoked when the entity is removed from the game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		protected virtual void OnRemoved()
		{
		}
	}
}