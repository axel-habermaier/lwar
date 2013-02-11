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
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		public Camera2D(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Updates the projection matrix based on the current camera configuration.
		/// </summary>
		/// <param name="matrix">The matrix that should hold the projection matrix once the method returns.</param>
		protected override void UpdateProjectionMatrix(out Matrix matrix)
		{
			matrix = Matrix.CreateOrthographic(Viewport.Left, Viewport.Right, Viewport.Bottom, Viewport.Height, 1, -1);
		}
	}
}