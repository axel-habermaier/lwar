namespace Pegasus.AssetsCompiler.Assets.Attributes
{
	using System;

	/// <summary>
	///   When applied to an asset assembly, overrides the default compilation settings of an asset.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public abstract class AssetAttribute : Attribute
	{
		/// <summary>
		///   The name of the asset.
		/// </summary>
		public string Name { get; protected set; }
	}
}