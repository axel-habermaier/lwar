using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using Assets;
	using Compilers;
	using Framework;

	/// <summary>
	///   Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public abstract class CompilationUnit : DisposableObject
	{
		/// <summary>
		///   The list of asset compilers that is used to compile the assets.
		/// </summary>
		private static readonly IAssetCompiler[] Compilers;

		/// <summary>
		///   The list of assets that are compiled by the compilation unit.
		/// </summary>
		private readonly List<Asset> _assets = new List<Asset>();

		/// <summary>
		///   Initializes the type.
		/// </summary>
		static CompilationUnit()
		{
			Compilers = Assembly.GetExecutingAssembly()
								.GetTypes()
								.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IAssetCompiler)))
								.Select(Activator.CreateInstance)
								.Cast<IAssetCompiler>()
								.ToArray();
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
		///   Removes the hash files of all assets, as well as their compiled outputs in the temp and target directories.
		/// </summary>
		public void Clean()
		{
			Log.Info("Cleaning compiled assets and temporary files...");

			foreach (var asset in _assets)
			{
				if (File.Exists(asset.TempPath))
					File.Delete(asset.TempPath);

				if (File.Exists(asset.TargetPath))
					File.Delete(asset.TargetPath);

				if (File.Exists(asset.HashPath))
					File.Delete(asset.HashPath);
			}
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <returns></returns>
		internal static CompilationUnit Create()
		{
			var assembly = Assembly.LoadFile(Configuration.AssetListPath);
			var compilationUnit = assembly.GetTypes()
										  .Where(t => t.IsClass && t.BaseType == typeof(CompilationUnit))
										  .Select(Activator.CreateInstance)
										  .OfType<CompilationUnit>()
										  .Single();

			compilationUnit.AddSpecialAssets();
			compilationUnit.AddRemainingAssets();

			return compilationUnit;
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

			foreach (var asset in assets.Where(path => _assets.All(a => a.RelativePath != path)).ToArray())
			{
				if (asset.EndsWith(".png"))
					Add(new Texture2DAsset(asset));
				else if (asset.EndsWith(".vs"))
					Add(new VertexShaderAsset(asset));
				else if (asset.EndsWith(".fs"))
					Add(new FragmentShaderAsset(asset));
				else if (asset.EndsWith(".fnt"))
					Add(new FontAsset(asset));
				else if (asset.EndsWith(".cs"))
					Add(new CSharpAsset(asset));
				else
					Log.Warn("Ignoring asset '{0}': Unable to determine compilation settings.", asset);
			}
		}

		/// <summary>
		///   Adds assets to the compilation unit that require special compilation settings.
		/// </summary>
		protected abstract void AddSpecialAssets();

		/// <summary>
		///   Adds a compiler to the compilation unit.
		/// </summary>
		/// <param name="asset">The compiler that should be added.</param>
		protected void Add(Asset asset)
		{
			Assert.ArgumentNotNull(asset, () => asset);
			Assert.That(_assets.All(a => a.RelativePath != asset.RelativePath), "The asset has already been added.");

			_assets.Add(asset);
		}
	}
}