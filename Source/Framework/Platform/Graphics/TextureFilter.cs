using System;

namespace Pegasus.Framework.Platform.Graphics
{
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
		Anisotropic = 2304
	}
}