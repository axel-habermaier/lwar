﻿using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using Graphics;
	using Logging;
	using Memory;
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
		private const string AssetDirectory = "";

		/// <summary>
		///   The path to the asset compiler.
		/// </summary>
		private const string AssetCompiler = "pgc.exe";

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
			Assert.ArgumentNotNull(device);

			_device = device;
			Commands.OnReloadAssets += ReloadAssets;
		}

		/// <summary>
		///   Loads the given asset.
		/// </summary>
		/// <param name="asset">The asset that should be loaded.</param>
		/// <param name="assetName">The name of the asset that should be loaded.</param>
		private static void Load(Asset asset, string assetName)
		{
			var path = Path.Combine(AssetDirectory, assetName) + PlatformInfo.AssetExtension;
			using (var reader = BufferReader.Create(File.ReadAllBytes(path)))
				asset.Load(reader, assetName);
		}

		/// <summary>
		///   Reloads all assets.
		/// </summary>
		private void ReloadAssets()
		{
			try
			{
				int exitCode;
				using (var compiler = new ExternalProcess(AssetCompiler, "compile"))
					foreach (var output in compiler.Run(out exitCode))
						output.RaiseLogEvent();

				if (exitCode != 0)
				{
					Log.Error("Errors occurred during asset compilation. Asset reloading aborted.");
					return;
				}

				foreach (var pair in _assets)
				{
					try
					{
						Log.Info("Reloading {1} '{0}'...", pair.Key, pair.Value.FriendlyName);
						Load(pair.Value, pair.Key);
					}
					catch (IOException e)
					{
						Log.Die("Failed to reload asset '{0}': {1}", pair.Key, e.Message);
					}
				}
			}
			catch (Win32Exception e)
			{
				if (e.NativeErrorCode == 2)
					Log.Warn("{0} not found.", AssetCompiler);
				else
					Log.Error("{0} failed: {1}", AssetCompiler, e.Message);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_assets.Values.SafeDisposeAll();
			_assets.Clear();

			Commands.OnReloadAssets -= ReloadAssets;
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
			Assert.ArgumentNotNullOrWhitespace(assetName);
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
			Assert.ArgumentNotNullOrWhitespace(assetName);

			assetName = assetName.Replace("\\", "/");
			var asset = Find<TAsset>(assetName);

			if (asset != null)
				return asset;

			asset = new TAsset { GraphicsDevice = _device, Assets = this };
			try
			{
				Log.Info("Loading {0} '{1}'...", asset.FriendlyName, assetName);
				Load(asset, assetName);

				_assets.Add(assetName, asset);
				return asset;
			}
			catch (Exception e)
			{
				asset.SafeDispose();
				Log.Die("Failed to load asset '{0}': {1}", assetName, e.Message);
			}

			return null;
		}

		/// <summary>
		///   Loads a font.
		/// </summary>
		/// <param name="font">The identifier of the font asset that should be loaded.</param>
		public Font LoadFont(AssetIdentifier<Font> font)
		{
			return Load<FontAsset>(font.AssetName).Font;
		}

		/// <summary>
		///   Loads a vertex shader.
		/// </summary>
		/// <param name="shaderFilePath">The path to the vertex shader file.</param>
		internal VertexShader LoadVertexShader(string shaderFilePath)
		{
			Assert.ArgumentNotNullOrWhitespace(shaderFilePath);
			return Load<VertexShaderAsset>(shaderFilePath).Shader;
		}

		/// <summary>
		///   Loads a fragment shader.
		/// </summary>
		/// <param name="shaderFilePath">The path to the fragment shader file.</param>
		internal FragmentShader LoadFragmentShader(string shaderFilePath)
		{
			Assert.ArgumentNotNullOrWhitespace(shaderFilePath);
			return Load<FragmentShaderAsset>(shaderFilePath).Shader;
		}

		/// <summary>
		///   Loads a 2D texture.
		/// </summary>
		/// <param name="texture">The identifier of the texture 2D asset that should be loaded.</param>
		public Texture2D LoadTexture2D(AssetIdentifier<Texture2D> texture)
		{
			return Load<Texture2DAsset>(texture.AssetName).Texture;
		}

		/// <summary>
		///   Loads a cube map.
		/// </summary>
		/// <param name="cubeMap">The identifier of the cube map asset that should be loaded.</param>
		public CubeMap LoadCubeMap(AssetIdentifier<CubeMap> cubeMap)
		{
			return Load<CubeMapAsset>(cubeMap.AssetName).Texture;
		}
	}
}