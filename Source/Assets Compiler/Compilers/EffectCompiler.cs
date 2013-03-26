using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Assets;
	using ShaderCompilation;
	using ShaderCompilation.MetaModel;

	/// <summary>
	///   Cross-compiles shaders written in C# to HLSL and GLSL.
	/// </summary>
	internal class EffectCompiler : IAssetCompiler
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The asset that should be compiled.</param>
		public bool Compile(IEnumerable<Asset> assets)
		{
			var files = assets.OfType<CSharpAsset>().Select(cs => cs.SourcePath).ToList();

			var effects = Configuration.AssetListAssembly
									   .GetTypes()
									   .Select(t => t.GetTypeInfo())
									   .Where(t => t.GetCustomAttribute<EffectAttribute>() != null)
									   .Select(t => new EffectClass(t))
									   .ToArray();

			return true;
		}
	}
}