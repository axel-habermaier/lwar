using System;

namespace Lwar.Assets.Templates.Compilation
{
	using Pegasus.AssetsCompiler.Assets;

	/// <summary>
	///   Represents a C# file that contains an entity template definition.
	/// </summary>
	public class TemplateAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public TemplateAsset(string relativePath)
			: base(relativePath)
		{
		}
	}
}