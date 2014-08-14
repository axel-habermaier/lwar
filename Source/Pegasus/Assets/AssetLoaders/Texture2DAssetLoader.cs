namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     Loads 2-dimensional textures.
	/// </summary>
	public class Texture2DAssetLoader : TextureAssetLoader
	{
		/// <summary>
		///     Gets the type of the asset supported by the loader.
		/// </summary>
		public override byte AssetType
		{
			get { return (byte)Assets.AssetType.Texture2D; }
		}

		/// <summary>
		///     Gets the name of the asset type supported by the loader.
		/// </summary>
		public override string AssetTypeName
		{
			get { return "2D Texture"; }
		}

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override IDisposable Allocate(GraphicsDevice graphicsDevice, string assetName)
		{
			var texture = new Texture2D(graphicsDevice);
			texture.SetName(assetName);
			return texture;
		}
	}
}