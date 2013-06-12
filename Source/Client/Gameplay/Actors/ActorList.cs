using System;

namespace Lwar.Client.Gameplay.Actors
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Memory;
	using Rendering;

	/// <summary>
	///   Manages the active actors of a game session.
	/// </summary>
	public sealed class ActorList : DisposableObject
	{
		/// <summary>
		///   The list of active actors.
		/// </summary>
		private readonly DeferredList<IActor> _actors = new DeferredList<IActor>(false);

		/// <summary>
		///   The game session the actor list belongs to.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The render context that is used to draw the actors.
		/// </summary>
		private readonly RenderContext _renderContext;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the actor list belongs to.</param>
		/// <param name="renderContext">The render context that should be used to draw the actors.</param>
		public ActorList(GameSession gameSession, RenderContext renderContext)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(renderContext);

			_gameSession = gameSession;
			_renderContext = renderContext;
		}

		/// <summary>
		///   Adds the given actor to the list.
		/// </summary>
		/// <param name="actor">The actor that should be added.</param>
		public void Add(IActor actor)
		{
			Assert.ArgumentNotNull(actor);

			_actors.Add(actor);
			actor.Added(_gameSession, _renderContext);
		}

		/// <summary>
		///   Removes the given actor from the list.
		/// </summary>
		/// <param name="actor">The actor that should be removed.</param>
		public void Remove(IActor actor)
		{
			Assert.ArgumentNotNull(actor);

			_actors.Remove(actor);
			actor.Removed();
		}

		/// <summary>
		///   Updates the entity list.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public void Update(Clock clock)
		{
			Assert.ArgumentNotNull(clock);
			_actors.Update();

			foreach (var actor in _actors)
				actor.Update(clock);
		}

		/// <summary>
		///   Enumerates all active entities.
		/// </summary>
		public List<IActor>.Enumerator GetEnumerator()
		{
			return _actors.GetEnumerator();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_actors.SafeDispose();
		}
	}
}