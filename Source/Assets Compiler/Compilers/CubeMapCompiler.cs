using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Compiles cubemap textures.
	/// </summary>
	[UsedImplicitly]
	internal sealed class CubeMapCompiler : AssetCompiler<CubeMapAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void CompileCore(CubeMapAsset asset, BufferWriter buffer)
		{
			asset.Load();

			if (asset.Uncompressed)
				asset.Write(buffer);
			else
				CompileCompressed(asset, buffer);
		}

		/// <summary>
		///   Compiles a cube map that should be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private static void CompileCompressed(CubeMapAsset asset, BufferWriter buffer)
		{
			if (!asset.IsPowerOfTwo())
				Log.Die("All texture dimensions must be power-of-two.");

			var paths = new[] { "-Z.png", "-X.png", "+Z.png", "+X.png", "-Y.png", "+Y.png" }
				.Select(path => asset.TempPathWithoutExtension + path).ToArray();
			var faces = asset.ExtractFaces();

			for (var i = 0; i < 6; ++i)
				faces[i].Save(paths[i]);

			var assembledFile = asset.TempPathWithoutExtension + ".dds";
			ExternalTool.NvAssemble(paths, assembledFile);

			var outFile = asset.TempPathWithoutExtension + "-compressed" + PlatformInfo.AssetExtension;
			ExternalTool.NvCompress(assembledFile, outFile, asset.CompressedFormat, asset.Mipmaps);

			using (var ddsBuffer = BufferReader.Create(File.ReadAllBytes(outFile)))
			{
				var ddsImage = new DirectDrawSurface(ddsBuffer);
				ddsImage.Write(buffer);
			}
		}
	}
}