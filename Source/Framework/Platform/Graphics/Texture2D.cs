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
		public Texture2D(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, TextureType.CubeMap)
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
			get { return new Size((int)Description.Width, (int)Description.Height); }
		}

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			var description = new TextureDescription
			{
				Width = 1,
				Height = 1,
				Depth = 1,
				ArraySize = 1,
				Type = TextureType.Texture2D,
				Format = SurfaceFormat.Rgba8,
				Mipmaps = Mipmaps.None
			};

			var surfaces = new[]
			{
				new Surface
				{
					Data = new byte[] { 255, 255, 255, 255 },
					Width = 1,
					Height = 1,
					Depth = 1,
					Size = 4,
					Stride = 4
				}
			};

			White = new Texture2D(graphicsDevice);
			White.Reinitialize(description, surfaces);
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