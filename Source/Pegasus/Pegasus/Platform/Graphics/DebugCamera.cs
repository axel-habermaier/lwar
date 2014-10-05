namespace Pegasus.Platform.Graphics
{
	using System;
	using Framework.UserInterface.Input;
	using Math;

	/// <summary>
	///     Represents a six-degrees-of-freedom debug camera.
	/// </summary>
	public class DebugCamera : Camera
	{
		/// <summary>
		///     The rotation speed of the camera.
		/// </summary>
		private const float RotationSpeed = 5f;

		/// <summary>
		///     The move speed of the camera.
		/// </summary>
		private const float MoveSpeed = 3000.0f;

		/// <summary>
		///     Triggered when the user wants to move backward.
		/// </summary>
		private readonly LogicalInput _backward;

		/// <summary>
		///     Triggered when the user wants to move forward.
		/// </summary>
		private readonly LogicalInput _forward;

		/// <summary>
		///     The logical input device that provides the input for the camera.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     Triggered when the user wants to strafe left.
		/// </summary>
		private readonly LogicalInput _left;

		/// <summary>
		///     Triggered when the user wants to strafe right.
		/// </summary>
		private readonly LogicalInput _right;

		/// <summary>
		///     The clock that is used to scale the movement of the camera.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     The mouse movement since the last update.
		/// </summary>
		private Vector2 _mouseDelta;

		/// <summary>
		///     The current position of the camera.
		/// </summary>
		private Vector3 _position;

		/// <summary>
		///     The current rotation of the camera around the X and Y axis.
		/// </summary>
		private Vector2 _rotation;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		/// <param name="inputDevice">The logical input device that provides the input for the camera.</param>
		public DebugCamera(GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(inputDevice);

			_inputDevice = inputDevice;
			_inputDevice.MouseMoved += MouseMoved;

			_forward = new LogicalInput(Key.W.IsPressed());
			_backward = new LogicalInput(Key.S.IsPressed());
			_left = new LogicalInput(Key.A.IsPressed());
			_right = new LogicalInput(Key.D.IsPressed());

			inputDevice.Add(_forward);
			inputDevice.Add(_backward);
			inputDevice.Add(_left);
			inputDevice.Add(_right);

			Reset();
		}

		/// <summary>
		///     Gets or sets a value indicating whether the debug camera is active.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		///     Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateProjectionMatrixCore()
		{
			_projection = Matrix.CreatePerspectiveFieldOfView(MathUtils.DegToRad(30), Viewport.Width / Viewport.Height, 1, 1000);
		}

		/// <summary>
		///     Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateViewMatrixCore()
		{
			var rotation = Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y);

			var target = new Vector3(0, 0, 1);
			target = _position + Vector3.Transform(ref target, ref rotation);

			var up = new Vector3(0, 1, 0);
			up = Vector3.Transform(ref up, ref rotation);

			_view = Matrix.CreateLookAt(_position, target, up);
		}

		/// <summary>
		///     Resets the debug camera.
		/// </summary>
		public void Reset()
		{
			_position = new Vector3(0, 0, -200);
			_rotation = Vector2.Zero;
			_mouseDelta = Vector2.Zero;
		}

		/// <summary>
		///     Updates the position and viewing angle of the camera based on the input provided by the user.
		/// </summary>
		public void Update()
		{
			if (!IsActive)
				return;

			var move = Vector3.Zero;

			if (_forward.IsTriggered)
				move.Z += 1;
			if (_backward.IsTriggered)
				move.Z -= 1;
			if (_left.IsTriggered)
				move.X += 1;
			if (_right.IsTriggered)
				move.X -= 1;

			_rotation += new Vector2(-_mouseDelta.Y * Viewport.Height / Viewport.Width, _mouseDelta.X) * RotationSpeed;
			_mouseDelta = Vector2.Zero;

			if (_rotation.Y < -MathUtils.TwoPi)
				_rotation.Y += MathUtils.TwoPi;
			else if (_rotation.Y > MathUtils.TwoPi)
				_rotation.Y -= MathUtils.TwoPi;

			var maxX = MathUtils.DegToRad(85);
			if (_rotation.X < -maxX)
				_rotation.X = -maxX;
			else if (_rotation.X > maxX)
				_rotation.X = maxX;

			var rotation = Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y);
			move = Vector3.Transform(ref move, ref rotation).Normalize();
			_position += move * MoveSpeed * (float)_clock.Seconds;

			_clock.Reset();
			UpdateViewMatrix();
		}

		/// <summary>
		///     Invoked when the position of the mouse has changed.
		/// </summary>
		/// <param name="eventArgs">Contains the new position of the mouse.</param>
		private void MouseMoved(MouseEventArgs eventArgs)
		{
			if (IsActive)
				_mouseDelta += eventArgs.NormalizedPosition;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.MouseMoved -= MouseMoved;
			_inputDevice.Remove(_forward);
			_inputDevice.Remove(_backward);
			_inputDevice.Remove(_left);
			_inputDevice.Remove(_right);

			base.OnDisposing();
		}
	}
}