using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a compiled asset that can be loaded and reloaded.
	/// </summary>
	internal abstract class CompiledAsset : DisposableObject
	{
		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal abstract string FriendlyName { get; }

		/// <summary>
		///   The asset manager that manages this asset.
		/// </summary>
		internal AssetsManager Assets { get; set; }

		/// <summary>
		///   The graphics device for which the assets are loaded.
		/// </summary>
		internal GraphicsDevice GraphicsDevice { get; set; }

		/// <summary>
		///   Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the asset.</param>
		internal abstract void Load(AssetReader assetReader);
	}
}