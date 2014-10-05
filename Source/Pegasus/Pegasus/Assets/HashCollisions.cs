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
		private static Dictionary<int, string> _hashes;

		/// <summary>
		///     In debug builds, checks whether a hash collision occurred.
		/// </summary>
		/// <param name="assetName">The name of the asset.</param>
		/// <param name="hash">The hash of the asset.</param>
		[Conditional("DEBUG")]
		public static void Validate(string assetName, int hash)
		{
			if (_hashes == null)
				_hashes = new Dictionary<int, string>();

			if (_hashes.ContainsKey(hash))
				Log.Die("Asset hash collision detected between '{0}' and '{1}'.", assetName, _hashes[hash]);

			_hashes.Add(hash, assetName);
		}
	}
}