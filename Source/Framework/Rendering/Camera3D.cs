using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a camera that can be used to draw 3D scenes.
	/// </summary>
	public class Camera3D : Camera
	{
		/// <summary>
		///   The field of view of the camera in radians.
		/// </summary>
		private float _fieldOfView;

		/// <summary>
		///   The camera's position within the world.
		/// </summary>
		private Vector3 _position;

		/// <summary>
		///   The target the camera looks at.
		/// </summary>
		private Vector3 _target;

		/// <summary>
		///   The up vector.
		/// </summary>
		private Vector3 _up;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		public Camera3D(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Gets or sets the camera's position within the world.
		/// </summary>
		public Vector3 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///   Gets or sets the target the camera looks at.
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
		///   Gets or sets the up vector.
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
		///   Gets or sets the field of view of the camera in radians.
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
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the projection matrix once the method returns.</param>
		protected override void UpdateProjectionMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, Viewport.Width / (float)Viewport.Height, 1, 1000);
		}

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the view matrix once the method returns.</param>
		protected override void UpdateViewMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreateLookAt(Position, Target, Up);
		}
	}
}