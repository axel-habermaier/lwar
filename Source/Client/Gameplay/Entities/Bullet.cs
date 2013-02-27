using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Pegasus.Framework;
	using Rendering;

	/// <summary>
	///   Represents a bullet.
	/// </summary>
	public class Bullet : PooledObject<Bullet>, IEntity
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Bullet()
		{
			Transform = new Transformation();
		}

		/// <summary>
		///   Gets the transformation of the bullet.
		/// </summary>
		public Transformation Transform { get; private set; }

		/// <summary>
		///   Gets or sets the generational identifier of the object.
		/// </summary>
		public Identifier Id { get; set; }

		/// <summary>
		///   Adds the entity to the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be added to.</param>
		/// <param name="renderContext">The render context the entity should be added to.</param>
		public void Added(GameSession gameSession, RenderContext renderContext)
		{
			Transform.Reset();
			Transform.Attach(gameSession.RootTransform);

			renderContext.BulletRenderer.Add(this);
		}

		/// <summary>
		///   Removes the entity from the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be removed from.</param>
		/// <param name="renderContext">The render context the entity should be removed from.</param>
		/// <remarks> The remove method is not called when the game session is shut down.</remarks>
		public void Removed(GameSession gameSession, RenderContext renderContext)
		{
			renderContext.BulletRenderer.Remove(this);
			Transform.Detach();
		}

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		public void Update()
		{
		}

		///// <summary>
		/////   Applies the remote update record to the entity's state.
		///// </summary>
		///// <param name="update">The update record that has been sent by the server for this entity.</param>
		///// <param name="timestamp">The timestamp that indicates when the update record has been sent.</param>
		//public void RemoteUpdate(UpdateRecord update, uint timestamp)
		//{
		//	Assert.That(update.Type == UpdateRecordType.Position, "Unsupported update type.");
		//	Transform.Position = new Vector3(update.Position.X, 0, update.Position.Y);
		//}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the bullet.</param>
		public static Bullet Create(Identifier id)
		{
			var bullet = GetInstance();
			bullet.Id = id;
			return bullet;
		}
	}
}