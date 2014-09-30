namespace Pegasus.Assets
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Platform.Logging;

	/// <summary>
	///     Used to check for asset hash collisions in debug builds.
	/// </summary>
	internal static class HashCollisions 
	{
		/// <summary>
		///     Stores a list of all asset hash codes and the corresponding asset names.
		/// </summary>
		private static Dictionary<int, string> _hashCodes;

		/// <summary>
		///     In debug builds, checks whether a hash collision occurred.
		/// </summary>
		/// <param name="assetName">The name of the asset.</param>
		/// <param name="hashCode">The hash code of the asset.</param>
		[Conditional("DEBUG")]
		public static void Validate(string assetName, int hashCode)
		{
			if (_hashCodes == null)
				_hashCodes = new Dictionary<int, string>();

			if (_hashCodes.ContainsKey(hashCode))
				Log.Die("Asset hash collision detected between '{0}' and '{1}'.", assetName, _hashCodes[hashCode]);

			_hashCodes.Add(hashCode, assetName);
		}
	}
}