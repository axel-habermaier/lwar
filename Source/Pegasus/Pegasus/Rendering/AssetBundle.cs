namespace Pegasus.Rendering
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Represents a set of assets that are loaded and unloaded together.
	/// </summary>
	public abstract class AssetBundle : DisposableObject
	{
		/// <summary>
		///     The number of assets contained in the bundle.
		/// </summary>
		private readonly ushort _assetCount;

		/// <summary>
		///     The name of the asset bundle file.
		/// </summary>
		private readonly string _bundleFile;

		/// <summary>
		///     The unique identifier of the asset bundle.
		/// </summary>
		private readonly Guid _id;

		/// <summary>
		///     The render context the asset bundle belongs to.
		/// </summary>
		private readonly RenderContext _renderContext;

		/// <summary>
		///     Indicates whether all assets of the bundle have already been created.
		/// </summary>
		private bool _assetsCreated;

		/// <summary>
		///     The buffer that is used to load the asset bundle.
		/// </summary>
		private BufferReader _buffer;

		/// <summary>
		///     The fonts contained in the asset bundle.
		/// </summary>
		private Font[] _fonts;

		/// <summary>
		///     The number of assets that have been loaded.
		/// </summary>
		private ushort _loadedAssetCount;

		/// <summary>
		///     The task that was used to load the asset bundle from the disk.
		/// </summary>
		private Task<byte[]> _loadTask;

		/// <summary>
		///     The state of the asset bundle.
		/// </summary>
		private State _state = State.Unloaded;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context the asset bundle belongs to.</param>
		/// <param name="id">The unique identifier of the asset bundle.</param>
		/// <param name="bundleFile">The name of the asset bundle file.</param>
		/// <param name="assetCount">The number of assets contained in the bundle.</param>
		protected AssetBundle(RenderContext renderContext, Guid id, string bundleFile, ushort assetCount)
		{
			Assert.ArgumentNotNull(renderContext);
			Assert.ArgumentNotNullOrWhitespace(bundleFile);

			_renderContext = renderContext;
			_id = id;
			_bundleFile = bundleFile;
			_assetCount = assetCount;

			renderContext.AddAssetBundle(this);
		}

		/// <summary>
		///     Gets the fonts contained in the asset bundle.
		/// </summary>
		protected abstract IEnumerable<Font> Fonts { get; }

		/// <summary>
		///     Gets a value indicating whether all assets contained in the bundle have been fully loaded and initialized.
		/// </summary>
		public bool LoadingCompleted
		{
			get { return _state == State.AssetsInitialized; }
		}

		/// <summary>
		///     Reloads the asset bundle.
		/// </summary>
		internal void Reload()
		{
			// If we're in the middle of an asynchronous load, complete it before reloading the bundle 
			// to avoid resource leaks
			if (_state != State.Unloaded && _state != State.AssetsInitialized)
				Load();

			_state = State.Unloaded;
			_buffer.SafeDispose();
			_loadedAssetCount = 0;

			Load();
		}
		
		/// <summary>
		///     Loads the asset bundle file from the file system and decompresses its contents.
		/// </summary>
		private byte[] LoadAssetFile()
		{
			var data = FileSystem.ReadAllBytes(_bundleFile);
			using (var buffer = new BufferReader(data, Endianess.Little))
			{
				// Validate the header
				if (!buffer.CanRead(16))
					Log.Die("Asset bundle is corrupted: Header information missing.");

				if (_id != new Guid(buffer.ReadByteArray(16)))
					Log.Die("Asset bundle is corrupted: The header contains an invalid hash.");

				// Decompress the bundle's content
				var content = new byte[buffer.ReadInt32()];
				using (var stream = new GZipStream(new MemoryStream(buffer.ReadByteArray()), CompressionMode.Decompress))
					stream.Read(content, 0, content.Length);

				return content;
			}
		}

		/// <summary>
		///     Loads the asset bundle asynchronously. The function tries to return within the given timeout (in seconds). True is
		///     returned to indicate that the bundle has been fully loaded. As long as false is returned, the function should be
		///     called in regular intervals.
		/// </summary>
		/// <param name="timeoutInMilliseconds">The amount of time in milliseconds that can be used to load assets.</param>
		public bool LoadAsync(double timeoutInMilliseconds = Double.MaxValue)
		{
			try
			{
				switch (_state)
				{
					case State.Unloaded:
						Log.Info("Loading asset bundle '{0}'...", _bundleFile);
						_state = State.LoadingBundle;
						_loadTask = Task.Run((Func<byte[]>)LoadAssetFile);
						return false;
					case State.LoadingBundle:
						if (!_loadTask.IsCompleted)
							return false;

						if (_loadTask.IsFaulted)
							throw new InvalidOperationException(String.Join("\n", _loadTask.Exception.InnerExceptions.Select(e => e.Message)));

						_buffer = new BufferReader(_loadTask.Result, Endianess.Little);
						_loadTask = null;

						goto case State.InitializingAssets;
					case State.InitializingAssets:
					{
						var start = Clock.GetTime();

						while (_loadedAssetCount < _assetCount)
						{
							if (_assetsCreated)
								ReloadAsset(ref _buffer, _loadedAssetCount++);
							else
								CreateAsset(_renderContext.GraphicsDevice, ref _buffer, _loadedAssetCount++);

							if (!ContinueLoading(start, timeoutInMilliseconds))
								return false;
						}

						Assert.That(_buffer.EndOfBuffer, "Expected end of buffer.");

						_buffer.SafeDispose();
						_assetsCreated = true;
						_state = State.AssetsInitialized;
						_fonts = Fonts.ToArray();
						break;
					}
					case State.AssetsInitialized:
						return true;
					default:
						throw new InvalidOperationException("Unexpected state.");
				}
			}
			catch (Exception e)
			{
				Log.Die("Failed to load asset bundle '{0}'. {1}", _bundleFile, e.Message);
			}

			return true;
		}

		/// <summary>
		///     Creates the asset with the given number.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to initialize the asset.</param>
		/// <param name="buffer">The buffer the asset should be initialized from.</param>
		/// <param name="assetNumber">The number of the asset that should be initialized.</param>
		protected abstract void CreateAsset(GraphicsDevice graphicsDevice, ref BufferReader buffer, ushort assetNumber);

		/// <summary>
		///     Reloads the asset with the given number.
		/// </summary>
		/// <param name="buffer">The buffer the asset should be initialized from.</param>
		/// <param name="assetNumber">The number of the asset that should be initialized.</param>
		protected abstract void ReloadAsset(ref BufferReader buffer, ushort assetNumber);

		/// <summary>
		///     Loads the asset bundle synchronously.
		/// </summary>
		public void Load()
		{
			while (!LoadingCompleted)
				LoadAsync();
		}

		/// <summary>
		///     Check if there's enough time for another asset; that's just a wild guess, obviously, but we assume that we're
		///     good if we've more than a certain percentage of the time left.
		/// </summary>
		/// <param name="start">The system time when loading was started.</param>
		/// <param name="timeout">The loading timeout.</param>
		private static bool ContinueLoading(double start, double timeout)
		{
			const double percentage = 0.2;
			return Clock.GetTime() - start < timeout - (timeout * percentage);
		}

		/// <summary>
		///     Checks whether the next asset is of the expected type.
		/// </summary>
		/// <param name="expectedType">The expected type identifier.</param>
		/// <param name="expectedTypeName">The user-friendly name of the expected asset type.</param>
		protected void CheckAssetType(byte expectedType, string expectedTypeName)
		{
			if (_buffer.ReadByte() != expectedType)
				Log.Die("Asset bundle is corrupted: Encountered unexpected asset type instead of '{0}'.", expectedTypeName);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_renderContext.RemoveAssetBundle(this);
		}

		/// <summary>
		///     Gets a font matching the given search criteria.
		/// </summary>
		/// <param name="family">The family of the font.</param>
		/// <param name="size">The size of the font.</param>
		/// <param name="bold">Indicates whether the font should be bold.</param>
		/// <param name="italic">Indicates whether the font should be italic.</param>
		/// <param name="aliased">Indicates whether the font should be aliased.</param>
		internal Font GetFont(string family, int size, bool bold, bool italic, bool aliased)
		{
			Assert.ArgumentNotNullOrWhitespace(family);

			foreach (var font in _fonts)
			{
				if (font.Family == family && font.Size == size && font.Bold == bold && font.Italic == italic && font.Aliased == aliased)
					return font;
			}

			return null;
		}

		/// <summary>
		///     Describes the state of the asset bundle.
		/// </summary>
		private enum State : byte
		{
			/// <summary>
			///     Indicates that the asset bundle has not yet been loaded.
			/// </summary>
			Unloaded,

			/// <summary>
			///     Indicates that the asset bundle's data is being loaded from the file system.
			/// </summary>
			LoadingBundle,

			/// <summary>
			///     Indicates that the assets are being initialized.
			/// </summary>
			InitializingAssets,

			/// <summary>
			///     Indicates that the asset bundle is fully initialized.
			/// </summary>
			AssetsInitialized
		}
	}
}