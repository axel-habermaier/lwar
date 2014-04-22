namespace Pegasus.Platform.Assets
{
	using System;
	using Graphics;

	/// <summary>
	///     Represents a cube map asset.
	/// </summary>
	internal sealed class CubeMapAsset : TextureAsset<CubeMap>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public CubeMapAsset()
			: base(() => new CubeMap())
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		internal override AssetType Type
		{
			get { return AssetType.CubeMap; }
		}
	}
}