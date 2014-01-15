namespace Pegasus.Rendering.UserInterface
{
	using System;
	using Math;

	/// <summary>
	///     Provides the metadata for a glyph used by a font.
	/// </summary>
	internal struct Glyph
	{
		/// <summary>
		///     The number of pixels to advance the drawing cursor after drawing this glyph.
		/// </summary>
		public short AdvanceX;

		/// <summary>
		///     The area of the glyph, relative to the current drawing cursor.
		/// </summary>
		public Rectangle Area;

		/// <summary>
		///     The amount of kerning pairs with this glyph as the first partner.
		/// </summary>
		public int KerningCount;

		/// <summary>
		///     The index of the first kerning pair with this glyph as the first partner.
		/// </summary>
		public int KerningStart;

		/// <summary>
		///     The area of the font texture that contains the glyph's image data.
		/// </summary>
		public RectangleF TextureArea;

		/// <summary>
		///     Gets a value indicating whether the glyph is invalid, as not all fonts support all glyph.
		/// </summary>
		public bool IsInvalid
		{
			get { return Area == Rectangle.Empty && TextureArea == RectangleF.Empty; }
		}
	}
}