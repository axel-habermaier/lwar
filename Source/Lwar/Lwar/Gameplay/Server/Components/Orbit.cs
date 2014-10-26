namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Marks an entity that orbits around another entity.
	/// </summary>
	public class Orbit : Component
	{
		/// <summary>
		///     An offset to the current position on the orbital trajectory.
		/// </summary>
		public float Offset;

		/// <summary>
		///     The radius of the orbit.
		/// </summary>
		public float Radius;

		/// <summary>
		///     The speed of the movement. The sign of the speed determines the direction of the orbital movement.
		/// </summary>
		public float Speed;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Orbit()
		{
			ConstructorCache.Register(() => new Orbit());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Orbit()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="radius">The radius of the orbit.</param>
		/// <param name="speed">The speed of the movement. The sign of the speed determines the direction of the orbital movement.</param>
		/// <param name="offset">An offset to the position on the orbital trajectory.</param>
		public static Orbit Create(PoolAllocator allocator, float radius, float speed, float offset)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Orbit>();
			component.Radius = radius;
			component.Speed = speed;
			component.Offset = offset;
			return component;
		}
	}
}