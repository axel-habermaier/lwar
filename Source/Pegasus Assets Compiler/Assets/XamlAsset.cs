namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.IO;

	/// <summary>
	///     Represents a Xaml file that is cross-compiled into C# code targeting the Pegasus UI library.
	/// </summary>
	public class XamlAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="context">The context the asset is compiled in.</param>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public XamlAsset(CompilationContext context, string relativePath)
			: base(context, relativePath)
		{
		}
	}
}