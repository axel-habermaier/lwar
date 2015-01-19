namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes a texture filtering method.
	/// </summary>
	public enum TextureFilter
	{
		/// <summary>
		///     Indicates that the texture filtering method is point filtering.
		/// </summary>
		Nearest,

		/// <summary>
		///     Indicates that the texture filtering method is bilinear filtering.
		/// </summary>
		Bilinear,

		/// <summary>
		///     Indicates that the texture filtering method is trilinear filtering.
		/// </summary>
		Trilinear,

		/// <summary>
		///     Indicates that the texture filtering method is anisotropic filtering.
		/// </summary>
		Anisotropic,

		/// <summary>
		///     Indicates that the texture filtering method is point filtering, without using any mipmap levels other than the most
		///     detailed one.
		/// </summary>
		NearestNoMipmaps,

		/// <summary>
		///     Indicates that the texture filtering method is bilinear filtering, without using any mipmap levels other than the
		///     most detailed one.
		/// </summary>
		BilinearNoMipmaps
	}
}