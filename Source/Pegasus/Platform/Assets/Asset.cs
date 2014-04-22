namespace Pegasus.Platform.Assets
{
	using System;
	using Graphics;
	using Memory;

	/// <summary>
	///     Represents a compiled asset that can be loaded and reloaded.
	/// </summary>
	internal abstract class Asset : DisposableObject
	{
		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		internal abstract AssetType Type { get; }

		/// <summary>
		///     The asset manager that manages this asset.
		/// </summary>
		internal AssetsManager Assets { get; set; }

		/// <summary>
		///     Loads or reloads the asset from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		/// <param name="name">The name of the asset.</param>
		internal abstract void Load(BufferReader buffer, string name);
	}
}