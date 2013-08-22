using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.IO;

	/// <summary>
	///   Represents a Xaml file that is cross-compiled into C# code targeting the Pegasus UI library.
	/// </summary>
	public class XamlAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public XamlAsset(string relativePath)
			: base(relativePath)
		{
		}

		/// <summary>
		///   Gets the absolute path to the generated C# file, i.e. C:/Source/Assets/UI.xaml.cs.
		/// </summary>
		public override string TargetPath
		{
			get { return Path.Combine(Configuration.SourceDirectory, RelativePath) + ".cs"; }
		}
	}
}