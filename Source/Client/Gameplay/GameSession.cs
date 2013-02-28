using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Rendering;

	/// <summary>
	///   Represents a game session, managing the state of entities, players, etc.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context that is used to draw the game session.</param>
		public GameSession(RenderContext renderContext)
		{
			Assert.ArgumentNotNull(renderContext, () => renderContext);

			Entities = new EntityList(this, renderContext);
			Players = new PlayerList();
			RootTransform = new Transformation();
		}

		/// <summary>
		///   The entities that are currently active.
		/// </summary>
		public EntityList Entities { get; private set; }

		/// <summary>
		///   The players that are currently playing.
		/// </summary>
		public PlayerList Players { get; private set; }

		/// <summary>
		///   Gets the root transformation.
		/// </summary>
		public Transformation RootTransform { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Entities.SafeDispose();
			Players.SafeDispose();
		}

		/// <summary>
		///   Updates the game session.
		/// </summary>
		public void Update()
		{
			Players.Update();
			Entities.Update();
			RootTransform.Update();
		}
	}
}