namespace Pegasus.Scene
{
	using System;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a behavior that can be attached to a scene node of the given type or of a derived type.
	/// </summary>
	/// <typeparam name="T">The base type of the scene nodes the behavior can be attached to.</typeparam>
	public abstract class Behavior<T> : PooledObject, IBehavior
		where T : SceneNode
	{
		/// <summary>
		///     Gets or sets the next behavior in an intrusive list.
		/// </summary>
		private IBehavior _next;

		/// <summary>
		///     Gets or sets the previous behavior in an intrusive list.
		/// </summary>
		private IBehavior _previous;

		/// <summary>
		///     Gets the scene node the behavior is attached to.
		/// </summary>
		protected T SceneNode { get; private set; }

		/// <summary>
		///     Gets the scene node the behavior is attached to.
		/// </summary>
		SceneNode IBehavior.SceneNode
		{
			get { return SceneNode; }
		}

		/// <summary>
		///     Gets or sets the next behavior in an intrusive list.
		/// </summary>
		IBehavior IBehavior.Next
		{
			get { return _next; }
			set { _next = value; }
		}

		/// <summary>
		///     Gets or sets the previous behavior in an intrusive list.
		/// </summary>
		IBehavior IBehavior.Previous
		{
			get { return _previous; }
			set { _previous = value; }
		}

		/// <summary>
		///     Attaches the behavior to the given scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node the behavior should be attached to.</param>
		public void Attach(SceneNode sceneNode)
		{
			Assert.NotPooled(this);
			Assert.ArgumentNotNull(sceneNode);
			Assert.IsNull(SceneNode, "The behavior has already been attached to a scene node.");
			Assert.ArgumentOfType<T>(sceneNode);

			if (sceneNode.Behavior != null)
				sceneNode.Behavior.Previous = this;

			SceneNode = (T)sceneNode;
			_next = sceneNode.Behavior;
			sceneNode.Behavior = this;

			OnAttached();
		}

		/// <summary>
		///     Detaches the behavior from the scene node it is attached to.
		/// </summary>
		public void Detach()
		{
			Assert.NotPooled(this);
			Assert.NotNull(SceneNode, "The behavior is not attached to any scene node.");

			OnDetached();

			if (_previous != null)
				_previous.Next = _next;

			if (_next != null)
				_next.Previous = _previous;

			if (SceneNode.Behavior == this)
				SceneNode.Behavior = _next;

			_previous = null;
			_next = null;
			SceneNode = null;
		}

		/// <summary>
		///     Invoked when the behavior should execute a step.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behavior.</param>
		public virtual void Execute(float elapsedSeconds)
		{
		}

		/// <summary>
		///     Invoked when the behavior is attached to a scene node.
		/// </summary>
		protected virtual void OnAttached()
		{
		}

		/// <summary>
		///     Invoked when the behavior is detached from the scene node it is attached to.
		/// </summary>
		/// <remarks>This method is not called when the scene graph is disposed.</remarks>
		protected virtual void OnDetached()
		{
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override sealed void OnReturning()
		{
			SceneNode = null;
			_previous = null;
			_next = null;
		}
	}
}