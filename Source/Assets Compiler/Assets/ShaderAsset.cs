using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a shader that requires compilation.
	/// </summary>
	public class ShaderAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="sourceDirectory">The source directory of the asset.</param>
		public ShaderAsset(string relativePath, string sourceDirectory)
			: base(relativePath, sourceDirectory)
		{
		}
	}
}