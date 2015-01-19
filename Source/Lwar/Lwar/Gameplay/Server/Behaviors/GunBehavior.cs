namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Fires a bullet when the weapon is triggered.
	/// </summary>
	internal class GunBehavior : WeaponBehavior
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static GunBehavior()
		{
			ConstructorCache.Register(() => new GunBehavior());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private GunBehavior()
		{
		}

		/// <summary>
		///     Fires a single shot of a non-continuous weapon.
		/// </summary>
		protected override void Fire()
		{
			var direction = Vector2.FromAngle(Weapon.Ship.Orientation);
			var velocity = direction.Normalize() * Weapon.Template.BaseSpeed + Weapon.Ship.Velocity;

			var bullet = Bullet.Create(Weapon.GameSession, Weapon.Player, Weapon.WorldPosition2D, velocity);
			bullet.AttachTo(Weapon.GameSession.SceneGraph.Root);
		}

		/// <summary>
		///     Allocates an instance using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the behavior.</param>
		public static GunBehavior Create(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);
			return allocator.Allocate<GunBehavior>();
		}
	}
}