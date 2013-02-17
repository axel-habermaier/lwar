using System;

namespace Pegasus.AssetsCompiler
{
	using Framework.Platform;

	/// <summary>
    ///   Asset processor base class.
    /// </summary>
    public abstract class AssetProcessor
    {
        /// <summary>
        ///   Processes the given file, writing the compiled output to the given target destination.
        /// </summary>
        /// <param name="source">The source file that should be processed.</param>
        /// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
        /// <param name="writer">The writer that should be used to write the compiled asset file.</param>
        public abstract void Process(string source, string sourceRelative, BufferWriter writer);
    }
}