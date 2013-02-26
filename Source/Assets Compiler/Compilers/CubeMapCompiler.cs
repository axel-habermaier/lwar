using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Drawing;
	using System.IO;
	using System.Threading.Tasks;
	using Assets;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles cubemap textures.
	/// </summary>
	internal sealed class CubeMapCompiler : TextureCompiler<CubeMapAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override async Task CompileCore(Asset asset, BufferWriter buffer)
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

				var negativeZPath = asset.TempPathWithoutExtension + "-Z.png";
				var negativeXPath = asset.TempPathWithoutExtension + "-X.png";
				var positiveZPath = asset.TempPathWithoutExtension + "+Z.png";
				var positiveXPath = asset.TempPathWithoutExtension + "+X.png";
				var negativeYPath = asset.TempPathWithoutExtension + "-Y.png";
				var positiveYPath = asset.TempPathWithoutExtension + "+Y.png";

				negativeZ.Save(negativeZPath);
				negativeX.Save(negativeXPath);
				positiveZ.Save(positiveZPath);
				positiveX.Save(positiveXPath);
				negativeY.Save(negativeYPath);
				positiveY.Save(positiveYPath);

				var assembledFile = asset.TempPathWithoutExtension + ".dds";
				await ExternalTool.NvAssemble(negativeZPath, negativeXPath, positiveZPath, positiveXPath, negativeYPath, positiveYPath,
											  assembledFile);

				var outFile = asset.TempPathWithoutExtension + "-compressed" + PlatformInfo.AssetExtension;
				var format = ChooseCompression(bitmap.PixelFormat);
				await ExternalTool.NvCompress(assembledFile, outFile, format);

				using (var ddsBuffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				{
					var ddsImage = new DirectDrawSurface(ddsBuffer);
					ddsImage.Write(buffer);
				}
			}
		}
	}
}