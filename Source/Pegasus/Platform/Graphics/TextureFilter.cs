namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Describes a texture filtering method.
	/// </summary>
	public enum TextureFilter
	{
		/// <summary>
		///   Indicates that the texture filtering method is point filtering.
		/// </summary>
		Nearest = 2301,

		/// <summary>
		///   Indicates that the texture filtering method is bilinear filtering.
		/// </summary>
		Bilinear = 2302,

		/// <summary>
		///   Indicates that the texture filtering method is trilinear filtering.
		/// </summary>
		Trilinear = 2303,

		/// <summary>
		///   Indicates that the texture filtering method is anisotropic filtering.
		/// </summary>
		Anisotropic = 2304,

		/// <summary>
		///   Indicates that the texture filtering method is point filtering, without using any mipmap levels other than the most
		///   detailed one.
		/// </summary>
		NearestNoMipmaps = 2305,

		/// <summary>
		///   Indicates that the texture filtering method is bilinear filtering, without using any mipmap levels other than the
		///   most detailed one.
		/// </summary>
		BilinearNoMipmaps = 2306,
	}
}