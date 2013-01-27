using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.Drawing.Imaging;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Processes textures, converting them to an premultiplied RGBA format.
	/// </summary>
	public sealed class TextureProcessor : AssetProcessor
	{
		/// <summary>
		///   Returns true if the processor can process a file with the given extension.
		/// </summary>
		/// <param name="extension">The extension of the file that should be processed.</param>
		public override bool CanProcess(string extension)
		{
			return extension == ".png" || extension == ".jpg";
		}

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
			SurfaceFormat format;
			switch (bitmap.PixelFormat)
			{
				case PixelFormat.Format32bppArgb:
					format = SurfaceFormat.Color;
					break;
				default:
					throw new InvalidOperationException(String.Format("Unsupported texture format: {0}.",
																	  bitmap.PixelFormat));
			}

			var length = bitmap.Width * bitmap.Height * format.ComponentCount();
			BitmapData bitmapData = null;
			try
			{
				bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
											 bitmap.PixelFormat);

				Assert.That(bitmap.Width < UInt16.MaxValue, "Texture width exceeds limit.");
				Assert.That(bitmap.Height < UInt16.MaxValue, "Texture height exceeds limit.");

				writer.WriteUInt16((ushort)bitmap.Width);
				writer.WriteUInt16((ushort)bitmap.Height);
				writer.WriteByte((byte)format.ComponentCount());

				if (format.ComponentCount() == 4)
				{
					var sourceData = (byte*)bitmapData.Scan0;
					// Switch from BGRA to RGBA and to premultiplied alpha.
					for (int i = 0; i < length; i += 4)
					{
						var b = sourceData[i] / 255.0f;
						var g = sourceData[i + 1] / 255.0f;
						var r = sourceData[i + 2] / 255.0f;
						var a = sourceData[i + 3] / 255.0f;

						writer.WriteByte((byte)(r * a * 255));
						writer.WriteByte((byte)(g * a * 255));
						writer.WriteByte((byte)(b * a * 255));
						writer.WriteByte((byte)(a * 255));
					}
				}
				else
					throw new NotSupportedException("Textures with less than 4 channels are not supported.");
			}
			finally
			{
				if (bitmapData != null)
					bitmap.UnlockBits(bitmapData);
			}
		}
	}
}