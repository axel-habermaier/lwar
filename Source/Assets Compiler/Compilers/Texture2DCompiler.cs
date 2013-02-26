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
	///   Compiles 2D textures.
	/// </summary>
	internal sealed class Texture2DCompiler : TextureCompiler<Texture2DAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override async Task CompileCore(Asset asset, BufferWriter buffer)
		{
			using (var bitmap = (Bitmap)Image.FromFile(asset.SourcePath))
			{
				if (bitmap.Width < 1 || bitmap.Width > Int16.MaxValue || !IsPowerOfTwo(bitmap.Width))
					Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Width, Int16.MaxValue);
				if (bitmap.Height < 1 || bitmap.Height > Int16.MaxValue || !IsPowerOfTwo(bitmap.Height))
					Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Height, Int16.MaxValue);

				var outFile = asset.TempPathWithoutExtension + ".dds";
				var format = ChooseCompression(bitmap.PixelFormat);
				await ExternalTool.NvCompress(asset.SourcePath, outFile, format);

				using (var ddsBuffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				{
					var ddsImage = new DirectDrawSurface(ddsBuffer);
					ddsImage.Write(buffer);
				}
			}
		}
	}
}