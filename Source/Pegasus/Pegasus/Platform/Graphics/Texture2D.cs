namespace Pegasus.Platform.Graphics
{
	using System;
	using Math;
	using Memory;
	using Utilities;

	/// <summary>
	///     A 2D texture manages two-dimensional texel data.
	/// </summary>
	public sealed class Texture2D : Texture
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		internal Texture2D(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">The description of the texture's properties.</param>
		/// <param name="surfaces">The optional surface data for the texture.</param>
		public Texture2D(GraphicsDevice graphicsDevice, ref TextureDescription description, Surface[] surfaces)
			: base(graphicsDevice)
		{
			Initialize(ref description, surfaces);
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="size">The size of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="flags">The flags that indicates which operations are supported on the texture.</param>
		public Texture2D(GraphicsDevice graphicsDevice, Size size, SurfaceFormat format, TextureFlags flags)
			: this(graphicsDevice, size.IntegralWidth, size.IntegralHeight, format, flags)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="flags">The flags that indicates which operations are supported on the texture.</param>
		public Texture2D(GraphicsDevice graphicsDevice, int width, int height, SurfaceFormat format, TextureFlags flags)
			: base(graphicsDevice)
		{
			Assert.ArgumentInRange(width, 1, 8192);
			Assert.ArgumentInRange(height, 1, 8192);

			var description = new TextureDescription
			{
				Width = (uint)width,
				Height = (uint)height,
				Depth = 1,
				ArraySize = 1,
				Flags = flags,
				SurfaceCount = 1,
				Format = format,
				Mipmaps = 1,
				Type = TextureType.Texture2D
			};

			Initialize(ref description, null);
		}

		/// <summary>
		///     Gets the size of the texture.
		/// </summary>
		public Size Size
		{
			get
			{
				ValidateAccess();
				return new Size(Description.Width, Description.Height);
			}
		}

		/// <summary>
		///     Gets the width of the texture.
		/// </summary>
		public int Width
		{
			get
			{
				ValidateAccess();
				return (int)Description.Width;
			}
		}

		/// <summary>
		///     Gets the height of the texture.
		/// </summary>
		public int Height
		{
			get
			{
				ValidateAccess();
				return (int)Description.Height;
			}
		}

		/// <summary>
		///     Gets the format of the texture.
		/// </summary>
		public SurfaceFormat Format
		{
			get
			{
				ValidateAccess();
				return Description.Format;
			}
		}

		/// <summary>
		///     Loads a texture from the given buffer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the texture should be created for.</param>
		/// <param name="buffer">The buffer the texture should be read from.</param>
		public static Texture2D Create(GraphicsDevice graphicsDevice, ref BufferReader buffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			var texture = new Texture2D(graphicsDevice);
			texture.Load(ref buffer);
			return texture;
		}
	}
}