using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a cube map asset.
	/// </summary>
	internal sealed class CubeMapAsset : TextureAsset<CubeMap>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public CubeMapAsset()
			: base((device, format, mipmaps) => new CubeMap(device, format, mipmaps))
		{
		}

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "Cube Map"; }
		}
	}
}