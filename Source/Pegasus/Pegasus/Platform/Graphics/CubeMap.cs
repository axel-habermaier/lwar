namespace Pegasus.Platform.Graphics
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a two-dimensional cube map.
	/// </summary>
	public sealed class CubeMap : Texture
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		private CubeMap(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Loads a cube map from the given buffer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the cube map should be created for.</param>
		/// <param name="buffer">The buffer the cube map should be read from.</param>
		public static CubeMap Create(GraphicsDevice graphicsDevice, ref BufferReader buffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			var cubeMap = new CubeMap(graphicsDevice);
			cubeMap.Load(ref buffer);
			return cubeMap;
		}
	}
}