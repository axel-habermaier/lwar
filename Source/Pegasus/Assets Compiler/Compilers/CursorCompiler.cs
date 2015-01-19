namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Xml.Linq;
	using Assets;
	using Utilities;

	/// <summary>
	///     Compiles cursor textures.
	/// </summary>
	[UsedImplicitly]
	internal sealed class CursorCompiler : AssetCompiler<CursorAsset>
	{
		/// <summary>
		///     Creates an asset instance for the given XML element or returns null if the type of the asset is not
		///     supported by the compiler.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be compiled.</param>
		protected override CursorAsset CreateAsset(XElement assetMetadata)
		{
			if (assetMetadata.Name == "Cursor")
				return new CursorAsset(assetMetadata);

			return null;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected override void Compile(CursorAsset asset, AssetWriter writer)
		{
			var metadata = new XElement("Texture2D",
				new XAttribute("File", asset.SourcePath),
				new XAttribute("Compress", "false"),
				new XAttribute("GenerateMipmaps", "false"));

			var compiler = new Texture2DCompiler();
			compiler.CompileSingle(new Texture2DAsset(metadata), writer);

			writer.WriteInt16((short)asset.HotSpotX);
			writer.WriteInt16((short)asset.HotSpotY);
		}
	}
}