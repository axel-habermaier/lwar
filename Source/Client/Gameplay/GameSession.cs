using System;

namespace Lwar.Client.Gameplay
{
	using Actors;
	using Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Memory;
	using Rendering;

	/// <summary>
	///   Represents a game session, managing the state of entities, players, etc.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   The clock that is used for time measurements.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context that is used to draw the game session.</param>
		public GameSession(RenderContext renderContext)
		{
			Assert.ArgumentNotNull(renderContext);

			Actors = new ActorList(this, renderContext);
			Entities = new EntityList(this, renderContext);
			Players = new PlayerList();
			RootTransform = new Transformation();
			EventMessages = new EventMessageList(this);
		}

		/// <summary>
		///   The entities that are currently active.
		/// </summary>
		public EntityList Entities { get; private set; }

		/// <summary>
		///   The actors that are currently active, not including any entities.
		/// </summary>
		public ActorList Actors { get; private set; }

		/// <summary>
		///   The players that are currently playing.
		/// </summary>
		public PlayerList Players { get; private set; }

		/// <summary>
		///   Gets the root transformation.
		/// </summary>
		public Transformation RootTransform { get; private set; }

		/// <summary>
		///   Gets the event messages that display game session events to the user such as player kills or received chat messages.
		/// </summary>
		public EventMessageList EventMessages { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Actors.SafeDispose();
			Entities.SafeDispose();
			Players.SafeDispose();
			EventMessages.SafeDispose();
			_clock.SafeDispose();
		}

		private float t = 10;
		/// <summary>
		///   Updates the game session.
		/// </summary>
		public void Update()
		{
			Players.Update();
			Entities.Update(_clock);
			Actors.Update(_clock);
			RootTransform.Update();
			EventMessages.Update();

			t += (float)_clock.Seconds;
			if (t > 3 && Players.LocalPlayer != null && Players.LocalPlayer.Ship != null)
			{
				t = 0;
				Players.LocalPlayer.Ship.CollidedWith(null, Players.LocalPlayer.Ship.Position - new Vector2(64, 64));
			}

			_clock.Reset();
		}
	}
}