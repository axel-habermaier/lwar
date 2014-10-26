namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides the length of a ray and its target entity.
	/// </summary>
	public class Ray : Component
	{
		/// <summary>
		///     The current length of the ray.
		/// </summary>
		public float Length;

		/// <summary>
		///     The maximum length of the ray.
		/// </summary>
		public float MaxLength;

		/// <summary>
		///     The target of the ray. Can be null if the ray has no target.
		/// </summary>
		public Entity Target;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Ray()
		{
			ConstructorCache.Set(() => new Ray());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Ray()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="maxLength">The maximum length of the ray.</param>
		public static Ray Create(PoolAllocator allocator, float maxLength)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Ray>();
			component.MaxLength = maxLength;
			component.Length = maxLength;
			component.Target = Entity.None;
			return component;
		}
	}
}