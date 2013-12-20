namespace Pegasus.Platform.Assets
{
	using System;
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
			: base(device => new Texture2D(device))
		{
		}

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "2D Texture"; }
		}
	}
}