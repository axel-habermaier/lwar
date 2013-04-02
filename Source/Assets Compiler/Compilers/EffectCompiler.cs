using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Effects.Compilation;
	using Framework;

	/// <summary>
	///   Compiles effects written in C#.
	/// </summary>
	[UsedImplicitly]
	public class EffectCompiler : IAssetCompiler
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type. Returns true to indicate that the compilation of all assets
		///   has been successful.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public bool Compile(IEnumerable<Asset> assets)
		{
			var project = new EffectsProject();
			var success = project.Compile(GetChangedAssets(assets).ToArray());
			var shaders = project.ShaderAssets.ToArray();

			var vertexShaderCompiler = new VertexShaderCompiler();
			var fragmentShaderCompiler = new FragmentShaderCompiler();

			try
			{
				success &= vertexShaderCompiler.Compile(shaders);
				success &= fragmentShaderCompiler.Compile(shaders);
			}
			finally
			{
				shaders.SafeDisposeAll();
			}

			return success;
		}

		/// <summary>
		/// Gets the C# assets that have changed and require compilation.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		private IEnumerable<CSharpAsset> GetChangedAssets(IEnumerable<Asset> assets)
		{
			foreach (var asset in assets.OfType<CSharpAsset>())
			{
				var action = CompilationAction.Process;
				if (File.Exists(asset.HashPath))
				{
					var oldHash = Hash.FromFile(asset.HashPath);
					var newHash = Hash.Compute(asset.SourcePath);

					if (oldHash == newHash)
						action = CompilationAction.Skip;
				}

				action.Describe(asset);

				if (action == CompilationAction.Process)
				{
					Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);
					yield return asset;
				}
			}
		}
	}
}