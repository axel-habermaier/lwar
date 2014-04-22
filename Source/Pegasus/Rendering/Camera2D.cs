namespace Pegasus.Rendering
{
	using System;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Represents a camera that can be used to draw 2D scenes.
	/// </summary>
	public class Camera2D : Camera
	{
		/// <summary>
		///     The camera's position within the world.
		/// </summary>
		private Vector2 _position;

		/// <summary>
		///     Gets or sets the camera's position within the world.
		/// </summary>
		public new Vector2 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		///     Updates the projection matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateProjectionMatrixCore()
		{
			Projection = Matrix.CreateOrthographic(Viewport.Left, Viewport.Right, Viewport.Bottom, Viewport.Top, -1, 0);
		}

		/// <summary>
		///     Updates the view matrix based on the current camera configuration.
		/// </summary>
		protected override void UpdateViewMatrixCore()
		{
			View = Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, 0),
									   new Vector3(Position.X, Position.Y, -1),
									   new Vector3(0, 1, 0));
		}
	}
}