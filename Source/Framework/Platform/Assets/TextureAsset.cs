using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a texture asset.
	/// </summary>
	internal abstract class TextureAsset<T> : Asset
		where T : Texture, IDisposable
	{
		/// <summary>
		///   Creates a new texture object.
		/// </summary>
		private readonly Func<GraphicsDevice, T> _createTexture;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="createTexture">Creates a new texture object.</param>
		protected TextureAsset(Func<GraphicsDevice, T> createTexture)
		{
			Assert.ArgumentNotNull(createTexture, () => createTexture);
			_createTexture = createTexture;
		}

		/// <summary>
		///   The texture that is managed by this asset instance.
		/// </summary>
		internal T Texture { get; private set; }

		/// <summary>
		///   Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the asset.</param>
		internal override void Load(AssetReader assetReader)
		{
			Assert.ArgumentNotNull(assetReader, () => assetReader);

			var reader = assetReader.Reader;
			

			if (Texture == null)
				Texture = _createTexture(GraphicsDevice);
		
			//Texture.Reinitialize(format, mipmaps);

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