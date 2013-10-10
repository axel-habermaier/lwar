﻿using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using Assets.Attributes;
	using CodeGeneration;
	using Compilers;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public class CompilationUnit : DisposableObject
	{
		/// <summary>
		///   The list of assets that are compiled by the compilation unit.
		/// </summary>
		private readonly List<Asset> _assets = new List<Asset>();

		/// <summary>
		///   Loads all assets that should be compiled.
		/// </summary>
		public void LoadAssets()
		{
			var assetNames = GetAssetNames();
			var attributes = Configuration.AssetListAssembly.GetCustomAttributes(false).OfType<AssetAttribute>().ToArray();

			var assets = CreateAssets(assetNames, attributes).ToArray();
			if (Configuration.XamlFilesOnly)
				assets = assets.OfType<XamlAsset>().ToArray();
			else
				assets = ValidateAssets(assets, assetNames).ToArray();

			_assets.AddRange(assets);
		}

		/// <summary>
		///   Searches for all types implementing the given interface or base class and returns an instance of each type.
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
		///   Gets the names of all assets that should be compiled.
		/// </summary>
		private string[] GetAssetNames()
		{
			var ignoredAssets = Configuration.AssetListAssembly.GetCustomAttributes(false)
											 .OfType<IgnoreAttribute>()
											 .Select(ignore => ignore.Name);

			return Configuration.AssetsProject.Assets.Except(ignoredAssets).ToArray();
		}

		/// <summary>
		///   Creates all asset instances.
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
		///   Checks whether compilation settings could be uniquely determined for each asset.
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

					group.SafeDisposeAll();
				}
				else
					yield return group.First();
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_assets.SafeDisposeAll();
		}

		/// <summary>
		///   Compiles all assets and returns the names of the assets that have been changed. Returns true to indicate that the
		///   compilation of all assets has been successful.
		/// </summary>
		public bool Compile()
		{
				var compilers = CreateTypeInstances<IAssetCompiler>();

			try
			{
				if (Configuration.XamlFilesOnly)
					return compilers.OfType<XamlCompiler>().Single().Compile(_assets);

				var success = true;

				foreach (var compiler in compilers)
					success &= compiler.Compile(_assets);

				if (success)
				{
					var assetListGenerator = new AssetIdentifierListGenerator(_assets);
					assetListGenerator.Generate(Configuration.AssetsProject.RootNamespace);
				}

				return success;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return false;
			}
			finally
			{
				compilers.SafeDisposeAll();
			}
		}

		/// <summary>
		///   Removes all compiled assets and temporary files.
		/// </summary>
		public void Clean()
		{
			Log.Info("Cleaning compiled assets and temporary files...");

			var compilers = CreateTypeInstances<IAssetCompiler>();
			try
			{
				foreach (var compiler in compilers)
					compiler.Clean(_assets);
			}
			finally
			{
				compilers.SafeDisposeAll();
			}
		}

		/// <summary>
		///   Adds a compiler to the compilation unit.
		/// </summary>
		/// <param name="asset">The compiler that should be added.</param>
		private void Add(Asset asset)
		{
			Assert.ArgumentNotNull(asset);
			Assert.That(_assets.All(a => a.RelativePath != asset.RelativePath), "The asset has already been added.");

			_assets.Add(asset);
		}
	}
}