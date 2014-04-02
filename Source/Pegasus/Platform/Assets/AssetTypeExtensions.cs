using System;

namespace Pegasus.Platform.Assets
{
	using Logging;

	/// <summary>
	/// Provides extension methods for the 'AssetType' enumeration.
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