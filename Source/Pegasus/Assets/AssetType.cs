namespace Pegasus.Assets
{
	using System;
	using Platform.Logging;

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

	/// <summary>
	///     Provides extension methods for the 'AssetType' enumeration.
	/// </summary>
	internal static class AssetTypeExtensions
	{
		/// <summary>
		///     Gets the friendly name of the given asset type.
		/// </summary>
		/// <param name="assetType">The asset type the friendly name should be returned for.</param>
		public static string ToDisplayString(this AssetType assetType)
		{
			switch (assetType)
			{
				case AssetType.Font:
					return "Font";
				case AssetType.FragmentShader:
					return "Fragment Shader";
				case AssetType.VertexShader:
					return "Vertex Shader";
				case AssetType.CubeMap:
					return "Cube Map";
				case AssetType.Texture2D:
					return "2D Texture";
				default:
					Log.Die("Unsupported or unknown asset type.");
					return "Unknown";
			}
		}
	}
}