namespace Pegasus.AssetsCompiler.Fonts
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///     Represents a single character of a font face.
	/// </summary>
	internal class Glyph : DisposableObject
	{
		/// <summary>
		///     Creates the bitmap for the glyph.
		/// </summary>
		/// <param name="glyphInfo">Provides information about the glyph.</param>
		/// <param name="index">The index of the glyph in the font file.</param>
		/// <param name="character">The character of the glyph.</param>
		/// <param name="renderMode">Indicates whether anti-aliasing should be used when rendering the glyph.</param>
		public Glyph(FreeType.GlyphSlot glyphInfo, uint index, char character, RenderMode renderMode)
		{
			Assert.ArgumentSatisfies(!Char.IsControl(character), "The given character is not printable.");

			Index = index;
			Character = character;
			Size = new Size(glyphInfo.bitmap.width, glyphInfo.bitmap.rows);
			AdvanceX = (int)glyphInfo.advance_x / 64;
			OffsetX = glyphInfo.bitmap_left;
			OffsetY = glyphInfo.bitmap_top;

			if (Size.Width != 0 && Size.Height == 0 || Size.Width == 0 && Size.Height != 0)
				Log.Die("The width or height of the glyph for '{0}' is 0.", Character);

			// Use a 1x1 bitmap for glyphs without any visual representation.
			if (Size.Width == 0 && Size.Height == 0)
			{
				Bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
				Bitmap.SetPixel(0, 0, Color.FromArgb(0, 0, 0, 0));
				return;
			}

			// Copy the glyph bitmap into a .NET bitmap
			switch (renderMode)
			{
				case RenderMode.Aliased:
					Bitmap = GetAliasedBitmap(glyphInfo.bitmap);
					break;
				case RenderMode.Antialiased:
					Bitmap = GetAntiAliasedBitmap(glyphInfo.bitmap);
					break;
				default:
					throw new InvalidOperationException("Unknown render mode.");
			}
		}

		/// <summary>
		///     Gets the index of the glyph in the font file.
		/// </summary>
		public uint Index { get; private set; }

		/// <summary>
		///     Gets the offset in pixels from the drawing position to the leftmost border of the glyph.
		/// </summary>
		public int OffsetX { get; private set; }

		/// <summary>
		///     Gets the offset in pixels from the drawing position on the base line to the topmost border of the glyph.
		/// </summary>
		public int OffsetY { get; private set; }

		/// <summary>
		///     Gets the number of pixels that the pen should advance after drawing the glyph.
		/// </summary>
		public int AdvanceX { get; private set; }

		/// <summary>
		///     Gets the bitmap containing the glyph.
		/// </summary>
		public Bitmap Bitmap { get; private set; }

		/// <summary>
		///     Gets the character of the glyph.
		/// </summary>
		public char Character { get; private set; }

		/// <summary>
		///     Gets the size of the glyph.
		/// </summary>
		public Size Size { get; private set; }

		/// <summary>
		///     Gets an aliased bitmap from the given glyph bitmap.
		/// </summary>
		/// <param name="glyph">The glyph the should be copied to a bitmap.</param>
		private static unsafe Bitmap GetAliasedBitmap(FreeType.Bitmap glyph)
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
		///     Gets an anti-aliased bitmap from the given glyph bitmap.
		/// </summary>
		/// <param name="glyph">The glyph the should be copied to a bitmap.</param>
		private static unsafe Bitmap GetAntiAliasedBitmap(FreeType.Bitmap glyph)
		{
			var bitmap = new Bitmap(glyph.width, glyph.rows, PixelFormat.Format32bppArgb);
			var destination = (byte*)glyph.buffer.ToPointer();

			for (var x = 0; x < glyph.pitch; ++x)
			{
				for (var y = 0; y < glyph.rows; ++y)
				{
					var value = destination[y * glyph.pitch + x];
					bitmap.SetPixel(x, y, Color.FromArgb(value, 255, 255, 255));
				}
			}

			return bitmap;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Bitmap.SafeDispose();
		}
	}
}