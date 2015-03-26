namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Xml.Linq;

	/// <summary>
	///     Represents a C# file.
	/// </summary>
	internal class CSharpAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="path">The path of the C# file.</param>
		public CSharpAsset(string path)
			: base(new XElement("CSharpFile", new XAttribute("File", path)), "File", hasTempFile: false)
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