namespace Pegasus.Rendering
{
	using System;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Represents a camera that can be used to draw 3D scenes.
	/// </summary>
	public class Camera3D : Camera
	{
		/// <summary>
		///     The distance to the far clipping plane.
		/// </summary>
		private float _farDistance;

		/// <summary>
		///     The field of view of the camera in radians.
		/// </summary>
		private float _fieldOfView;

		/// <summary>
		///     The distance to the near clipping plane.
		/// </summary>
		private float _nearDistance;

		/// <summary>
		///     The target the camera looks at.
		/// </summary>
		private Vector3 _target;

		/// <summary>
		///     The up vector.
		/// </summary>
		private Vector3 _up;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		public Camera3D(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			_nearDistance = 1.0f;
			_farDistance = 1000.0f;
		}

		/// <summary>
		///     Gets or sets the target the camera looks at.
		/// </summary>
		public Vector3 Target
		{
			get { return _target; }
			set
			{
				_target = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///     Gets or sets the up vector.
		/// </summary>
		public Vector3 Up
		{
			get { return _up; }
			set
			{
				_up = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///     Gets or sets the field of view of the camera in radians.
		/// </summary>
		public float FieldOfView
		{
			get { return _fieldOfView; }
			set
			{
				_fieldOfView = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		///     Gets or sets the distance to the far clipping plane.
		/// </summary>
		public float FarDistance
		{
			get { return _farDistance; }
			set
			{
				_farDistance = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		///     Gets or sets the distance to the near clipping plane.
		/// </summary>
		public float NearDistance
		{
			get { return _nearDistance; }
			set
			{
				_nearDistance = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		///     Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateProjectionMatrixCore()
		{
			_projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, Viewport.Width / (float)Viewport.Height, _nearDistance, _farDistance);
		}

		/// <summary>
		///     Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateViewMatrixCore()
		{
			_view = Matrix.CreateLookAt(Position, Target, Up);
		}
	}
}