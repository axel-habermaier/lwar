using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.IO;
	using Assets;
	using Framework.Platform;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;

	/// <summary>
	///   Compiles 2D textures.
	/// </summary>
	[UsedImplicitly]
	internal sealed class Texture2DCompiler : AssetCompiler<Texture2DAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(Texture2DAsset asset, BufferWriter buffer)
		{
			asset.Load();

			if (asset.Uncompressed)
				asset.Write(buffer);
			else
				CompileCompressed(asset, buffer);
		}

		/// <summary>
		///   Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected override void Clean(Texture2DAsset asset)
		{
			File.Delete(GetAssembledFilePath(asset));
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

			var outFile = GetAssembledFilePath(asset);
			ExternalTool.NvCompress(asset.SourcePath, outFile, asset.CompressedFormat, asset.Mipmaps);

			using (var ddsBuffer = BufferReader.Create(File.ReadAllBytes(outFile)))
			{
				var ddsImage = new DirectDrawSurface(ddsBuffer);
				ddsImage.Write(buffer);
			}
		}

		/// <summary>
		///   Gets the path of the temporary assembled texture file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetAssembledFilePath(Asset asset)
		{
			return asset.TempPathWithoutExtension + ".dds";
		}
	}
}