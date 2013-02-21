using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.IO;
	using DDS;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Processes cubemap textures, converting them to a premultiplied format.
	/// </summary>
	public sealed class CubeMapProcessor : TextureProcessor
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
				var width = bitmap.Width / 6;
				if (bitmap.Width < 1 || bitmap.Width > Int16.MaxValue || !IsPowerOfTwo(width))
					Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Width, Int16.MaxValue);
				if (bitmap.Height < 1 || bitmap.Height > Int16.MaxValue || !IsPowerOfTwo(bitmap.Height))
					Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Height, Int16.MaxValue);

				var negativeZ = bitmap.Clone(new Rectangle(0, 0, width, bitmap.Height), bitmap.PixelFormat);
				var negativeX = bitmap.Clone(new Rectangle(width, 0, width, bitmap.Height), bitmap.PixelFormat);
				var positiveZ = bitmap.Clone(new Rectangle(2 * width, 0, width, bitmap.Height), bitmap.PixelFormat);
				var positiveX = bitmap.Clone(new Rectangle(3 * width, 0, width, bitmap.Height), bitmap.PixelFormat);
				var negativeY = bitmap.Clone(new Rectangle(4 * width, 0, width, bitmap.Height), bitmap.PixelFormat);
				var positiveY = bitmap.Clone(new Rectangle(5 * width, 0, width, bitmap.Height), bitmap.PixelFormat);

				var negativeZPath = asset.TempPath + "-Z.png";
				var negativeXPath = asset.TempPath + "-X.png";
				var positiveZPath = asset.TempPath + "+Z.png";
				var positiveXPath = asset.TempPath + "+X.png";
				var negativeYPath = asset.TempPath + "-Y.png";
				var positiveYPath = asset.TempPath + "+Y.png";

				negativeZ.Save(negativeZPath);
				negativeX.Save(negativeXPath);
				positiveZ.Save(positiveZPath);
				positiveX.Save(positiveXPath);
				negativeY.Save(negativeYPath);
				positiveY.Save(positiveYPath);

				var assembledFile = asset.TempPath + ".dds";
				ExternalTool.NvAssemble(negativeZPath, negativeXPath, positiveZPath, positiveXPath, negativeYPath, positiveYPath,
										assembledFile);

				var outFile = asset.TempPath + "-compressed" + PlatformInfo.AssetExtension;
				var format = ChooseCompression(bitmap.PixelFormat);
				ExternalTool.NvCompress(assembledFile, outFile, format);

				using (var buffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				{
					var ddsImage = new DirectDrawSurface(buffer);
					Write(ddsImage, writer);
				}
			}
		}
	}
}