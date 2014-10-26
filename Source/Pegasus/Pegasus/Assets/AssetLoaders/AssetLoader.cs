namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Platform;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Asset loaders are used by assets managers to load assets from disk.
	/// </summary>
	[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	public abstract class AssetLoader
	{
		/// <summary>
		///     The asset loaders that can be used to load assets.
		/// </summary>
		private static readonly Dictionary<byte, AssetLoader> Loaders = new Dictionary<byte, AssetLoader>();

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static AssetLoader()
		{
			var loaders = from assembly in AppDomain.CurrentDomain.GetAssemblies()
						  from type in assembly.GetTypes()
						  where type.IsClass && !type.IsAbstract && typeof(AssetLoader).IsAssignableFrom(type)
						  select (AssetLoader)Activator.CreateInstance(type);

			foreach (var loader in loaders)
				Register(loader);
		}

		/// <summary>
		///     Gets the type of the asset supported by the loader.
		/// </summary>
		public byte AssetType { get; protected set; }

		/// <summary>
		///     Gets the name of the asset type supported by the loader.
		/// </summary>
		public string AssetTypeName { get; protected set; }

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public abstract void Load(BufferReader buffer, object asset, string assetName);

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		/// <param name="assetName">The name of the asset.</param>
		public abstract IDisposable Allocate(GraphicsDevice graphicsDevice, string assetName);

		/// <summary>
		///     Gets an asset loader that can allocate and load an asset of the given type.
		/// </summary>
		/// <param name="assetType">The type of the asset that should be loaded.</param>
		internal static AssetLoader Get(byte assetType)
		{
			Assert.That(Loaders.ContainsKey(assetType), "No asset loader could be found for an asset of type '{0}'.", assetType);
			return Loaders[assetType];
		}

		/// <summary>
		///     Registers an asset loader that assets manager can subsequently use to load assets.
		/// </summary>
		/// <param name="assetLoader">The asset loader that should be registered.</param>
		private static void Register(AssetLoader assetLoader)
		{
			Assert.ArgumentNotNull(assetLoader);
			Assert.That(!Loaders.ContainsKey(assetLoader.AssetType), "An asset loader for this asset type has already been registered.");

			Loaders.Add(assetLoader.AssetType, assetLoader);
		}

		/// <summary>
		///     Reads and validates the asset file header in the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the asset file header should be read from.</param>
		/// <param name="assetType">The type of the asset that is expected to follow in the buffer.</param>
		internal static void ValidateHeader(BufferReader buffer, byte assetType)
		{
			Assert.ArgumentNotNull(buffer);

			if (!buffer.CanRead(5))
				throw new InvalidOperationException("Asset is corrupted: Header information missing.");

			if (buffer.ReadByte() != 'p' || buffer.ReadByte() != 'g')
				throw new InvalidOperationException("Asset is corrupted: Application identifier mismatch in asset file header.");

			var assetVersion = buffer.ReadUInt16();
			if (assetVersion < PlatformInfo.AssetFileVersion)
				throw new InvalidOperationException("Asset is stored in an outdated version of the compiled asset format and must be re-compiled.");

			if (assetVersion > PlatformInfo.AssetFileVersion)
				throw new InvalidOperationException("Asset is stored in a newer version of the compiled asset format.");

			var actualType = buffer.ReadByte();
			if (actualType != assetType)
				throw new InvalidOperationException("Unexpected asset type stored in asset file.");
		}
	}
}