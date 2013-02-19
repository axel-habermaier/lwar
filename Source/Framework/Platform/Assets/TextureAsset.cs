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
		private readonly Func<GraphicsDevice, SurfaceFormat, Mipmap[], T> _create;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="create">Creates a new texture object.</param>
		protected TextureAsset(Func<GraphicsDevice, SurfaceFormat, Mipmap[], T> create)
		{
			Assert.ArgumentNotNull(create, () => create);
			_create = create;
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
			var format = (SurfaceFormat)reader.ReadInt32();
			var mipmaps = new Mipmap[GraphicsDevice.MaxMipmaps];
			var count = 0;

			for (; count < GraphicsDevice.MaxMipmaps; ++count)
			{
				mipmaps[count] = new Mipmap
				{
					Level = reader.ReadInt32(),
					Width = reader.ReadInt32(),
					Height = reader.ReadInt32(),
					Size = reader.ReadInt32(),
				};

				mipmaps[count].Data = new byte[mipmaps[count].Size];
				reader.Copy(mipmaps[count].Data);
			}

			if (Texture == null)
				Texture = _create(GraphicsDevice, format, mipmaps);
			else
				Texture.Reinitialize(format, mipmaps);

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