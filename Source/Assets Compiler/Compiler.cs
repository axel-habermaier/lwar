using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Framework;
	using Framework.Platform.Assets;

	/// <summary>
	///   Compiles assets (textures, fonts, sounds, etc.) into an efficient byte format.
	/// </summary>
	public sealed class Compiler : IAssetsCompiler
	{
		/// <summary>
		///   Gets the list of assets that should be compiled.
		/// </summary>
		private static IEnumerable<Asset> Assets
		{
			get
			{
				var assembly = Assembly.LoadFile(Configuration.AssetListPath);
				return assembly.GetTypes()
							   .Where(t => t.IsClass && t.BaseType == typeof(AssetList))
							   .Select(Activator.CreateInstance)
							   .OfType<AssetList>()
							   .Single()
							   .Assets;
			}
		}

		/// <summary>
		///   Compiles all assets and returns the names of the assets that have been changed.
		/// </summary>
		public IEnumerable<string> Compile()
		{
			var compiledAssets = new List<string>();

			try
			{
				var grouped = Assets.GroupBy(asset => asset.Processor.GetType());
				foreach (var group in grouped)
				{
					var first = group.First();
					Log.Info("Processing {0}...", first.Processor.AssetType);

					foreach (var asset in group)
					{
						if (Process(asset))
							compiledAssets.Add(asset.RelativePathWithoutExtension);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}

			return compiledAssets;
		}

		/// <summary>
		///   Uses the given processor to process the given asset.
		/// </summary>
		/// <param name="asset">The asset that should be processed.</param>
		private bool Process(Asset asset)
		{
			if (!asset.RequiresCompilation)
			{
				Log.Info("   Skipping '{0}' (no changes detected).", asset.RelativePath);
				return false;
			}

			Log.Info("   Compiling '{0}'...", asset.RelativePath);
			asset.UpdateHashFile();
			using (var writer = new AssetWriter(asset.TargetPath))
				asset.Processor.Process(asset, writer.Writer);

			return true;
		}
	}
}