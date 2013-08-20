using System;

namespace Pegasus.AssetsCompiler.Fonts
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Represents a typeface in a given style.
	/// </summary>
	internal class Font : DisposableObject
	{
		/// <summary>
		///   The native freetype font instance.
		/// </summary>
		private readonly FreeType.Face _font;

		/// <summary>
		///   The native freetype font instance pointer.
		/// </summary>
		private readonly IntPtr _fontPtr;

		/// <summary>
		///   The printable ASCII-glyphs.
		/// </summary>
		private readonly List<Glyph> _glyphs = new List<Glyph>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="font">The native freetype font instance.</param>
		/// <param name="size">The size (in pixels) of the characters.</param>
		/// <param name="renderMode">Indicates whether anti-aliasing should be used when rendering the glyphs.</param>
		public Font(IntPtr font, int size, RenderMode renderMode)
		{
			Assert.ArgumentNotNull(font);
			Assert.InRange(renderMode);

			_fontPtr = font;
			_font = (FreeType.Face)Marshal.PtrToStructure(font, typeof(FreeType.Face));

			FreeType.Invoke(() => FreeType.SetPixelSize(_fontPtr, 0, (uint)size));

			// Add the printable ASCII-glyphs
			for (var character = (char)0; character < 256; ++character)
				AddGlyph(renderMode, character);

			// Add the 'box' glyph that is used to show non-printable or non-supported characters
			AddGlyph(renderMode, '□');
		}

		/// <summary>
		///   Gets the line height of the font.
		/// </summary>
		public ushort LineHeight
		{
			get
			{
				var metrics = (FreeType.Size)Marshal.PtrToStructure(_font.size, typeof(FreeType.Size));
				return (ushort)(metrics.height / 64);
			}
		}

		/// <summary>
		///   Gets the base line offset for the font.
		/// </summary>
		public ushort Baseline
		{
			get
			{
				var metrics = (FreeType.Size)Marshal.PtrToStructure(_font.size, typeof(FreeType.Size));
				return (ushort)(LineHeight + metrics.descender / 64);
			}
		}

		/// <summary>
		///   Gets a value indicating whether the font provides kerning information.
		/// </summary>
		public bool HasKerning
		{
			get { return (_font.face_flags & FreeType.Kerning) == FreeType.Kerning; }
		}

		/// <summary>
		///   Gets the printable ASCII-glyphs.
		/// </summary>
		public IEnumerable<Glyph> Glyphs
		{
			get { return _glyphs; }
		}

		/// <summary>
		///   Adds the glyph for the given character.
		/// </summary>
		/// <param name="character">The character whose corresponding glyph should be added.</param>
		/// <param name="renderMode">Indicates whether anti-aliasing should be used when rendering the glyph.</param>
		private void AddGlyph(RenderMode renderMode, char character)
		{
			if (Char.IsControl(character))
				return;

			var glyphIndex = FreeType.GetGlyphIndex(_fontPtr, character);
			if (glyphIndex == 0)
				Log.Die("The font does not contain a glyph for '{0}'.", character);

			var loadFlags = renderMode == RenderMode.Aliased ? FreeType.LoadTargetMono : FreeType.LoadTargetNormal;
			FreeType.Invoke(() => FreeType.LoadGlyph(_fontPtr, glyphIndex, loadFlags));
			FreeType.Invoke(() => FreeType.RenderGlyph(_font.glyph, renderMode));

			var glyphInfo = (FreeType.GlyphSlot)Marshal.PtrToStructure(_font.glyph, typeof(FreeType.GlyphSlot));
			_glyphs.Add(new Glyph(glyphInfo, glyphIndex, character, renderMode));
		}

		/// <summary>
		///   Gets the kerning offset (in X direction) for the two glyphs.
		/// </summary>
		/// <param name="left">The left glyph.</param>
		/// <param name="right">The right glyph.</param>
		public int GetKerning(Glyph left, Glyph right)
		{
			Assert.ArgumentNotNull(left);
			Assert.ArgumentNotNull(right);

			var kerning = new FreeType.Vector();
			FreeType.Invoke(() => FreeType.GetKerning(_fontPtr, left.Index, right.Index, 0, out kerning));
			return (int)kerning.x / 64;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			FreeType.Invoke(() => FreeType.DisposeFace(_fontPtr));
			_glyphs.SafeDisposeAll();
		}
	}
}