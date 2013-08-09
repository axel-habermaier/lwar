using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using Assets;
	using Assets.Attributes;
	using Compilers;
	using Framework;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;

	/// <summary>
	///   Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public class CompilationUnit : DisposableObject
	{
		/// <summary>
		///   The list of asset compilers that is used to compile the assets.
		/// </summary>
		private static readonly IAssetCompiler[] Compilers;

		/// <summary>
		///   The list of asset factories that is used to find and create the compiled assets.
		/// </summary>
		private static readonly IAssetFactory[] Factories;

		/// <summary>
		///   The list of assets that are compiled by the compilation unit.
		/// </summary>
		private readonly List<Asset> _assets = new List<Asset>();

		/// <summary>
		///   Initializes the type.
		/// </summary>
		static CompilationUnit()
		{
			var assetCompilerTypes = Assembly.GetExecutingAssembly().GetTypes();
			var assetListTypes = Configuration.AssetListAssembly.GetTypes();

			Compilers = assetListTypes.Union(assetCompilerTypes)
									  .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IAssetCompiler)))
									  .Select(Activator.CreateInstance)
									  .Cast<IAssetCompiler>()
									  .ToArray();

			Factories = assetListTypes.Union(assetCompilerTypes)
									  .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IAssetFactory)))
									  .Select(Activator.CreateInstance)
									  .Cast<IAssetFactory>()
									  .ToArray();
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public CompilationUnit()
		{
			AddSpecialAssets();
			AddRemainingAssets();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_assets.SafeDisposeAll();
			Compilers.SafeDisposeAll();
		}

		/// <summary>
		///   Compiles all assets and returns the names of the assets that have been changed. Returns true to indicate that the
		///   compilation of all assets has been successful.
		/// </summary>
		public bool Compile()
		{
			try
			{
				var success = true;

				foreach (var compiler in Compilers)
					success &= compiler.Compile(_assets);

				return success;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return false;
			}
		}

		/// <summary>
		///   Removes all compiled assets and temporary files.
		/// </summary>
		public void Clean()
		{
			Log.Info("Cleaning compiled assets and temporary files...");

			foreach (var compiler in Compilers)
				compiler.Clean(_assets);
		}

		/// <summary>
		///   Adds the remaining assets to the compilation unit that do not require any special compilation settings.
		/// </summary>
		private void AddRemainingAssets()
		{
			var root = XDocument.Load(Configuration.AssetsProject).Root;
			XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

			var assets = root.Descendants(ns + "None")
							 .Union(root.Descendants(ns + "Content"))
							 .Union(root.Descendants(ns + "Compile"))
							 .Select(element => element.Attribute("Include").Value)
							 .Select(asset => asset.Replace("\\", "/"));

			var ignoredAssets = Configuration.AssetListAssembly.GetCustomAttributes(false)
											 .OfType<IgnoreAttribute>()
											 .Select(ignore => ignore.Name);
			assets = assets.Where(path => _assets.All(a => a.RelativePath != path));
			assets = assets.Except(ignoredAssets);

			foreach (var asset in assets)
			{
				var assetObjs = Factories.Select(f => f.CreateAsset(asset)).Where(a => a != null).ToArray();
				if (assetObjs.Length == 0)
					Log.Warn("Ignoring asset '{0}': Unable to determine compilation settings.", asset);
				else if (assetObjs.Length > 1)
					Log.Warn("Ignoring asset '{0}': Asset type is ambiguous: One of {1}.", asset, 
							 String.Join(", ", assetObjs.Select(a => a.GetType().Name)));
				else
					Add(assetObjs.Single());
			}
		}

		/// <summary>
		///   Adds assets to the compilation unit that require special compilation settings.
		/// </summary>
		private void AddSpecialAssets()
		{
			var assets = Configuration.AssetListAssembly.GetCustomAttributes(false).OfType<AssetAttribute>();

			foreach (var asset in assets)
				Add(asset.Asset);
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