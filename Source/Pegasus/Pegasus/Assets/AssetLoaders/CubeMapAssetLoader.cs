namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     Loads cube maps.
	/// </summary>
	public class CubeMapAssetLoader : TextureAssetLoader
	{
		/// <summary>
		///     Gets the type of the asset supported by the loader.
		/// </summary>
		public override byte AssetType
		{
			get { return (byte)Assets.AssetType.CubeMap; }
		}

		/// <summary>
		///     Gets the name of the asset type supported by the loader.
		/// </summary>
		public override string AssetTypeName
		{
			get { return "Cube Map"; }
		}

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override IDisposable Allocate(GraphicsDevice graphicsDevice, string assetName)
		{
			var cubeMap = new CubeMap(graphicsDevice);
			cubeMap.SetName(assetName);
			return cubeMap;
		}
	}
}