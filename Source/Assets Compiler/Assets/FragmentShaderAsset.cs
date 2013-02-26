using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a fragment shader that requires compilation.
	/// </summary>
	public class FragmentShaderAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public FragmentShaderAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}