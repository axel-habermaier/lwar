namespace Pegasus.Rendering
{
	using System;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;

	/// <summary>
	///     Represents a six-degrees-of-freedom debug camera.
	/// </summary>
	public class DebugCamera : Camera
	{
		/// <summary>
		///     The rotation speed of the camera.
		/// </summary>
		private const float RotationSpeed = 0.01f;

		/// <summary>
		///     The move speed of the camera.
		/// </summary>
		private const float MoveSpeed = 1000.0f;

		/// <summary>
		///     Triggered when the user wants to move backward.
		/// </summary>
		private readonly LogicalInput _backward;

		/// <summary>
		///     The clock that is used to scale the movement of the camera.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///     Triggered when the user wants to move forward.
		/// </summary>
		private readonly LogicalInput _forward;

		/// <summary>
		///     The logical input device that provides the input for the camera.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     The input layer that must be activated to control the camera.
		/// </summary>
		private readonly InputLayer _layer;

		/// <summary>
		///     Triggered when the user wants to strafe left.
		/// </summary>
		private readonly LogicalInput _left;

		/// <summary>
		///     Triggered when the user wants to strafe right.
		/// </summary>
		private readonly LogicalInput _right;

		/// <summary>
		///     The mouse movement since the last update.
		/// </summary>
		private Vector2i _mouseDelta;

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
		/// <param name="inputDevice">The logical input device that provides the input for the camera.</param>
		/// <param name="layer">The input layer that must be activated to control the camera.</param>
		public DebugCamera(LogicalInputDevice inputDevice, InputLayer layer)
		{
			Assert.ArgumentNotNull(inputDevice);
			Assert.ArgumentSatisfies(layer.IsPrimitive, "Invalid input layer.");

			_inputDevice = inputDevice;
			_inputDevice.Mouse.Moved += MouseMoved;
			_layer = layer;

			_forward = new LogicalInput(Key.W.IsPressed(), layer);
			_backward = new LogicalInput(Key.S.IsPressed(), layer);
			_left = new LogicalInput(Key.A.IsPressed(), layer);
			_right = new LogicalInput(Key.D.IsPressed(), layer);

			inputDevice.Add(_forward);
			inputDevice.Add(_backward);
			inputDevice.Add(_left);
			inputDevice.Add(_right);

			Reset();
		}

		/// <summary>
		///     Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateProjectionMatrixCore()
		{
			Projection = Matrix.CreatePerspectiveFieldOfView(MathUtils.DegToRad(30), Viewport.Width / (float)Viewport.Height, 1, 1000);
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

			View = Matrix.CreateLookAt(_position, target, up);
		}

		/// <summary>
		///     Resets the debug camera.
		/// </summary>
		public void Reset()
		{
			_position = new Vector3(0, 0, -200);
			_rotation = Vector2.Zero;
			_mouseDelta = Vector2i.Zero;
		}

		/// <summary>
		///     Updates the position and viewing angle of the camera based on the input provided by the user.
		/// </summary>
		public void Update()
		{
			if (_inputDevice.InputLayer != _layer)
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

			_rotation += new Vector2(-_mouseDelta.Y, _mouseDelta.X) * RotationSpeed;
			_mouseDelta = Vector2i.Zero;

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
		/// <param name="x">The new position in X direction.</param>
		/// <param name="y">The new position in Y direction.</param>
		private void MouseMoved(int x, int y)
		{
			if (_inputDevice.InputLayer != _layer)
				return;

			_mouseDelta += new Vector2i(x, y) - new Vector2i(Viewport.Width, Viewport.Height) / 2;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Mouse.Moved -= MouseMoved;
			_inputDevice.Remove(_forward);
			_inputDevice.Remove(_backward);
			_inputDevice.Remove(_left);
			_inputDevice.Remove(_right);
			_clock.SafeDispose();

			base.OnDisposing();
		}
	}
}