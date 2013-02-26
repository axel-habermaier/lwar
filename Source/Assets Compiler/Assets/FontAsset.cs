using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a font asset that requires compilation.
	/// </summary>
	public class FontAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public FontAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}