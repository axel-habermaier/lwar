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
			var csharpAssets = assets.OfType<CSharpAsset>().ToArray();

			if (DetermineAction(csharpAssets) == CompilationAction.Skip)
			{
				Log.Info("Skipping effect compilation (no changes detected).");
				return true;
			}

			Log.Info("Compiling effects...");
			var project = new EffectsProject();
			var success = project.Compile(csharpAssets);

			foreach (var asset in csharpAssets)
				Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);

			return success;
		}

		/// <summary>
		///   Checks whether any of the C# effect assets have changed.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		private static CompilationAction DetermineAction(IEnumerable<Asset> assets)
		{
			var action = CompilationAction.Skip;

			foreach (var asset in assets.OfType<CSharpAsset>())
			{
				if (!File.Exists(asset.HashPath))
					action = CompilationAction.Process;
				else
				{
					var oldHash = Hash.FromFile(asset.HashPath);
					var newHash = Hash.Compute(asset.SourcePath);

					if (oldHash != newHash)
						action = CompilationAction.Process;
				}
			}

			return action;
		}
	}
}