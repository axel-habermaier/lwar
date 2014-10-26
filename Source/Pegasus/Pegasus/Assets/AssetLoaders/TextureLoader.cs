namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Loads texture assets.
	/// </summary>
	public abstract class TextureLoader : AssetLoader
	{
		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override unsafe void Load(BufferReader buffer, object asset, string assetName)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(asset);
			Assert.ArgumentOfType<Texture>(asset);
			Assert.ArgumentNotNullOrWhitespace(assetName);

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

			var texture = (Texture)asset;
			texture.Reinitialize(description, surfaces);
			texture.SetName(assetName);
		}
	}
}