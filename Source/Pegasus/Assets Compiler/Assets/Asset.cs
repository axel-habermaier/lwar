namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using System.Xml.Linq;
	using Commands;
	using Utilities;

	/// <summary>
	///     Represents a source asset that requires compilation.
	/// </summary>
	public abstract class Asset
	{
		/// <summary>
		///     The base path of the asset.
		/// </summary>
		private readonly string _basePath;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		/// <param name="sourcePath">The metadata attribute the source path should be extracted from.</param>
		/// <param name="basePath">Overrides the default base path of the asset.</param>
		protected Asset(XElement metadata, string sourcePath, string basePath = null)
		{
			Assert.ArgumentNotNullOrWhitespace(sourcePath);
			Assert.ArgumentNotNull(metadata);

			Metadata = metadata;
			SourcePath = GetStringMetadata(sourcePath);
			SourcePath = SourcePath.Replace("\\", "/");
			_basePath = basePath ?? Configuration.BasePath.Replace("\\", "/");

			Directory.CreateDirectory(Path.GetDirectoryName(TempPath));

			if (!File.Exists(AbsoluteSourcePath))
				Log.Die("Asset '{0}' could not be found.", SourcePath);
		}

		/// <summary>
		///     Gets or sets the asset's metadata.
		/// </summary>
		private XElement Metadata { get; set; }

		/// <summary>
		///     Gets the path to the source asset.
		/// </summary>
		public string SourcePath { get; private set; }

		/// <summary>
		///     Gets the absolute path to the source asset.
		/// </summary>
		public string AbsoluteSourcePath
		{
			get { return Path.Combine(_basePath, SourcePath); }
		}

		/// <summary>
		///     Gets the file name the asset, i.e., Tex.png.
		/// </summary>
		public string FileName
		{
			get { return Path.GetFileName(SourcePath); }
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public abstract byte AssetType { get; }

		/// <summary>
		///     Gets the file name the asset excluding the file extension, i.e., Tex.
		/// </summary>
		public string FileNameWithoutExtension
		{
			get { return Path.GetFileNameWithoutExtension(SourcePath); }
		}

		/// <summary>
		///     Gets the relative path to the asset directory in the source directory, i.e. /Textures/.
		/// </summary>
		public string RelativeDirectory
		{
			get { return Path.GetDirectoryName(SourcePath); }
		}

		/// <summary>
		///     Gets the path to the asset relative to the asset source directory without the file extensions, i.e., Textures/Tex.
		/// </summary>
		public string RelativePathWithoutExtension
		{
			get
			{
				var path = Path.Combine(Path.GetDirectoryName(SourcePath), Path.GetFileNameWithoutExtension(SourcePath));
				return path.Replace("\\", "/");
			}
		}

		/// <summary>
		///     Gets the path to the compiled asset file in the temporary directory without the file extension..
		/// </summary>
		private string TempPathWithoutExtension
		{
			get
			{
				var relativeDirectory = RelativeDirectory.Replace("\\", "/").Replace("../", "");
				return Path.Combine(Configuration.TempDirectory, relativeDirectory, FileNameWithoutExtension);
			}
		}

		/// <summary>
		///     Gets the path to the compiled asset file in the temporary directory.
		/// </summary>
		public string TempPath
		{
			get { return TempPathWithoutExtension + "." + RuntimeName + ".tmp"; }
		}

		/// <summary>
		///     Gets the path to the asset's metadata file.
		/// </summary>
		private string MetadataPath
		{
			get { return TempPathWithoutExtension + "." + RuntimeName + ".metadata"; }
		}

		/// <summary>
		///     Gets the runtime type of the asset.
		/// </summary>
		public abstract string RuntimeType { get; }

		/// <summary>
		///     Gets the runtime name of the asset.
		/// </summary>
		public virtual string RuntimeName
		{
			get { return Path.GetFileNameWithoutExtension(FileNameWithoutExtension.Replace(".", "").Replace(" ", "")); }
		}

		/// <summary>
		///     Gets a value indicating whether the file is up-to-date and can be skipped.
		/// </summary>
		public bool RequiresCompilation
		{
			get
			{
				if (!File.Exists(MetadataPath) || !File.Exists(TempPath))
					return true;

				var oldMetadata = File.ReadAllLines(MetadataPath);
				if (oldMetadata.Length < 3)
					return true;

				var oldHash = oldMetadata[0];
				var newHash = ComputeHash();

				if (oldHash != newHash)
					return true;

				var oldVersion = oldMetadata[1];
				var newVersion = Configuration.AssetFileVersion.ToString();

				if (oldVersion != newVersion)
					return true;

				var oldXml = XElement.Parse(String.Join(" ", oldMetadata.Skip(2)));
				var newXml = Metadata;

				if (!XNode.DeepEquals(oldXml, newXml))
					return true;

				return false;
			}
		}

		/// <summary>
		///     Computes a hash for the asset.
		/// </summary>
		public byte[] Hash
		{
			get
			{
				using (var cryptoProvider = new MD5CryptoServiceProvider())
				using (var stream = new FileStream(AbsoluteSourcePath, FileMode.Open, FileAccess.Read))
					return cryptoProvider.ComputeHash(stream);
			}
		}

		/// <summary>
		///     Computes a hash for the asset.
		/// </summary>
		private string ComputeHash()
		{
			return BitConverter.ToString(Hash);
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} '{1}'", GetType().Name, SourcePath);
		}

		/// <summary>
		///     Writes the asset file header into the given buffer.
		/// </summary>
		/// <param name="writer">The writer the asset file header should be written to.</param>
		public void WriteHeader(AssetWriter writer)
		{
			Assert.ArgumentNotNull(writer);
			writer.WriteByte(AssetType);
		}

		/// <summary>
		///     Writes a hash of the source asset to disk.
		/// </summary>
		public void WriteMetadata()
		{
			var metadata = new StringBuilder();
			metadata.AppendLine(ComputeHash());
			metadata.AppendLine(Configuration.AssetFileVersion.ToString());
			metadata.AppendLine(Metadata.ToString());

			File.WriteAllText(MetadataPath, metadata.ToString());
		}

		/// <summary>
		///     Deletes the metadata file, if it exists.
		/// </summary>
		public void DeleteMetadata()
		{
			File.Delete(MetadataPath);
		}

		/// <summary>
		///     Gets the specified metadata value as a string.
		/// </summary>
		/// <param name="metadataName">The name of the metadata.</param>
		protected string GetStringMetadata(string metadataName)
		{
			Assert.ArgumentNotNullOrWhitespace(metadataName);

			var attribute = Metadata.Attribute(metadataName);
			if (attribute == null)
				Log.Die("'{0}' attribute missing for asset '{1}'.", metadataName, SourcePath);

			return attribute.Value;
		}

		/// <summary>
		///     Gets the specified metadata value as a Boolean.
		/// </summary>
		/// <param name="metadataName">The name of the metadata.</param>
		protected bool GetBoolMetadata(string metadataName)
		{
			var value = GetStringMetadata(metadataName);
			if (value == "true")
				return true;

			if (value == "false")
				return false;

			Log.Die("Expected 'true' or 'false' for attribute '{0}' of asset '{1}'.", metadataName, SourcePath);
			return false;
		}

		/// <summary>
		///     Gets the specified metadata value as a Int32.
		/// </summary>
		/// <param name="metadataName">The name of the metadata.</param>
		protected int GetIntMetadata(string metadataName)
		{
			var value = GetStringMetadata(metadataName);
			int intValue;
			if (Int32.TryParse(value, out intValue))
				return intValue;

			Log.Die("Expected an integer for attribute '{0}' of asset '{1}'.", metadataName, SourcePath);
			return 0;
		}
	}
}