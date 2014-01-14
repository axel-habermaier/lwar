namespace Lwar.AssetsCompiler.EntityTemplates.Compilation
{
	using System;
	using Pegasus.AssetsCompiler;
	using Pegasus.AssetsCompiler.Assets;

	/// <summary>
	///     Represents a C# file that contains an entity template definition.
	/// </summary>
	internal class EntityTemplateAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="context">The context the asset is compiled in.</param>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public EntityTemplateAsset(CompilationContext context, string relativePath)
			: base(context, relativePath, doNotCreateTargetPath: true)
		{
		}
	}
}