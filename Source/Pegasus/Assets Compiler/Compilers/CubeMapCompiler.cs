namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using Assets;
	using Textures;
	using Utilities;

	/// <summary>
	///     Compiles cubemap textures.
	/// </summary>
	[UsedImplicitly]
	internal sealed class CubeMapCompiler : AssetCompiler<CubeMapAsset>
	{
		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected override CubeMapAsset CreateAsset(XElement assetMetadata)
		{
			if (assetMetadata.Name == "CubeMap")
				return new CubeMapAsset(assetMetadata);

			return null;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(CubeMapAsset asset, AssetWriter writer)
		{
			asset.Load();

			if (!asset.Compressed)
				asset.Write(writer);
			else
				CompileCompressed(asset, writer);
		}

		/// <summary>
		///     Compiles a cube map that should be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		private static void CompileCompressed(CubeMapAsset asset, AssetWriter writer)
		{
			if (!asset.IsPowerOfTwo)
				Log.Die("All texture dimensions must be power-of-two.");

			var paths = GetFacePaths(asset).ToArray();
			var faces = asset.ExtractFaces();

			for (var i = 0; i < 6; ++i)
				faces[i].Save(paths[i]);

			var assembledFile = asset.TempPath + ".dds";
			ExternalTool.NvAssemble(paths, assembledFile);

			var outFile = asset.TempPath + "-compressed.dds";
			ExternalTool.NvCompress(assembledFile, outFile, asset.CompressedFormat, asset.Mipmaps);

			var ddsImage = new DirectDrawSurface(File.ReadAllBytes(outFile));
			ddsImage.Write(writer);
		}

		/// <summary>
		///     Gets the paths of the temporary cubemap face files.
		/// </summary>
		/// <param name="asset">The asset the paths should be returned for.</param>
		private static IEnumerable<string> GetFacePaths(Asset asset)
		{
			return new[] { "-Z.png", "-X.png", "+Z.png", "+X.png", "-Y.png", "+Y.png" }
				.Select(path => asset.TempPath + path);
		}
	}
}