namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides a time to live to an entity, allowing it to be removed when the time runs out.
	/// </summary>
	public class TimeToLive : Component
	{
		/// <summary>
		///     The remaining lifetime of the entity in seconds.
		/// </summary>
		public float RemainingTime;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static TimeToLive()
		{
			ConstructorCache.Set(() => new TimeToLive());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private TimeToLive()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="timeToLive">The lifetime in seconds.</param>
		public static TimeToLive Create(PoolAllocator allocator, float timeToLive)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<TimeToLive>();
			component.RemainingTime = timeToLive;
			return component;
		}
	}
}