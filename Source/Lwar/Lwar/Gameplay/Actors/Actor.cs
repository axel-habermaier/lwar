namespace Lwar.Gameplay.Actors
{
	using System;
	using Pegasus;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;
	using Rendering;

	/// <summary>
	///     The base class of all actors.
	/// </summary>
	public abstract class Actor<TActor> : OldPooledObject<TActor>, IActor
		where TActor : Actor<TActor>, new()
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected Actor()
		{
			Transform = new Transformation();
		}

		/// <summary>
		///     Gets the game session the actor belongs to.
		/// </summary>
		protected GameSession GameSession { get; private set; }

		/// <summary>
		///     Gets the render context that draws the actor.
		/// </summary>
		protected RenderContext RenderContext { get; private set; }

		/// <summary>
		///     Gets the transformation of the actor.
		/// </summary>
		public Transformation Transform { get; private set; }

		/// <summary>
		///     Adds the actor to the game session and the render context.
		/// </summary>
		/// <param name="gameSession">The game session the actor should be added to.</param>
		/// <param name="renderContext">The render context the actor should be added to.</param>
		public void Added(GameSession gameSession, RenderContext renderContext)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(renderContext);

			GameSession = gameSession;
			RenderContext = renderContext;

			Transform.Reset();
			Transform.AttachTo(gameSession.RootTransform);

			RenderContext.Add(this as TActor);
			OnAdded();
		}

		/// <summary>
		///     Removes the actor from the game session and the render context.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		public void Removed()
		{
			Transform.Detach();
			RenderContext.Remove(this as TActor);

			OnRemoved();
		}

		/// <summary>
		///     Updates the actor's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public virtual void Update(Clock clock)
		{
		}

		/// <summary>
		///     Invoked when the actor is added to the game session.
		/// </summary>
		protected virtual void OnAdded()
		{
		}

		/// <summary>
		///     Invoked when the actor is removed from the game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		protected virtual void OnRemoved()
		{
		}
	}
}