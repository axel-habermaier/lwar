using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using Math;

	/// <summary>
	///   A 2D texture manages two-dimensional texel data.
	/// </summary>
	public sealed class Texture2D : Texture
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="mipmaps">The base texture and its mipmaps that should be uploaded to the GPU.</param>
		public Texture2D(GraphicsDevice graphicsDevice, SurfaceFormat format, Mipmap[] mipmaps)
			: base(graphicsDevice, TextureType.CubeMap, format, mipmaps)
		{
		}

		/// <summary>
		///   Gets a 1x1 pixels fully white two-dimensional texture object.
		/// </summary>
		public static Texture2D White { get; private set; }

		/// <summary>
		///   Gets the size of the texture.
		/// </summary>
		public Size Size
		{
			get { return new Size(Width, Height); }
		}

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			var mipmaps = new[]
			{
				new Mipmap
				{
					Data = new byte[] { 255, 255, 255, 255 },
					Width = 1,
					Height = 1,
					Level = 0,
					Size = 4
				}
			};
			White = new Texture2D(graphicsDevice, SurfaceFormat.Rgba8, mipmaps);
		}

		/// <summary>
		///   Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			White.SafeDispose();
			White = null;
		}
	}
}