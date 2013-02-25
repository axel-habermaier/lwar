using System;

namespace Pegasus.AssetsCompiler
{
	using System.Drawing;
	using System.IO;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles cubemap textures.
	/// </summary>
	public sealed class CubeMapCompiler : TextureCompiler
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		public CubeMapCompiler(string asset)
			: base(asset)
		{
		}

		/// <summary>
		///   Gets a description of the type of the asset that is compiled by the compiler.
		/// </summary>
		internal override string AssetType
		{
			get { return "Cube Maps"; }
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		protected override void CompileCore()
		{
			using (var bitmap = (Bitmap)Image.FromFile(Asset.SourcePath))
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

				var negativeZPath = Asset.TempPathWithoutExtension + "-Z.png";
				var negativeXPath = Asset.TempPathWithoutExtension + "-X.png";
				var positiveZPath = Asset.TempPathWithoutExtension + "+Z.png";
				var positiveXPath = Asset.TempPathWithoutExtension + "+X.png";
				var negativeYPath = Asset.TempPathWithoutExtension + "-Y.png";
				var positiveYPath = Asset.TempPathWithoutExtension + "+Y.png";

				negativeZ.Save(negativeZPath);
				negativeX.Save(negativeXPath);
				positiveZ.Save(positiveZPath);
				positiveX.Save(positiveXPath);
				negativeY.Save(negativeYPath);
				positiveY.Save(positiveYPath);

				var assembledFile = Asset.TempPathWithoutExtension + ".dds";
				ExternalTool.NvAssemble(negativeZPath, negativeXPath, positiveZPath, positiveXPath, negativeYPath, positiveYPath,
										assembledFile);

				var outFile = Asset.TempPathWithoutExtension + "-compressed" + PlatformInfo.AssetExtension;
				var format = ChooseCompression(bitmap.PixelFormat);
				ExternalTool.NvCompress(assembledFile, outFile, format);

				using (var buffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				{
					var ddsImage = new DirectDrawSurface(buffer);
					ddsImage.Write(Buffer);
				}
			}
		}
	}
}