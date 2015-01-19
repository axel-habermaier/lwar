namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using Assets;
	using Utilities;

	/// <summary>
	///     Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	/// <typeparam name="TAsset">The type of the asset that is compiled</typeparam>
	public abstract class AssetCompiler<TAsset> : IAssetCompiler
		where TAsset : Asset
	{
		/// <summary>
		///     The assets compiled by the asset compiler.
		/// </summary>
		private readonly List<Asset> _assets = new List<Asset>();

		/// <summary>
		///     Indicates whether the asset compiler supports single threaded compilation only.
		/// </summary>
		private readonly bool _singleThreaded;

		/// <summary>
		///     The object used for thread synchronization.
		/// </summary>
		private readonly object _syncObject = new object();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="singleThreaded">Indicates whether the asset compiler supports single threaded compilation only.</param>
		protected AssetCompiler(bool singleThreaded = false)
		{
			_singleThreaded = singleThreaded;
		}

		/// <summary>
		///     Gets the assets compiled by the asset compiler.
		/// </summary>
		public IEnumerable<Asset> Assets
		{
			get { return _assets; }
		}

		/// <summary>
		///     Compiles the assets.
		/// </summary>
		/// <param name="assets">The metadata of the assets that should be compiled.</param>
		public async Task<bool> Compile(IEnumerable<XElement> assets)
		{
			var compiled = await Task.WhenAll(assets.Select(Compile));
			return compiled.Any(c => c);
		}

		/// <summary>
		///     Cleans the asset if the compiler supports the given asset type.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be cleaned.</param>
		public void Clean(XElement assetMetadata)
		{
			var asset = CreateAsset(assetMetadata);
			if (asset != null)
				asset.DeleteMetadata();
		}

		/// <summary>
		///     Compiles the asset if the compiler supports the given asset type.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		private async Task<bool> Compile(XElement assetMetadata)
		{
			var asset = CreateAsset(assetMetadata);
			if (asset == null)
				return false;

			_assets.Add(asset);

			return await Task.Factory.StartNew(() =>
			{
				if (_singleThreaded)
				{
					lock (_syncObject)
						return CompileAsset(asset);
				}

				return CompileAsset(asset);
			}, TaskCreationOptions.LongRunning);
		}

		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected abstract TAsset CreateAsset(XElement assetMetadata);

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		private bool CompileAsset(TAsset asset)
		{
			if (!asset.RequiresCompilation)
				return false;

			using (var writer = new AssetWriter())
			{
				asset.WriteHeader(writer);
				CompileAndHandleExceptions(asset, writer);
				File.WriteAllBytes(asset.TempPath, writer.ToArray());
			}

			Log.Info("Compiled {1} '{0}'.", asset.RuntimeName, asset.GetType().Name.Substring(0, asset.GetType().Name.Length - 5));

			asset.WriteMetadata();
			return true;
		}

		/// <summary>
		///     Compiles the asset, appending the compiled output to the given writer.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		internal void CompileSingle(TAsset asset, AssetWriter writer)
		{
			CompileAndHandleExceptions(asset, writer);
		}

		/// <summary>
		///     Compiles the asset and handles any exceptions that might occur during the compilation.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		private void CompileAndHandleExceptions(TAsset asset, AssetWriter writer)
		{
			try
			{
				Compile(asset, writer);
			}
			catch (Exception e)
			{
				Log.Error("Compilation of '{0}' failed: ({2}) {1}\n{3}", asset.SourcePath, e.Message, e.GetType().FullName, e.StackTrace);
				asset.DeleteMetadata();
				throw;
			}
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected abstract void Compile(TAsset asset, AssetWriter writer);
	}
}