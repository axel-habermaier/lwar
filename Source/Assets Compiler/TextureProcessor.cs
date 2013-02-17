using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	public abstract class TextureProcessor : AssetProcessor
	{
		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="source">The source file that should be processed.</param>
		/// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public override unsafe void Process(string source, string sourceRelative, BufferWriter writer)
		{
			Assert.ArgumentNotNullOrWhitespace(source, () => source);
			Assert.ArgumentNotNull(writer, () => writer);

			var bitmap = (Bitmap)Image.FromFile(source);

			BitmapData bitmapData = null;

			try
			{
				bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				var sourceData = (byte*)bitmapData.Scan0;
				var length = bitmap.Width * bitmap.Height;
				SurfaceFormat format;
				byte[] data;

				switch (bitmap.PixelFormat)
				{
					case PixelFormat.Format32bppArgb:
						format = SurfaceFormat.Rgba8;
						length *= ComponentCount(format);
						data = new byte[length];

						// Switch from BGRA to premultiplied alpha RGBA
						for (var i = 0; i < length; i += 4)
						{
							var b = sourceData[i] / 255.0f;
							var g = sourceData[i + 1] / 255.0f;
							var r = sourceData[i + 2] / 255.0f;
							var a = sourceData[i + 3] / 255.0f;

							data[i] = ((byte)(r * a * 255));
							data[i + 1] = ((byte)(g * a * 255));
							data[i + 2] = ((byte)(b * a * 255));
							data[i + 3] = ((byte)(a * 255));
						}
						break;
					case PixelFormat.Format24bppRgb:
						format = SurfaceFormat.Rgb8;
						length *= ComponentCount(format);
						data = new byte[length];

						// Switch from BGR to RGB
						for (var i = 0; i < length; i += 3)
						{
							data[i + 2] = sourceData[i];
							data[i + 1] = sourceData[i + 1];
							data[i] = sourceData[i + 2];
						}
						break;
					case PixelFormat.Format8bppIndexed:
						format = SurfaceFormat.Rgba8;
						data = new byte[length];

						for (var i = 0; i < length; ++i)
							data[i] = sourceData[i];
						break;
					default:
						throw new InvalidOperationException(String.Format("Unsupported texture format: {0}.", bitmap.PixelFormat));
				}

				Process(data, bitmap.Width, bitmap.Height, format, writer);
			}
			finally
			{
				if (bitmapData != null)
					bitmap.UnlockBits(bitmapData);
			}
		}

		/// <summary>
		///   Checks whether the given number is a power of two.
		/// </summary>
		/// <param name="number">The number that should be checked.</param>
		protected static bool IsPowerOfTwo(int number)
		{
			Assert.ArgumentInRange(number, () => number, 1, Int32.MaxValue);
			return (number & (number - 1)) == 0;
		}

		/// <summary>
		///   Processes the texture.
		/// </summary>
		/// <param name="data">The textue data.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		protected abstract void Process(byte[] data, int width, int height, SurfaceFormat format, BufferWriter writer);

		/// <summary>
		///   Gets the number of components from the surface format.
		/// </summary>
		/// <param name="format">The surface format.</param>
		private static byte ComponentCount(SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Rgba8:
					return 4;
				case SurfaceFormat.Rgb8:
					return 3;
				case SurfaceFormat.R8:
					return 1;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}
		}

		/// <summary>
		///   Enumerates all mipmap levels for a texture of the given size in descending order, not including the original texture
		///   itself.
		/// </summary>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		protected IEnumerable<Size> ComputeMipmaps(int width, int height)
		{
			while (width > 1 || height > 1)
			{
				width = Math.Max(1, width / 2);
				height = Math.Max(1, height / 2);

				yield return new Size(width, height);
			}
		}

		/// <summary>
		///   Computes the mipmapped data for the given mipmap size.
		/// </summary>
		/// <param name="data">The original texture data.</param>
		/// <param name="size">The size of the mipmap.</param>
		/// <param name="scale">The scaling factor between the mipmap and the original texture.</param>
		/// <param name="format">The format of the texture.</param>
		protected byte[] GenerateMipmap(byte[] data, Size size, Size scale, SurfaceFormat format)
		{
			var width = size.Width;
			var height = size.Height;

			var componentCount = ComponentCount(format);
			var mipmap = new byte[width * height * componentCount];
			var stride = width * componentCount;

			switch (format)
			{
				case SurfaceFormat.Rgba8:
					for (var x = 0; x < width; ++x)
					{
						for (var y = 0; y < height; ++y)
						{
							var offset = y * stride + x;
							mipmap[offset] = Filter(data, x, y, stride, scale, 0);
							mipmap[offset + 1] = Filter(data, x, y, stride, scale, 1);
							mipmap[offset + 2] = Filter(data, x, y, stride, scale, 2);
							mipmap[offset + 3] = Filter(data, x, y, stride, scale, 3);
						}
					}
					break;
				case SurfaceFormat.Rgb8:
					break;
				case SurfaceFormat.R8:
					break;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}

			return mipmap;
		}

		private static byte Filter(byte[] data, int x, int y, int stride, Size scale, int offset)
		{
			x *= scale.Width;
			y *= scale.Height;

			var a = data[y * stride + offset + x];
			var b = data[(y + 1) * stride + offset + x];
			var c = data[y * stride + offset + (x + 1)];
			var d = data[(y + 1) * stride + offset + (x + 1)];

			return (byte)((a + b + c + d) / 4);
		}
	}
}