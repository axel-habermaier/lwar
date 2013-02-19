using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
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
		public const string SourcePath = "../../Assets";

		/// <summary>
		///   The path where the temporary asset files should be stored.
		/// </summary>
		public const string TempPath = "../../Assets/obj";

#if DEBUG
		/// <summary>
		///   The path where the compiled assets should be stored.
		/// </summary>
		public const string TargetPath = "../../Binaries/Debug/Assets";
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
				Log.Info("   Compiling '{0}'...", asset);
				Process(processor, asset);
			}
		}

		/// <summary>
		///   Uses the given processor to process the given asset.
		/// </summary>
		/// <param name="processor">The processor that should be used to process the asset.</param>
		/// <param name="asset">The asset that should be processed.</param>
		private static void Process(AssetProcessor processor, string asset)
		{
			var assetName = Path.Combine(Path.GetDirectoryName(asset), Path.GetFileNameWithoutExtension(asset));
			using (var writer = new AssetWriter(Path.Combine(TargetPath, assetName)))
				processor.Process(Path.Combine(SourcePath, asset), asset, writer.Writer);
		}

		/// <summary>
		///   Ensures that all target and temp paths exist.
		/// </summary>
		private static void EnsurePathsExist()
		{
			foreach (var asset in Assets.All)
			{
				var assetPath = Path.Combine(TargetPath, Path.GetDirectoryName(asset));

				if (!Directory.Exists(assetPath))
					Directory.CreateDirectory(assetPath);

				assetPath = Path.Combine(TempPath, Path.GetDirectoryName(asset));
				if (!Directory.Exists(assetPath))
					Directory.CreateDirectory(assetPath);
			}
		}

		/// <summary>
		///   Compiles the asset at the given path, writing the result to the target directory.
		/// </summary>
		/// <param name="asset">The path of the asset that should be compiled, relative to the Assets base directory.</param>
		public string Compile(string asset)
		{
			Assert.ArgumentNotNullOrWhitespace(asset, () => asset);

			var assetName = asset;
			if (asset.EndsWith(".hlsl") || asset.EndsWith(".glsl"))
				assetName = asset.Substring(0, asset.Length - 5);

			assetName = assetName.Replace("\\", "/");
			Log.Info("Compiling '{0}'...", assetName);
			Process(GetProcessor(assetName), assetName);
			return Path.Combine(Path.GetDirectoryName(asset), Path.GetFileNameWithoutExtension(asset));
		}

		/// <summary>
		///   Gets a processor that can process the given asset.
		/// </summary>
		/// <param name="asset">The asset that should be processed.</param>
		private static AssetProcessor GetProcessor(string asset)
		{
			var getProcessors = new Func<string, AssetProcessor>[]
			{
				a => GetProcessor<CubeMapProcessor>(Assets.CubeMaps, a),
				a => GetProcessor<FontProcessor>(Assets.Fonts, a),
				a => GetProcessor<Texture2DProcessor>(Assets.Textures2D, a),
				a => GetProcessor<FragmentShaderProcessor>(Assets.FragmentShaders, a),
				a => GetProcessor<VertexShaderProcessor>(Assets.VertexShaders, a),
			};

			return getProcessors.Select(p => p(asset)).Single(p => p != null);
		}

		/// <summary>
		/// Checks whether the asset is in the given asset list and if so, returns a processor instance.
		/// </summary>
		/// <typeparam name="T">The type of the asset processor.</typeparam>
		/// <param name="assets">The assets that are processed by the processor.</param>
		/// <param name="asset">The asset that should be processed.</param>
		private static AssetProcessor GetProcessor<T>(IEnumerable<string> assets, string asset)
			where T : AssetProcessor, new()
		{
			return assets.Any(a => a == asset) ? new T() : null;
		}

		private static void Main(string[] args)
		{
			var compiler = new Compiler();
			compiler.Compile();
		}
	}
}