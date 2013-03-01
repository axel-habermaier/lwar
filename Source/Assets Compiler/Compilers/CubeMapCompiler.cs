using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform;
	using Bitmap = Image2D;

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
		protected override void CompileCore(CubeMapAsset asset, BufferWriter buffer)
		{
			if (asset.Uncompressed)
				CompileUncompressed(asset, buffer);
			else
				CompileCompressed(asset, buffer);
		}

		/// <summary>
		///   Compiles a cube map that should not be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private static void CompileUncompressed(CubeMapAsset asset, BufferWriter buffer)
		{
			using (var image = new CubeMap(asset))
				image.Write(buffer);
		}

		/// <summary>
		///   Compiles a cube map that should be compressed.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private static void CompileCompressed(CubeMapAsset asset, BufferWriter buffer)
		{
			using (var bitmap = (System.Drawing.Bitmap)Image.FromFile(asset.SourcePath))
			{
				var width = bitmap.Width / 6;
				if (bitmap.Width < 1 || bitmap.Width > Int16.MaxValue || !IsPowerOfTwo(width))
					Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Width, Int16.MaxValue);
				if (bitmap.Height < 1 || bitmap.Height > Int16.MaxValue || !IsPowerOfTwo(bitmap.Height))
					Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Height, Int16.MaxValue);

				var paths = new[] { "-Z.png", "-X.png", "+Z.png", "+X.png", "-Y.png", "+Y.png" }
					.Select(path => asset.TempPathWithoutExtension + path).ToArray();
				var faces = CubeMap.ExtractFaces(bitmap);

				for (var i = 0; i < 6; ++i)
					faces[i].Save(paths[i]);

				var assembledFile = asset.TempPathWithoutExtension + ".dds";
				ExternalTool.NvAssemble(paths, assembledFile);

				var outFile = asset.TempPathWithoutExtension + "-compressed" + PlatformInfo.AssetExtension;
				var format = ChooseCompression(bitmap.PixelFormat);
				ExternalTool.NvCompress(assembledFile, outFile, format, asset.Mipmaps);

				using (var ddsBuffer = BufferReader.Create(File.ReadAllBytes(outFile)))
				using (var ddsImage = new DirectDrawSurface(ddsBuffer))
					ddsImage.Write(buffer);
			}
		}
	}
}