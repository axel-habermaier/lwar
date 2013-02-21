using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.IO;
	using DDS;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Processes 2D textures, converting them to a premultiplied format.
	/// </summary>
	public sealed class Texture2DProcessor : TextureProcessor
	{
		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="asset">The asset that should be processed.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public override void Process(Asset asset, BufferWriter writer)
		{
			using (var bitmap = (Bitmap)Image.FromFile(asset.SourcePath))
			{
				if (bitmap.Width < 1 || bitmap.Width > Int16.MaxValue || !IsPowerOfTwo(bitmap.Width))
					Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Width, Int16.MaxValue);
				if (bitmap.Height < 1 || bitmap.Height > Int16.MaxValue || !IsPowerOfTwo(bitmap.Height))
					Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Height, Int16.MaxValue);

				var outFile = asset.TempPath + ".dds";
				var format = ChooseCompression(bitmap.PixelFormat);
				ExternalTool.NvCompress(asset.SourcePath, outFile, format);

				using (var buffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				{
					var ddsImage = new DirectDrawSurface(buffer);
					Write(ddsImage, writer);
				}
			}
		}
	}
}