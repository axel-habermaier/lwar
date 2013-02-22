using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Base class for all objects belong to a graphics device.
	/// </summary>
	public abstract class GraphicsObject : DisposableObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected GraphicsObject(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			GraphicsDevice = graphicsDevice;
		}

		/// <summary>
		///   Gets the graphics device this instance belongs to.
		/// </summary>
		internal GraphicsDevice GraphicsDevice { get; private set; }
	}
}