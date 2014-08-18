namespace Lwar.Gameplay
{
	using System;
	using Entities;
	using Network;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;
	using Scripting;

	/// <summary>
	///     Manages the input state of the local player.
	/// </summary>
	public class InputManager : DisposableObject
	{
		/// <summary>
		///     The game session the input is provided for.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     The input device that provides the input by the user.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     When triggered, causes the player to respawn after death.
		/// </summary>
		private readonly LogicalInput _respawn = new LogicalInput(Cvars.InputRespawnCvar, KeyTriggerType.WentDown, MouseTriggerType.WentDown);

		/// <summary>
		///     The clock that is used for time measurements.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     The input message that is sent to the server periodically.
		/// </summary>
		private InputMessage _inputMessage;

		/// <summary>
		///     The current target position of the mouse.
		/// </summary>
		private Vector2 _targetPosition;

		#region Network-synced input states

		private InputState _backward;
		private InputState _forward;
		private InputState _shooting1;
		private InputState _shooting2;
		private InputState _shooting3;
		private InputState _shooting4;
		private InputState _strafeLeft;
		private InputState _strafeRight;

		#endregion

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the input should be provided for.</param>
		public InputManager(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_gameSession = gameSession;
			_inputDevice = Application.Current.Window.InputDevice;

			_forward.Input = new LogicalInput(Cvars.InputForwardCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_backward.Input = new LogicalInput(Cvars.InputBackwardCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_strafeLeft.Input = new LogicalInput(Cvars.InputStrafeLeftCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_strafeRight.Input = new LogicalInput(Cvars.InputStrafeRightCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_shooting1.Input = new LogicalInput(Cvars.InputPrimaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_shooting2.Input = new LogicalInput(Cvars.InputSecondaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_shooting3.Input = new LogicalInput(Cvars.InputTertiaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);
			_shooting4.Input = new LogicalInput(Cvars.InputQuaternaryWeaponCvar, KeyTriggerType.Pressed, MouseTriggerType.Pressed);

			_inputDevice.Add(_respawn);
			_inputDevice.Add(_forward.Input);
			_inputDevice.Add(_backward.Input);
			_inputDevice.Add(_strafeLeft.Input);
			_inputDevice.Add(_strafeRight.Input);
			_inputDevice.Add(_shooting1.Input);
			_inputDevice.Add(_shooting2.Input);
			_inputDevice.Add(_shooting3.Input);
			_inputDevice.Add(_shooting4.Input);
			_inputDevice.MouseMoved += OnMouseMoved;
		}

		/// <summary>
		///     Updates the current target position.
		/// </summary>
		private void OnMouseMoved(MouseEventArgs args)
		{
			_targetPosition = args.NormalizedPosition;
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
			_shooting1.Triggered |= _shooting1.Input.IsTriggered;
			_shooting2.Triggered |= _shooting2.Input.IsTriggered;
			_shooting3.Triggered |= _shooting3.Input.IsTriggered;
			_shooting4.Triggered |= _shooting4.Input.IsTriggered;

			// Send a respawn message when the player is dead and the respawn input is triggered
			if (_gameSession.LocalPlayer.Ship == null && _respawn.IsTriggered)
				_gameSession.NetworkSession.Send(SelectionMessage.Create(_gameSession.LocalPlayer,
					EntityType.Ship, EntityType.Gun, EntityType.Phaser, EntityType.Phaser, EntityType.Phaser));
		}

		/// <summary>
		///     Sends an input message to the server.
		/// </summary>
		public void SendInput()
		{
			// Ensure we don't spam the server with input message
			if (_clock.Milliseconds < 1000.0 / Specification.InputUpdateFrequency)
				return;

			// Reset the clock and update the frame number and player identifier
			_clock.Reset();
			_inputMessage.Player = _gameSession.LocalPlayer.Identifier;
			++_inputMessage.FrameNumber;

			// Add the input states to the message
			_forward.Update(ref _inputMessage.Forward);
			_backward.Update(ref _inputMessage.Backward);
			_strafeLeft.Update(ref _inputMessage.StrafeLeft);
			_strafeRight.Update(ref _inputMessage.StrafeRight);
			_shooting1.Update(ref _inputMessage.Shooting1);
			_shooting2.Update(ref _inputMessage.Shooting2);
			_shooting3.Update(ref _inputMessage.Shooting3);
			_shooting4.Update(ref _inputMessage.Shooting4);

			// Add the target to the message
			var worldCoordinates = _gameSession.Camera.ToWorldCoordinates(_targetPosition);
			if (_gameSession.LocalPlayer.Ship != null)
				_inputMessage.Target = worldCoordinates - _gameSession.LocalPlayer.Ship.Position;

			// Send the input message to the server
			_gameSession.NetworkSession.Send(new Message { Type = MessageType.Input, Input = _inputMessage });
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Remove(_respawn);
			_inputDevice.Remove(_forward.Input);
			_inputDevice.Remove(_backward.Input);
			_inputDevice.Remove(_strafeLeft.Input);
			_inputDevice.Remove(_strafeRight.Input);
			_inputDevice.Remove(_shooting1.Input);
			_inputDevice.Remove(_shooting2.Input);
			_inputDevice.Remove(_shooting3.Input);
			_inputDevice.Remove(_shooting4.Input);
			_inputDevice.MouseMoved -= OnMouseMoved;
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
			///     Indicates whether the state has been triggered since the last update of the server.
			/// </summary>
			public bool Triggered;

			/// <summary>
			///     Removes the oldest trigger state from the given input state and adds the current one.
			/// </summary>
			/// <param name="inputState">The input state that stores the last eight trigger states.</param>
			public void Update(ref byte inputState)
			{
				inputState = (byte)((inputState << 1) | (Triggered ? 1 : 0));
				Triggered = false;
			}
		}
	}
}