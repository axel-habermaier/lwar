namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Associates an entity with the player that owns it.
	/// </summary>
	public class Owner : Component
	{
		/// <summary>
		///     The player that owns the entity.
		/// </summary>
		public Player Player;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Owner()
		{
			ConstructorCache.Set(() => new Owner());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Owner()
		{
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="player">The player that owns the entity.</param>
		public static Owner Create(PoolAllocator allocator, Player player)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Owner>();
			component.Player = player;
			return component;
		}
	}
}