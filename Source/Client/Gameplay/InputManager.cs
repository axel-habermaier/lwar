using System;

namespace Lwar.Client.Gameplay
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;

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
		///   The input message payload that is sent to the server periodically.
		/// </summary>
		private Message.InputPayload _inputMessage;

		#region Input states

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
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			_inputDevice = inputDevice;

			_forward.Input = new LogicalInput(Key.W.IsPressed() | Key.Up.IsPressed(), InputModes.Game);
			_backward.Input = new LogicalInput(Key.S.IsPressed() | Key.Down.IsPressed(), InputModes.Game);
			_turnLeft.Input = new LogicalInput(Key.A.IsPressed() | Key.Left.IsPressed(), InputModes.Game);
			_turnRight.Input = new LogicalInput(Key.D.IsPressed() | Key.Right.IsPressed(), InputModes.Game);
			_strafeLeft.Input = new LogicalInput(Key.Q.IsPressed(), InputModes.Game);
			_strafeRight.Input = new LogicalInput(Key.E.IsPressed(), InputModes.Game);
			_shooting1.Input = new LogicalInput(MouseButton.Left.IsPressed(), InputModes.Game);
			_shooting2.Input = new LogicalInput(MouseButton.Right.IsPressed(), InputModes.Game);
			_shooting3.Input = new LogicalInput(Key.Num1.IsPressed(), InputModes.Game);
			_shooting4.Input = new LogicalInput(Key.Num2.IsPressed(), InputModes.Game);

			_inputDevice.Register(_forward.Input);
			_inputDevice.Register(_backward.Input);
			_inputDevice.Register(_turnLeft.Input);
			_inputDevice.Register(_turnRight.Input);
			_inputDevice.Register(_strafeLeft.Input);
			_inputDevice.Register(_strafeRight.Input);
			_inputDevice.Register(_shooting1.Input);
			_inputDevice.Register(_shooting2.Input);
			_inputDevice.Register(_shooting3.Input);
			_inputDevice.Register(_shooting4.Input);
		}

		/// <summary>
		///   Updates the current input state, periodically sending the input to the server.
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
		///   Creates an input message that is subsequently sent to the server..
		/// </summary>
		/// <param name="localPlayer">The local player that generated the input.</param>
		/// <param name="camera">The camera that should be used to convert the mouse position into world coordinates.</param>
		/// <param name="windowSize">
		///   The size of the window that should be used to convert the mouse position into world coordinates.
		/// </param>
		public Message CreateInputMessage(Player localPlayer, Camera camera, Size windowSize)
		{
			Assert.ArgumentNotNull(localPlayer, () => localPlayer);
			Assert.ArgumentNotNull(camera, () => camera);

			// The mouse position in window coordinates
			var mousePos = _inputDevice.Mouse.Position;

			// Move the origin of the mouse position to the center of the window
			mousePos = new Vector2i(mousePos.X - windowSize.Width / 2, mousePos.Y - windowSize.Height / 2);

			// Translate the target to into world coordinates
			_inputMessage.Target = new Vector2(mousePos.X, mousePos.Y) - new Vector2(camera.Position.X, camera.Position.Z);

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

			++_inputMessage.FrameNumber;
			_inputMessage.Player = localPlayer.Id;
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