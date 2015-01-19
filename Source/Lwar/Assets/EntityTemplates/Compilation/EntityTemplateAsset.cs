namespace Lwar.Assets.EntityTemplates.Compilation
{
	using System;
	using System.Xml.Linq;
	using Pegasus.AssetsCompiler.Assets;

	/// <summary>
	///     Represents an entity template.
	/// </summary>
	internal class EntityTemplateAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public EntityTemplateAsset(XElement metadata)
			: base(metadata, "File")
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { throw new NotSupportedException(); }
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