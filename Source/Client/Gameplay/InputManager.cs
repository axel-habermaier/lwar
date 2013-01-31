﻿using System;

namespace Lwar.Client.Gameplay
{
	using System.Threading.Tasks;
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
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
		///   Indicates whether the player moves backwards.
		/// </summary>
		private InputState _backward;

		/// <summary>
		///   Indicates whether the player moves foward
		/// </summary>
		private InputState _forward;

		/// <summary>
		///   Indicates whether the player moves to the left.
		/// </summary>
		private InputState _left;

		/// <summary>
		///   Indicates whether the player moves to the left.
		/// </summary>
		private InputState _right;

		/// <summary>
		///   Indicates whether the player is shooting.
		/// </summary>
		private InputState _shooting;

		/// <summary>
		///   The current input state.
		/// </summary>
		private InputStateHistory _state = new InputStateHistory();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="session">The game session the input manager belongs to.</param>
		public InputManager(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			_session = session;

			_forward.Input = new LogicalInput(Key.W.IsPressed());
			_backward.Input = new LogicalInput(Key.S.IsPressed());
			_left.Input = new LogicalInput(Key.A.IsPressed());
			_right.Input = new LogicalInput(Key.D.IsPressed());
			_shooting.Input = new LogicalInput(Key.Space.IsPressed());

			_session.InputDevice.Register(_forward.Input);
			_session.InputDevice.Register(_backward.Input);
			_session.InputDevice.Register(_left.Input);
			_session.InputDevice.Register(_right.Input);
			_session.InputDevice.Register(_shooting.Input);

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
				_left.Triggered |= _left.Input.IsTriggered;
				_right.Triggered |= _right.Input.IsTriggered;
				_shooting.Triggered |= _shooting.Input.IsTriggered;

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
				_state.Update(_forward.Triggered, _backward.Triggered, _left.Triggered, _right.Triggered, _shooting.Triggered);
				_session.ServerProxy.Send(UpdateClientInput.Create(_session.LocalPlayer.Id, _state));

				_forward.Triggered = false;
				_backward.Triggered = false;
				_left.Triggered = false;
				_right.Triggered = false;
				_shooting.Triggered = false;

				await context.Delay(1000 / Specification.UpdateInputFrequency);
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
			_session.InputDevice.Remove(_left.Input);
			_session.InputDevice.Remove(_right.Input);
			_session.InputDevice.Remove(_shooting.Input);
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