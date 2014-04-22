﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using Math;
	using Memory;

	/// <summary>
	///     A 2D texture manages two-dimensional texel data.
	/// </summary>
	public sealed class Texture2D : Texture
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		internal Texture2D()
			: base()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="size">The size of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="flags">The flags that indicates which operations are supported on the texture.</param>
		public Texture2D(Size size, SurfaceFormat format, TextureFlags flags)
			: this(size.Width, size.Height, format, flags)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="flags">The flags that indicates which operations are supported on the texture.</param>
		public Texture2D(int width, int height, SurfaceFormat format, TextureFlags flags)
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

			Reinitialize(description, null);
		}

		/// <summary>
		///     Gets a 1x1 pixels fully white two-dimensional texture object.
		/// </summary>
		public static Texture2D White { get; private set; }

		/// <summary>
		///     Gets the size of the texture.
		/// </summary>
		public Size Size
		{
			get { return new Size((int)Description.Width, (int)Description.Height); }
		}

		/// <summary>
		///     Gets the width of the texture.
		/// </summary>
		public int Width
		{
			get { return (int)Description.Width; }
		}

		/// <summary>
		///     Gets the height of the texture.
		/// </summary>
		public int Height
		{
			get { return (int)Description.Height; }
		}

		/// <summary>
		///     Gets the format of the texture.
		/// </summary>
		public SurfaceFormat Format
		{
			get { return Description.Format; }
		}

		/// <summary>
		///     Initializes the default instances.
		/// </summary>
		internal static unsafe void InitializeDefaultInstances()
		{
			var description = new TextureDescription
			{
				Width = 1,
				Height = 1,
				Depth = 1,
				ArraySize = 1,
				Type = TextureType.Texture2D,
				Format = SurfaceFormat.Rgba8,
				Mipmaps = 1,
				SurfaceCount = 1
			};

			var buffer = new byte[] { 255, 255, 255, 255 };
			using (var pointer = BufferPointer.Create(buffer))
			{
				var surfaces = new[]
				{
					new Surface
					{
						Data = pointer.Pointer,
						Width = 1,
						Height = 1,
						Depth = 1,
						Size = 4,
						Stride = 4
					}
				};

				White = new Texture2D();
				White.Reinitialize(description, surfaces);
				White.SetName("White");
			}
		}

		/// <summary>
		///     Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			White.SafeDispose();
			White = null;
		}
	}
}