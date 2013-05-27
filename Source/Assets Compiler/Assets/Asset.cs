using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.IO;
	using Compilers;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Memory;

	/// <summary>
	///   Represents a source asset that requires compilation.
	/// </summary>
	public abstract class Asset : DisposableObject
	{
		/// <summary>
		///   The source directory of the asset.
		/// </summary>
		private readonly string _sourceDirectory;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		protected Asset(string relativePath)
			: this(relativePath, Configuration.SourceDirectory)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="relativePath">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		/// <param name="sourceDirectory">The source directory of the asset.</param>
		protected Asset(string relativePath, string sourceDirectory)
		{
			Assert.ArgumentNotNullOrWhitespace(relativePath, () => relativePath);
			Assert.ArgumentNotNullOrWhitespace(sourceDirectory, () => sourceDirectory);

			SetDescription(relativePath);

			_sourceDirectory = sourceDirectory;
			RelativePath = relativePath;

			Directory.CreateDirectory(Path.GetDirectoryName(TargetPath));
			Directory.CreateDirectory(Path.GetDirectoryName(TempPathWithoutExtension));
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
			get { return Path.Combine(_sourceDirectory, RelativePath); }
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
			get { return TempPathWithoutExtension + ".md5"; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Default implementation
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} '{1}'", GetType().Name, RelativePath);
		}

		/// <summary>
		///   Gets a value indicating which action the compiler has to take.
		/// </summary>
		internal CompilationAction GetRequiredAction()
		{
			if (!File.Exists(TempPath))
				return CompilationAction.Process;

			if (!File.Exists(HashPath))
				return CompilationAction.Process;

			var oldHash = Hash.FromFile(HashPath);
			var newHash = Hash.Compute(SourcePath);

			if (oldHash != newHash)
				return CompilationAction.Process;

			if (!File.Exists(TargetPath))
				return CompilationAction.Copy;

			var targetHash = Hash.Compute(TargetPath);
			var tempHash = Hash.Compute(TempPath);

			if (targetHash != tempHash)
				return CompilationAction.Copy;

			return CompilationAction.Skip;
		}

		/// <summary>
		///   Writes a hash of the source asset to disk.
		/// </summary>
		internal void WriteHash()
		{
			Hash.Compute(SourcePath).WriteTo(HashPath);
		}
	}
}