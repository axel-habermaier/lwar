using System;

namespace Lwar.Client.Gameplay
{
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Memory;
	using Scripting;

	/// <summary>
	///   Manages the input state of the local player.
	/// </summary>
	public class InputManager : DisposableObject
	{
		/// <summary>
		///   The input device that provides the input by the user.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///   The input message that is sent to the server periodically.
		/// </summary>
		private InputMessage _inputMessage;

		#region Network-synced input states

		private InputState _backward;
		private InputState _forward;
		private InputState _shooting1;
		private InputState _shooting2;
		private InputState _shooting3;
		private InputState _shooting4;
		private InputState _strafeLeft;
		private InputState _strafeRight;
		private InputState _turnLeft;
		private InputState _turnRight;

		#endregion

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="inputDevice">The input device that provides the input by the user.</param>
		public InputManager(LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(inputDevice);

			_inputDevice = inputDevice;

			_forward.Input = new LogicalInput(Cvars.InputForwardCvar, InputLayers.Game);
			_backward.Input = new LogicalInput(Cvars.InputBackwardCvar, InputLayers.Game);
			_turnLeft.Input = new LogicalInput(Cvars.InputTurnLeftCvar, InputLayers.Game);
			_turnRight.Input = new LogicalInput(Cvars.InputTurnRightCvar, InputLayers.Game);
			_strafeLeft.Input = new LogicalInput(Cvars.InputStrafeLeftCvar, InputLayers.Game);
			_strafeRight.Input = new LogicalInput(Cvars.InputStrafeRightCvar, InputLayers.Game);
			_shooting1.Input = new LogicalInput(Cvars.InputPrimaryWeaponCvar, InputLayers.Game);
			_shooting2.Input = new LogicalInput(Cvars.InputSecondaryWeaponCvar, InputLayers.Game);
			_shooting3.Input = new LogicalInput(Cvars.InputTertiaryWeaponCvar, InputLayers.Game);
			_shooting4.Input = new LogicalInput(Cvars.InputQuaternaryWeaponCvar, InputLayers.Game);

			_inputDevice.Add(_forward.Input);
			_inputDevice.Add(_backward.Input);
			_inputDevice.Add(_turnLeft.Input);
			_inputDevice.Add(_turnRight.Input);
			_inputDevice.Add(_strafeLeft.Input);
			_inputDevice.Add(_strafeRight.Input);
			_inputDevice.Add(_shooting1.Input);
			_inputDevice.Add(_shooting2.Input);
			_inputDevice.Add(_shooting3.Input);
			_inputDevice.Add(_shooting4.Input);
		}

		/// <summary>
		///   Updates the current input state.
		/// </summary>
		public void Update()
		{
			_forward.Triggered |= _forward.Input.IsTriggered;
			_backward.Triggered |= _backward.Input.IsTriggered;
			_turnLeft.Triggered |= _turnLeft.Input.IsTriggered;
			_turnRight.Triggered |= _turnRight.Input.IsTriggered;
			_strafeLeft.Triggered |= _strafeLeft.Input.IsTriggered;
			_strafeRight.Triggered |= _strafeRight.Input.IsTriggered;
			_shooting1.Triggered |= _shooting1.Input.IsTriggered;
			_shooting2.Triggered |= _shooting2.Input.IsTriggered;
			_shooting3.Triggered |= _shooting3.Input.IsTriggered;
			_shooting4.Triggered |= _shooting4.Input.IsTriggered;
		}

		/// <summary>
		///   Copies the current input state into the returned message. The returned message is missing the identifier of the local
		///   player and the target is given in screen coordinates instead of world coordinates.
		/// </summary>
		public Message CreateInputMessage()
		{
			Update(ref _inputMessage.Forward, _forward.Triggered);
			Update(ref _inputMessage.Backward, _backward.Triggered);
			Update(ref _inputMessage.TurnLeft, _turnLeft.Triggered);
			Update(ref _inputMessage.TurnRight, _turnRight.Triggered);
			Update(ref _inputMessage.StrafeLeft, _strafeLeft.Triggered);
			Update(ref _inputMessage.StrafeRight, _strafeRight.Triggered);
			Update(ref _inputMessage.Shooting1, _shooting1.Triggered);
			Update(ref _inputMessage.Shooting2, _shooting2.Triggered);
			Update(ref _inputMessage.Shooting3, _shooting3.Triggered);
			Update(ref _inputMessage.Shooting4, _shooting4.Triggered);

			_forward.Triggered = false;
			_backward.Triggered = false;
			_turnLeft.Triggered = false;
			_turnRight.Triggered = false;
			_strafeLeft.Triggered = false;
			_strafeRight.Triggered = false;
			_shooting1.Triggered = false;
			_shooting2.Triggered = false;
			_shooting3.Triggered = false;
			_shooting4.Triggered = false;

			_inputMessage.Target = new Vector2(_inputDevice.Mouse.Position.X, _inputDevice.Mouse.Position.Y);
			++_inputMessage.FrameNumber;

			return new Message
			{
				Type = MessageType.Input,
				Input = _inputMessage
			};
		}

		/// <summary>
		///   Removes the oldest trigger state from the given input state and adds the current one.
		/// </summary>
		/// <param name="inputState">The input state that stores the last eight trigger states.</param>
		/// <param name="triggered">The new triggered state that should be stored in the input state.</param>
		private static void Update(ref byte inputState, bool triggered)
		{
			inputState = (byte)((inputState << 1) | (triggered ? 1 : 0));
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Remove(_forward.Input);
			_inputDevice.Remove(_backward.Input);
			_inputDevice.Remove(_turnLeft.Input);
			_inputDevice.Remove(_turnRight.Input);
			_inputDevice.Remove(_strafeLeft.Input);
			_inputDevice.Remove(_strafeRight.Input);
			_inputDevice.Remove(_shooting1.Input);
			_inputDevice.Remove(_shooting2.Input);
			_inputDevice.Remove(_shooting3.Input);
			_inputDevice.Remove(_shooting4.Input);
		}

		/// <summary>
		///   Stores the state of an input until it is sent to the server.
		/// </summary>
		private struct InputState
		{
			/// <summary>
			///   The logical input that is used to determine whether the input is triggered.
			/// </summary>
			public LogicalInput Input;

			/// <summary>
			///   Indicates whether the state has been triggered since the last update of the server.
			/// </summary>
			public bool Triggered;
		}
	}
}