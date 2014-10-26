namespace Lwar.Gameplay.Server.Scripts
{
	using System;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Fires a bullet when the corresponding player input is triggered.
	/// </summary>
	public class FirePhaserScript : Script
	{
		/// <summary>
		///     The index of the input trigger that causes a bullet to be fired.
		/// </summary>
		private int _inputIndex;

		/// <summary>
		///     Indicates whether the phaser is currently firing.
		/// </summary>
		private bool _isFiring;

		/// <summary>
		///     The phaser entity.
		/// </summary>
		private Entity _phaser;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static FirePhaserScript()
		{
			ConstructorCache.Set(() => new FirePhaserScript());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private FirePhaserScript()
		{
		}

		/// <summary>
		///     Updates the state of the script.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override void Update(float elapsedSeconds)
		{
			if (_isFiring == Input.FireWeapons[_inputIndex])
				return;

			_isFiring = Input.FireWeapons[_inputIndex];

			if (_isFiring)
				_phaser = GameSession.Templates.CreatePhaser(Owner.Player, Entity);
			else
				_phaser.Remove();
		}

		/// <summary>
		///     Deinitializes the script when the entity has been removed from the game session.
		/// </summary>
		protected override void Deinitialize()
		{
			_phaser.Remove();
		}

		/// <summary>
		///     Allocates a script instance using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="inputIndex">The index of the input trigger that should cause a bullet to be fired.</param>
		public static FirePhaserScript Create(PoolAllocator allocator, int inputIndex)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.InRange(inputIndex, 0, NetworkProtocol.WeaponSlotCount - 1);

			var component = allocator.Allocate<FirePhaserScript>();
			component._inputIndex = inputIndex;
			component._isFiring = false;
			component._phaser = Entity.None;
			return component;
		}
	}
}