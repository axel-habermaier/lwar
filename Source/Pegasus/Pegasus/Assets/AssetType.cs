namespace Pegasus.Assets
{
	using System;

	/// <summary>
	///     Describes the type of an asset supported by the framework.
	/// </summary>
	public enum AssetType : byte
	{
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
	}
}