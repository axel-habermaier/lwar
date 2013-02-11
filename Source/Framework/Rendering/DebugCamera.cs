using System;

namespace Pegasus.Framework.Rendering
{
	using System.Threading.Tasks;
	using Math;
	using Platform.Graphics;
	using Platform.Input;
	using Processes;

	/// <summary>
	///   Represents a six-degrees-of-freedom debug camera.
	/// </summary>
	public class DebugCamera : Camera3D
	{
		/// <summary>
		///   The update frequency of the camera in Hz.
		/// </summary>
		private const int UpdateFrequency = 60;

		/// <summary>
		///   Triggered when the user wants to move backward.
		/// </summary>
		private readonly LogicalInput _backward;

		/// <summary>
		///   Triggered when the user wants to move forward.
		/// </summary>
		private readonly LogicalInput _forward;

		/// <summary>
		///   The logical input device that provides the input for the camera.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///   Triggered when the user wants to strafe left.
		/// </summary>
		private readonly LogicalInput _left;

		/// <summary>
		///   Triggered when the user wants to strafe right.
		/// </summary>
		private readonly LogicalInput _right;

		/// <summary>
		///   The process that updates the position and viewing angle of the camera based on the input provided by the user.
		/// </summary>
		private readonly IProcess _updateProcess;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		/// <param name="inputDevice">The logical input device that provides the input for the camera.</param>
		/// <param name="scheduler">The scheduler that should be used to schedule the camera's update process.</param>
		public DebugCamera(GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice, ProcessScheduler scheduler)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			_inputDevice = inputDevice;

			_forward = new LogicalInput(Key.W.IsPressed(), InputModes.Debug);
			_backward = new LogicalInput(Key.S.IsPressed(), InputModes.Debug);
			_left = new LogicalInput(Key.A.IsPressed(), InputModes.Debug);
			_right = new LogicalInput(Key.D.IsPressed(), InputModes.Debug);

			inputDevice.Register(_forward);
			inputDevice.Register(_backward);
			inputDevice.Register(_left);
			inputDevice.Register(_right);

			_updateProcess = scheduler.CreateProcess(UpdateAsync);
		}

		/// <summary>
		///   Updates the position and viewing angle of the camera based on the input provided by the user.
		/// </summary>
		/// <param name="context">The context in which the process should be executed.</param>
		private async Task UpdateAsync(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				var move = Vector3.Zero;

				if (_forward.IsTriggered)
					move.Z += 1;
				if (_backward.IsTriggered)
					move.Z -= 1;
				if (_left.IsTriggered)
					move.X -= 1;
				if (_right.IsTriggered)
					move.X += 1;

				move.Normalize();

				var mouseDelta = _inputDevice.Mouse.Position;
				Log.Info(mouseDelta.ToString());
				await context.Delay(UpdateFrequency);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Remove(_forward);
			_inputDevice.Remove(_backward);
			_inputDevice.Remove(_left);
			_inputDevice.Remove(_right);
			_updateProcess.SafeDispose();

			base.OnDisposing();
		}
	}
}