using System;

namespace Pegasus.Platform.Assets
{
	using Graphics;
	using Memory;

	/// <summary>
	///   Represents a compiled asset that can be loaded and reloaded.
	/// </summary>
	internal abstract class Asset : DisposableObject
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
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		/// <param name="name">The name of the asset.</param>
		internal abstract void Load(BufferReader buffer, string name);
	}
}