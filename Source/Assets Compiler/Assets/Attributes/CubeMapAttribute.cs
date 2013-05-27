using System;

namespace Pegasus.AssetsCompiler.Assets.Attributes
{
	using Framework;

	/// <summary>
	///   When applied to an asset assembly, overrides the default compilation settings of an asset.
	/// </summary>
	public class CubeMapAttribute : AssetAttribute
	{
		/// <summary>
		///   The name of the asset.
		/// </summary>
		private readonly string _name;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the asset.</param>
		public CubeMapAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			_name = name;
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

		/// <summary>
		///   Gets the asset that should be compiled.
		/// </summary>
		internal override Asset Asset
		{
			get { return new CubeMapAsset(_name) { Mipmaps = Mipmaps, Uncompressed = Uncompressed }; }
		}
	}
}