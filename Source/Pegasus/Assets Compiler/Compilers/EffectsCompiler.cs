namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using Assets;
	using Commands;
	using Effects.Compilation;
	using CSharp;
	using Utilities;

	/// <summary>
	///     Compiles effects written in C#.
	/// </summary>
	[UsedImplicitly]
	internal class EffectsCompiler : AssetCompiler<EffectAsset>
	{
		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected override EffectAsset CreateAsset(XElement assetMetadata)
		{
			if (assetMetadata.Name == "Effect")
				return new EffectAsset(assetMetadata);

			return null;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(EffectAsset asset, AssetWriter writer)
		{
			var context = new CompilationContext { Writer = writer, TempPath = asset.TempPath };
			var project = new EffectsProject(new CSharpFile(Configuration.BasePath, asset.SourcePath), context);

			if (!project.Compile())
				throw new InvalidOperationException(String.Format("Compilation of effect '{0}' failed.", asset.SourcePath));

			var effect = project.EffectFiles.Single().Effects.Single();
			asset.SetRuntimeInfo(effect.Name, effect.FullName);
		}
	}
}