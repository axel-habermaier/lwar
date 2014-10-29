namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a propulsion system with an after burner that can increase the maximum speed of an entity for a certain
	///     amount of time.
	/// </summary>
	public class Propulsion : Component
	{
		/// <summary>
		///     The acceleration in 2D space.
		/// </summary>
		public Vector2 Acceleration;

		/// <summary>
		///     Indicates whether the after burner is currently enabled.
		/// </summary>
		public bool AfterBurnerEnabled;

		/// <summary>
		///     The amount of energy to deplete per second when the after burner is enabled.
		/// </summary>
		public float DepleteSpeed;

		/// <summary>
		///     The maximum allowed acceleration.
		/// </summary>
		public float MaxAcceleration;

		/// <summary>
		///     The maximum speed of the entity when the after burner is enabled.
		/// </summary>
		public float MaxAfterBurnerSpeed;

		/// <summary>
		///     The maximum energy level of the after burner.
		/// </summary>
		public float MaxEnergy;

		/// <summary>
		///     The maximum speed of the entity when the after burner is disabled.
		/// </summary>
		public float MaxSpeed;

		/// <summary>
		///     The amount of energy to recharge per second when the after burner is not enabled.
		/// </summary>
		public float RechargeSpeed;

		/// <summary>
		///     The remaining amount of after burner energy.
		/// </summary>
		public float RemainingEnergy;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Propulsion()
		{
			ConstructorCache.Register(() => new Propulsion());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Propulsion()
		{
		}

		/// <summary>
		///     Allocates a script instance using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="maxAcceleration">The maximum acceleration.</param>
		/// <param name="maxSpeed">The maximum speed of the entity when the after burner is disabled.</param>
		/// <param name="maxAfterBurnerSpeed">The maximum speed of the entity when the after burner is enabled.</param>
		/// <param name="maxEnergy">The maximum energy level of the after burner.</param>
		/// <param name="rechargeSpeed"> The amount of energy to recharge per second when the after burner is not enabled.</param>
		/// <param name="depleteSpeed"> The amount of energy to deplete per second when the after burner is enabled.</param>
		public static Propulsion Create(PoolAllocator allocator, float maxAcceleration, float maxSpeed, float maxAfterBurnerSpeed, float maxEnergy,
										float rechargeSpeed, float depleteSpeed)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<Propulsion>();
			component.MaxSpeed = maxSpeed;
			component.MaxAcceleration = maxAcceleration;
			component.Acceleration = Vector2.Zero;
			component.MaxAfterBurnerSpeed = maxAfterBurnerSpeed;
			component.DepleteSpeed = depleteSpeed;
			component.MaxEnergy = maxEnergy;
			component.RechargeSpeed = rechargeSpeed;
			component.RemainingEnergy = maxEnergy;
			component.AfterBurnerEnabled = false;
			return component;
		}
	}
}