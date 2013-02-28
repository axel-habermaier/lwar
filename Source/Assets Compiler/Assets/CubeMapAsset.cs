using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a cube map asset that requires compilation.
	/// </summary>
	public class CubeMapAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="mipmaps">Indicates whether mipmaps should be generated for the cube map.</param>
		public CubeMapAsset(string relativePath, bool mipmaps = true)
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