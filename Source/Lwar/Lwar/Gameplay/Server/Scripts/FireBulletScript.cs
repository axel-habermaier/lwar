namespace Lwar.Gameplay.Server.Scripts
{
	using System;
	using Network;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Fires a bullet when the corresponding player input is triggered.
	/// </summary>
	public class FireBulletScript : Script
	{
		/// <summary>
		///     The base speed of a bullet, to which the ship's speed is added or subtracted, depending on the shooting direction.
		/// </summary>
		public float BaseSpeed;

		/// <summary>
		///     The index of the input trigger that causes a bullet to be fired.
		/// </summary>
		private int _inputIndex;

		/// <summary>
		///     The time in seconds to wait before spawning the next bullet.
		/// </summary>
		private float _remainingCooldown;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static FireBulletScript()
		{
			ConstructorCache.Register(() => new FireBulletScript());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private FireBulletScript()
		{
		}

		/// <summary>
		///     Gets or sets the time in seconds to wait between spawning two consecutive bullets.
		/// </summary>
		public float Cooldown { get; set; }

		/// <summary>
		///     Updates the state of the script.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override void Update(float elapsedSeconds)
		{
			_remainingCooldown -= elapsedSeconds;
			if (_remainingCooldown > 0 || !Input.FireWeapons[_inputIndex])
				return;

			_remainingCooldown = Cooldown;

			var position = Transform.Position;
			var direction = Vector2.FromAngle(Transform.Orientation);
			var velocity = direction.Normalize() * BaseSpeed + Motion.Velocity;
			EntityFactory.CreateBullet(Owner.Player, position, velocity);
		}

		/// <summary>
		///     Allocates a script instance using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="inputIndex">The index of the input trigger that should cause a bullet to be fired.</param>
		/// <param name="cooldown">The time in seconds to wait between spawning two consecutive bullets.</param>
		/// <param name="baseSpeed">The base speed of a bullet, to which the ship's speed is added or subtracted.</param>
		public static FireBulletScript Create(PoolAllocator allocator, int inputIndex, float cooldown, float baseSpeed = 3000)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.InRange(inputIndex, 0, NetworkProtocol.WeaponSlotCount - 1);

			var component = allocator.Allocate<FireBulletScript>();
			component.Cooldown = cooldown;
			component.BaseSpeed = baseSpeed;
			component._remainingCooldown = 0;
			component._inputIndex = inputIndex;
			return component;
		}
	}
}