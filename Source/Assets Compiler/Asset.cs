using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using System.Security.Cryptography;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Represents an asset that is compiled.
	/// </summary>
	public class Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public Asset(string relativePath)
		{
			Assert.ArgumentNotNullOrWhitespace(relativePath, () => relativePath);

			RelativePath = relativePath;
			EnsurePathsExist(Path.GetDirectoryName(TargetPath));
			EnsurePathsExist(Path.GetDirectoryName(TempPath));

			// Try to guess the correct processor for this asset
			if (relativePath.EndsWith(".png"))
				Processor = new Texture2DProcessor();
			if (relativePath.EndsWith(".vs"))
				Processor = new VertexShaderProcessor();
			if (relativePath.EndsWith(".fs"))
				Processor = new FragmentShaderProcessor();
			if (relativePath.EndsWith(".fnt"))
				Processor = new FontProcessor();
		}

		/// <summary>
		///   Gets or sets the processor that processes the asset.
		/// </summary>
		public AssetProcessor Processor { get; set; }

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
		///   Gets the absolute path to the asset in the target directory, i.e. C:/Binaries/Assets/Textures/Tex.png.
		/// </summary>
		public string TargetPath
		{
			get { return Path.Combine(Configuration.TargetDirectory, RelativePathWithoutExtension) + PlatformInfo.AssetExtension; }
		}

		/// <summary>
		///   Gets the absolute path to the asset in the temp directory without the file extension, i.e.
		///   C:/AssetsSources/obj/Assets/Textures/Tex.
		/// </summary>
		public string TempPath
		{
			get { return Path.Combine(Configuration.TempDirectory, RelativePathWithoutExtension); }
		}

		/// <summary>
		///   Gets the path to the asset hash file.
		/// </summary>
		private string HashPath
		{
			get { return TempPath + ".hash"; }
		}

		/// <summary>
		///   Gets a value indicating whether the asset must be compiled.
		/// </summary>
		public bool RequiresCompilation
		{
			get
			{
				if (!File.Exists(TargetPath))
					return true;

				if (!File.Exists(HashPath))
					return true;

				var oldHash = File.ReadAllBytes(HashPath);
				var newHash = ComputeHash();

				if (oldHash.Length != newHash.Length)
					return true;

				for (var i = 0; i < oldHash.Length; ++i)
				{
					if (oldHash[i] != newHash[i])
						return true;
				}

				return false;
			}
		}

		/// <summary>
		///   Ensures that the target and temp paths exist.
		/// </summary>
		/// <param name="path">The path that should exist.</param>
		private static void EnsurePathsExist(string path)
		{
			Assert.ArgumentNotNullOrWhitespace(path, () => path);

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		/// <summary>
		///   Computes the hash of the current source file.
		/// </summary>
		private byte[] ComputeHash()
		{
			using (var cryptoProvider = new SHA1CryptoServiceProvider())
			{
				var buffer = File.ReadAllBytes(SourcePath);
				return cryptoProvider.ComputeHash(buffer);
			}
		}

		/// <summary>
		///   Writes the current source file hash to the hash file.
		/// </summary>
		public void UpdateHashFile()
		{
			File.WriteAllBytes(HashPath, ComputeHash());
		}
	}
}