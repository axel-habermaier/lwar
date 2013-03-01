using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.IO;
	using Assets;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles 2D textures.
	/// </summary>
	internal sealed class Texture2DCompiler : AssetCompiler<Texture2DAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void CompileCore(Texture2DAsset asset, BufferWriter buffer)
		{
			asset.Load();

			if (asset.Uncompressed)
				asset.Write(buffer);
			else
				CompileCompressed(asset, buffer);
		}

		/// <summary>
		///   Compiles a texture that should be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private static void CompileCompressed(Texture2DAsset asset, BufferWriter buffer)
		{
			if (!asset.IsPowerOfTwo())
				Log.Die("All texture dimensions must be power-of-two.");

			var outFile = asset.TempPathWithoutExtension + ".dds";
			ExternalTool.NvCompress(asset.SourcePath, outFile, asset.CompressedFormat, asset.Mipmaps);

			using (var ddsBuffer = BufferReader.Create(File.ReadAllBytes(outFile)))
			{
				var ddsImage = new DirectDrawSurface(ddsBuffer);
				ddsImage.Write(buffer);
			}
		}
	}
}