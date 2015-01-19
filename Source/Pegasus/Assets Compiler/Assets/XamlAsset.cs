namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Xml.Linq;

	/// <summary>
	///     Represents a Xaml file.
	/// </summary>
	internal class XamlAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public XamlAsset(XElement metadata)
			: base(metadata, "File")
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return 0; }
		}

		/// <summary>
		///     Gets the runtime type of the asset.
		/// </summary>
		public override string RuntimeType
		{
			get { throw new NotSupportedException(); }
		}
	}
}