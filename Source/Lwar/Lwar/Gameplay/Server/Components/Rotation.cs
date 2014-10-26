namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides a rotational motion in 2D space.
	/// </summary>
	public class Rotation : Component
	{
		/// <summary>
		///     The maximum allowed angular speed.
		/// </summary>
		public float MaxSpeed;

		/// <summary>
		///     The angular velocity in 2D space.
		/// </summary>
		public float Velocity;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Rotation()
		{
			ConstructorCache.Set(() => new Rotation());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Rotation()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="velocity">The initial angular velocity.</param>
		/// <param name="maxSpeed">The maximum allowed angular speed.</param>
		public static Rotation Create(PoolAllocator allocator, float velocity = 0, float maxSpeed = 0)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Rotation>();
			component.Velocity = velocity;
			component.MaxSpeed = maxSpeed;
			return component;
		}
	}
}