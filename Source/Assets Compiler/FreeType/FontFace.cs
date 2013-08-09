using System;

namespace Pegasus.AssetsCompiler.FreeType
{
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
	using Framework;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;

	/// <summary>
	///   Represents a typeface in a given style.
	/// </summary>
	internal class FontFace : DisposableObject
	{
		/// <summary>
		///   The native freetype font instance.
		/// </summary>
		private readonly NativeMethods.FreeTypeFace _font;

		/// <summary>
		///   The native freetype font instance pointer.
		/// </summary>
		private readonly IntPtr _fontPtr;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="font">The native freetype font instance.</param>
		public FontFace(IntPtr font)
		{
			_fontPtr = font;
			_font = (NativeMethods.FreeTypeFace)Marshal.PtrToStructure(font, typeof(NativeMethods.FreeTypeFace));
		}

		/// <summary>
		///   Sets the size (in pixels) of the characters.
		/// </summary>
		public int Size
		{
			set { NativeMethods.Invoke(() => NativeMethods.SetPixelSize(_fontPtr, 0, (uint)value)); }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.Invoke(() => NativeMethods.DisposeFace(_fontPtr));
		}

		/// <summary>
		///   Gets the bitmap for the glyph corresponding to the given character.
		/// </summary>
		/// <param name="character">The character the bitmap should be returned for.</param>
		/// <param name="renderMode">Indicates whether anti-aliasing should be used when rendering the glyph.</param>
		public Bitmap GetGlyphBitmap(char character, RenderMode renderMode)
		{
			Assert.InRange(renderMode);

			var glyphIndex = NativeMethods.GetGlyphIndex(_fontPtr, character);
			if (glyphIndex == 0)
				Log.Die("The font does not contain a glyph for '{0}'.", character);

			var loadFlags = renderMode == RenderMode.Aliased ? NativeMethods.LoadTargetMono : NativeMethods.LoadTargetNormal;
			NativeMethods.Invoke(() => NativeMethods.LoadGlyph(_fontPtr, glyphIndex, loadFlags));
			NativeMethods.Invoke(() => NativeMethods.RenderGlyph(_font.glyph, renderMode));

			var glyph = (NativeMethods.GlyphSlot)Marshal.PtrToStructure(_font.glyph, typeof(NativeMethods.GlyphSlot));

			switch (renderMode)
			{
				case RenderMode.Aliased:
					return GetAliasedBitmap(glyph.bitmap);
				case RenderMode.Antialiased:
					return GetAntiAliasedBitmap(glyph.bitmap);
				default:
					throw new InvalidOperationException("Unknown render mode.");
			}
		}

		/// <summary>
		///   Gets an aliased bitmap from the given glyph bitmap.
		/// </summary>
		/// <param name="glyph">The glyph the should be copied to a bitmap.</param>
		private static unsafe Bitmap GetAliasedBitmap(NativeMethods.FreeTypeBitmap glyph)
		{
			using (var bitmap = new Bitmap(glyph.width, glyph.rows, PixelFormat.Format1bppIndexed))
			{
				var locked = bitmap.LockBits(new Rectangle(0, 0, glyph.width, glyph.rows), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

				var target = (byte*)locked.Scan0.ToPointer();
				var destination = (byte*)glyph.buffer.ToPointer();

				for (var x = 0; x < glyph.pitch; ++x)
					for (var y = 0; y < glyph.rows; ++y)
						target[y * locked.Stride + x] = destination[y * glyph.pitch + x];

				bitmap.UnlockBits(locked);

				bitmap.Palette.Entries[0] = Color.FromArgb(0, 0, 0, 0);
				bitmap.Palette.Entries[1] = Color.FromArgb(1, 0, 0, 0);

				var rgbaBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
				for (var x = 0; x < bitmap.Width; ++x)
				{
					for (var y = 0; y < bitmap.Height; ++y)
					{
						var color = bitmap.GetPixel(x, y);
						var value = color.R == 255 ? 255 : 0;
						color = Color.FromArgb(value, value, value, value);
						rgbaBitmap.SetPixel(x, y, color);
					}
				}

				return rgbaBitmap;
			}
		}

		/// <summary>
		///   Gets an anti-aliased bitmap from the given glyph bitmap.
		/// </summary>
		/// <param name="glyph">The glyph the should be copied to a bitmap.</param>
		private static unsafe Bitmap GetAntiAliasedBitmap(NativeMethods.FreeTypeBitmap glyph)
		{
			var bitmap = new Bitmap(glyph.width, glyph.rows, PixelFormat.Format32bppArgb);
			var destination = (byte*)glyph.buffer.ToPointer();

			for (var x = 0; x < glyph.pitch; ++x)
			{
				for (var y = 0; y < glyph.rows; ++y)
				{
					var value = destination[y * glyph.pitch + x];
					bitmap.SetPixel(x, y, Color.FromArgb(value, 255,255,255));
				}
			}

			return bitmap;
		}
	}
}