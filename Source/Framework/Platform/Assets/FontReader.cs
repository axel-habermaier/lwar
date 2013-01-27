using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Math;
	using Rendering.UserInterface;

    /// <summary>
    ///   Loads compiled fonts.
    /// </summary>
    internal sealed class FontReader
    {
        /// <summary>
        ///   The asset manager that should be used to load the font textures.
        /// </summary>
        private readonly AssetsManager _assets;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="assets">The asset manager that should be used to load the font texture.</param>
        public FontReader(AssetsManager assets)
        {
            Assert.ArgumentNotNull(assets, () => assets);
            _assets = assets;
        }

        /// <summary>
        ///   Loads a font.
        /// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the font.</param>
        public Font Load(AssetReader assetReader)
        {
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

            var textureName = reader.ReadString();
            var texture = _assets.LoadTexture2D(textureName);

            return new Font(glyphs, lowestGlyphId, kernings, texture, lineHeight);
        }
    }
}