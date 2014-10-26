namespace Lwar.Gameplay.Client.Actors
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Rendering;

	/// <summary>
	///     The base class of all actors.
	/// </summary>
	/// <typeparam name="TActor">The specialized actor type.</typeparam>
	public abstract class Actor<TActor> : UniquePooledObject, IActor
		where TActor : Actor<TActor>, new()
	{
		/// <summary>
		///     The parent actor.
		/// </summary>
		private IActor _parent;

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
		protected ClientGameSession GameSession { get; private set; }

		/// <summary>
		///     Gets the render context that draws the actor.
		/// </summary>
		protected RenderContext RenderContext { get; private set; }

		/// <summary>
		///     Gets the transformation of the actor.
		/// </summary>
		public Transformation Transform { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the actor has been removed from the game session.
		/// </summary>
		public bool IsRemoved { get; private set; }

		/// <summary>
		///     Gets or sets the parent actor.
		/// </summary>
		public IActor Parent
		{
			get { return _parent; }
			set
			{
				if (_parent == value)
					return;

				_parent = value;

				if (_parent != null)
					Transform.AttachTo(_parent.Transform);
				else
					Transform.AttachTo(GameSession.RootTransform);
			}
		}

		/// <summary>
		///     Adds the actor to the game session and the render context.
		/// </summary>
		/// <param name="gameSession">The game session the actor should be added to.</param>
		/// <param name="renderContext">The render context the actor should be added to.</param>
		public void Added(ClientGameSession gameSession, RenderContext renderContext)
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
			Parent = null;
			Transform.Detach();
			RenderContext.Remove(this as TActor);

			IsRemoved = true;
			OnRemoved();
		}

		/// <summary>
		///     Updates the actor's internal state.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public virtual void Update(float elapsedSeconds)
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

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			IsRemoved = false;
		}
	}
}