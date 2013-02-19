using System;

namespace Pegasus.AssetsCompiler
{
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Processes cubemap textures, converting them to a premultiplied format.
	/// </summary>
	public sealed class CubeMapProcessor : TextureProcessor
	{
		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="source">The source file that should be processed.</param>
		/// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public override void Process(string source, string sourceRelative, BufferWriter writer)
		{
			using (var bitmap = (Bitmap)Image.FromFile(source))
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

				var sourceFile = Path.Combine(Path.GetDirectoryName(sourceRelative), Path.GetFileNameWithoutExtension(sourceRelative));
				var tempPng = Path.Combine(Environment.CurrentDirectory, Compiler.TempPath, sourceFile);
				var negativeZPath = tempPng + "-Z.png";
				var negativeXPath = tempPng + "-X.png";
				var positiveZPath = tempPng + "+Z.png";
				var positiveXPath = tempPng + "+X.png";
				var negativeYPath = tempPng + "-Y.png";
				var positiveYPath = tempPng + "+Y.png";

				negativeZ.Save(negativeZPath);
				negativeX.Save(negativeXPath);
				positiveZ.Save(positiveZPath);
				positiveX.Save(positiveXPath);
				negativeY.Save(negativeYPath);
				positiveY.Save(positiveYPath);

				var assembledFile = Path.Combine(Environment.CurrentDirectory, Compiler.TempPath, sourceFile) + ".dds";
				ExternalTool.NvAssemble(negativeZPath, negativeXPath, positiveZPath, positiveXPath, negativeYPath, positiveYPath, assembledFile);

				var outFile = Path.Combine(Environment.CurrentDirectory, Compiler.TempPath, sourceFile) + "-compressed" + PlatformInfo.AssetExtension;
				var format = ChooseCompression(bitmap.PixelFormat);
				ExternalTool.NvCompress(assembledFile, outFile, format);

				writer.WriteInt32((int)format);
				writer.Copy(File.ReadAllBytes(outFile));
			}
		}
	}
}