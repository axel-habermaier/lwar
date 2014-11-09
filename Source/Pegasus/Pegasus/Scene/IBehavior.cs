namespace Pegasus.Scene
{
	using System;
	using Platform.Memory;

	/// <summary>
	///     Represents a behavior that can be attached to a scene node.
	/// </summary>
	public interface IBehavior : ISharedPooledObject
	{
		/// <summary>
		///     Gets or sets the next behavior in an intrusive list.
		/// </summary>
		IBehavior Next { get; set; }

		/// <summary>
		///     Gets or sets the previous behavior in an intrusive list.
		/// </summary>
		IBehavior Previous { get; set; }

		/// <summary>
		///     Gets the scene node the behavior is attached to.
		/// </summary>
		SceneNode SceneNode { get; }

		/// <summary>
		///     Attaches the behavior to the given scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node the behavior should be attached to.</param>
		void Attach(SceneNode sceneNode);

		/// <summary>
		///     Detaches the behavior from the scene node it is attached to.
		/// </summary>
		void Detach();

		/// <summary>
		///     Invoked when the behavior should execute a step.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behavior.</param>
		void Execute(float elapsedSeconds);
	}
}