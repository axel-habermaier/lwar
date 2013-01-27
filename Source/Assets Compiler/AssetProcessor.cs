using System;

namespace Pegasus.AssetsCompiler
{
    /// <summary>
    ///   Asset processor base class.
    /// </summary>
    public abstract class AssetProcessor
    {
        /// <summary>
        ///   Gets a value indicating whether the file extension should be removed from the compiled asset name.
        /// </summary>
        public virtual bool RemoveExtension
        {
            get { return true; }
        }

        /// <summary>
        ///   Returns true if the processor can process a file with the given extension.
        /// </summary>
        /// <param name="extension">The extension of the file that should be processed.</param>
        public abstract bool CanProcess(string extension);

        /// <summary>
        ///   Processes the given file, writing the compiled output to the given target destination.
        /// </summary>
        /// <param name="source">The source file that should be processed.</param>
        /// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
        /// <param name="writer">The asset writer that should be used to write the compiled asset file.</param>
        public abstract void Process(string source, string sourceRelative, AssetWriter writer);
    }
}