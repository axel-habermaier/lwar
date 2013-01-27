using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Framework;

	/// <summary>
	///   Processes GLSL shaders.
	/// </summary>
	public sealed class GlslShaderProcessor : AssetProcessor
	{
		/// <summary>
		///   Gets a value indicating whether the file extension should be removed from the compiled asset name.
		/// </summary>
		public override bool RemoveExtension
		{
			get { return false; }
		}

		/// <summary>
		///   Returns true if the processor can process a file with the given extension.
		/// </summary>
		/// <param name="extension">The extension of the file that should be processed.</param>
		public override bool CanProcess(string extension)
		{
			return extension == ".glsl";
		}

		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="source">The source file that should be processed.</param>
		/// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
		/// <param name="writer">The asset writer that should be used to write the compiled asset file.</param>
		public override void Process(string source, string sourceRelative, AssetWriter writer)
		{
			Assert.ArgumentNotNullOrWhitespace(source, () => source);
			Assert.ArgumentNotNull(writer, () => writer);

			writer.WriteString(File.ReadAllText(source));
		}
	}
}