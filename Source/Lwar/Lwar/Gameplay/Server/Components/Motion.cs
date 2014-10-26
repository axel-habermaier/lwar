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
		///     The acceleration in 2D space.
		/// </summary>
		public Vector2 Acceleration;

		/// <summary>
		///     The maximum allowed acceleration.
		/// </summary>
		public float MaxAcceleration;

		/// <summary>
		///     The maximum allowed speed.
		/// </summary>
		public float MaxSpeed;

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
		/// <param name="acceleration">The initial acceleration.</param>
		/// <param name="velocity">The initial velocity.</param>
		/// <param name="maxAcceleration">The maximum allowed acceleration.</param>
		/// <param name="maxSpeed">The maximum allowed speed.</param>
		public static Motion Create(PoolAllocator allocator,
									Vector2 acceleration = default(Vector2), Vector2 velocity = default(Vector2),
									float maxAcceleration = Single.MaxValue, float maxSpeed = Single.MaxValue)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Motion>();
			component.Acceleration = acceleration;
			component.MaxAcceleration = maxAcceleration;
			component.Velocity = velocity;
			component.MaxSpeed = maxSpeed;
			return component;
		}
	}
}