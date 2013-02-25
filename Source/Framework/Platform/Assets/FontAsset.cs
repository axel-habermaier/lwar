using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Math;
	using Rendering.UserInterface;

	/// <summary>
	///   Represents a font asset.
	/// </summary>
	internal sealed class FontAsset : Asset
	{
		/// <summary>
		///   The font texture.
		/// </summary>
		private Texture2DAsset _texture;

		/// <summary>
		///   The font that is managed by this asset instance.
		/// </summary>
		internal Font Font { get; private set; }

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "Font"; }
		}

		/// <summary>
		///   Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the asset.</param>
		internal override void Load(AssetReader assetReader)
		{
			Assert.ArgumentNotNull(assetReader, () => assetReader);

			var reader = assetReader.Reader;

			// Load the font metadata
			var scaleW = reader.ReadUInt16();
			var scaleH = reader.ReadUInt16();
			var lineHeight = reader.ReadUInt16();

			// Load the glyph metadata
			var numGlyphs = reader.ReadUInt16();
			var lowestGlyphId = reader.ReadUInt16();
			var highestGlyphId = reader.ReadUInt16();
			var glyphs = new Glyph[highestGlyphId - lowestGlyphId + 1];

			for (var i = 0; i < numGlyphs; ++i)
			{
				var id = reader.ReadUInt16();
				var index = id - lowestGlyphId;

				glyphs[index].Area.Width = reader.ReadUInt16();
				glyphs[index].Area.Height = reader.ReadUInt16();
				glyphs[index].Area.Left = reader.ReadInt16();
				glyphs[index].Area.Top = reader.ReadInt16();
				glyphs[index].AdvanceX = reader.ReadInt16();

				var x = reader.ReadUInt16();
				var y = reader.ReadUInt16();

				var textureLeft = x / (float)scaleW;
				var textureRight = (x + glyphs[index].Area.Width) / (float)scaleW;
				var textureTop = (y + glyphs[index].Area.Height) / (float)scaleH;
				var textureBottom = y / (float)scaleH;

				var textureArea = new RectangleF(textureLeft, textureTop,
												 textureRight - textureLeft, textureBottom - textureTop);

				glyphs[index].TextureArea = textureArea;
			}

			// Load the kerning data
			var kerningCount = reader.ReadUInt16();
			KerningPair[] kernings = null;
			if (kerningCount != 0)
			{
				kernings = new KerningPair[kerningCount];

				for (var i = 0; i < kerningCount; ++i)
				{
					var first = reader.ReadUInt16();
					var second = reader.ReadUInt16();
					var offset = reader.ReadInt16();

					kernings[i] = new KerningPair((char)first, (char)second, offset);

					if (glyphs[first - lowestGlyphId].KerningStart == 0)
						glyphs[first - lowestGlyphId].KerningStart = i;

					++glyphs[first - lowestGlyphId].KerningCount;
				}
			}

			_texture.SafeDispose();
			_texture = new Texture2DAsset { GraphicsDevice = GraphicsDevice, Assets = Assets };
			_texture.Load(assetReader);

			if (Font == null)
				Font = new Font(glyphs, lowestGlyphId, kernings, _texture.Texture, lineHeight);
			else
				Font.Reinitialize(glyphs, lowestGlyphId, kernings, _texture.Texture, lineHeight);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_texture.SafeDispose();
		}
	}
}