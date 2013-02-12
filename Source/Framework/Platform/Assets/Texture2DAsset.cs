using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a 2D texture asset.
	/// </summary>
	internal sealed class Texture2DAsset : TextureAsset<Texture2D>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Texture2DAsset()
			: base((device, data, width, height, format) => new Texture2D(device, data, width, height, format),
				   (texture, data, width, height, format) => texture.Reinitialize(data, width, height, format))
		{
		}
	}
}