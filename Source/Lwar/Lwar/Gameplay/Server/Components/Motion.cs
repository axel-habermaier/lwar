namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides a velocity in 2D space.
	/// </summary>
	public class Motion : Component
	{
		/// <summary>
		///     The velocity in 2D space.
		/// </summary>
		public Vector2 Velocity;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Motion()
		{
			ConstructorCache.Register(() => new Motion());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Motion()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="velocity">The initial velocity.</param>
		public static Motion Create(PoolAllocator allocator, Vector2 velocity = default(Vector2))
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Motion>();
			component.Velocity = velocity;
			return component;
		}
	}
}