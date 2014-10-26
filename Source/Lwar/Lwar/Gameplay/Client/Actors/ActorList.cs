namespace Lwar.Gameplay.Client.Actors
{
	using System;
	using System.Collections.Generic;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Rendering;

	/// <summary>
	///     Manages the active actors of a game session.
	/// </summary>
	public sealed class ActorList : DisposableObject
	{
		/// <summary>
		///     The list of active actors.
		/// </summary>
		private readonly List<IActor> _actors = new List<IActor>();

		/// <summary>
		///     The game session the actor list belongs to.
		/// </summary>
		private readonly ClientGameSession _gameSession;

		/// <summary>
		///     The render context that is used to draw the actors.
		/// </summary>
		private readonly RenderContext _renderContext;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the actor list belongs to.</param>
		/// <param name="renderContext">The render context that should be used to draw the actors.</param>
		public ActorList(ClientGameSession gameSession, RenderContext renderContext)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(renderContext);

			_gameSession = gameSession;
			_renderContext = renderContext;
		}

		/// <summary>
		///     Adds the given actor to the list.
		/// </summary>
		/// <param name="actor">The actor that should be added.</param>
		public void Add(IActor actor)
		{
			Assert.ArgumentNotNull(actor);

			_actors.Add(actor);
			actor.Added(_gameSession, _renderContext);
		}

		/// <summary>
		///     Removes the given actor from the list.
		/// </summary>
		/// <param name="actor">The actor that should be removed.</param>
		public void Remove(IActor actor)
		{
			Assert.ArgumentNotNull(actor);
			actor.Removed();
		}

		/// <summary>
		///     Updates the entity list.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			for (var i = 0; i < _actors.Count; ++i)
			{
				if (_actors[i].IsRemoved)
				{
					_actors[i].SafeDispose();
					_actors[i] = _actors[_actors.Count - 1];
					_actors.RemoveAt(_actors.Count - 1);
					--i;
				}
				else
					_actors[i].Update(elapsedSeconds);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_actors.SafeDisposeAll();
		}
	}
}