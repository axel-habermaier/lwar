namespace Pegasus.AssetsCompiler.Assets
{
	using System;

	/// <summary>
	///     Represents a C# shader effect file that requires compilation.
	/// </summary>
	public class EffectAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="context">The context the asset is compiled in.</param>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public EffectAsset(CompilationContext context, string relativePath)
			: base(context, relativePath)
		{
		}
	}
}