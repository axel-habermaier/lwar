using System;

namespace Pegasus.Platform.Assets
{
	using System.Collections.Generic;
	using Logging;

	/// <summary>
	///     Used to check for asset hash collisions in debug builds.
	/// </summary>
	internal static class AssetHashCollision
	{
		/// <summary>
		///     Stores a list of all asset hash codes and the corresponding asset names.
		/// </summary>
		private static readonly Dictionary<int, string> HashCodes = new Dictionary<int, string>();

		/// <summary>
		///     Checks whether a hash collision occurred.
		/// </summary>
		/// <param name="assetName">The name of the asset.</param>
		/// <param name="hashCode">The hash code of the asset.</param>
		public static void Validate(string assetName, int hashCode)
		{
			if (HashCodes.ContainsKey(hashCode))
				Log.Die("Asset hash collision detected between '{0}' and '{1}'.", assetName, HashCodes[hashCode]);

			HashCodes.Add(hashCode, assetName);
		}
	}
}