using System;

namespace Pegasus.Framework.Platform.Assets.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;

	/// <summary>
	///   Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public abstract class CompilationUnit
	{
		/// <summary>
		///   The list of items that are compiled by the compilation unit.
		/// </summary>
		private readonly List<AssetCompiler> _compilers = new List<AssetCompiler>();

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
		///   Compiles all assets and returns the names of the assets that have been changed.
		/// </summary>
		internal IEnumerable<string> Compile()
		{
			var compiledAssets = new List<string>();

			try
			{
				var grouped = _compilers.GroupBy(compiler => compiler.GetType());
				foreach (var group in grouped)
				{
					var first = group.First();
					Log.Info("Processing {0}...", first.AssetType);

					foreach (var compiler in group)
					{
						if (compiler.Compile())
							compiledAssets.Add(compiler.Asset.RelativePathWithoutExtension);
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

			foreach (var asset in assets.Except(_compilers.Select(c => c.Asset.RelativePath)).ToArray())
			{
				if (_compilers.Any(c => c.Asset.RelativePath == asset))
					continue;

				if (asset.EndsWith(".png"))
					Add(new Texture2DCompiler(asset));
				else if (asset.EndsWith(".vs"))
					Add(new VertexShaderCompiler(asset));
				else if (asset.EndsWith(".fs"))
					Add(new FragmentShaderCompiler(asset));
				else if (asset.EndsWith(".fnt"))
					Add(new FontCompiler(asset));
				else
					Log.Error("Ignoring asset '{0}': Unable to determine compilation settings.", asset);
			}
		}

		/// <summary>
		///   Adds assets to the compilation unit that require special compilation settings.
		/// </summary>
		protected abstract void AddSpecialAssets();

		/// <summary>
		///   Adds a compiler to the compilation unit.
		/// </summary>
		/// <param name="compiler">The compiler that should be added.</param>
		protected void Add(AssetCompiler compiler)
		{
			Assert.ArgumentNotNull(compiler, () => compiler);

			var oldSettings = _compilers.SingleOrDefault(c => c.Asset.RelativePath == compiler.Asset.RelativePath);
			if (oldSettings != null)
			{
				Log.Warn("Overriding compilation settings for asset '{0}'.", compiler.Asset.RelativePath);
				_compilers.Remove(oldSettings);
			}

			_compilers.Add(compiler);
		}
	}
}