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
			: base((device, data, width, height, format) => new CubeMap(device, data, width, height, format),
				   (cubeMap, data, width, height, format) => cubeMap.Reinitialize(data, width, height, format))
		{
		}
	}
}