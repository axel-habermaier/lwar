﻿namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Pegasus.Assets;
	using Platform;
	using Platform.Logging;
	using Utilities;
	using BinaryWriter = AssetsCompiler.BinaryWriter;

	/// <summary>
	///     Compiles cubemap textures.
	/// </summary>
	[UsedImplicitly]
	internal sealed class CubeMapCompiler : AssetCompiler<CubeMapAsset>
	{
		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(CubeMapAsset asset, BinaryWriter writer)
		{
			asset.Load();
			WriteAssetHeader(writer, (byte)AssetType.CubeMap);

			if (asset.Uncompressed)
				asset.Write(writer);
			else
				CompileCompressed(asset, writer);
		}

		/// <summary>
		///     Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected override void Clean(CubeMapAsset asset)
		{
			File.Delete(GetAssembledFilePath(asset));
			File.Delete(GetCompressedFilePath(asset));

			foreach (var path in GetFacePaths(asset))
				File.Delete(path);
		}

		/// <summary>
		///     Compiles a cube map that should be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		private static void CompileCompressed(CubeMapAsset asset, BinaryWriter writer)
		{
			if (!asset.IsPowerOfTwo())
				Log.Die("All texture dimensions must be power-of-two.");

			var paths = GetFacePaths(asset).ToArray();
			var faces = asset.ExtractFaces();

			for (var i = 0; i < 6; ++i)
				faces[i].Save(paths[i]);

			var assembledFile = GetAssembledFilePath(asset);
			ExternalTool.NvAssemble(paths, assembledFile);

			var outFile = GetCompressedFilePath(asset);
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
				.Select(path => asset.TempPathWithoutExtension + path);
		}

		/// <summary>
		///     Gets the path of the temporary assembled cubemap file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetAssembledFilePath(Asset asset)
		{
			return asset.TempPathWithoutExtension + ".dds";
		}

		/// <summary>
		///     Gets the path of the temporary compressed cubemap file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetCompressedFilePath(Asset asset)
		{
			return asset.TempPathWithoutExtension + "-compressed" + PlatformInfo.AssetExtension;
		}
	}
}