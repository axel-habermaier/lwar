using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Represents a font asset that requires compilation.
	/// </summary>
	public class FontAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public FontAsset(string relativePath)
			: base(relativePath)
		{
		}

		/// <summary>
		///   The identifier type that should be used for the asset when generating the asset identifier list. If null is returned,
		///   no asset identifier is generated for this asset instance.
		/// </summary>
		public override string IdentifierType
		{
			get { return "Pegasus.Rendering.UserInterface.Font"; }
		}

		/// <summary>
		///   The name that should be used for the asset identifier. If null is returned, no asset identifier is generated for this
		///   asset instance.
		/// </summary>
		public override string IdentifierName
		{
			get { return FileNameWithoutExtension; }
		}
	}
}