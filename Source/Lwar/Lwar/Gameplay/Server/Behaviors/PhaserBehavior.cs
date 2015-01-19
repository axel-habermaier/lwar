namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Fires a phaser when the weapon is triggered.
	/// </summary>
	internal class PhaserBehavior : WeaponBehavior
	{
		/// <summary>
		///     The phaser entity.
		/// </summary>
		private Phaser _phaser;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PhaserBehavior()
		{
			ConstructorCache.Register(() => new PhaserBehavior());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PhaserBehavior()
		{
		}

		/// <summary>
		///     Starts firing a continuous weapon.
		/// </summary>
		protected override void StartFiring()
		{
			Assert.IsNull(_phaser, "Unexpected active phaser entity.");

			_phaser = Phaser.Create(Weapon.GameSession, Weapon.Player);
			_phaser.AttachTo(Weapon);
			_phaser.AcquireOwnership();
		}

		/// <summary>
		///     Stops firing a continuous weapon.
		/// </summary>
		protected override void StopFiring()
		{
			Assert.NotNull(_phaser, "Expected an active phaser entity.");

			_phaser.Remove();
			_phaser.SafeDispose();
			_phaser = null;
		}

		/// <summary>
		///     Invoked when the behavior is detached from the scene node it is attached to.
		/// </summary>
		/// <remarks>This method is not called when the scene graph is disposed.</remarks>
		protected override void OnDetached()
		{
			if (_phaser == null)
				return;

			_phaser.Remove();
			_phaser.SafeDispose();
			_phaser = null;
		}

		/// <summary>
		///     Allocates an instance using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the behavior.</param>
		public static PhaserBehavior Create(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);

			var behavior = allocator.Allocate<PhaserBehavior>();
			behavior._phaser = null;
			return behavior;
		}
	}
}