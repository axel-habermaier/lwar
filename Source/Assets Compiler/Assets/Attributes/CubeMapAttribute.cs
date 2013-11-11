namespace Pegasus.AssetsCompiler.Assets.Attributes
{
	using System;

	/// <summary>
	///   When applied to an asset assembly, overrides the default compilation settings of a CubeMap asset.
	/// </summary>
	public class CubeMapAttribute : AssetAttribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the asset.</param>
		public CubeMapAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			Name = name;
			Mipmaps = true;
		}

		/// <summary>
		///   Gets or sets a value indicating whether mipmaps should be generated for the texture. Default is true.
		/// </summary>
		public bool Mipmaps { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether the texture should not use texture compression. Default is false.
		/// </summary>
		public bool Uncompressed { get; set; }
	}
}