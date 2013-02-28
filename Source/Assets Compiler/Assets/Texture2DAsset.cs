using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a 2D texture that requires compilation.
	/// </summary>
	public class Texture2DAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="mipmaps">Indicates whether mipmaps should be generated for the cube map.</param>
		public Texture2DAsset(string relativePath, bool mipmaps = true)
			: base(relativePath)
		{
			Mipmaps = mipmaps;
		}

		/// <summary>
		///   Gets a value indicating whether mipmaps should be generated for the cube map.
		/// </summary>
		public bool Mipmaps { get; private set; }
	}
}