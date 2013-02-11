using System;

namespace Pegasus.Framework.Rendering
{
	using Platform.Graphics;

	/// <summary>
	///   Represents a six-degrees-of-freedom debug camera.
	/// </summary>
	public class DebugCamera : Camera3D
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device for which the camera is created.</param>
		public DebugCamera(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}
	}
}