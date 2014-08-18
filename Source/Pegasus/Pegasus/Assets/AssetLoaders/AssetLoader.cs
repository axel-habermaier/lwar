namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Asset loaders are used by assets managers to load assets from disk.
	/// </summary>
	public abstract class AssetLoader
	{
		/// <summary>
		///     Gets the type of the asset supported by the loader.
		/// </summary>
		public abstract byte AssetType { get; }

		/// <summary>
		///     Gets the name of the asset type supported by the loader.
		/// </summary>
		public abstract string AssetTypeName { get; }

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public abstract void Load(BufferReader buffer, object asset, string assetName);

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		/// <param name="assetName">The name of the asset.</param>
		public abstract IDisposable Allocate(GraphicsDevice graphicsDevice, string assetName);
	}
}