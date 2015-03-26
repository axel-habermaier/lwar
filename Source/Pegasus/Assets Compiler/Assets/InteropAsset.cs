namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Xml.Linq;

	/// <summary>
	///     Represents a C# interop file.
	/// </summary>
	internal class InteropAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="path">The path of the interop file.</param>
		public InteropAsset(string path)
			: base(new XElement("Interop", new XAttribute("File", path)), "File", hasTempFile: false)
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