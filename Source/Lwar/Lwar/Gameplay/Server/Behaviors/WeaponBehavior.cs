namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Entities;
	using Pegasus.Math;
	using Pegasus.Scene;
	using Pegasus.Utilities;

	/// <summary>
	///     A base class for weapon behaviors, managing weapon cooldown and energy.
	/// </summary>
	internal abstract class WeaponBehavior : Behavior<Weapon>
	{
		/// <summary>
		///     Indicates whether the weapon is currently firing.
		/// </summary>
		private bool _isFiring;

		/// <summary>
		///     The remaining amount of time in seconds before the weapon can be fired again.
		/// </summary>
		private float _remainingCooldown;

		/// <summary>
		///     The remaining amount of time in seconds before the weapon energy is starting to recharge.
		/// </summary>
		private float _remainingRechargeDelay;

		/// <summary>
		///     The current state of the weapon.
		/// </summary>
		private State _state;

		/// <summary>
		///     Gets the weapon entity the behavior is attached to.
		/// </summary>
		protected Weapon Weapon
		{
			get { return SceneNode; }
		}

		/// <summary>
		///     Invoked when the behavior is attached to a scene node.
		/// </summary>
		protected override sealed void OnAttached()
		{
			_isFiring = false;
			_remainingCooldown = 0;
			_remainingRechargeDelay = 0;
			_state = State.FullyCharged;
		}

		/// <summary>
		///     Fires a single shot of a non-continuous weapon.
		/// </summary>
		protected virtual void Fire()
		{
			Assert.NotReached("Non-continuous weapons must override the Fire method.");
		}

		/// <summary>
		///     Starts firing a continuous weapon.
		/// </summary>
		protected virtual void StartFiring()
		{
			Assert.NotReached("Continuous weapons must override the StartFiring method.");
		}

		/// <summary>
		///     Stops firing a continuous weapon.
		/// </summary>
		protected virtual void StopFiring()
		{
			Assert.NotReached("Continuous weapons must override the StopFiring method.");
		}

		/// <summary>
		///     Invoked when the behavior should execute a step.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behavior.</param>
		public override sealed void Execute(float elapsedSeconds)
		{
			switch (_state)
			{
				case State.FullyCharged:
					break;
				case State.Recharging:
					Weapon.Energy += Weapon.Template.RechargeSpeed * elapsedSeconds;
					if (Weapon.Energy > Weapon.Template.MaxEnergy)
						_state = State.FullyCharged;
					break;
				case State.Firing:
					_remainingRechargeDelay = Weapon.Template.RechargeDelay;
					Weapon.Energy -= Weapon.Template.DepleteSpeed * elapsedSeconds;

					if (_remainingCooldown <= 0 && !Weapon.Template.FiresContinuously)
					{
						_remainingCooldown = Weapon.Template.Cooldown;
						Fire();
					}

					if (Weapon.Energy < 0)
					{
						_state = State.WaitingForRecharging;
						if (Weapon.Template.FiresContinuously)
						{
							_isFiring = false;
							StopFiring();
						}
					}
					break;
				case State.WaitingForRecharging:
					if (Single.IsNaN(Weapon.Template.RechargeDelay))
						_state = State.OutOfEnergy;
					else
					{
						_remainingRechargeDelay -= elapsedSeconds;
						if (_remainingRechargeDelay < 0)
							_state = State.Recharging;
					}
					break;
				default:
					throw new InvalidOperationException("Unknown warp drive state.");
			}

			_remainingCooldown -= elapsedSeconds;
			Weapon.Energy = MathUtils.Clamp(Weapon.Energy, 0, Weapon.Template.MaxEnergy);
		}

		/// <summary>
		///     Handles the given player input.
		/// </summary>
		/// <param name="fireWeapon">Indicates whether the weapon should be fired.</param>
		public void HandlePlayerInput(bool fireWeapon)
		{
			var canFire = Weapon.Energy >= Weapon.Template.ActivationEnergy && _remainingCooldown <= 0;
			var fire = fireWeapon && Weapon.Energy > 0 && (_state == State.Firing || canFire);

			if (!_isFiring && fire && Weapon.Template.FiresContinuously)
			{
				_isFiring = true;
				StartFiring();
			}

			if (_isFiring && !fire && Weapon.Template.FiresContinuously)
			{
				_isFiring = false;
				StopFiring();
			}

			_state = fire ? State.Firing : State.WaitingForRecharging;
		}

		/// <summary>
		///     Describes the state of the weapon.
		/// </summary>
		private enum State
		{
			/// <summary>
			///     Indicates that the weapon is fully charged and can be fired.
			/// </summary>
			FullyCharged,

			/// <summary>
			///     Indicates that the weapon is inactive and recharging. It can only be activated if the minimum required energy
			///     level has been recharged.
			/// </summary>
			Recharging,

			/// <summary>
			///     Indicates that the weapon is currently firing.
			/// </summary>
			Firing,

			/// <summary>
			///     Indicates that the weapon is inactive and waiting to be recharged once the recharge delay has passed.
			/// </summary>
			WaitingForRecharging,

			/// <summary>
			///     Indicates that the non-recharging weapon is out of energy.
			/// </summary>
			OutOfEnergy
		}
	}
}