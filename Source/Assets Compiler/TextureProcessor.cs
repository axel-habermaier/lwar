using System;

namespace Pegasus.AssetsCompiler
{
	using DDS;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;
	using PixelFormat = System.Drawing.Imaging.PixelFormat;

	public abstract class TextureProcessor : AssetProcessor
	{
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
		///   Converts the pixel format into a surface format.
		/// </summary>
		/// <param name="format">The pixel format that should be converted.</param>
		private static SurfaceFormat Convert(PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.Format32bppArgb:
					return SurfaceFormat.Rgba8;
				case PixelFormat.Format24bppRgb:
					return SurfaceFormat.Rgb8;
				case PixelFormat.Format8bppIndexed:
					return SurfaceFormat.Rgba8;
				default:
					throw new InvalidOperationException(String.Format("Unsupported texture format: {0}.", format));
			}
		}

		/// <summary>
		///   Chooses a suitable compressed format for the given uncompressed format.
		/// </summary>
		/// <param name="format">The pixel format for which a suitable compressed format should be chosen.</param>
		protected static SurfaceFormat ChooseCompression(PixelFormat format)
		{
			switch (Convert(format))
			{
				case SurfaceFormat.R8:
					return SurfaceFormat.Bc4;
				case SurfaceFormat.Rgb8:
					return SurfaceFormat.Bc1;
				case SurfaceFormat.Rgba8:
					return SurfaceFormat.Bc3;
				default:
					throw new InvalidOperationException("Unsupported uncompressed format.");
			}
		}

		/// <summary>
		///   Serializes the given DDS image into the given buffer.
		/// </summary>
		/// <param name="texture">The DDS image that should be serialized.</param>
		/// <param name="writer">The buffer the DDS image should be serialized into.</param>
		protected static void Write(DirectDrawSurface texture, BufferWriter writer)
		{
			writer.WriteUInt32(texture.Description.Width);
			writer.WriteUInt32(texture.Description.Height);
			writer.WriteUInt32(texture.Description.Depth);
			writer.WriteUInt32(texture.Description.ArraySize);
			writer.WriteInt32((int)texture.Description.Type);
			writer.WriteInt32((int)texture.Description.Format);
			writer.WriteInt32((int)texture.Description.Mipmaps);

			foreach (var surface in texture.Surfaces)
			{
				writer.WriteUInt32(surface.Width);
				writer.WriteUInt32(surface.Height);
				writer.WriteUInt32(surface.Depth);
				writer.WriteUInt32(surface.Size);
				writer.WriteUInt32(surface.Stride);
				writer.Copy(surface.Data);
			}
		}
	}
}