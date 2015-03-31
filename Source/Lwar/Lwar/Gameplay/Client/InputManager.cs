namespace Lwar.Gameplay.Client
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;
	using Pegasus.UserInterface.Input;
	using Pegasus.Utilities;
	using Scripting;

	/// <summary>
	///     Manages the input state of the local player.
	/// </summary>
	internal class InputManager : DisposableObject
	{
		/// <summary>
		///     A cached array of weapon fire inputs.
		/// </summary>
		private readonly byte[] _fireWeaponStates = new byte[NetworkProtocol.WeaponSlotCount];

		/// <summary>
		///     The game session the input is provided for.
		/// </summary>
		private readonly ClientGameSession _gameSession;

		/// <summary>
		///     When triggered, causes the player to respawn after death.
		/// </summary>
		private readonly LogicalInput _respawn = new LogicalInput(Cvars.InputRespawnCvar, KeyTriggerType.WentDown, MouseTriggerType.WentDown);

		/// <summary>
		///     A cached array of weapon slot types.
		/// </summary>
		private readonly EntityType[] _weaponTypes = { EntityType.Gun, EntityType.Phaser, EntityType.RocketLauncher, EntityType.RocketLauncher };

		/// <summary>
		///     The clock that is used for time measurements.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     The input frame number.
		/// </summary>
		private uint _frameNumber;

		#region Network-synced input states

		private readonly InputState[] _fireWeapons;
		private InputState _afterBurner;
		private InputState _backward;
		private InputState _forward;
		private InputState _strafeLeft;
		private InputState _strafeRight;

		#endregion

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the input should be provided for.</param>
		public InputManager(ClientGameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_gameSession = gameSession;

			_forward.Input = new LogicalInput(Cvars.InputForwardCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_backward.Input = new LogicalInput(Cvars.InputBackwardCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_strafeLeft.Input = new LogicalInput(Cvars.InputStrafeLeftCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_strafeRight.Input = new LogicalInput(Cvars.InputStrafeRightCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_afterBurner.Input = new LogicalInput(Cvars.InputAfterBurnerCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);

			_fireWeapons = new InputState[NetworkProtocol.WeaponSlotCount];
			_fireWeapons[0].Input = new LogicalInput(Cvars.InputPrimaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_fireWeapons[1].Input = new LogicalInput(Cvars.InputSecondaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_fireWeapons[2].Input = new LogicalInput(Cvars.InputTertiaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_fireWeapons[3].Input = new LogicalInput(Cvars.InputQuaternaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);

			_gameSession.InputDevice.Add(_respawn);
			_gameSession.InputDevice.Add(_forward.Input);
			_gameSession.InputDevice.Add(_backward.Input);
			_gameSession.InputDevice.Add(_strafeLeft.Input);
			_gameSession.InputDevice.Add(_strafeRight.Input);
			_gameSession.InputDevice.Add(_afterBurner.Input);

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				_gameSession.InputDevice.Add(_fireWeapons[i].Input);
		}

		/// <summary>
		///     Updates the current input state.
		/// </summary>
		public void Update()
		{
			if (!_gameSession.IsGameCameraActive)
				return;

			_forward.Triggered |= _forward.Input.IsTriggered;
			_backward.Triggered |= _backward.Input.IsTriggered;
			_strafeLeft.Triggered |= _strafeLeft.Input.IsTriggered;
			_strafeRight.Triggered |= _strafeRight.Input.IsTriggered;
			_afterBurner.Triggered |= _afterBurner.Input.IsTriggered;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				_fireWeapons[i].Triggered |= _fireWeapons[i].Input.IsTriggered;

			// Send a respawn message when the player is dead and the respawn input is triggered
			if (_gameSession.LocalPlayer.Ship == null && _respawn.IsTriggered)
				_gameSession.Connection.Send(PlayerLoadoutMessage.Create(_gameSession.Allocator, _gameSession.LocalPlayer.Identity,
					EntityType.Ship, _weaponTypes));
		}

		/// <summary>
		///     Sends an input message to the server.
		/// </summary>
		public void SendInput()
		{
			// Ensure we don't spam the server with input message
			if (_clock.Milliseconds < 1000.0 / NetworkProtocol.InputUpdateFrequency)
				return;

			_clock.Reset();

			// Update the input states
			_forward.Update();
			_backward.Update();
			_strafeLeft.Update();
			_strafeRight.Update();
			_afterBurner.Update();

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
			{
				_fireWeapons[i].Update();
				_fireWeaponStates[i] = _fireWeapons[i].State;
			}

			// Get the coordinates the player currently targets
			var target = _gameSession.Camera.ToWorldCoordinates(_gameSession.InputDevice.NormalizedMousePosition);
			if (_gameSession.LocalPlayer.Ship != null)
				target = target - _gameSession.LocalPlayer.Ship.Position;

			// Send the input message to the server
			_gameSession.Connection.Send(PlayerInputMessage.Create(
				_gameSession.Allocator, _gameSession.LocalPlayer.Identity, ++_frameNumber, target,
				_forward.State, _backward.State,
				_strafeLeft.State, _strafeRight.State,
				0, 0,
				_afterBurner.State,
				_fireWeaponStates));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_gameSession.InputDevice.Remove(_respawn);
			_gameSession.InputDevice.Remove(_forward.Input);
			_gameSession.InputDevice.Remove(_backward.Input);
			_gameSession.InputDevice.Remove(_strafeLeft.Input);
			_gameSession.InputDevice.Remove(_strafeRight.Input);
			_gameSession.InputDevice.Remove(_afterBurner.Input);

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				_gameSession.InputDevice.Remove(_fireWeapons[i].Input);
		}

		/// <summary>
		///     Stores the state of an input until it is sent to the server.
		/// </summary>
		private struct InputState
		{
			/// <summary>
			///     The logical input that is used to determine whether the input is triggered.
			/// </summary>
			public LogicalInput Input;

			/// <summary>
			///     The current trigger state, also including the seven previous ones.
			/// </summary>
			public byte State;

			/// <summary>
			///     Indicates whether the state has been triggered since the last update of the server.
			/// </summary>
			public bool Triggered;

			/// <summary>
			///     Removes the oldest trigger state from the given input state and adds the current one.
			/// </summary>
			public void Update()
			{
				State = (byte)((State << 1) | (Triggered ? 1 : 0));
				Triggered = false;
			}
		}
	}
}
