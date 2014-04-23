namespace Pegasus.Assets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///     Tracks all assets that it loaded. If an asset has already been loaded and it is
	///     requested again, the previously loaded instance is returned instead of loading the asset again.
	/// </summary>
	public sealed class AssetsManager : DisposableObject
	{
		/// <summary>
		///     The path to the asset compiler.
		/// </summary>
		private const string AssetCompiler = "pgc.exe";

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
		///     Reloads all assets of the given asset projects.
		/// </summary>
		/// <param name="paths">
		///     The path to the assets project files that should be recompiled and reloaded. Different assets projects
		///     should be separated by semicolon.
		/// </param>
		private void ReloadAssets(string paths)
		{
			var assetProjects = paths.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var assetProject in assetProjects)
			{
				Log.Info("\\cyanCompiling assets project '{0}'...", assetProject);

				try
				{
					int exitCode;
					var commandLine = String.Format("compile \"{0}\"", assetProject);

					using (var compiler = new ExternalProcess(AssetCompiler, commandLine))
						foreach (var output in compiler.Run(out exitCode))
							output.RaiseLogEvent();

					Log.Info("\\cyanCompleted compilation of assets project '{0}'.", assetProject);

					if (exitCode != 0)
					{
						Log.Error("Errors occurred during asset compilation. Asset reloading of asset project '{0}' aborted.", assetProject);
						continue;
					}

					foreach (var info in _loadedAssets)
					{
						try
						{
							Log.Info("Reloading {1} {0}...", GetAssetDisplayName(info), info.Type.ToDisplayString());
							Load(info);
						}
						catch (IOException e)
						{
							Log.Die("Failed to reload asset {0}: {1}", GetAssetDisplayName(info), e.Message);
						}
					}

					foreach (var program in _loadedPrograms)
						program.Reinitialize();
				}
				catch (Win32Exception e)
				{
					if (e.NativeErrorCode == 2)
						Log.Warn("{0} not found.", AssetCompiler);
					else
						Log.Error("{0} failed: {1}", AssetCompiler, e.Message);
				}
			}
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
		///     Loads a font.
		/// </summary>
		/// <param name="font">The identifier of the font asset that should be loaded.</param>
		public Font Load(AssetIdentifier<Font> font)
		{
			return (Font)LoadAsset(font);
		}

		/// <summary>
		///     Loads a vertex shader.
		/// </summary>
		/// <param name="shader">The identifier of the vertex shader asset that should be loaded.</param>
		internal VertexShader Load(AssetIdentifier<VertexShader> shader)
		{
			return (VertexShader)LoadAsset(shader);
		}

		/// <summary>
		///     Loads a fragment shader.
		/// </summary>
		/// <param name="shader">The identifier of the fragment shader asset that should be loaded.</param>
		internal FragmentShader Load(AssetIdentifier<FragmentShader> shader)
		{
			return (FragmentShader)LoadAsset(shader);
		}

		/// <summary>
		///     Loads a 2D texture.
		/// </summary>
		/// <param name="texture">The identifier of the texture 2D asset that should be loaded.</param>
		public Texture2D Load(AssetIdentifier<Texture2D> texture)
		{
			return (Texture2D)LoadAsset(texture);
		}

		/// <summary>
		///     Loads a cube map.
		/// </summary>
		/// <param name="cubeMap">The identifier of the cube map asset that should be loaded.</param>
		public CubeMap Load(AssetIdentifier<CubeMap> cubeMap)
		{
			return (CubeMap)LoadAsset(cubeMap);
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

					Log.Info("Loading {0} {1}...", info.Type.ToDisplayString(), GetAssetDisplayName(info));
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
				using (var buffer = BufferReader.Create(File.ReadAllBytes(info.Path)))
				{
					AssetHeader.Validate(buffer, info.Type);

					switch (info.Type)
					{
						case AssetType.CubeMap:
							var cubeMap = (CubeMap)info.Asset;
							LoadTexture(buffer, cubeMap);
							cubeMap.SetName(GetAssetDisplayName(info));
							break;
						case AssetType.Texture2D:
							var texture2D = (Texture2D)info.Asset;
							LoadTexture(buffer, texture2D);
							texture2D.SetName(GetAssetDisplayName(info));
							break;
						case AssetType.FragmentShader:
							var fragmentShader = (FragmentShader)info.Asset;
							LoadFragmentShader(buffer, fragmentShader);
							fragmentShader.SetName(GetAssetDisplayName(info));
							break;
						case AssetType.VertexShader:
							var vertexShader = (VertexShader)info.Asset;
							LoadVertexShader(buffer, vertexShader);
							vertexShader.SetName(GetAssetDisplayName(info));
							break;
						case AssetType.Font:
							var font = (Font)info.Asset;
							LoadFont(buffer, font);
							font.Texture.SetName(GetAssetDisplayName(info));
							break;
						default:
							Log.Die("Unexpected asset type.");
							break;
					}
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
		///     Allocates the asset instance for the given asset identifier. If asynchronous loading is enabled,
		///     actual loading of the asset is deferred.
		/// </summary>
		/// <param name="identifier">The identifier of the asset the instance should be allocated for.</param>
		private IDisposable LoadAsset<T>(AssetIdentifier<T> identifier)
			where T : class, IDisposable
		{
			// Check if we've loaded the requested asset before
			var asset = Find(identifier.HashCode);
			if (asset != null)
				return asset;

			switch (identifier.AssetType)
			{
				case AssetType.CubeMap:
					asset = new CubeMap(_graphicsDevice);
					break;
				case AssetType.Texture2D:
					asset = new Texture2D(_graphicsDevice);
					break;
				case AssetType.FragmentShader:
					asset = new FragmentShader(_graphicsDevice);
					break;
				case AssetType.VertexShader:
					asset = new VertexShader(_graphicsDevice);
					break;
				case AssetType.Font:
					asset = new Font(_graphicsDevice);
					break;
				default:
					Log.Die("Unexpected asset type.");
					break;
			}

			var info = AssetInfo.Create(identifier, asset);
			if (_asyncLoading)
				_pendingAssets.Add(info);
			else
			{
				_loadedAssets.Add(info);
				Load(info);
			}

			return asset;
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
		///     Loads the texture from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the texture data should be read from.</param>
		/// <param name="texture">The texture instance that should be reinitialized with the loaded data.</param>
		private static unsafe void LoadTexture(BufferReader buffer, Texture texture)
		{
			var description = new TextureDescription
			{
				Width = buffer.ReadUInt32(),
				Height = buffer.ReadUInt32(),
				Depth = buffer.ReadUInt32(),
				ArraySize = buffer.ReadUInt32(),
				Type = (TextureType)buffer.ReadInt32(),
				Format = (SurfaceFormat)buffer.ReadInt32(),
				Mipmaps = buffer.ReadUInt32(),
				SurfaceCount = buffer.ReadUInt32()
			};

			var surfaces = new Surface[description.SurfaceCount];

			for (var i = 0; i < description.SurfaceCount; ++i)
			{
				surfaces[i] = new Surface
				{
					Width = buffer.ReadUInt32(),
					Height = buffer.ReadUInt32(),
					Depth = buffer.ReadUInt32(),
					Size = buffer.ReadUInt32(),
					Stride = buffer.ReadUInt32(),
					Data = buffer.Pointer
				};

				var surfaceSize = surfaces[i].Size * surfaces[i].Depth;
				buffer.Skip((int)surfaceSize);
			}

			texture.Reinitialize(description, surfaces);
		}

		/// <summary>
		///     Loads the shader from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the shader data should be read from.</param>
		/// <param name="shader">The shader instance that should be reinitialized with the loaded data.</param>
		private static unsafe void LoadFragmentShader(BufferReader buffer, FragmentShader shader)
		{
			byte* data;
			int length;
			ExtractShaderCode(buffer, out data, out length);

			shader.Reinitialize(data, length);
		}

		/// <summary>
		///     Loads the shader from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the shader data should be read from.</param>
		/// <param name="shader">The shader instance that should be reinitialized with the loaded data.</param>
		private static unsafe void LoadVertexShader(BufferReader buffer, VertexShader shader)
		{
			var inputCount = buffer.ReadByte();
			var inputs = new ShaderInput[inputCount];

			for (var i = 0; i < inputCount; ++i)
			{
				inputs[i] = new ShaderInput
				{
					Format = (VertexDataFormat)buffer.ReadByte(),
					Semantics = (DataSemantics)buffer.ReadByte()
				};
			}

			byte* data;
			int length;
			ExtractShaderCode(buffer, out data, out length);

			shader.Reinitialize(data, length, inputs);
		}

		/// <summary>
		///     Loads the font from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the font data should be read from.</param>
		/// <param name="font">The shader instance that should be reinitialized with the font data.</param>
		private static void LoadFont(BufferReader buffer, Font font)
		{
			// Load the font map
			AssetHeader.Validate(buffer, AssetType.Texture2D);
			LoadTexture(buffer, font.Texture);

			// Load the font metadata
			var lineHeight = buffer.ReadUInt16();

			// Load the glyph metadata
			var numGlyphs = buffer.ReadUInt16();
			var glyphs = new Glyph[256];

			for (var i = 0; i < numGlyphs; ++i)
			{
				var index = buffer.ReadByte();

				// Read the texture coordinates
				var x = buffer.ReadUInt16();
				var y = buffer.ReadUInt16();
				glyphs[index].Area.Width = buffer.ReadUInt16();
				glyphs[index].Area.Height = buffer.ReadUInt16();

				// Compute the texture coordinates
				var textureLeft = x / (float)font.Texture.Width;
				var textureRight = (x + glyphs[index].Area.Width) / (float)font.Texture.Width;
				var textureTop = (y + glyphs[index].Area.Height) / (float)font.Texture.Height;
				var textureBottom = y / (float)font.Texture.Height;

				glyphs[index].TextureArea = new RectangleF(textureLeft, textureTop,
														   textureRight - textureLeft,
														   textureBottom - textureTop);

				// Read the glyph offsets
				glyphs[index].Area.Left = buffer.ReadInt16();
				glyphs[index].Area.Top = buffer.ReadInt16();
				glyphs[index].AdvanceX = buffer.ReadInt16();
			}

			// Load the kerning data
			var kerningCount = buffer.ReadUInt16();
			KerningPair[] kernings = null;
			if (kerningCount != 0)
			{
				kernings = new KerningPair[kerningCount];

				for (var i = 0; i < kerningCount; ++i)
				{
					var first = buffer.ReadUInt16();
					var second = buffer.ReadUInt16();
					var offset = buffer.ReadInt16();

					kernings[i] = new KerningPair((char)first, (char)second, offset);

					if (glyphs[first].KerningStart == 0)
						glyphs[first].KerningStart = i;

					++glyphs[first].KerningCount;
				}
			}

			font.Reinitialize(glyphs, kernings, lineHeight);
		}

		/// <summary>
		///     Extracts the graphics API-dependent shader code from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the shader.</param>
		/// <param name="shaderCode">The extracted shader source code.</param>
		/// <param name="length">The length of the extracted shader code in bytes.</param>
		private static unsafe void ExtractShaderCode(BufferReader buffer, out byte* shaderCode, out int length)
		{
			switch (NativeLibrary.GraphicsApi)
			{
				case GraphicsApi.Direct3D11:
					buffer.Skip(buffer.ReadInt32());
					if (buffer.EndOfBuffer)
						Log.Die("The HLSL version of the shader cannot be loaded as it was not compiled into the shader asset file.");

					shaderCode = buffer.Pointer;
					length = buffer.BufferSize - buffer.Count;
					break;
				case GraphicsApi.OpenGL3:
					length = buffer.ReadInt32();
					shaderCode = buffer.Pointer;
					break;
				default:
					throw new InvalidOperationException("Unsupported Graphics API.");
			}
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
			public AssetType Type { get; private set; }

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