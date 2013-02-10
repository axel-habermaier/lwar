using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   The camera that is used during regular game sessions.
	/// </summary>
	public class Camera : ICamera
	{
		/// <summary>
		///   The position of the camera.
		/// </summary>
		private Vector2 _position;

		/// <summary>
		///   Gets or sets the position of the camera.
		/// </summary>
		public Vector2 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				View = Matrix.CreateTranslation(value.X, value.Y, 0);
			}
		}

		/// <summary>
		///   Gets or sets the camera's projection matrix.
		/// </summary>
		public Matrix Projection { get; set; }

		/// <summary>
		///   Gets or sets the camera's view matrix.
		/// </summary>
		public Matrix View { get; set; }
	}
}