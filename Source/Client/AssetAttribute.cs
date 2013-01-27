using System;

namespace Lwar.Client
{
	/// <summary>
	///   The asset attribute is used to indicate to the application which asset should be loaded into a public static field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class AssetAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assetName">The name of the asset that should be loaded.</param>
		public AssetAttribute(string assetName)
		{
			AssetName = assetName;
		}

		/// <summary>
		///   Gets the name of the asset that should be loaded.
		/// </summary>
		public string AssetName { get; private set; }
	}
}