namespace Pegasus.Scene
{
	using System;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Removes scene nodes after a specific amount of time.
	/// </summary>
	public class TimeToLiveBehavior : Behavior<SceneNode>
	{
		/// <summary>
		///     The remaining number of seconds before the scene node is removed.
		/// </summary>
		private float _remainingTime;

		/// <summary>
		///     Invoked when the behavior should execute a step.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behavior.</param>
		public override void Execute(float elapsedSeconds)
		{
			_remainingTime -= elapsedSeconds;

			if (_remainingTime < 0)
				SceneNode.Remove();
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate pooled objects.</param>
		/// <param name="seconds">The amount of time to wait in seconds before the scene node is removed.</param>
		public static TimeToLiveBehavior Create(PoolAllocator allocator, float seconds)
		{
			Assert.ArgumentNotNull(allocator);

			var behavior = allocator.Allocate<TimeToLiveBehavior>();
			behavior._remainingTime = seconds;
			return behavior;
		}
	}
}