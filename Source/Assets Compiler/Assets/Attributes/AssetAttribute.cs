using System;

namespace Pegasus.AssetsCompiler.Assets.Attributes
{
	/// <summary>
	///   When applied to an asset assembly, overrides the default compilation settings of an asset.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public abstract class AssetAttribute : Attribute
	{
		/// <summary>
		///   Gets the asset that should be compiled.
		/// </summary>
		internal abstract Asset Asset { get; }
	}
}