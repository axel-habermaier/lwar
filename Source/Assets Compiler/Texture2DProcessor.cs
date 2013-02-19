using System;

namespace Pegasus.AssetsCompiler
{
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Processes 2D textures, converting them to a premultiplied format.
	/// </summary>
	public sealed class Texture2DProcessor : TextureProcessor
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
				if (bitmap.Width < 1 || bitmap.Width > Int16.MaxValue || !IsPowerOfTwo(bitmap.Width))
					Log.Die("Invalid texture width '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Width, Int16.MaxValue);
				if (bitmap.Height < 1 || bitmap.Height > Int16.MaxValue || !IsPowerOfTwo(bitmap.Height))
					Log.Die("Invalid texture height '{0}' (must be power-of-two and between 0 and {1}).", bitmap.Height, Int16.MaxValue);

				writer.WriteInt32(bitmap.Width);
				writer.WriteInt32(bitmap.Height);

				var tempFile = Path.Combine(Environment.CurrentDirectory, Compiler.TempPath, sourceRelative) + ".dds";
				var process = new Process
				{
					EnableRaisingEvents = true,
					StartInfo = new ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, NvCompressPath),
													 String.Format("-dds10 \"{0}\" \"{1}\"", source, tempFile))
				};
				process.StartInfo.UseShellExecute = false;
				process.Start();
				process.WaitForExit();
			}
		}
	}
}