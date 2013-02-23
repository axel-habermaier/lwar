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
		///   Gets a description of the asset type.
		/// </summary>
		public abstract string AssetType { get; }

		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="asset">The asset that should be processed.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public abstract void Process(Asset asset, BufferWriter writer);
	}
}