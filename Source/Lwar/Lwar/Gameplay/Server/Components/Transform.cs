namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides a position and orientation in 2D space.
	/// </summary>
	public class Transform : Component
	{
		/// <summary>
		///     The orientation in radians.
		/// </summary>
		public float Orientation;

		/// <summary>
		///     The position in 2D space.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Transform()
		{
			ConstructorCache.Register(() => new Transform());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Transform()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="position">The initial position.</param>
		/// <param name="orientation">The initial orientation.</param>
		public static Transform Create(PoolAllocator allocator, Vector2 position = default(Vector2), float orientation = 0)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Transform>();
			component.Position = position;
			component.Orientation = orientation;
			return component;
		}
	}
}