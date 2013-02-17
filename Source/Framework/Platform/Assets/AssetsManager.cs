using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.Collections.Generic;
	using System.IO;
	using Graphics;
	using Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///   Tracks all assets that it loaded. If an asset has already been loaded and it is
	///   requested again, the previously loaded instance is returned instead of loading the asset again.
	/// </summary>
	public sealed class AssetsManager : DisposableObject
	{
		/// <summary>
		///   The name of the assets directory.
		/// </summary>
		private const string AssetDirectory = "Assets";

		/// <summary>
		///   The loaded assets.
		/// </summary>
		private readonly Dictionary<string, Asset> _assets = new Dictionary<string, Asset>();

		/// <summary>
		///   The graphics device for which the assets are managed.
		/// </summary>
		private readonly GraphicsDevice _device;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The graphics device that should be used to load the assets.</param>
		internal AssetsManager(GraphicsDevice device)
		{
			Assert.ArgumentNotNull(device, () => device);

			_device = device;
			Commands.ReloadAsset.Invoked += ReloadAsset;
		}

		/// <summary>
		///   Reloads an asset.
		/// </summary>
		/// <param name="assetName">The asset that should be reloaded.</param>
		private void ReloadAsset(string assetName)
		{
			Assert.ArgumentNotNull(assetName, () => assetName);

			Asset asset;
			assetName = assetName.Replace("\\", "/");

			if (!_assets.TryGetValue(assetName, out asset))
				Log.Warn("No asset with name '{0}' has been loaded.", assetName);
			else
			{
				try
				{
					Log.Info("Reloading {1} '{0}'...", assetName, asset.FriendlyName);
					using (var reader = new AssetReader(Path.Combine(AssetDirectory, assetName)))
						asset.Load(reader);
				}
				catch (IOException e)
				{
					Log.Die("Failed to reload asset '{0}': {1}", assetName, e.Message);
				}
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.ReloadAsset.Invoked -= ReloadAsset;

			_assets.Values.SafeDisposeAll();
			_assets.Clear();
		}

		/// <summary>
		///   Finds and returns the asset with the given name or returns null if the asset has not yet been loaded.
		///   Throws an exception if an asset with the same name but a different type has previously been loaded.
		/// </summary>
		/// <typeparam name="TAsset">The type of the asset that should be found.</typeparam>
		/// <param name="assetName">The name of the asset that should be found.</param>
		private TAsset Find<TAsset>(string assetName)
			where TAsset : class
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNullOrWhitespace(assetName, () => assetName);
			Asset asset;

			if (_assets.TryGetValue(assetName, out asset))
			{
				var typedAsset = asset as TAsset;
				if (typedAsset == null)
				{
					const string message = "Asset '{0}' is already loaded and has type '{1}'.";
					Log.Die(message, assetName, asset.FriendlyName);
				}
				return typedAsset;
			}

			return null;
		}

		/// <summary>
		///   Loads and returns an asset with the given type and name or returns a previously loaded instance.
		/// </summary>
		/// <typeparam name="TAsset">The type of the asset that should be loaded.</typeparam>
		/// <param name="assetName">The name of the asset that should be loaded.</param>
		private TAsset Load<TAsset>(string assetName)
			where TAsset : Asset, new()
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNullOrWhitespace(assetName, () => assetName);

			assetName = assetName.Replace("\\", "/");
			var asset = Find<TAsset>(assetName);

			if (asset != null)
				return asset;

			try
			{
				asset = new TAsset { GraphicsDevice = _device, Assets = this };
				Log.Info("Loading {0} '{1}'...", asset.FriendlyName, assetName);

				using (var reader = new AssetReader(Path.Combine(AssetDirectory, assetName)))
					asset.Load(reader);
			}
			catch (IOException e)
			{
				Log.Die("Failed to load asset '{0}': {1}", assetName, e.Message);
			}

			_assets.Add(assetName, asset);
			return asset;
		}

		/// <summary>
		///   Loads a font.
		/// </summary>
		/// <param name="fontFilePath">The path to the font description file.</param>
		public Font LoadFont(string fontFilePath)
		{
			Assert.ArgumentNotNullOrWhitespace(fontFilePath, () => fontFilePath);
			return Load<FontAsset>(fontFilePath).Font;
		}

		/// <summary>
		///   Loads a vertex shader.
		/// </summary>
		/// <param name="shaderFilePath">The path to the vertex shader file.</param>
		public VertexShader LoadVertexShader(string shaderFilePath)
		{
			Assert.ArgumentNotNullOrWhitespace(shaderFilePath, () => shaderFilePath);
			return Load<VertexShaderAsset>(shaderFilePath).Shader;
		}

		/// <summary>
		///   Loads a fragment shader.
		/// </summary>
		/// <param name="shaderFilePath">The path to the fragment shader file.</param>
		public FragmentShader LoadFragmentShader(string shaderFilePath)
		{
			Assert.ArgumentNotNullOrWhitespace(shaderFilePath, () => shaderFilePath);
			return Load<FragmentShaderAsset>(shaderFilePath).Shader;
		}

		/// <summary>
		///   Loads a 2D texture.
		/// </summary>
		/// <param name="texturePath">The path to the texture file.</param>
		public Texture2D LoadTexture2D(string texturePath)
		{
			Assert.ArgumentNotNullOrWhitespace(texturePath, () => texturePath);
			return Load<Texture2DAsset>(texturePath).Texture;
		}

		/// <summary>
		///   Loads a cube map.
		/// </summary>
		/// <param name="cubeMapPath">The path to the cube map file.</param>
		public CubeMap LoadCubeMap(string cubeMapPath)
		{
			Assert.ArgumentNotNullOrWhitespace(cubeMapPath, () => cubeMapPath);
			return Load<CubeMapAsset>(cubeMapPath).Texture;
		}
	}
}