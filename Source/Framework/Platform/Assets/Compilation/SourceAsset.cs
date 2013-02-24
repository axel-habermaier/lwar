using System;

namespace Pegasus.Framework.Platform.Assets.Compilation
{
	using System.IO;

	/// <summary>
	///   Represents a source asset that requires compilation.
	/// </summary>
	internal struct SourceAsset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public SourceAsset(string relativePath)
			: this()
		{
			Assert.ArgumentNotNullOrWhitespace(relativePath, () => relativePath);
			RelativePath = relativePath;
		}

		/// <summary>
		///   Gets the path to the asset relative to the asset source directory, i.e., Textures/Tex.png.
		/// </summary>
		public string RelativePath { get; private set; }

		/// <summary>
		///   Gets the path to the asset relative to the asset source directory without the file extensions, i.e., Textures/Tex.
		/// </summary>
		public string RelativePathWithoutExtension
		{
			get
			{
				var path = Path.Combine(Path.GetDirectoryName(RelativePath), Path.GetFileNameWithoutExtension(RelativePath));
				return path.Replace("\\", "/");
			}
		}

		/// <summary>
		///   Gets the absolute path to the asset in the source directory, i.e. C:/AssetsSources/Textures/Tex.png.
		/// </summary>
		public string SourcePath
		{
			get { return Path.Combine(Configuration.SourceDirectory, RelativePath); }
		}

		/// <summary>
		///   Gets the absolute path to the asset in the target directory, i.e. C:/Binaries/Assets/Textures/Tex.{ext}.
		/// </summary>
		public string TargetPath
		{
			get { return Path.Combine(Configuration.TargetDirectory, RelativePathWithoutExtension) + PlatformInfo.AssetExtension; }
		}

		/// <summary>
		///   Gets the absolute path to the compiled asset in the temp directory, i.e.
		///   C:/AssetsSources/obj/Assets/Textures/Tex.{ext}.
		/// </summary>
		public string TempPath
		{
			get { return TempPathWithoutExtension + PlatformInfo.AssetExtension; }
		}

		/// <summary>
		///   Gets the absolute path to the asset in the temp directory without the file extension, i.e.
		///   C:/AssetsSources/obj/Assets/Textures/Tex.
		/// </summary>
		public string TempPathWithoutExtension
		{
			get { return Path.Combine(Configuration.TempDirectory, RelativePathWithoutExtension); }
		}

		/// <summary>
		///   Gets the path to the asset hash file.
		/// </summary>
		public string HashPath
		{
			get { return TempPathWithoutExtension + ".hash"; }
		}
	}
}