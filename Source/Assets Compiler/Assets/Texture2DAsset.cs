using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a 2D texture that requires compilation.
	/// </summary>
	public class Texture2DAsset : TextureAsset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public Texture2DAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}