namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Loads font assets.
	/// </summary>
	public class FontAssetLoader : AssetLoader
	{
		/// <summary>
		///     Gets the type of the asset supported by the loader.
		/// </summary>
		public override byte AssetType
		{
			get { return (byte)Assets.AssetType.Font; }
		}

		/// <summary>
		///     Gets the name of the asset type supported by the loader.
		/// </summary>
		public override string AssetTypeName
		{
			get { return "Font"; }
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override void Load(BufferReader buffer, object asset, string assetName)
		{
			Load(buffer, asset as Font, assetName);
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="font">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public static void Load(BufferReader buffer, Font font, string assetName)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNullOrWhitespace(assetName);

			// Load the font map
			AssetHeader.Validate(buffer, (byte)Assets.AssetType.Texture2D);
			TextureAssetLoader.Load(buffer, font.Texture, assetName);

			// Load the font metadata
			var lineHeight = buffer.ReadUInt16();

			// Load the glyph metadata
			var numGlyphs = buffer.ReadUInt16();
			var glyphs = new Font.Glyph[256];

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
			Font.KerningPair[] kernings = null;
			if (kerningCount != 0)
			{
				kernings = new Font.KerningPair[kerningCount];

				for (var i = 0; i < kerningCount; ++i)
				{
					var first = buffer.ReadUInt16();
					var second = buffer.ReadUInt16();
					var offset = buffer.ReadInt16();

					kernings[i] = new Font.KerningPair((char)first, (char)second, offset);

					if (glyphs[first].KerningStart == 0)
						glyphs[first].KerningStart = i;

					++glyphs[first].KerningCount;
				}
			}

			font.Reinitialize(glyphs, kernings, lineHeight);
		}

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		public override IDisposable Allocate(GraphicsDevice graphicsDevice)
		{
			return new Font(graphicsDevice);
		}
	}
}