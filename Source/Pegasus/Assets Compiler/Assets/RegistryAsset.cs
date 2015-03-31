namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.IO;
	using System.Xml.Linq;
	using Commands;

	/// <summary>
	///     Represents a C# registry file.
	/// </summary>
	internal class RegistryAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="path">The path of the registry specification.</param>
		/// <param name="targetNamespace">The target namespace of the generated code.</param>
		/// <param name="import">The optional imported registry.</param>
		public RegistryAsset(string path, string targetNamespace, string import)
			: base(new XElement("Registry",
				new XAttribute("File", path),
				new XAttribute("Namespace", targetNamespace),
				new XAttribute("Import", import ?? "")),
				"File", hasTempFile: false)
		{
			Namespace = targetNamespace;

			if (!String.IsNullOrWhiteSpace(import))
				ImportedRegistry = new RegistryAsset(import, "", "");
		}

		/// <summary>
		///     Gets the namespace of the generated code.
		/// </summary>
		public string Namespace { get; private set; }

		/// <summary>
		///     Gets the absolute path of the generated registry file.
		/// </summary>
		public string GeneratedFilePath
		{
			get { return Path.Combine(Configuration.BasePath, RelativeDirectory, FileName.StartsWith("I") ? FileName.Substring(1) : FileName); }
		}

		/// <summary>
		///     Gets the imported registry, if any.
		/// </summary>
		public RegistryAsset ImportedRegistry { get; private set; }

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