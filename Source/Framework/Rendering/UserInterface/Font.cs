using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
    using Platform.Graphics;
    using Math;
    using Rendering;

    /// <summary>
    ///   Provides the metadata and operations for drawing text.
    /// </summary>
    public sealed class Font : DisposableObject
    {
        /// <summary>
        ///   The glyphs of the font.
        /// </summary>
        private readonly Glyph[] _glyphs;

        /// <summary>
        ///   Stores the kerning pairs in a space-efficient way.
        /// </summary>
        private readonly KerningPair[] _kerning;

        /// <summary>
        ///   The lowest glyph id.
        /// </summary>
        private readonly int _lowestGlyphId;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="glyphs">The glyphs of the font.</param>
        /// <param name="lowestGlyphId">The lowest glyph id.</param>
        /// <param name="kerning">The kerning information of the font.</param>
        /// <param name="texture">The texture containing the font's glyph images.</param>
        /// <param name="lineHeight">The height of a single line.</param>
        internal Font(Glyph[] glyphs, int lowestGlyphId, KerningPair[] kerning, Texture2D texture, int lineHeight)
        {
            _glyphs = glyphs;
            _lowestGlyphId = lowestGlyphId;
            _kerning = kerning;
            Texture = texture;
            LineHeight = lineHeight;
        }

        /// <summary>
        ///   Gets a value indicating whether the font supportes kerning.
        /// </summary>
        public bool KerningSupported
        {
            get { return _kerning != null; }
        }

        /// <summary>
        ///   Gets the line height.
        /// </summary>
        public int LineHeight { get; private set; }

        /// <summary>
        ///   Gets the font texture.
        /// </summary>
        internal Texture2D Texture { get; private set; }

        /// <summary>
        ///   Returns the width of the text.
        /// </summary>
        /// <param name="text">The text whose width should be computed.</param>
        public int MeasureWidth(string text)
        {
            Assert.ArgumentNotNull(text, () => text);
            Assert.NotDisposed(this);

            return MeasureWidth(text, 0, text.Length);
        }

        /// <summary>
        ///   Returns the width of the text from text[start] to text[end - 1].
        /// </summary>
        /// <param name="text">The text whose width should be computed.</param>
        /// <param name="start">The index of the first character.</param>
        /// <param name="end">The index of the first character that is not measured.</param>
        public int MeasureWidth(string text, int start, int end)
        {
            Assert.NotDisposed(this);
            Assert.ArgumentNotNull(text, () => text);
            Assert.ArgumentSatisfies(start >= 0, () => start, "Out of bounds.");
            Assert.ArgumentSatisfies(end <= text.Length, () => start, "Out of bounds.");
            Assert.That(start <= end, "Start must be less than or equal to end.");

            var width = 0;

            for (var i = start; i < end; ++i)
            {
                width += GetGlyph(text[i]).AdvanceX;

                if (KerningSupported && i > start)
                    width += GetKerningOffset(text[i - 1], text[i]);
            }

            return width;
        }

        /// <summary>
        ///   Gets the kerning offset for the character pair.
        /// </summary>
        /// <param name="first">The first character.</param>
        /// <param name="second">The second character.</param>
        private int GetKerningOffset(char first, char second)
        {
            var firstGlyph = GetGlyph(first);
            var end = firstGlyph.KerningStart + firstGlyph.KerningCount;

            for (var i = firstGlyph.KerningStart; i < end; ++i)
            {
                if (_kerning[i].SecondGlyph == second)
                    return _kerning[i].Offset;
            }

            return 0;
        }

        /// <summary>
        ///   Gets the area of the glyph that corresponds to the character at the given index of
        ///   the text. The start index is used to decide whether a kerning offset has to be
        ///   applied to the glyph position.
        /// </summary>
        /// <param name="text">The text for which the glyph are should be computed.</param>
        /// <param name="start">The first index of the text that is considered; required for font kerning.</param>
        /// <param name="index">The index of the character for which the glyph area should be returned.</param>
        /// <param name="offset">
        ///   The offset that should be applied to the glyph's position. The X-value of
        ///   the offset is updated to reflect the glyph width and the kerning offset.
        /// </param>
        /// <returns></returns>
		internal Rectangle GetGlyphArea(string text, int start, int index, ref Vector2i offset)
        {
            Assert.NotDisposed(this);
            Assert.ArgumentNotNull(text, () => text);
            Assert.ArgumentSatisfies(start >= 0, () => start, "Out of bounds.");
            Assert.ArgumentSatisfies(start < text.Length, () => start, "Out of bounds.");
            Assert.ArgumentSatisfies(index >= 0, () => index, "Out of bounds.");
            Assert.ArgumentSatisfies(index < text.Length, () => index, "Out of bounds.");
            Assert.That(start <= index, "Start must be less than or equal to index.");

            var glyph = GetGlyph(text[index]);
            var area = glyph.Area.Offset(offset);
            offset.X += glyph.AdvanceX;

            if (start == index || !KerningSupported)
                return area;

            var kerningOffset = GetKerningOffset(text[index - 1], text[index]);
            offset.X += kerningOffset;
            return area.Offset(kerningOffset, 0);
        }

        /// <summary>
        ///   Creates a quad for the character at the given index. Not all characters are visible, so
        ///   in some cases, no quad is created for a given index. The function returns true to indicate
        ///   that a valid quad was created; false, otherwise.
        /// </summary>
        /// <param name="character">The character for which the quad should be created.</param>
        /// <param name="area">The area of the glyph.</param>
        /// <param name="color">The color of the character.</param>
        /// <param name="quad">Returns the created quad.</param>
        internal bool CreateGlyphQuad(char character, ref Rectangle area, Color color, out Quad quad)
        {
            Assert.NotDisposed(this);

            // Spaces and new lines are invisible, so don't bother drawing them
            if (character == ' ' || character == '\n')
            {
                quad = new Quad();
                return false;
            }

            var glyph = GetGlyph(character);
            quad = new Quad(new RectangleF(area.Left, area.Top, area.Width, area.Height), color, glyph.TextureArea);

            return true;
        }

        /// <summary>
        ///   Gets the glyph instance for the given glyph character.
        /// </summary>
        /// <param name="glyph">The glyph character.</param>
        private Glyph GetGlyph(char glyph)
        {
            var index = glyph - _lowestGlyphId;

            // Ensure that a '.' is printed for all characters that are not supported by the font.
            if (index < 0 || index >= _glyphs.Length || _glyphs[index].IsInvalid)
                return GetGlyph('.'); // The font processor guarantees that '.' is supported by the font

            // New lines should not be visible at all, and should have no width
            if (glyph == '\n')
                return new Glyph();

            return _glyphs[index];
        }
    }
}