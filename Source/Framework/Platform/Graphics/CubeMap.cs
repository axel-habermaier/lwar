using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using Math;

	/// <summary>
	///   Represents a two-dimensional cube map.
	/// </summary>
	public sealed class CubeMap : Texture
	{
		/// <summary>
		///   Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		public CubeMap(GraphicsDevice graphicsDevice, byte[] data, int width, int height, SurfaceFormat format)
			: base(graphicsDevice, TextureType.CubeMap, data, width, height, 0, format)
		{
		}

		/// <summary>
		///   Reinitializes the texture.
		/// </summary>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		internal void Reinitialize(byte[] data, int width, int height, SurfaceFormat format)
		{
			Assert.ArgumentNotNull(data, () => data);
			Assert.ArgumentSatisfies(width > 0, () => width, "Width must be greater than 0.");
			Assert.ArgumentSatisfies(height > 0, () => height, "Height must be greater than 0.");
			Assert.ArgumentInRange(format, () => format);

			Reinitialize(data, width, height, 0, format);
		}
	}
}