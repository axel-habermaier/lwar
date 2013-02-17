using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a texture asset.
	/// </summary>
	internal class TextureAsset<T> : Asset
		where T : class, IDisposable
	{
		/// <summary>
		///   Creates a new texture object.
		/// </summary>
		private readonly Func<GraphicsDevice, byte[], int, int, SurfaceFormat, T> _create;

		/// <summary>
		///   Reinitializes the texture object.
		/// </summary>
		private readonly Action<T, byte[], int, int, SurfaceFormat> _reinitialize;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="create">Creates a new texture object.</param>
		/// <param name="reinitialize">Reinitializes the texture object.</param>
		public TextureAsset(Func<GraphicsDevice, byte[], int, int, SurfaceFormat, T> create,
							Action<T, byte[], int, int, SurfaceFormat> reinitialize)
		{
			Assert.ArgumentNotNull(create, () => create);
			Assert.ArgumentNotNull(reinitialize, () => reinitialize);

			_create = create;
			_reinitialize = reinitialize;
		}

		/// <summary>
		///   The texture that is managed by this asset instance.
		/// </summary>
		internal T Texture { get; private set; }

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "2D Texture"; }
		}

		/// <summary>
		///   Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the asset.</param>
		internal override void Load(AssetReader assetReader)
		{
			Assert.ArgumentNotNull(assetReader, () => assetReader);

			var reader = assetReader.Reader;
			var width = reader.ReadInt32();
			var height = reader.ReadInt32();
			var format = (SurfaceFormat)reader.ReadInt32();
			var data = reader.ReadByteArray();

			if (Texture == null)
				Texture = _create(GraphicsDevice, data, width, height, format);

			_reinitialize(Texture, data, width, height, format);

			for (var i = 0; i < GraphicsDevice.State.Textures.Length; ++i)
				GraphicsDevice.State.Textures[i] = null;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Texture.SafeDispose();
		}
	}
}