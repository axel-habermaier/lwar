using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Pegasus.Framework;
	using Rendering;

	/// <summary>
	///   Represents a ship that is controlled by a local or remote player.
	/// </summary>
	public class Ship : PooledObject<Ship>, IEntity
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Ship()
		{
			Transform = new Transformation();
		}

		/// <summary>
		///   Gets or sets the player the ship belongs to.
		/// </summary>
		public Player Player { get; private set; }

		/// <summary>
		///   Gets the transformation of the ship.
		/// </summary>
		public Transformation Transform { get; private set; }

		/// <summary>
		///   The health of the ship.
		/// </summary>
		public int Health { get; private set; }

		/// <summary>
		///   Gets or sets the generational identifier of the object.
		/// </summary>
		public Identifier Id { get; private set; }

		/// <summary>
		///   Adds the entity to the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be added to.</param>
		/// <param name="renderContext">The render context the entity should be added to.</param>
		public void Added(GameSession gameSession, RenderContext renderContext)
		{
			Transform.Reset();
			Transform.Attach(gameSession.RootTransform);

			renderContext.ShipRenderer.Add(this);
		}

		/// <summary>
		///   Removes the entity from the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be removed from.</param>
		/// <param name="renderContext">The render context the entity should be removed from.</param>
		/// <remarks> The remove method is not called when the game session is shut down.</remarks>
		public void Removed(GameSession gameSession, RenderContext renderContext)
		{
			renderContext.ShipRenderer.Remove(this);
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
		//	Assert.That(update.Type == UpdateRecordType.Full, "Unsupported update type.");

		//	Transform.Position = new Vector3(update.Full.Position.X, 0, update.Full.Position.Y);
		//	Transform.Rotation = new Vector3(0, update.Full.Rotation, 0);
		//	Health = update.Full.Health;
		//}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the ship.</param>
		/// <param name="player">The player the ship belongs to.</param>
		public static Ship Create(Identifier id, Player player)
		{
			Assert.ArgumentNotNull(player, () => player);

			var ship = GetInstance();
			ship.Id = id;
			ship.Player = player;
			return ship;
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Ship {0}, Player: {1}", Id, Player);
		}
	}
}