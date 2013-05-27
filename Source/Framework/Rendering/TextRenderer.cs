using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;
	using UserInterface;

	/// <summary>
	///   Improves text drawing performance by caching the quads of a text.
	/// </summary>
	internal struct TextRenderer
	{
		/// <summary>
		///   The text color.
		/// </summary>
		private Color _color;

		/// <summary>
		///   The number of cached quads.
		/// </summary>
		private int _numQuads;

		/// <summary>
		///   The quads of the text.
		/// </summary>
		private Quad[] _quads;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="color">The text color.</param>
		public TextRenderer(Color color)
			: this()
		{
			_color = color;
		}

		/// <summary>
		///   Gets or sets the text color.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;

				for (var i = 0; _quads != null && i < _numQuads; ++i)
					_quads[i].SetColor(value);
			}
		}

		/// <summary>
		///   Rebuilds the cache of the layouted text quads for more efficient rendering.
		/// </summary>
		/// <param name="font">
		///   The font that was used to layout the text and that should be used
		///   to draw the text.
		/// </param>
		/// <param name="text">The text that was layouted and should be drawn.</param>
		/// <param name="layoutData">The layouting data for the individual characters of the text.</param>
		internal void RebuildCache(Font font, string text, Rectangle[] layoutData)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(text);
			Assert.ArgumentNotNull(layoutData);
			Assert.That(text.Length <= layoutData.Length, "Layout data missing.");

			_numQuads = 0;
			if (String.IsNullOrWhiteSpace(text))
				return;

			// Ensure that the quads list does not have to be resized by settings its capacity to the number of
			// characters; however, this wastes some space as not all characters generate quads
			if (_quads == null || text.Length >= _quads.Length)
				_quads = new Quad[text.Length];

			Quad quad;
			for (var i = 0; i < text.Length; ++i)
			{
				if (font.CreateGlyphQuad(text[i], ref layoutData[i], _color, out quad))
					_quads[_numQuads++] = quad;
			}
		}

		/// <summary>
		///   Draws a single line of non-layouted, non-cached text.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the text.</param>
		/// <param name="font">The font that should be used to draw the text.</param>
		/// <param name="text">The text that should be drawn.</param>
		/// <param name="color">The color that should be used to draw the text.</param>
		/// <param name="position">The position of the text's top left corner.</param>
		internal static void Draw(SpriteBatch spriteBatch, Font font, string text, Color color, Vector2i position)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(text);

			if (String.IsNullOrWhiteSpace(text))
				return;

			Quad quad;
			for (var i = 0; i < text.Length; ++i)
			{
				var area = font.GetGlyphArea(text, 0, i, ref position);

				if (font.CreateGlyphQuad(text[i], ref area, color, out quad))
					spriteBatch.Draw(ref quad, font.Texture);
			}
		}

		/// <summary>
		///   Draws the cached text.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the text.</param>
		/// <param name="texture">The font texture.</param>
		internal void DrawCached(SpriteBatch spriteBatch, Texture2D texture)
		{
			if (_numQuads != 0)
				spriteBatch.Draw(_quads, _numQuads, texture);
		}
	}
}