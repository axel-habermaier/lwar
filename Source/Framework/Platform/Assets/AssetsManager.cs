using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.Collections.Generic;
	using System.IO;
	using Graphics;
	using Rendering.UserInterface;

	/// <summary>
	///   Tracks all assets that it loaded. If an asset has already been loaded and it is
	///   requested again, the previously loaded instance is returned instead of loading the asset again.
	/// </summary>
	public sealed class AssetsManager : DisposableObject
	{
		/// <summary>
		///   The loaded assets.
		/// </summary>
		private readonly Dictionary<string, IDisposable> _assets = new Dictionary<string, IDisposable>(1024);

		/// <summary>
		///   The reader used to load fonts.
		/// </summary>
		private readonly FontReader _fontReader;

		/// <summary>
		///   The reader used to load shaders.
		/// </summary>
		private readonly ShaderReader _shaderReader;

		/// <summary>
		///   The reader used to load textures.
		/// </summary>
		private readonly TextureReader _textureReader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The graphics device that should be used to load the assets.</param>
		internal AssetsManager(GraphicsDevice device)
		{
			_fontReader = new FontReader(this);
			_shaderReader = new ShaderReader(device);
			_textureReader = new TextureReader(device);
		}

		/// <summary>
		///   Gets the extension that should be appended to shader file names in order to distinguish between different
		///   shaders for different graphics APIs.
		/// </summary>
		private string ShaderExtension
		{
			get
			{
				switch (PlatformInfo.GraphicsApi)
				{
					case GraphicsApi.Direct3D11:
						return ".hlsl";
					case GraphicsApi.OpenGL3:
						return ".glsl";
					default:
						throw new InvalidOperationException("Unknown graphics api.");
				}
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			foreach (var asset in _assets.Values)
				asset.Dispose();
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
			IDisposable asset;

			if (_assets.TryGetValue(assetName, out asset))
			{
				var typedAsset = asset as TAsset;
				if (typedAsset == null)
				{
					const string message = "Asset '{0}' is already loaded and has type '{1}'. Expected type '{2}'.";
					Log.Die(message, assetName, asset.GetType().FullName, typeof(TAsset).FullName);
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
		/// <param name="loader">A function that actually creates and loads the asset instance.</param>
		private TAsset Load<TAsset>(string assetName, Func<AssetReader, TAsset> loader)
			where TAsset : class, IDisposable
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNullOrWhitespace(assetName, () => assetName);
			Assert.ArgumentNotNull(loader, () => loader);

			var asset = Find<TAsset>(assetName);

			if (asset != null)
				return asset;

			Log.Info("Loading {0} '{1}'...", typeof(TAsset).Name, assetName);

			try
			{
				using (var reader = new AssetReader(Path.Combine("Assets", assetName)))
					asset = loader(reader);
			}
			catch (DirectoryNotFoundException e)
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
			return Load(fontFilePath, reader => _fontReader.Load(reader));
		}

		/// <summary>
		///   Loads a vertex shader.
		/// </summary>
		/// <param name="shaderFilePath">The path to the vertex shader file.</param>
		public VertexShader LoadVertexShader(string shaderFilePath)
		{
			shaderFilePath = shaderFilePath + ShaderExtension;
			return Load(shaderFilePath, reader => _shaderReader.LoadVertexShader(reader));
		}

		/// <summary>
		///   Loads a fragment shader.
		/// </summary>
		/// <param name="shaderFilePath">The path to the fragment shader file.</param>
		public FragmentShader LoadFragmentShader(string shaderFilePath)
		{
			shaderFilePath = shaderFilePath + ShaderExtension;
			return Load(shaderFilePath, reader => _shaderReader.LoadFragmentShader(reader));
		}

		/// <summary>
		///   Loads a 2D texture.
		/// </summary>
		/// <param name="texturePath">The path to the texture file.</param>
		public Texture2D LoadTexture2D(string texturePath)
		{
			return Load(texturePath, reader => _textureReader.Load(reader));
		}
	}
}