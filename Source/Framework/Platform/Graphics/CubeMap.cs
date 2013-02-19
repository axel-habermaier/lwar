using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a two-dimensional cube map.
	/// </summary>
	public sealed class CubeMap : Texture
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="mipmaps">The base texture and its mipmaps that should be uploaded to the GPU.</param>
		public CubeMap(GraphicsDevice graphicsDevice, SurfaceFormat format, Mipmap[] mipmaps)
			: base(graphicsDevice, TextureType.CubeMap, format, mipmaps)
		{
		}
	}
}