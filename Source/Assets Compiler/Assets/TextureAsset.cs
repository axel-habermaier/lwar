using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a texture asset that requires compilation.
	/// </summary>
	public abstract class TextureAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		protected TextureAsset(string relativePath)
			: base(relativePath)
		{
			Mipmaps = true;
			Uncompressed = false;
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