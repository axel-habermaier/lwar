namespace Pegasus.Platform.Assets
{
	using System;

	/// <summary>
	///     Describes the type of an asset.
	/// </summary>
	public enum AssetType : byte
	{
		/// <summary>
		///     Indicates that the type of the asset is unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		///     Indicates that the asset is a font.
		/// </summary>
		Font = 1,

		/// <summary>
		///     Indicates that the asset is a fragment shader.
		/// </summary>
		FragmentShader = 2,

		/// <summary>
		///     Indicates that the asset is a vertex shader.
		/// </summary>
		VertexShader = 3,

		/// <summary>
		///     Indicates that the asset is a cube map.
		/// </summary>
		CubeMap = 4,

		/// <summary>
		///     Indicates that the asset is a 2D texture.
		/// </summary>
		Texture2D = 5,

		/// <summary>
		///     Indicates that the asset is an effect.
		/// </summary>
		Effect = 6,

		/// <summary>
		///     Indicates that the asset is a Xaml resource.
		/// </summary>
		Xaml = 7,
	}
}