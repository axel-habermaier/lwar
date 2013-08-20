using System;

namespace Pegasus.Rendering.UserInterface
{
	/// <summary>
	///   A kerning pair stores an position offset for two glyphs.
	/// </summary>
	internal struct KerningPair
	{
		/// <summary>
		///   The kerning offset.
		/// </summary>
		public readonly short Offset;

		/// <summary>
		///   The second glyph of the kerning pair.
		/// </summary>
		public readonly char SecondGlyph;

		/// <summary>
		///   The first glyph of the kerning pair.
		/// </summary>
		public char FirstGlyph;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="firstGlyph">The first glyph of the kerning pair.</param>
		/// <param name="secondGlyph">The second glyph of the kerning pair.</param>
		/// <param name="offset">The kerning offset.</param>
		public KerningPair(char firstGlyph, char secondGlyph, short offset)
		{
			FirstGlyph = firstGlyph;
			SecondGlyph = secondGlyph;
			Offset = offset;
		}
	}
}