using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a camera that can be used to draw 2D scenes.
	/// </summary>
	public class Camera2D : Camera
	{
		/// <summary>
		///   The distance of the camera to the X,Z plane.
		/// </summary>
		private float _distance;

		/// <summary>
		///   The camera's position within the world.
		/// </summary>
		private Vector2 _position;

		/// <summary>
		///   The camera's viewport.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		public Camera2D(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Gets or sets the distance of the camera to the X,Z plane.
		/// </summary>
		public float Distance
		{
			get { return _distance; }
			set
			{
				_distance = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///   Gets or sets the camera's position within the world.
		/// </summary>
		public Vector2 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///   Gets or sets the camera's viewport.
		/// </summary>
		public new  Rectangle Viewport
		{
			get { return _viewport; }
			set
			{
				_viewport = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the projection matrix once the method returns.</param>
		protected override void UpdateProjectionMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreateOrthographic(Viewport.Left, Viewport.Right, Viewport.Bottom, Viewport.Height, 1, -1);
		}

		/// <summary>
		///   Updates the view matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the view matrix once the method returns.</param>
		protected override void UpdateViewMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreateLookAt(new Vector3(Position.X, Distance, Position.Y),
										 new Vector3(Position.X, 0, Position.Y),
										 new Vector3(1, 0, 0));
		}
	}
}