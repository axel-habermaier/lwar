using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;
	using Math;

	/// <summary>
	///   Represents a texture asset.
	/// </summary>
	internal sealed class Texture2DAsset : Asset
	{
		/// <summary>
		///   The texture that is managed by this asset instance.
		/// </summary>
		internal Texture2D Texture { get; private set; }

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
			var width = reader.ReadUInt16();
			var height = reader.ReadUInt16();
			var componentCount = reader.ReadByte();

			var length = width * height * componentCount;
			var data = new byte[length];
			for (var i = 0; i < length; ++i)
				data[i] = reader.ReadByte();

			var format = SurfaceFormat.Color;
			if (componentCount == 4)
				format = SurfaceFormat.Color;
			else
				Log.Die("All compiled textures should have 4 channels.");

			if (Texture == null)
				Texture = new Texture2D(GraphicsDevice, data, new Size(width, height), format);

			Texture.Reinitialize(data, new Size(width, height), format);

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