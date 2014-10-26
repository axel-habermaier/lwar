namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides a position and orientation in 2D space relative to a parent entity.
	/// </summary>
	public class RelativeTransform : Component
	{
		/// <summary>
		///     The orientation in radians relative to the parent's orientation.
		/// </summary>
		public float Orientation;

		/// <summary>
		///     The parent entity.
		/// </summary>
		public Entity ParentEntity;

		/// <summary>
		///     The position in 2D space relative to the parent's position.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static RelativeTransform()
		{
			ConstructorCache.Set(() => new RelativeTransform());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private RelativeTransform()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="parentEntity">The parent entity.</param>
		/// <param name="position">The initial position.</param>
		/// <param name="orientation">The initial orientation.</param>
		public static RelativeTransform Create(PoolAllocator allocator, Entity parentEntity, Vector2 position = default(Vector2),
											   float orientation = 0)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentSatisfies(parentEntity.IsAlive, "Invalid dead parent entity.");

			var component = allocator.Allocate<RelativeTransform>();
			component.Position = position;
			component.Orientation = orientation;
			component.ParentEntity = parentEntity;
			return component;
		}
	}
}