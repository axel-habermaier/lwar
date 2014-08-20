namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     Loads 2-dimensional textures.
	/// </summary>
	public class Texture2DLoader : TextureLoader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Texture2DLoader()
		{
			AssetType = (byte)Assets.AssetType.Texture2D;
			AssetTypeName = "2D Texture";
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