namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
	using System.Xml.Linq;
	using Textures;
	using Utilities;

	/// <summary>
	///     Represents a texture asset that requires compilation.
	/// </summary>
	internal abstract class TextureAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		/// <param name="basePath">Overrides the default base path of the asset.</param>
		protected TextureAsset(XElement metadata, string basePath = null)
			: base(metadata, "File", basePath)
		{
			Mipmaps = GetBoolMetadata("GenerateMipmaps");
			Compressed = GetBoolMetadata("Compress");
		}

		/// <summary>
		///     Gets a value indicating whether mipmaps should be generated for the texture.
		/// </summary>
		public bool Mipmaps { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the texture should use texture compression.
		/// </summary>
		public bool Compressed { get; private set; }

		/// <summary>
		///     Gets the texture description.
		/// </summary>
		protected TextureDescription Description { get; set; }

		/// <summary>
		///     Gets the surfaces of the texture.
		/// </summary>
		protected Surface[] Surfaces { get; set; }

		/// <summary>
		///     Gets or sets the source bitmap.
		/// </summary>
		protected Bitmap Bitmap { get; set; }

		/// <summary>
		///     Gets the suitable compressed format for the texture.
		/// </summary>
		public SurfaceFormat CompressedFormat
		{
			get
			{
				Assert.NotNull(Bitmap);
				switch (Bitmap.PixelFormat)
				{
					case PixelFormat.Format8bppIndexed:
						return SurfaceFormat.Bc4;
					case PixelFormat.Format24bppRgb:
						return SurfaceFormat.Bc1;
					case PixelFormat.Format32bppArgb:
						return SurfaceFormat.Bc3;
					default:
						throw new InvalidOperationException("Unsupported uncompressed format.");
				}
			}
		}

		/// <summary>
		///     Checks whether the texture dimensions are a power of two.
		/// </summary>
		public bool IsPowerOfTwo
		{
			get
			{
				return ((Description.Width & (Description.Width - 1)) == 0) &&
					   ((Description.Height & (Description.Height - 1)) == 0) &&
					   ((Description.Depth & (Description.Depth - 1)) == 0);
			}
		}

		/// <summary>
		///     Serializes the uncompressed texture into the given buffer.
		/// </summary>
		/// <param name="writer">The writer the DDS image should be serialized into.</param>
		internal void Write(AssetWriter writer)
		{
			Assert.That(!Mipmaps, "Mipmap generation is not supported for uncompressed cube maps.");
			Assert.That(!Compressed, "Texture compression is not supported.");

			writer.WriteInt32(Description.Width);
			writer.WriteInt32(Description.Height);
			writer.WriteInt32(Description.Depth);
			writer.WriteInt32(Description.ArraySize);
			writer.WriteInt32((int)Description.Type);
			writer.WriteInt32((int)Description.Format);
			writer.WriteInt32(Description.Mipmaps);
			writer.WriteInt32(Description.SurfaceCount);

			foreach (var surface in Surfaces)
			{
				writer.WriteInt32(surface.Width);
				writer.WriteInt32(surface.Height);
				writer.WriteInt32(surface.Depth);
				writer.WriteInt32(surface.Size);
				writer.WriteInt32(surface.Stride);

				for (var i = 0; i < surface.Size * surface.Depth; ++i)
					writer.WriteByte(surface.Data[i]);
			}
		}

		/// <summary>
		///     Chooses a suitable compressed format for the given uncompressed format.
		/// </summary>
		/// <param name="format">The pixel format for which a suitable compressed format should be chosen.</param>
		/// <param name="componentCount">Returns the number of color components of each pixel.</param>
		protected static SurfaceFormat ToSurfaceFormat(PixelFormat format, out int componentCount)
		{
			switch (format)
			{
				case PixelFormat.Format8bppIndexed:
					componentCount = 1;
					return SurfaceFormat.R8;
				case PixelFormat.Format32bppArgb:
				case PixelFormat.Format24bppRgb:
					componentCount = 4;
					return SurfaceFormat.Rgba8;
				default:
					throw new InvalidOperationException("Unsupported pixel format.");
			}
		}

		/// <summary>
		///     Loads and converts the image data.
		/// </summary>
		/// <param name="bitmap">The bitmap from which the data should be loaded.</param>
		protected static unsafe byte[] GetBitmapData(Bitmap bitmap)
		{
			int componentCount;
			ToSurfaceFormat(bitmap.PixelFormat, out componentCount);

			var buffer = new byte[bitmap.Width * bitmap.Height * componentCount];
			BitmapData imageData = null;

			try
			{
				var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				imageData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				var sourceData = (byte*)imageData.Scan0;

				switch (bitmap.PixelFormat)
				{
					case PixelFormat.Format8bppIndexed:
						Marshal.Copy(imageData.Scan0, buffer, 0, buffer.Length);
						break;
					case PixelFormat.Format24bppRgb:
						// Switch from BGR to RGBA and to premultiplied alpha.
						for (int i = 0, j = 0; j < buffer.Length; i += 3, j += 4)
						{
							var b = sourceData[i];
							var g = sourceData[i + 1];
							var r = sourceData[i + 2];

							buffer[j] = r;
							buffer[j + 1] = g;
							buffer[j + 2] = b;
							buffer[j + 3] = 255;
						}
						break;
					case PixelFormat.Format32bppArgb:
						// Switch from BGRA to RGBA and to premultiplied alpha.
						for (var i = 0; i < buffer.Length; i += 4)
						{
							var b = sourceData[i] / 255.0f;
							var g = sourceData[i + 1] / 255.0f;
							var r = sourceData[i + 2] / 255.0f;
							var a = sourceData[i + 3] / 255.0f;

							buffer[i] = ((byte)(r * a * 255));
							buffer[i + 1] = ((byte)(g * a * 255));
							buffer[i + 2] = ((byte)(b * a * 255));
							buffer[i + 3] = ((byte)(a * 255));
						}
						break;
					default:
						throw new InvalidOperationException("Unsupported pixel format.");
				}

				return buffer;
			}
			finally
			{
				if (imageData != null)
					bitmap.UnlockBits(imageData);
			}
		}

		/// <summary>
		///     Converts the bitmap into a premultiplied alpha format.
		/// </summary>
		/// <param name="bitmap">The bitmap that should be converted.</param>
		protected static unsafe void ToPremultipliedAlpha(Bitmap bitmap)
		{
			Assert.NotNull(bitmap);

			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				return;
			
			var width = bitmap.Width;
			var height = bitmap.Height;
			var data = GetBitmapData(bitmap);

			BitmapData imageData = null;
			try
			{
				var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				imageData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				var sourceData = (byte*)imageData.Scan0;

				for (var x = 0; x < width; ++x)
				{
					for (var y = 0; y < height; ++y)
					{
						sourceData[x * 4 + y * 4 * width + 3] = data[x * 4 + y * 4 * width + 3];
						sourceData[x * 4 + y * 4 * width + 0] = data[x * 4 + y * 4 * width + 0];
						sourceData[x * 4 + y * 4 * width + 1] = data[x * 4 + y * 4 * width + 1];
						sourceData[x * 4 + y * 4 * width + 2] = data[x * 4 + y * 4 * width + 2];
					}
				}
			}
			finally
			{
				if (imageData != null)
					bitmap.UnlockBits(imageData);
			}
		}
	}
}