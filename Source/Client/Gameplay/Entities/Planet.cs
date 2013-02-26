using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Rendering;

	/// <summary>
	///   Represents a planet.
	/// </summary>
	public class Planet : PooledObject<Planet>, IEntity
	{
		/// <summary>
		///   Gets the transformation of the planet.
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
			Transform = Transformation.Create(gameSession.RootTransform);
			renderContext.PlanetRenderer.Add(this);
		}

		/// <summary>
		///   Removes the entity from the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be removed from.</param>
		/// <param name="renderContext">The render context the entity should be removed from.</param>
		public void Removed(GameSession gameSession, RenderContext renderContext)
		{
			renderContext.PlanetRenderer.Remove(this);
			Transform.SafeDispose();
		}

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		public void Update()
		{
		}

		/// <summary>
		///   Applies the remote update record to the entity's state.
		/// </summary>
		/// <param name="update">The update record that has been sent by the server for this entity.</param>
		/// <param name="timestamp">The timestamp that indicates when the update record has been sent.</param>
		public void RemoteUpdate(UpdateRecord update, uint timestamp)
		{
			Assert.That(false, "Server updates are not supported for planets.");
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the planet.</param>
		public static Planet Create(Identifier id)
		{
			var planet = GetInstance();
			planet.Id = id;
			return planet;
		}
	}
}