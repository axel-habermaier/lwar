namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     Loads cube maps.
	/// </summary>
	public class CubeMapLoader : TextureLoader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public CubeMapLoader()
		{
			AssetType = (byte)Assets.AssetType.CubeMap;
			AssetTypeName = "Cube Map";
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