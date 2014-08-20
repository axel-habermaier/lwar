﻿namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
	using Platform.Graphics;

	/// <summary>
	///     Represents a texture asset that requires compilation.
	/// </summary>
	public abstract class TextureAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		protected TextureAsset(string relativePath)
			: base(relativePath)
		{
			Mipmaps = true;
			Uncompressed = false;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="sourceDirectory">The source directory of the asset.</param>
		protected TextureAsset(string relativePath, string sourceDirectory)
			: base(relativePath, sourceDirectory)
		{
		}

		/// <summary>
		///     Gets or sets a value indicating whether mipmaps should be generated for the texture. Default is true.
		/// </summary>
		public bool Mipmaps { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the texture should not use texture compression. Default is false.
		/// </summary>
		public bool Uncompressed { get; set; }

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
		///     Serializes the uncompressed texture into the given buffer.
		/// </summary>
		/// <param name="writer">The writer the DDS image should be serialized into.</param>
		internal unsafe void Write(BinaryWriter writer)
		{
			Assert.That(!Mipmaps, "Mipmap generation is not supported for uncompressed cube maps.");
			Assert.That(Uncompressed, "Texture compression is not supported.");

			writer.WriteUInt32(Description.Width);
			writer.WriteUInt32(Description.Height);
			writer.WriteUInt32(Description.Depth);
			writer.WriteUInt32(Description.ArraySize);
			writer.WriteInt32((int)Description.Type);
			writer.WriteInt32((int)Description.Format);
			writer.WriteUInt32(Description.Mipmaps);
			writer.WriteUInt32(Description.SurfaceCount);

			foreach (var surface in Surfaces)
			{
				writer.WriteUInt32(surface.Width);
				writer.WriteUInt32(surface.Height);
				writer.WriteUInt32(surface.Depth);
				writer.WriteUInt32(surface.Size);
				writer.WriteUInt32(surface.Stride);

				for (var i = 0; i < surface.Size * surface.Depth; ++i)
					writer.WriteByte(surface.Data[i]);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			if (Bitmap != null)
				Bitmap.Dispose();
		}

		/// <summary>
		///     Chooses a suitable compressed format for the given uncompressed format.
		/// </summary>
		/// <param name="format">The pixel format for which a suitable compressed format should be chosen.</param>
		/// <param name="componentCount">Returns the number of color components of each pixel.</param>
		protected static SurfaceFormat ToSurfaceFormat(PixelFormat format, out uint componentCount)
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
		protected unsafe byte[] GetBitmapData(Bitmap bitmap)
		{
			uint componentCount;
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
		///     Checks whether the texture dimensions are a power of two.
		/// </summary>
		public bool IsPowerOfTwo()
		{
			return ((Description.Width & (Description.Width - 1)) == 0) &&
				   ((Description.Height & (Description.Height - 1)) == 0) &&
				   ((Description.Depth & (Description.Depth - 1)) == 0);
		}

		/// <summary>
		///     Represents the surface of a texture, i.e., a single mipmap and/or face of a texture.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Surface
		{
			/// <summary>
			///     The width of the surface.
			/// </summary>
			public uint Width;

			/// <summary>
			///     The height of the surface.
			/// </summary>
			public uint Height;

			/// <summary>
			///     The depth of the surface.
			/// </summary>
			public uint Depth;

			/// <summary>
			///     The size of the surface data in bytes.
			/// </summary>
			public uint Size;

			/// <summary>
			///     The stride between two rows of the surface in bytes.
			/// </summary>
			public uint Stride;

			/// <summary>
			///     The surface data.
			/// </summary>
			public byte[] Data;
		}
	}
}