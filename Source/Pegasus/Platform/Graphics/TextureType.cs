namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the type of a texture.
	/// </summary>
	public enum TextureType
	{
		/// <summary>
		///     Indicates that a texture is one-dimensional.
		/// </summary>
		Texture1D = 2901,

		/// <summary>
		///     Indicates that a texture is two-dimensional.
		/// </summary>
		Texture2D = 2902,

		/// <summary>
		///     Indicates that a texture is a cube map.
		/// </summary>
		CubeMap = 2903,

		/// <summary>
		///     Indicates that a texture is three-dimensional.
		/// </summary>
		Texture3D = 2904
	}
}