using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Assets;

	/// <summary>
	///   Compiles assets (textures, fonts, sounds, etc.) into an efficient byte format.
	/// </summary>
	public sealed class Compiler : IAssetsCompiler
	{
		/// <summary>
		///   The list of all assets that have been compiled during the last execution of the compiler.
		/// </summary>
		private readonly List<string> _compiledAssets = new List<string>();

		/// <summary>
		///   Compiles all assets and returns the names of the assets that have been changed.
		/// </summary>
		public IEnumerable<string> Compile()
		{
			try
			{
				_compiledAssets.Clear();
				Process(new FontProcessor(), Assets.Fonts, "Fonts");
				Process(new Texture2DProcessor(), Assets.Textures2D, "2D Textures");
				Process(new CubeMapProcessor(), Assets.CubeMaps, "Cube Maps");
				Process(new VertexShaderProcessor(), Assets.VertexShaders, "Vertex Shaders");
				Process(new FragmentShaderProcessor(), Assets.FragmentShaders, "Fragment Shaders");
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}

			return _compiledAssets;
		}

		/// <summary>
		///   Uses the given processor to process all given assets.
		/// </summary>
		/// <param name="processor">The processor that should be used to process the assets.</param>
		/// <param name="assets">The assets that should be processed.</param>
		/// <param name="description">A description of the assets that are processed.</param>
		private void Process(AssetProcessor processor, IEnumerable<string> assets, string description)
		{
			Log.Info("Processing {0}...", description);
			foreach (var asset in assets)
				Process(processor, new Asset(asset));
		}

		/// <summary>
		///   Uses the given processor to process the given asset.
		/// </summary>
		/// <param name="processor">The processor that should be used to process the asset.</param>
		/// <param name="asset">The asset that should be processed.</param>
		private void Process(AssetProcessor processor, Asset asset)
		{
			if (!asset.RequiresCompilation)
			{
				Log.Info("   Skipping '{0}' (no changes detected).", asset.RelativePath);
				return;
			}

			_compiledAssets.Add(asset.RelativePathWithoutExtension);
			Log.Info("   Compiling '{0}'...", asset.RelativePath);
			asset.UpdateHashFile();
			using (var writer = new AssetWriter(asset.TargetPath))
				processor.Process(asset, writer.Writer);
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
		///   Checks whether the asset is in the given asset list and if so, returns a processor instance.
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