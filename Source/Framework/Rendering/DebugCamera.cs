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
	public class DebugCamera : Camera
	{
		/// <summary>
		///   The update frequency of the camera in Hz.
		/// </summary>
		private const int UpdateFrequency = 60;

		/// <summary>
		///   The rotation speed of the camera.
		/// </summary>
		private const float RotationSpeed = 0.01f;

		/// <summary>
		///   The move speed of the camera.
		/// </summary>
		private const float MoveSpeed = 5.0f;

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
		///   The current position of the camera.
		/// </summary>
		private Vector3 _position;

		/// <summary>
		///   The current rotation of the camera around the X and Y axis.
		/// </summary>
		private Vector2 _rotation;

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
			_inputDevice.Mouse.Moved += MouseMoved;

			_forward = new LogicalInput(Key.W.IsPressed(), InputModes.Debug);
			_backward = new LogicalInput(Key.S.IsPressed(), InputModes.Debug);
			_left = new LogicalInput(Key.A.IsPressed(), InputModes.Debug);
			_right = new LogicalInput(Key.D.IsPressed(), InputModes.Debug);

			inputDevice.Register(_forward);
			inputDevice.Register(_backward);
			inputDevice.Register(_left);
			inputDevice.Register(_right);

			_updateProcess = scheduler.CreateProcess(UpdateAsync);
			Reset();
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the projection matrix once the method returns.</param>
		protected override void UpdateProjectionMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreatePerspectiveFieldOfView(MathUtils.DegToRad(60), Viewport.Width / (float)Viewport.Height, 1, 1000);
		}

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the view matrix once the method returns.</param>
		protected override void UpdateViewMatrix(out Matrix matrix)
		{
			var rotation = Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y);

			var target = new Vector3(0, 0, 1);
			target = _position + Vector3.Transform(ref target, ref rotation);

			var up = new Vector3(0, 1, 0);
			up = Vector3.Transform(ref up, ref rotation);

			matrix = Matrix.CreateLookAt(_position, target, up);
		}

		/// <summary>
		/// Resets the debug camera.
		/// </summary>
		public void Reset()
		{
			_position = new Vector3(0, 0, -200);
			_rotation = Vector2.Zero;
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
					move.X += 1;
				if (_right.IsTriggered)
					move.X -= 1;

				var rotation = Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y);
				move = Vector3.Transform(ref move, ref rotation).Normalize();
				_position += move * MoveSpeed;

				UpdateViewMatrix();
				await context.Delay(UpdateFrequency);
			}
		}

		/// <summary>
		///   Invoked when the position of the mouse has changed.
		/// </summary>
		/// <param name="x">The new position in X direction.</param>
		/// <param name="y">The new position in Y direction.</param>
		private void MouseMoved(int x, int y)
		{
			var delta = new Vector2(x, y) - new Vector2(Viewport.Width / 2.0f, Viewport.Height / 2.0f);
			_rotation += new Vector2(-delta.Y, delta.X) * RotationSpeed;

			if (_rotation.Y < -MathUtils.TwoPi)
				_rotation.Y += MathUtils.TwoPi;
			else if (_rotation.Y > MathUtils.TwoPi)
				_rotation.Y -= MathUtils.TwoPi;

			var maxX = MathUtils.DegToRad(85);
			if (_rotation.X < -maxX)
				_rotation.X = -maxX;
			else if (_rotation.X > maxX)
				_rotation.X = maxX;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Mouse.Moved -= MouseMoved;
			_inputDevice.Remove(_forward);
			_inputDevice.Remove(_backward);
			_inputDevice.Remove(_left);
			_inputDevice.Remove(_right);
			_updateProcess.SafeDispose();

			base.OnDisposing();
		}
	}
}