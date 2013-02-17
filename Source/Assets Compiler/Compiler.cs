using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.IO;
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
		///   Initializes a new instance.
		/// </summary>
		public Compiler()
		{
			EnsurePathsExist();
		}

		/// <summary>
		///   Compiles all assets.
		/// </summary>
		public void Compile()
		{
			Process(new FontProcessor(), Assets.Fonts, "Fonts");
			Process(new Texture2DProcessor(), Assets.Textures2D, "2D Textures");
			Process(new CubeMapProcessor(), Assets.CubeMaps, "Cube Maps");
			Process(new VertexShaderProcessor(), Assets.VertexShaders, "Vertex Shaders");
			Process(new FragmentShaderProcessor(), Assets.FragmentShaders, "Fragment Shaders");
		}

		/// <summary>
		///   Uses the given processor to process all given assets.
		/// </summary>
		/// <param name="processor">The processor that should be used to process the assets.</param>
		/// <param name="assets">The assets that should be processed.</param>
		/// <param name="description">A description of the assets that are processed.</param>
		private static void Process(AssetProcessor processor, IEnumerable<string> assets, string description)
		{
			Log.Info("Processing {0}...", description);
			foreach (var asset in assets)
			{
				Log.Info("   {0}...", asset);
				var assetName = Path.Combine(Path.GetDirectoryName(asset), Path.GetFileNameWithoutExtension(asset));
				using (var writer = new AssetWriter(Path.Combine(TargetPath, assetName)))
					processor.Process(Path.Combine(SourcePath, asset), asset, writer.Writer);
			}
		}

		/// <summary>
		///   Ensures that all target paths exist.
		/// </summary>
		private static void EnsurePathsExist()
		{
			foreach (var asset in Assets.All)
			{
				var assetPath = Path.Combine(TargetPath, Path.GetDirectoryName(asset));
				if (!Directory.Exists(assetPath))
					Directory.CreateDirectory(assetPath);
			}
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
			//Assert.ArgumentNotNullOrWhitespace(asset, () => asset);
			//return Compile(new FileInfo(Path.Combine(SourcePath, asset)), Path.GetDirectoryName(asset));
			return null;
		}
	}
}