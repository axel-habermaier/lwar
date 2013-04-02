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
		///   Loads or reloads the asset using the given asset buffer.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		internal override unsafe void Load(BufferReader buffer)
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

			Assert.That(buffer.EndOfBuffer, "Not all data has been read.");
			if (Texture == null)
				Texture = _createTexture(GraphicsDevice);

			Texture.Reinitialize(description, surfaces);
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