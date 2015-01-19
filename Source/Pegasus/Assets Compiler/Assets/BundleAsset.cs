namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.IO;
	using System.Xml.Linq;
	using Commands;

	/// <summary>
	///     Represents a compiled asset bundle.
	/// </summary>
	internal class BundleAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public BundleAsset(XElement metadata)
			: base(metadata, "File")
		{
			Namespace = GetStringMetadata("Namespace");
			EffectCodePath = GetStringMetadata("EffectsCodePath");
			BundleCodePath = GetStringMetadata("BundleCodePath");
			Visibility = GetStringMetadata("Visibility");
		}

		/// <summary>
		///     Gets the path of a directory where generated C# effect code should be stored.
		/// </summary>
		public string EffectCodePath { get; set; }

		/// <summary>
		///     Gets the path of a directory where generated C# bundle code should be stored.
		/// </summary>
		public string BundleCodePath { get; set; }

		/// <summary>
		///     Gets the namespace the generated asset bundle C# could should live in.
		/// </summary>
		public string Namespace { get; private set; }

		/// <summary>
		///     Gets the class name of the bundle.
		/// </summary>
		public string ClassName
		{
			get { return Name + "Bundle"; }
		}

		/// <summary>
		///     Gets or sets the default visibility of generated C# classes.
		/// </summary>
		public string Visibility { get; private set; }

		/// <summary>
		///     Gets the name of the compiled bundle file.
		/// </summary>
		public string OutputFileName
		{
			get { return Name + Configuration.BundleFileExtension; }
		}

		/// <summary>
		///     Gets the path of the bundle code files without the file extension.
		/// </summary>
		public string CodeFilePath
		{
			get { return Path.Combine(Configuration.BundleCodePath, Name + "Bundle.cs"); }
		}

		/// <summary>
		///     Gets the name of the bundle.
		/// </summary>
		public string Name
		{
			get
			{
				if (FileNameWithoutExtension.EndsWith("Bundle"))
					return FileNameWithoutExtension.Substring(0, FileNameWithoutExtension.Length - "Bundle".Length);

				return FileNameWithoutExtension;
			}
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