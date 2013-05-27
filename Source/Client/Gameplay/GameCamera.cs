using System;

namespace Lwar.Client.Gameplay
{
	using Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents the camera that is used to draw the game session.
	/// </summary>
	public class GameCamera : Camera3D
	{
		/// <summary>
		///   The maximum allowed distance to the XZ plane.
		/// </summary>
		private const float MaxZoom = 5000.0f;

		/// <summary>
		///   The minimum allowed distance to the XZ plane.
		/// </summary>
		private const float MinZoom = 500.0f;

		/// <summary>
		///   Determines how fast the zoom level changes when the user scrolls the mouse wheel.
		/// </summary>
		private const float DeltaScale = 200.0f;

		/// <summary>
		///   Determines how fast the camera changes the distance to the XZ plane.
		/// </summary>
		private const float ZoomChangeSpeed = 100.0f;

		/// <summary>
		///   The clock that is used to animate changes of the XZ plane distance.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   The input device that provides the input for the camera.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///   The target distance to the XZ plane the camera is moving to.
		/// </summary>
		private float _targetZoom;

		/// <summary>
		///   The current distance to the XZ plane.
		/// </summary>
		private float _zoom = 1500.0f;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		/// <param name="inputDevice">The input device that provides the input for the camera.</param>
		public GameCamera(GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(inputDevice);

			FieldOfView = MathUtils.DegToRad(20);
			Up = new Vector3(0, 0, 1);

			_inputDevice = inputDevice;
			_inputDevice.Mouse.Wheel += OnZoomChanged;
			_targetZoom = _zoom;
		}

		/// <summary>
		///   Gets or sets the ship that the camera follows.
		/// </summary>
		public Ship Ship { get; set; }

		/// <summary>
		///   Updates the camera.
		/// </summary>
		public void Update()
		{
			if (Ship == null)
				return;

			// Scale to [0,1] range
			var zoom = _zoom / MaxZoom;
			var targetZoom = _targetZoom / MaxZoom;

			// Compute the time-sensitive slow down factor
			var slowDownFactor = ZoomChangeSpeed / (float)_clock.Milliseconds;

			// Compute the weighted average
			_zoom = ((zoom * (slowDownFactor - 1)) + targetZoom) / slowDownFactor;

			// Scale back to the [MinZoom, MaxZoom] range
			_zoom *= MaxZoom;

			Position = new Vector3(Ship.Position.X, _zoom, Ship.Position.Y);
			Target = new Vector3(Position.X, 0, Position.Z);

			_clock.Reset();
		}

		/// <summary>
		///   Converts the given vector in screen coordinates to world coordinates.
		/// </summary>
		/// <param name="screenCoordinates">The screen coordinates that should be converted to world coordinates.</param>
		public Vector2 ToWorldCoordinates(Vector2 screenCoordinates)
		{
			// The projection places the origin into the center of the window; translate the screen coordinates accordingly
			var center = new Vector2(Viewport.Width, Viewport.Height) / 2.0f;
			var centered = screenCoordinates - center;
			centered.X /= Viewport.Width;
			centered.Y /= Viewport.Height;

			var origin = Position;
			var direction = new Vector3(centered.X, Viewport.Height / (float)Viewport.Width, centered.Y).Normalize();

			var distance = -origin.Y / direction.Y;
			var x = origin.X + distance * direction.X;
			var z = origin.Z + distance * direction.Z;

			return new Vector2(x, z);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
			_inputDevice.Mouse.Wheel -= OnZoomChanged;

			base.OnDisposing();
		}

		/// <summary>
		///   Updates the camera's distance to the XZ plane.
		/// </summary>
		/// <param name="delta">The delta that should be applied to the camera's distance to the XZ plane.</param>
		private void OnZoomChanged(int delta)
		{
			if (_inputDevice.Modes == InputModes.Game)
				_targetZoom = MathUtils.Clamp(_targetZoom + -1 * delta * DeltaScale, MinZoom, MaxZoom);
		}
	}
}