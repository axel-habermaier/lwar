namespace Pegasus.AssetsCompiler.Fonts
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using Assets;
	using Commands;
	using Compilers;
	using Utilities;

	/// <summary>
	///     Represents a font as a bitmap, containing all glyphs of a given font.
	/// </summary>
	internal class FontMap
	{
		/// <summary>
		///     The padding around the glyphs in the font map in pixels.
		/// </summary>
		private const int Padding = 1;

		/// <summary>
		///     The glyphs stored in the font map.
		/// </summary>
		private readonly GlyphArea[] _glyphAreas;

		/// <summary>
		///     The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.
		/// </summary>
		private readonly string _path;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that should be stored in the font map.</param>
		/// <param name="path">The path to the asset relative to the asset source directory, i.e., Textures/Tex.png.</param>
		public FontMap(Font font, string path)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNullOrWhitespace(path);

			_path = path;
			_glyphAreas = font.Glyphs.Select(glyph => new GlyphArea { Glyph = glyph }).ToArray();

			Layout();
			GenerateBitmap();
		}

		/// <summary>
		///     Gets the size of the font map.
		/// </summary>
		public Size Size { get; private set; }

		/// <summary>
		///     Determines the size of the font map and the layout of the glyphs.
		/// </summary>
		private void Layout()
		{
			// Start with a small power-of-two size and double either width or height (the smaller one) when the glyphs don't fit.
			Size = new Size(64, 64);

			// The layouting algorithm is a simple line-based algorithm, although the glyphs are sorted by height 
			// before the layouting; this hopefully results in all lines being mostly occupied
			var glyphAreas = _glyphAreas.OrderBy(g => g.Glyph.Size.Height).ToArray();

			bool allFit;
			do
			{
				var x = 0;
				var y = 0;
				var lineHeight = 0;

				if (Size.Width <= Size.Height)
					Size = new Size(Size.Width * 2, Size.Height);
				else
					Size = new Size(Size.Width, Size.Height * 2);

				foreach (var info in glyphAreas)
				{
					var width = info.Glyph.Size.Width;
					var height = info.Glyph.Size.Height;

					if (width == 0 && height == 0)
						continue;

					// Check if there is enough horizontal space left, otherwise start a new line
					if (x + width > Size.Width)
					{
						x = 0;
						y += lineHeight;
						lineHeight = 0;
					}

					// Store the area
					info.Area = new Rectangle(x, y, width, height);

					// Advance the current position
					x += width + Padding;
					lineHeight = Math.Max(lineHeight, height + 1);
				}

				allFit = y + lineHeight <= Size.Height;
			} while (!allFit);
		}

		/// <summary>
		///     Generates the bitmap storing all glyphs and writes it to the temporary asset directory.
		/// </summary>
		private void GenerateBitmap()
		{
			using (var bitmap = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb))
			using (var graphics = Graphics.FromImage(bitmap))
			{
				foreach (var info in _glyphAreas.Where(g => g.Area.Width != 0 && g.Area.Height != 0))
					graphics.DrawImage(info.Glyph.Bitmap, new Point(info.Area.Left, info.Area.Top));

				bitmap.Save(Path.Combine(Configuration.TempDirectory, _path));
			}
		}

		/// <summary>
		///     Processes the font map and appends it to the given buffer.
		/// </summary>
		/// <param name="buffer"></param>
		public void Compile(AssetWriter buffer)
		{
			var asset = new Texture2DAsset(new XElement("Texture2D",
				new XAttribute("File", _path),
				new XAttribute("Compress", "false"),
				new XAttribute("GenerateMipmaps", "false")),
				Configuration.TempDirectory);

			var compiler = new Texture2DCompiler();
			compiler.CompileSingle(asset, buffer);
		}

		/// <summary>
		///     Gets the area that the glyph corresponding to the given character occupies in the font map.
		/// </summary>
		/// <param name="character">The character the glyph are should be returned for.</param>
		public Rectangle GetGlyphArea(char character)
		{
			var glyph = _glyphAreas.First(g => g.Glyph.Character == character);
			if (glyph == null)
				Log.Die("The font map does not contain a glyph for character '{0}'.", character);

			return glyph.Area;
		}

		/// <summary>
		///     Stores the area that a glyph occupies in the font map.
		/// </summary>
		private class GlyphArea
		{
			/// <summary>
			///     The area occupied by the glyph.
			/// </summary>
			public Rectangle Area;

			/// <summary>
			///     The glyph that occupies the area.
			/// </summary>
			public Glyph Glyph;
		}
	}
}
