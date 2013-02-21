using System;

namespace Lwar.Client.Gameplay
{
	using System.Threading.Tasks;
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   Manages the input state of the local player.
	/// </summary>
	public class InputManager : DisposableObject
	{
		/// <summary>
		///   The process that updates the input state.
		/// </summary>
		private readonly IProcess _inputProcess;

		/// <summary>
		///   The process that sends the current input state to the server.
		/// </summary>
		private readonly IProcess _sendInputProcess;

		/// <summary>
		///   The game session the input manager belongs to.
		/// </summary>
		private readonly GameSession _session;

		/// <summary>
		///   The current input state.
		/// </summary>
		private InputStateHistory _state = new InputStateHistory();

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
		/// <param name="session">The game session the input manager belongs to.</param>
		public InputManager(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			_session = session;

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

			_session.InputDevice.Register(_forward.Input);
			_session.InputDevice.Register(_backward.Input);
			_session.InputDevice.Register(_turnLeft.Input);
			_session.InputDevice.Register(_turnRight.Input);
			_session.InputDevice.Register(_strafeLeft.Input);
			_session.InputDevice.Register(_strafeRight.Input);
			_session.InputDevice.Register(_shooting1.Input);
			_session.InputDevice.Register(_shooting2.Input);
			_session.InputDevice.Register(_shooting3.Input);
			_session.InputDevice.Register(_shooting4.Input);

			_inputProcess = _session.Scheduler.CreateProcess(Update);
			_sendInputProcess = _session.Scheduler.CreateProcess(SendInput);
		}

		/// <summary>
		///   Updates the current input state.
		/// </summary>
		/// <param name="context">The context in which the input should be handled.</param>
		private async Task Update(ProcessContext context)
		{
			while (!context.IsCanceled)
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

				await context.NextFrame();
			}
		}

		/// <summary>
		///   Periodically sends the current input state to the server.
		/// </summary>
		/// <param name="context">The context in which the input should be handled.</param>
		private async Task SendInput(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				if (_session.LocalPlayer.Ship == null)
				{
					await context.NextFrame();
					continue;
				}

				// The mouse position in window coordinates
				var mousePos = _session.InputDevice.Mouse.Position;

				// Move the origin of the mouse position to the center of the window
				var windowSize = _session.Window.Size;
				mousePos = new Vector2i(mousePos.X - windowSize.Width / 2, mousePos.Y - windowSize.Height / 2);

				// Now move the mouse position to camera coordiantes
				var target = new Vector2(mousePos.X, mousePos.Y) - new Vector2(_session.Camera.Position.X, _session.Camera.Position.Y);

				// The ship position in camera coordinates
				var ship = _session.LocalPlayer.Ship.Position;

				// Don't update if ship and target are too close
				/*if ((target - ship).Length > MinimumOrientationUpdateDistance)
				{
					var orientation = MathUtils.ComputeAngle(ship, target, new Vector2(1, 0));
					orientation = MathUtils.RadToDeg(orientation);
					_orientation = (ushort)orientation;
				}*/

				_state.Update(_forward.Triggered, _backward.Triggered,
							  _turnLeft.Triggered, _turnRight.Triggered,
							  _strafeLeft.Triggered, _strafeRight.Triggered,
							  _shooting1.Triggered, _shooting2.Triggered, _shooting3.Triggered, _shooting4.Triggered,
							  target);

				_session.ServerProxy.Send(InputMessage.Create(_session.LocalPlayer.Id, _state));

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

				await context.Delay(1000 / Specification.InputUpdateFrequency);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputProcess.SafeDispose();
			_sendInputProcess.SafeDispose();

			_session.InputDevice.Remove(_forward.Input);
			_session.InputDevice.Remove(_backward.Input);
			_session.InputDevice.Remove(_turnLeft.Input);
			_session.InputDevice.Remove(_turnRight.Input);
			_session.InputDevice.Remove(_strafeLeft.Input);
			_session.InputDevice.Remove(_strafeRight.Input);
			_session.InputDevice.Remove(_shooting1.Input);
			_session.InputDevice.Remove(_shooting2.Input);
			_session.InputDevice.Remove(_shooting3.Input);
			_session.InputDevice.Remove(_shooting4.Input);
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