namespace Lwar.Gameplay.Server.Components
{
	using System;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Associates an entity with the input state of a player.
	/// </summary>
	public class PlayerInput : Component
	{
		/// <summary>
		///     The after burner input.
		/// </summary>
		public bool AfterBurner;

		/// <summary>
		///     The backwards input.
		/// </summary>
		public bool Backward;

		/// <summary>
		///     The forward input.
		/// </summary>
		public bool Forward;

		/// <summary>
		///     The strafe left input.
		/// </summary>
		public bool StrafeLeft;

		/// <summary>
		///     The strafe right input.
		/// </summary>
		public bool StrafeRight;

		/// <summary>
		///     The target position relative to the entity's position.
		/// </summary>
		public Vector2 Target;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerInput()
		{
			ConstructorCache.Register(() => new PlayerInput());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerInput()
		{
			FireWeapons = new bool[NetworkProtocol.WeaponSlotCount];
		}

		/// <summary>
		///     Gets the inputs for the weapon slots.
		/// </summary>
		public bool[] FireWeapons { get; private set; }

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		public static PlayerInput Create(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);

			var component = allocator.Allocate<PlayerInput>();
			component.Target = Vector2.Zero;
			component.Backward = false;
			component.Forward = false;
			component.StrafeLeft = false;
			component.StrafeRight = false;
			component.AfterBurner = false;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				component.FireWeapons[i] = false;

			return component;
		}
	}
}