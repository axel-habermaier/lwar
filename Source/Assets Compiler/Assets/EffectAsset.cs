namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using Platform.Assets;

	/// <summary>
	///     Represents a C# shader effect file that requires compilation.
	/// </summary>
	public class EffectAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public EffectAsset(string relativePath)
			: base(relativePath)
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override AssetType AssetType
		{
			get { return AssetType.Effect; }
		}
	}
}