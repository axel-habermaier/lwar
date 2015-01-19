namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Scene;
	using Pegasus.Utilities;

	/// <summary>
	///     Removes scene nodes that reach the boundaries of the galaxy.
	/// </summary>
	internal class BoundaryBehavior : Behavior<SceneNode>
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static BoundaryBehavior()
		{
			ConstructorCache.Register(() => new BoundaryBehavior());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private BoundaryBehavior()
		{
		}

		/// <summary>
		///     Invoked when the behavior should execute a step.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behavior.</param>
		public override void Execute(float elapsedSeconds)
		{
			if (SceneNode.Position.LengthSquared > Int16.MaxValue * Int16.MaxValue)
				SceneNode.Remove();
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate pooled objects.</param>
		public static BoundaryBehavior Create(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);
			return allocator.Allocate<BoundaryBehavior>();
		}
	}
}