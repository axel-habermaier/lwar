using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a vertex shader that requires compilation.
	/// </summary>
	public class VertexShaderAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public VertexShaderAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}