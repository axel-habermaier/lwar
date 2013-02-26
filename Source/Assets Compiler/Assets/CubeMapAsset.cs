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
		public CubeMapAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}