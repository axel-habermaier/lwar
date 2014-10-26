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
		///     The boolean state value for the backwards input.
		/// </summary>
		public bool Backward;

		/// <summary>
		///     The boolean state value for the forward input.
		/// </summary>
		public bool Forward;

		/// <summary>
		///     The boolean state value for the strafe left input.
		/// </summary>
		public bool StrafeLeft;

		/// <summary>
		///     The boolean state value for the strafe right input.
		/// </summary>
		public bool StrafeRight;

		/// <summary>
		///     The target position relative to the entity's position.
		/// </summary>
		public Vector2 Target;

		/// <summary>
		///     The boolean state value for the turn left input.
		/// </summary>
		public bool TurnLeft;

		/// <summary>
		///     The boolean state value for the turn right input.
		/// </summary>
		public bool TurnRight;

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
		///     Gets the boolean state values for the weapon slots.
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
			component.TurnLeft = false;
			component.TurnRight = false;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				component.FireWeapons[i] = false;

			return component;
		}
	}
}