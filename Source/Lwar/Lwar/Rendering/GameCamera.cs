namespace Lwar.Rendering
{
	using System;
	using Gameplay;
	using Pegasus;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	/// <summary>
	///     Represents the camera that is used to draw the game session.
	/// </summary>
	public class GameCamera : Camera3D
	{
		/// <summary>
		///     The maximum allowed distance to the XZ plane.
		/// </summary>
		private const float MaxZoom = 20000.0f;

		/// <summary>
		///     The minimum allowed distance to the XZ plane.
		/// </summary>
		private const float MinZoom = 1000.0f;

		/// <summary>
		///     The default distance to the XZ plane.
		/// </summary>
		private const float DefaultZoom = 3000.0f;

		/// <summary>
		///     Determines how fast the zoom level changes when the user scrolls the mouse wheel.
		/// </summary>
		private const float DeltaScale = 800.0f;

		/// <summary>
		///     Determines how fast the camera changes the distance to the XZ plane.
		/// </summary>
		private const float ZoomChangeSpeed = 100.0f;

		/// <summary>
		///     The factor that is applied to the zoom value when the star field zoom mode is selected.
		/// </summary>
		private const float StarfieldZoomFactor = 0.2f;

		/// <summary>
		///     The input device that provides the input for the camera.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     The local player of the game session.
		/// </summary>
		private readonly Player _localPlayer;

		/// <summary>
		///     The clock that is used to animate changes of the XZ plane distance.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     The target distance to the XZ plane the camera is moving to.
		/// </summary>
		private float _targetZoom;

		/// <summary>
		///     The current distance to the XZ plane.
		/// </summary>
		private float _zoom = DefaultZoom;

		/// <summary>
		///     The camera's current zoom mode.
		/// </summary>
		private ZoomMode _zoomMode = ZoomMode.Default;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		/// <param name="inputDevice">The input device that provides the input for the camera.</param>
		/// <param name="localPlayer">The local player of the game session.</param>
		public GameCamera(GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice, Player localPlayer)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(inputDevice);
			Assert.ArgumentNotNull(localPlayer);

			FieldOfView = MathUtils.DegToRad(20);
			Up = new Vector3(0, 0, 1);

			_localPlayer = localPlayer;
			_inputDevice = inputDevice;
			_inputDevice.MouseWheel += OnZoomChanged;
			_targetZoom = _zoom;
			_clock.Reset();
		}

		/// <summary>
		///     Gets or sets a value indicating whether the camera is currently active.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		///     Gets the 2D position of the camera.
		/// </summary>
		public Vector2 Position2D
		{
			get { return new Vector2(Position.X, Position.Z); }
			set { Position = new Vector3(value.X, Position.Y, value.Y); }
		}

		/// <summary>
		///     Gets or sets the camera's zoom mode.
		/// </summary>
		public ZoomMode ZoomMode
		{
			get { return _zoomMode; }
			set
			{
				if (_zoomMode == value)
					return;

				_zoomMode = value;
				UpdatePosition();
			}
		}

		/// <summary>
		///     Updates the camera.
		/// </summary>
		public void Update()
		{
			if (!IsActive)
				return;

			// Scale to [0,1] range
			var zoom = _zoom / MaxZoom;
			var targetZoom = _targetZoom / MaxZoom;

			// Compute the time-sensitive slow down factor
			var slowDownFactor = ZoomChangeSpeed / Math.Min((float)_clock.Milliseconds, 100);

			// Compute the weighted average
			_zoom = ((zoom * (slowDownFactor - 1)) + targetZoom) / slowDownFactor;

			// Scale back to the [MinZoom, MaxZoom] range
			_zoom *= MaxZoom;

			UpdatePosition();
			Target = new Vector3(Position.X, 0, Position.Z);

			_clock.Reset();
		}

		/// <summary>
		///     Updates the camera's position.
		/// </summary>
		private void UpdatePosition()
		{
			float x;
			float z;

			if (_localPlayer.Ship != null)
			{
				x = _localPlayer.Ship.Position.X;
				z = _localPlayer.Ship.Position.Y;
			}
			else
			{
				x = Position.X;
				z = Position.Z;
			}

			float zoom;
			switch (_zoomMode)
			{
				case ZoomMode.Default:
					zoom = _zoom;
					break;
				case ZoomMode.Starfield:
					zoom = _zoom * StarfieldZoomFactor;
					break;
				default:
					throw new InvalidOperationException("Unsupported zoom mode.");
			}

			Position = new Vector3(x, zoom, z);
		}

		/// <summary>
		///     Converts the given screen coordinates to world coordinates.
		/// </summary>
		/// <param name="screenCoordinates">The screen coordinates that should be converted to world coordinates.</param>
		public Vector2 ToWorldCoordinates(Vector2 screenCoordinates)
		{
			var viewDirection = Target - Position;
			viewDirection = viewDirection.Normalize();

			var left = Vector3.Cross(viewDirection, Up);
			left = left.Normalize();

			var up = Vector3.Cross(viewDirection, left);
			up = up.Normalize();

			// Map the viewport plane into world space
			var height = MathUtils.Tan(FieldOfView / 2.0f) * NearDistance;
			var width = height * Viewport.Width / Viewport.Height;

			left *= width;
			up *= height;

			// Compute the ray 
			var origin = Position + viewDirection * NearDistance + left * screenCoordinates.X + up * screenCoordinates.Y;
			var direction = origin - Position;

			var distance = -origin.Y / direction.Y;
			var x = origin.X + distance * direction.X;
			var z = origin.Z + distance * direction.Z;

			var worldCoordinates = new Vector2(x, z) * 2 - new Vector2(Position.X, Position.Z);
			return worldCoordinates;
		}

		/// <summary>
		///     Converts the given world coordinates to screen coordinates.
		/// </summary>
		/// <param name="worldCoordinates">The world coordinates that should be converted to screen coordinates.</param>
		public Vector2 ToScreenCoodinates(Vector2 worldCoordinates)
		{
			var coordinates = new Vector4(worldCoordinates.X, 0, worldCoordinates.Y);
			var transformation = _view * _projection;

			// Project to view space
			coordinates = Vector4.Transform(ref coordinates, ref transformation);
			coordinates /= coordinates.W;

			// Project to screen space
			coordinates.X = (coordinates.X * 0.5f + 0.5f) * Viewport.Width + Viewport.Left;
			coordinates.Y = (coordinates.Y * 0.5f + 0.5f) * Viewport.Height + Viewport.Top;

			return new Vector2(coordinates.X, coordinates.Y);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.MouseWheel -= OnZoomChanged;
			base.OnDisposing();
		}

		/// <summary>
		///     Updates the camera's distance to the XZ plane.
		/// </summary>
		/// <param name="args">Contains the delta that should be applied to the camera's distance to the XZ plane.</param>
		private void OnZoomChanged(MouseWheelEventArgs args)
		{
			if (IsActive)
				_targetZoom = MathUtils.Clamp(_targetZoom + -1 * args.Delta * DeltaScale, MinZoom, MaxZoom);
		}
	}
}