using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a C# code file that requires compilation.
	/// </summary>
	public class CSharpAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public CSharpAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}