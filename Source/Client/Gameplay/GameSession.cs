using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections.Generic;
	using Entities;
	using Pegasus.Framework;

	/// <summary>
	///   Represents a game session, managing the state of entities, players, etc.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public GameSession()
		{
			Entities = new DeferredList<IEntity>(false);
			EntityMap = new IdentifierMap<IEntity>();
			Players = new List<Player>();
			PlayerMap = new IdentifierMap<Player>();
			RootTransform = Transformation.Create();
		}

		/// <summary>
		///   The entities that are currently active.
		/// </summary>
		public DeferredList<IEntity> Entities { get; private set; }

		/// <summary>
		///   Maps generational identifiers to entity instances.
		/// </summary>
		public IdentifierMap<IEntity> EntityMap { get; private set; }

		/// <summary>
		///   The players that are currently playing.
		/// </summary>
		public List<Player> Players { get; private set; }

		/// <summary>
		///   Maps generational identifiers to player instances.
		/// </summary>
		public IdentifierMap<Player> PlayerMap { get; private set; }

		/// <summary>
		///   Gets or sets the player instance of the local player.
		/// </summary>
		public Player LocalPlayer { get; set; }

		/// <summary>
		///   Gets the root transformation.
		/// </summary>
		public Transformation RootTransform { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Entities.SafeDisposeAll();
			RootTransform.SafeDispose();
		}

		/// <summary>
		///   Updates the game session.
		/// </summary>
		public void Update()
		{
			foreach (var entity in Entities)
				entity.Update();
		}
	}
}