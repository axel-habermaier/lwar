using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Framework;
	using Framework.Platform.Assets;

	/// <summary>
	///   Compiles assets (textures, fonts, sounds, etc.) into an efficient byte format.
	/// </summary>
	public sealed class Compiler : IAssetsCompiler
	{
		/// <summary>
		///   The path to the source assets.
		/// </summary>
		private const string SourcePath = "../../Assets";

#if DEBUG
		/// <summary>
		///   The path where the compiled assets should be stored.
		/// </summary>
		private const string TargetPath = "../../Binaries/Debug/Assets";
#else
		/// <summary>
		/// The path where the compiled assets should be stored.
		/// </summary>
		private const string TargetPath = "../../Binaries/Release/Assets";
#endif

		/// <summary>
		///   The known asset processors.
		/// </summary>
		private readonly List<AssetProcessor> _processors;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Compiler()
		{
			_processors = Assembly.GetExecutingAssembly().GetTypes()
								  .Where(t => t.BaseType == typeof(AssetProcessor))
								  .Select(Activator.CreateInstance)
								  .Cast<AssetProcessor>().ToList();
		}

		/// <summary>
		///   Compiles all assets.
		/// </summary>
		public void Compile()
		{
			CompileAssets(new DirectoryInfo(SourcePath), "");
		}

		/// <summary>
		///   Recursively walks through the asset directory and compiles all files for which a processor is found.
		/// </summary>
		/// <param name="root">The root directory.</param>
		/// <param name="path">The path to the asset file relative to the asset root directory.</param>
		private void CompileAssets(DirectoryInfo root, string path)
		{
			// Create the target directory if it does not already exist
			if (!Directory.Exists(Path.Combine(TargetPath, path)))
				Directory.CreateDirectory(Path.Combine(TargetPath, path));

			// First, process all the files directly under this folder 
			var files = root.GetFiles("*.*");

			foreach (var file in files)
				Compile(file, path);

			// Now find all the subdirectories under this directory
			var subDirs = root.GetDirectories();

			foreach (var dirInfo in subDirs)
				CompileAssets(dirInfo, Path.Combine(path, dirInfo.Name));
		}

		private static void Main(string[] args)
		{
			var compiler = new Compiler();
			compiler.Compile();
		}

		/// <summary>
		///   Compiles the asset at the given path, writing the result to the target directory.
		/// </summary>
		/// <param name="asset">The path of the asset that should be compiled, relative to the Assets base directory.</param>
		public string Compile(string asset)
		{
			Assert.ArgumentNotNullOrWhitespace(asset, () => asset);
			return Compile(new FileInfo(Path.Combine(SourcePath, asset)), Path.GetDirectoryName(asset));
		}

		/// <summary>
		///   Compiles the asset file, writing the result to the target directory.
		/// </summary>
		/// <param name="file">The asset file that should be compiled.</param>
		/// <param name="targetDirectory">The target sub-directory of the compiled assets folder.</param>
		public string Compile(FileInfo file, string targetDirectory)
		{
			Assert.ArgumentNotNull(file, () => file);
			Assert.ArgumentNotNull(targetDirectory, () => targetDirectory);

			var processor = _processors.FirstOrDefault(p => p.CanProcess(file.Extension));
			if (processor == null)
				return String.Empty;

			var targetName = file.Name;
			if (processor.RemoveExtension)
				targetName = Path.GetFileNameWithoutExtension(file.Name);

			using (var writer = new AssetWriter(Path.Combine(TargetPath, Path.Combine(targetDirectory, targetName))))
				processor.Process(file.FullName, Path.Combine(targetDirectory, file.Name), writer.Writer);

			return Path.Combine(targetDirectory, targetName);
		}
	}
}