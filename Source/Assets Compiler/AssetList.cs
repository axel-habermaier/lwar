using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using Framework;

	/// <summary>
	///   Manages the list of assets that should be compiled.
	/// </summary>
	public abstract class AssetList
	{
		/// <summary>
		///   The assets that should be compiled.
		/// </summary>
		private Asset[] _assets;

		/// <summary>
		///   Gets the assets that should be compiled.
		/// </summary>
		public IEnumerable<Asset> Assets
		{
			get
			{
				if (_assets == null)
				{
					var root = XDocument.Load(Configuration.AssetsProject).Root;
					XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

					_assets = root.Descendants(ns + "None")
								  .Union(root.Descendants(ns + "Content"))
								  .Select(element => element.Attribute("Include").Value)
								  .Where(asset => !asset.EndsWith(".tt"))
								  .Select(asset => new Asset(asset.Replace("\\", "/")))
								  .ToArray();
					OverrideProcessors();
				}

				return _assets;
			}
		}

		/// <summary>
		///   Allows overriding the default asset processors of the assets.
		/// </summary>
		protected abstract void OverrideProcessors();

		/// <summary>
		///   Overrides the default processor of the asset.
		/// </summary>
		/// <param name="assetPath">The path of the asset whose default processor should be overridden.</param>
		/// <param name="processor">The processor that should replace the default one.</param>
		protected void OverrideProcessor(string assetPath, AssetProcessor processor)
		{
			Assert.ArgumentNotNullOrWhitespace(assetPath, () => assetPath);
			Assert.ArgumentNotNull(processor, () => processor);

			var asset = _assets.SingleOrDefault(a => a.RelativePath == assetPath);
			if (asset == null)
				Log.Warn("Attempted to override default processor of unknown asset '{0}'.", assetPath);
			else
				asset.Processor = processor;
		}
	}
}