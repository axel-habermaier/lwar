using System;
using Pegasus.AssetsCompiler.Assets.Attributes;

[assembly: Ignore("EntityTemplates/Compilation/EntityTemplateAsset.cs")]

namespace Lwar.Assets.EntityTemplates.Compilation
{
	using Pegasus.AssetsCompiler.Assets;

	/// <summary>
	///   Represents a C# file that contains an entity template definition.
	/// </summary>
	internal class EntityTemplateAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public EntityTemplateAsset(string relativePath)
			: base(relativePath, doNotCreateTargetPath: true)
		{
		}
	}
}