﻿namespace Pegasus.AssetsCompiler.Fonts
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Platform.Logging;

	/// <summary>
	///     Represents a typeface in a given style.
	/// </summary>
	internal class Font : IDisposable
	{
		/// <summary>
		///     The native freetype font instance.
		/// </summary>
		private readonly FreeType.Face _font;

		/// <summary>
		///     The native freetype font instance pointer.
		/// </summary>
		private readonly IntPtr _fontPtr;

		/// <summary>
		///     The printable ASCII-glyphs.
		/// </summary>
		private readonly List<Glyph> _glyphs = new List<Glyph>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="font">The native freetype font instance.</param>
		/// <param name="metadata">The font metadata.</param>
		public Font(IntPtr font, FontMetadata metadata)
		{
			Assert.ArgumentNotNull(font);

			_fontPtr = font;
			_font = (FreeType.Face)Marshal.PtrToStructure(font, typeof(FreeType.Face));

			FreeType.Invoke(() => FreeType.SetPixelSize(_fontPtr, 0, (uint)metadata.Size));

			// Add the glyph that is used to show non-printable or non-supported characters; must be the first glyph
			var renderMode = !metadata.Aliased ? RenderMode.Antialiased : RenderMode.Aliased;
			AddGlyph(renderMode, metadata.InvalidChar);

			// Add the printable ASCII-glyphs
			foreach (var character in metadata.Characters)
				AddGlyph(renderMode, character);
		}

		/// <summary>
		///     Gets the line height of the font.
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
		///     Gets the base line offset for the font.
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
		///     Gets a value indicating whether the font provides kerning information.
		/// </summary>
		public bool HasKerning
		{
			get { return (_font.face_flags & FreeType.Kerning) == FreeType.Kerning; }
		}

		/// <summary>
		///     Gets the printable ASCII-glyphs.
		/// </summary>
		public IEnumerable<Glyph> Glyphs
		{
			get { return _glyphs; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			FreeType.Invoke(() => FreeType.DisposeFace(_fontPtr));
			foreach (var glyph in _glyphs)
				glyph.Dispose();
		}

		/// <summary>
		///     Adds the glyph for the given character.
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
		///     Gets the kerning offset (in X direction) for the two glyphs.
		/// </summary>
		/// <param name="left">The left glyph.</param>
		/// <param name="right">The right glyph.</param>
		public int GetKerning(Glyph left, Glyph right)
		{
			Assert.ArgumentNotNull(left);
			Assert.ArgumentNotNull(right);

			var kerning = new FreeType.Vector();
			FreeType.Invoke(() => FreeType.GetKerning(_fontPtr, left.Index, right.Index, 0, out kerning));
			return kerning.x / 64;
		}
	}
}