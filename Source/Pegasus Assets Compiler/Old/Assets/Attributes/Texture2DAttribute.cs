namespace Pegasus.AssetsCompiler.Assets.Attributes
{
	using System;

	/// <summary>
	///     When applied to an asset assembly, overrides the default compilation settings of a Texture2D asset.
	/// </summary>
	public class Texture2DAttribute : AssetAttribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the asset.</param>
		public Texture2DAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			Name = name;
			Mipmaps = true;
		}

		/// <summary>
		///     Gets or sets a value indicating whether mipmaps should be generated for the texture. Default is true.
		/// </summary>
		public bool Mipmaps { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the texture should not use texture compression. Default is false.
		/// </summary>
		public bool Uncompressed { get; set; }
	}
}