using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using Effects.Compilation;
	using Framework;

	/// <summary>
	///   Cross-compiles shaders written in C# to HLSL and GLSL.
	/// </summary>
	internal class EffectCompiler : IAssetCompiler
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public bool Compile(IEnumerable<Asset> assets)
		{
			Log.Info("Cross-compiling effects to GLSL and HLSL...");

			var project = new EffectsProject(assets.OfType<CSharpAsset>());
			return project.Compile();
		}
	}
}