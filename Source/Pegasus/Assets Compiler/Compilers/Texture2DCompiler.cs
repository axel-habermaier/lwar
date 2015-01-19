namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.IO;
	using System.Xml.Linq;
	using Assets;
	using Textures;
	using Utilities;

	/// <summary>
	///     Compiles 2D textures.
	/// </summary>
	[UsedImplicitly]
	internal sealed class Texture2DCompiler : AssetCompiler<Texture2DAsset>
	{
		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected override Texture2DAsset CreateAsset(XElement assetMetadata)
		{
			if (assetMetadata.Name == "Texture2D")
				return new Texture2DAsset(assetMetadata);

			return null;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(Texture2DAsset asset, AssetWriter writer)
		{
			asset.Load();

			if (!asset.Compressed)
				asset.Write(writer);
			else
				CompileCompressed(asset, writer);
		}

		/// <summary>
		///     Compiles a texture that should be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The writer the compilation output should be appended to.</param>
		private static void CompileCompressed(Texture2DAsset asset, AssetWriter buffer)
		{
			if (!asset.IsPowerOfTwo)
				Log.Die("All texture dimensions must be power-of-two.");

			var inFile = asset.TempPath + ".premult";
			var outFile = asset.TempPath + ".dds";

			asset.Save(inFile);
			ExternalTool.NvCompress(inFile, outFile, asset.CompressedFormat, asset.Mipmaps);

			var ddsImage = new DirectDrawSurface(File.ReadAllBytes(outFile));
			ddsImage.Write(buffer);
		}
	}
}