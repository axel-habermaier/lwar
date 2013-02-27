using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Rendering;

	/// <summary>
	///   Represents a planet.
	/// </summary>
	public class Planet : PooledObject<Planet>, IEntity
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Planet()
		{
			Transform = new Transformation();
		}

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
			Transform.Reset();
			Transform.Attach(gameSession.RootTransform);

			renderContext.PlanetRenderer.Add(this);
		}

		/// <summary>
		///   Removes the entity from the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be removed from.</param>
		/// <param name="renderContext">The render context the entity should be removed from.</param>
		/// <remarks> The remove method is not called when the game session is shut down.</remarks>
		public void Removed(GameSession gameSession, RenderContext renderContext)
		{
			renderContext.PlanetRenderer.Remove(this);
			Transform.Detach();
		}

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		public void Update()
		{
		}

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.Update, "Unsupported update type.");
			Transform.Position = new Vector3(message.Update.Position.X, 0, message.Update.Position.Y);
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