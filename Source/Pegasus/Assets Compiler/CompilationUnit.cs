namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using Assets.Attributes;
	using Compilers;
	using Platform.Logging;

	/// <summary>
	///     Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public class CompilationUnit : IDisposable
	{
		/// <summary>
		///     The list of assets that are compiled by the compilation unit.
		/// </summary>
		private readonly List<Asset> _assets = new List<Asset>();

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			foreach (var asset in _assets)
				asset.Dispose();
		}

		/// <summary>
		///     Loads all assets that should be compiled.
		/// </summary>
		public void LoadAssets()
		{
			var assetNames = GetAssetNames();
			Asset[] assets;

			if (Configuration.XamlFilesOnly)
				assets = CreateAssets(assetNames, new AssetAttribute[0]).OfType<XamlAsset>().ToArray();
			else
			{
				var attributes = Configuration.AssetListAssembly.GetCustomAttributes(false).OfType<AssetAttribute>().ToArray();

				assets = CreateAssets(assetNames, attributes).ToArray();
				assets = ValidateAssets(assets, assetNames).ToArray();
			}

			_assets.AddRange(assets);
		}

		/// <summary>
		///     Searches for all types implementing the given interface or base class and returns an instance of each type.
		/// </summary>
		/// <typeparam name="T">The interface or base class that is implemented by all returned instances.</typeparam>
		private static T[] CreateTypeInstances<T>()
		{
			return Configuration.GetReflectionTypes()
								.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(T)))
								.Select(Activator.CreateInstance)
								.Cast<T>()
								.ToArray();
		}

		/// <summary>
		///     Gets the names of all assets that should be compiled.
		/// </summary>
		private string[] GetAssetNames()
		{
			if (Configuration.XamlFilesOnly)
				return Configuration.AssetsProject.Assets;

			var ignoredAssets = Configuration.AssetListAssembly.GetCustomAttributes(false)
											 .OfType<IgnoreAttribute>()
											 .Select(ignore => ignore.Name);

			return Configuration.AssetsProject.Assets.Except(ignoredAssets).ToArray();
		}

		/// <summary>
		///     Creates all asset instances.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		/// <param name="attributes">The attributes that affect the compilation settings of some assets.</param>
		private static IEnumerable<Asset> CreateAssets(string[] assets, AssetAttribute[] attributes)
		{
			var factories = CreateTypeInstances<IAssetFactory>();

			return from factory in factories
				   from asset in factory.CreateAssets(assets, attributes)
				   select asset;
		}

		/// <summary>
		///     Checks whether compilation settings could be uniquely determined for each asset.
		/// </summary>
		/// <param name="assets">The asset instances.</param>
		/// <param name="assetNames">The asset names.</param>
		private static IEnumerable<Asset> ValidateAssets(Asset[] assets, string[] assetNames)
		{
			var instanceNames = assets.Select(asset => asset.RelativePath);
			foreach (var undeterminedAsset in assetNames.Except(instanceNames))
				Log.Warn("Ignoring asset '{0}': Unable to determine compilation settings.", undeterminedAsset);

			foreach (var group in assets.GroupBy(asset => asset.RelativePath))
			{
				if (group.Count() > 1)
				{
					Log.Warn("Ignoring asset '{0}': Compilation settings are ambiguous: One of {1}.", group.Key,
						String.Join(", ", group.Select(a => a.GetType().Name)));

					foreach (var item in group)
						item.Dispose();
				}
				else
					yield return group.First();
			}
		}

		/// <summary>
		///     Compiles all assets and returns the names of the assets that have been changed. Returns true to indicate that the
		///     compilation of all assets has been successful.
		/// </summary>
		public bool Compile()
		{
			IAssetCompiler[] compilers = null;

			try
			{
				if (Configuration.XamlFilesOnly)
				{
					compilers = new[] { new XamlCompiler() };
					return compilers[0].Compile(_assets);
				}

				compilers = CreateTypeInstances<IAssetCompiler>();
				var success = true;

				foreach (var compiler in compilers)
					success &= compiler.Compile(_assets);

				if (success)
				{
					IEnumerable<Asset> assets = _assets;
					foreach (var compiler in compilers)
						assets = assets.Union(compiler.AdditionalAssets);

					var assetListGenerator = new AssetIdentifierListGenerator();
					assetListGenerator.Generate(assets, Configuration.AssetsProject.RootNamespace);
				}

				return success;
			}
			catch (Exception e)
			{
				Log.Error("{0}", e.Message);
				return false;
			}
			finally
			{
				foreach (var compiler in compilers)
					compiler.Dispose();
			}
		}

		/// <summary>
		///     Removes all compiled assets and temporary files.
		/// </summary>
		public void Clean()
		{
			Log.Info("Cleaning compiled assets and temporary files...");

			IAssetCompiler[] compilers = null;
			try
			{
				if (Configuration.XamlFilesOnly)
				{
					compilers = new[] { new XamlCompiler() };
					compilers[0].Clean(_assets);
					return;
				}

				compilers = CreateTypeInstances<IAssetCompiler>();
				foreach (var compiler in compilers)
					compiler.Clean(_assets);

				var assetListGenerator = new AssetIdentifierListGenerator();
				assetListGenerator.Clean();
			}
			finally
			{
				foreach (var compiler in compilers)
					compiler.Dispose();
			}
		}
	}
}