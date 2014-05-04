namespace Pegasus.Assets
{
	using System;
	using System.Collections.Generic;
	using AssetLoaders;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///     Tracks all assets that it loaded. If an asset has already been loaded and it is
	///     requested again, the previously loaded instance is returned instead of loading the asset again.
	/// </summary>
	public sealed class AssetsManager : DisposableObject
	{
		/// <summary>
		///     The registered asset loaders that can be used to load assets.
		/// </summary>
		private static readonly Dictionary<byte, AssetLoader> Loaders = new Dictionary<byte, AssetLoader>();

		/// <summary>
		///     Indicates whether the asset manager loads its asset asynchronously.
		/// </summary>
		private readonly bool _asyncLoading;

		/// <summary>
		///     The graphics device that is used to load the assets.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///     The list of loaded assets.
		/// </summary>
		private readonly List<AssetInfo> _loadedAssets = new List<AssetInfo>();

		/// <summary>
		///     The list of loaded shader programs.
		/// </summary>
		private readonly List<ShaderProgram> _loadedPrograms = new List<ShaderProgram>();

		/// <summary>
		///     The list of pending assets, i.e., assets that have not yet been loaded.
		/// </summary>
		private readonly List<AssetInfo> _pendingAssets;

		/// <summary>
		///     The list of pending shader programs, i.e., assets that have not yet been loaded.
		/// </summary>
		private readonly List<ShaderProgram> _pendingPrograms;

		/// <summary>
		///     Initializes the type, registering the default asset loaders.
		/// </summary>
		static AssetsManager()
		{
			RegisterAssetLoader(new FontAssetLoader());
			RegisterAssetLoader(new Texture2DAssetLoader());
			RegisterAssetLoader(new CubeMapAssetLoader());
			RegisterAssetLoader(new FragmentShaderAssetLoader());
			RegisterAssetLoader(new VertexShaderAssetLoader());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to load the assets.</param>
		/// <param name="asyncLoading">Indicates whether the asset manager should load its asset asynchronously.</param>
		internal AssetsManager(GraphicsDevice graphicsDevice, bool asyncLoading)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			_graphicsDevice = graphicsDevice;
			_asyncLoading = asyncLoading;

			if (asyncLoading)
			{
				_pendingAssets = new List<AssetInfo>();
				_pendingPrograms = new List<ShaderProgram>();
			}

			Commands.OnReloadAssets += ReloadAssets;
		}

		/// <summary>
		///     Indicates whether all assets have been loaded.
		/// </summary>
		public bool LoadingCompleted
		{
			get { return _pendingAssets.Count == 0 && _pendingPrograms.Count == 0; }
		}

		/// <summary>
		///     Registers an asset loader that assets manager can subsequently use to load assets.
		/// </summary>
		/// <param name="assetLoader">The asset loader that should be registered.</param>
		public static void RegisterAssetLoader(AssetLoader assetLoader)
		{
			Assert.ArgumentNotNull(assetLoader);
			Assert.That(!Loaders.ContainsKey(assetLoader.AssetType), "An asset loader for this asset type has already been registered.");

			Loaders.Add(assetLoader.AssetType, assetLoader);
		}

		/// <summary>
		///     Reloads all currently loaded assets.
		/// </summary>
		private void ReloadAssets()
		{
			foreach (var info in _loadedAssets)
			{
				try
				{
					Log.Info("Reloading {1} {0}...", GetAssetDisplayName(info), Loaders[info.Type].AssetTypeName);
					Load(info);
				}
				catch (FileSystemException e)
				{
					Log.Die("Failed to reload asset {0}: {1}", GetAssetDisplayName(info), e.Message);
				}
			}

			foreach (var program in _loadedPrograms)
				program.Reinitialize();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Clear(_loadedAssets);
			Clear(_pendingAssets);
			Clear(_loadedPrograms);
			Clear(_pendingPrograms);

			Commands.OnReloadAssets -= ReloadAssets;
		}

		/// <summary>
		///     Clears the given asset info list.
		/// </summary>
		/// <param name="infos">The list of asset infos that should be cleared.</param>
		private static void Clear(List<AssetInfo> infos)
		{
			if (infos == null)
				return;

			foreach (var asset in infos)
				asset.Asset.SafeDispose();

			infos.Clear();
		}

		/// <summary>
		///     Clears the given shader program list.
		/// </summary>
		/// <param name="infos">The list of shader programs that should be cleared.</param>
		private static void Clear(List<ShaderProgram> infos)
		{
			if (infos == null)
				return;

			infos.SafeDisposeAll();
			infos.Clear();
		}

		/// <summary>
		///     Loads a shader program for the given shaders.
		/// </summary>
		/// <param name="vertexShader">The vertex shader of the shader program.</param>
		/// <param name="fragmentShader">The fragment shader of the shader program.</param>
		public ShaderProgram Load(AssetIdentifier<VertexShader> vertexShader, AssetIdentifier<FragmentShader> fragmentShader)
		{
			var vs = Load(vertexShader);
			var fs = Load(fragmentShader);

			// Check if we've already created a shader program with the given shaders
			var shaderProgram = Find(vs, fs);
			if (shaderProgram != null)
				return shaderProgram;

			shaderProgram = new ShaderProgram(_graphicsDevice, vs, fs);
			if (_asyncLoading)
				_pendingPrograms.Add(shaderProgram);
			else
				_loadedPrograms.Add(shaderProgram);

			return shaderProgram;
		}

		/// <summary>
		///     Loads all pending assets. If the timeout is exceeded when loading the assets, the function returns without
		///     all assets loaded. However, it is guaranteed that at least one pending asset is loaded with every invocation
		///     of the function.
		/// </summary>
		/// <param name="timeoutInMilliseconds">The timeout in milliseconds.</param>
		public void LoadPending(double timeoutInMilliseconds)
		{
			Assert.That(_asyncLoading, "Async loading has not been enabled for this asset manager.");

			using (var clock = Clock.Create())
			{
				var start = clock.Milliseconds;
				for (var i = _pendingAssets.Count; i > 0; --i)
				{
					var info = _pendingAssets[i - 1];

					Log.Info("Loading {0} {1}...", Loaders[info.Type].AssetTypeName, GetAssetDisplayName(info));
					Load(info);

					_pendingAssets.RemoveAt(i - 1);
					_loadedAssets.Add(info);

					if (!ContinueLoading(clock, start, timeoutInMilliseconds))
						return;
				}

				// At this point, we know that all pending loads of all shaders are completed, hence we can now safely
				// reinitialize the pending shader programs
				for (var i = _pendingPrograms.Count; i > 0; --i)
				{
					var program = _pendingPrograms[i - 1];
					program.Reinitialize();

					_pendingPrograms.RemoveAt(i - 1);
					_loadedPrograms.Add(program);

					if (!ContinueLoading(clock, start, timeoutInMilliseconds))
						return;
				}
			}
		}

		/// <summary>
		///     Loads the asset corresponding to the asset info object.
		/// </summary>
		/// <param name="info">The info object of the asset that should be loaded.</param>
		private static void Load(AssetInfo info)
		{
			try
			{
				using (var buffer = BufferReader.Create(FileSystem.ReadAllBytes(info.Path)))
				{
					AssetHeader.Validate(buffer, info.Type);
					Loaders[info.Type].Load(buffer, info.Asset, info.Path);
				}
			}
			catch (Exception e)
			{
				Log.Die("Failed to load asset {0}. {1}", GetAssetDisplayName(info), e.Message);
			}
		}

		/// <summary>
		///     Searches the given asset info list for an asset with the given identifier.
		/// </summary>
		/// <param name="infos">The list of asset infos that should be searched.</param>
		/// <param name="hashCode">The hash code of the asset that should be searched for.</param>
		private static IDisposable Find(List<AssetInfo> infos, int hashCode)
		{
			if (infos == null)
				return null;

			foreach (var info in infos)
			{
				if (info.HashCode == hashCode)
					return info.Asset;
			}

			return null;
		}

		/// <summary>
		///     Searches the lists of pending and loaded assets for an asset with the given identifier.
		/// </summary>
		/// <param name="hashCode">The hash code of the asset that should be searched for.</param>
		private IDisposable Find(int hashCode)
		{
			return Find(_pendingAssets, hashCode) ?? Find(_loadedAssets, hashCode);
		}

		/// <summary>
		///     Searches the given shader program list for a shader program with the given shaders.
		/// </summary>
		/// <param name="shaderPrograms">The list of shader programs that should be searched.</param>
		/// <param name="vertexShader">The vertex shader of the shader program.</param>
		/// <param name="fragmentShader">The fragment shader of the shader program.</param>
		private static ShaderProgram Find(List<ShaderProgram> shaderPrograms, VertexShader vertexShader, FragmentShader fragmentShader)
		{
			if (shaderPrograms == null)
				return null;

			foreach (var shaderProgram in shaderPrograms)
			{
				if (shaderProgram.VertexShader == vertexShader && shaderProgram.FragmentShader == fragmentShader)
					return shaderProgram;
			}

			return null;
		}

		/// <summary>
		///     Searches the lists of pending and loaded shader program a shader program with the given shaders.
		/// </summary>
		/// <param name="vertexShader">The vertex shader of the shader program.</param>
		/// <param name="fragmentShader">The fragment shader of the shader program.</param>
		private ShaderProgram Find(VertexShader vertexShader, FragmentShader fragmentShader)
		{
			return Find(_pendingPrograms, vertexShader, fragmentShader) ?? Find(_loadedPrograms, vertexShader, fragmentShader);
		}

		/// <summary>
		///     Loads the given asset. If asynchronous loading is enabled, actual loading of the asset is deferred and an
		///     uninitialized object is returned.
		/// </summary>
		/// <param name="assetIdentifier">The identifier of the asset that should be loaded.</param>
		public T Load<T>(AssetIdentifier<T> assetIdentifier)
			where T : class, IDisposable
		{
			// Check if we've loaded the requested asset before
			var asset = Find(assetIdentifier.HashCode);
			if (asset != null)
				return (T)asset;

			asset = Loaders[assetIdentifier.AssetType].Allocate(_graphicsDevice);

			var info = AssetInfo.Create(assetIdentifier, asset);
			if (_asyncLoading)
				_pendingAssets.Add(info);
			else
			{
				_loadedAssets.Add(info);
				Load(info);
			}

			return (T)asset;
		}

		/// <summary>
		///     Checks whether asynchronous loading should continue.
		/// </summary>
		private static bool ContinueLoading(Clock clock, double start, double timeout)
		{
			// Check if there's enough time for another asset; that's just a wild guess, obviously, but we assume that we're
			// good if we've more than 20% of the time left
			return clock.Milliseconds - start < timeout - (timeout * 0.2);
		}

		/// <summary>
		///     Gets the display name for the asset.
		/// </summary>
		/// <param name="info">The asset info object the display name should be returned for.</param>
		private static string GetAssetDisplayName(AssetInfo info)
		{
			// We know that the path ends with '.[AssetsProjectName].pg'. We'll remove that from the string and display the
			// project information separately.

			var dotIndex = info.Path.LastIndexOf('.', info.Path.Length - 4);
			if (dotIndex == -1)
				Log.Die("Asset name has unexpected structure: '{0}'.", info.Path);

			var project = info.Path.Substring(dotIndex + 1, info.Path.Length - 4 - dotIndex);
			var name = info.Path.Substring(0, dotIndex);
			return String.Format("'{0}://{1}'", project, name);
		}

		/// <summary>
		///     Provides information about loaded assets.
		/// </summary>
		private struct AssetInfo
		{
			/// <summary>
			///     Gets the path of the asset, required for reloading support.
			/// </summary>
			public string Path { get; private set; }

			/// <summary>
			///     Gets the globally unique hash code of the asset.
			/// </summary>
			public int HashCode { get; private set; }

			/// <summary>
			///     Gets the type of the asset.
			/// </summary>
			public byte Type { get; private set; }

			/// <summary>
			///     Gets the asset this info object provides information about.
			/// </summary>
			public IDisposable Asset { get; private set; }

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			public static AssetInfo Create<T>(AssetIdentifier<T> identifier, IDisposable asset)
				where T : class, IDisposable
			{
				return new AssetInfo
				{
					Path = identifier.AssetName,
					HashCode = identifier.HashCode,
					Type = identifier.AssetType,
					Asset = asset
				};
			}
		}
	}
}