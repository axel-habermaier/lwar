namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Memory;
	using Utilities;

	/// <summary>
	///     Base class for all objects belonging to an OpenGL3-based graphics device.
	/// </summary>
	internal abstract class GraphicsObjectD3D11 : DisposableObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected GraphicsObjectD3D11(GraphicsDeviceD3D11 graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			Device = graphicsDevice.Device;
			Context = graphicsDevice.Context;
		}

		/// <summary>
		///     Gets the Direct3D11 device that should be used by the graphics object.
		/// </summary>
		protected D3D11Device Device { get; private set; }

		/// <summary>
		///     Gets the Direct3D11 device context that should be used by the graphics object.
		/// </summary>
		protected D3D11DeviceContext Context { get; private set; }
	}
}