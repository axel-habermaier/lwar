using System;

namespace Lwar.Assets.Templates.Compilation
{
	using System.Globalization;
	using Pegasus.AssetsCompiler.Assets;

	/// <summary>
	///   Creates template asset instances.
	/// </summary>
	public class TemplateAssetFactory : IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for the asset with the given name. If the asset type is not supported, null must be
		///   returned.
		/// </summary>
		/// <param name="assetName">The name of the asset that should be created.</param>
		public Asset CreateAsset(string assetName)
		{
			if (assetName.EndsWith(".Templates.cs", ignoreCase: true, culture: CultureInfo.InvariantCulture))
				return new TemplateAsset(assetName);

			return null;
		}
	}
}