using System;

namespace Pegasus.AssetsCompiler.Fonts
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;

	/// <summary>
	///   Represents a typeface in a given style.
	/// </summary>
	internal class FontFace : DisposableObject
	{
		/// <summary>
		///   The native freetype font instance.
		/// </summary>
		private readonly FreeType.FreeTypeFace _font;

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
		public FontFace(IntPtr font, int size, RenderMode renderMode)
		{
			Assert.ArgumentNotNull(font);
			Assert.InRange(renderMode);

			_fontPtr = font;
			_font = (FreeType.FreeTypeFace)Marshal.PtrToStructure(font, typeof(FreeType.FreeTypeFace));

			FreeType.Invoke(() => FreeType.SetPixelSize(_fontPtr, 0, (uint)size));

			// Add the printable ASCII-glyphs
			for (var character = (char)0; character < 256; ++character)
				AddGlyph(renderMode, character);

			// Add the 'box' glyph that is used to show non-printable or non-supported characters
			AddGlyph(renderMode, '□');
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
			_glyphs.Add(new Glyph(glyphInfo, character, renderMode));
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