namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Memory;
	using Utilities;

	/// <summary>
	///     Base class for all objects belonging to an OpenGL3-based graphics device.
	/// </summary>
	internal abstract class GraphicsObjectGL3 : DisposableObject
	{
		/// <summary>
		///     Provides access to all OpenGL3 entry points.
		/// </summary>
		// ReSharper disable once InconsistentNaming
		protected readonly EntryPoints _gl;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected GraphicsObjectGL3(GraphicsDeviceGL3 graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			GraphicsDevice = graphicsDevice;
			_gl = graphicsDevice._gl;
		}

		/// <summary>
		///     Gets the graphics device the graphics object belongs to.
		/// </summary>
		protected GraphicsDeviceGL3 GraphicsDevice { get; private set; }
	}
}