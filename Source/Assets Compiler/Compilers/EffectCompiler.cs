using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;

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
			files.Insert(0, Configuration.AssetListPath);

			ExternalTool.Fsi("ShaderCompiler.fsx", String.Join(" ", files.Select(f => String.Format("\"{0}\"", f))));

			return true;
		}
	}
}