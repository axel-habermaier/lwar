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
		Texture1D,

		/// <summary>
		///     Indicates that a texture is two-dimensional.
		/// </summary>
		Texture2D,

		/// <summary>
		///     Indicates that a texture is a cube map.
		/// </summary>
		CubeMap,

		/// <summary>
		///     Indicates that a texture is three-dimensional.
		/// </summary>
		Texture3D
	}
}