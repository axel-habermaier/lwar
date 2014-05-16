namespace Pegasus.Framework.UserInterface
{
	using System;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Represents layouted text.
	/// </summary>
	internal struct TextLayout
	{
		/// <summary>
		///     The alignment of the text within the desired drawing area.
		/// </summary>
		private TextAlignment _alignment;

		/// <summary>
		///     The layouted areas of the individual characters of the text.
		/// </summary>
		private Rectangle[] _characterAreas;

		/// <summary>
		///     The default text color.
		/// </summary>
		private Color _color;

		/// <summary>
		///     The desired drawing size of the text. If the text doesn't fit, it overlaps vertically.
		/// </summary>
		private Size _desiredSize;

		/// <summary>
		///     A value indicating whether the cached text quads are dirty.
		/// </summary>
		private bool _dirty;

		/// <summary>
		///     The font that is used to determine the size of the individual characters.
		/// </summary>
		private Font _font;

		/// <summary>
		///     The number of lines that are currently used.
		/// </summary>
		private int _lineCount;

		/// <summary>
		///     The amount of spacing between consecutive lines.
		/// </summary>
		private int _lineSpacing;

		/// <summary>
		///     The individual lines of the text.
		/// </summary>
		private TextLine[] _lines;

		/// <summary>
		///     The number of cached quads.
		/// </summary>
		private int _numQuads;

		/// <summary>
		///     The draw position of the text.
		/// </summary>
		private Vector2i _position;

		/// <summary>
		///     The quads of the text.
		/// </summary>
		private Quad[] _quads;

		/// <summary>
		///     The layouted text.
		/// </summary>
		private string _text;

		/// <summary>
		///     Indicates whether the text should be wrapped.
		/// </summary>
		private TextWrapping _wrapping;

		/// <summary>
		///     The actual text drawing size. Usually, the actual size is smaller than the desired size.
		///     If any words overlap, however, the actual size is bigger.
		/// </summary>
		public Size Size { get; private set; }

		/// <summary>
		///     Updated the text layout.
		/// </summary>
		/// <param name="font">The font that should be used to draw the size of the individual characters.</param>
		/// <param name="text">The text that should be layouted and measured.</param>
		/// <param name="desiredSize">The size of the desired drawing area of the text. If the text doesn't fit, it overlaps vertically.</param>
		/// <param name="lineSpacing">The amount of spacing between consecutive lines.</param>
		/// <param name="alignment">The alignment of the text within the desired drawing area.</param>
		/// <param name="wrapping">Indicates whether the text should be wrapped.</param>
		public void Update(Font font, string text, Size desiredSize, int lineSpacing, TextAlignment alignment, TextWrapping wrapping)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(text);
			Assert.ArgumentInRange(alignment);
			Assert.ArgumentInRange(wrapping);

			if (_font == font && _text == text && _desiredSize == desiredSize && _lineSpacing == lineSpacing &&
				_alignment == alignment && _wrapping == wrapping)
				return;

			_font = font;
			_text = text;
			_desiredSize = desiredSize;
			_lineSpacing = lineSpacing;
			_alignment = alignment;
			_wrapping = wrapping;
			_lineCount = 0;
			_position = Vector2i.Zero;
			_color = Color.White;
			_numQuads = 0;
			_dirty = true;

			// We always wrap - we fake no wrapping text by removing the width restriction
			if (wrapping == TextWrapping.NoWrap)
				_desiredSize.Width = Int32.MaxValue;

			ComputeCharacterAreasAndLines();
			ComputeActualSize();

			// Reset the desired size so that the text can be correctly aligned
			if (wrapping == TextWrapping.NoWrap)
				_desiredSize = desiredSize;

			AlignLines();
		}

		/// <summary>
		///     Draws the layouted text.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		/// <param name="position">The position of the top left corner of the text's drawing area.</param>
		/// <param name="color">The default color that should be used to draw the text.</param>
		public void Draw(SpriteBatch spriteBatch, Vector2i position, Color color)
		{
			Assert.ArgumentNotNull(spriteBatch);
			Assert.That(_font != null, "Update() must be called at least once before drawing.");

			if (_dirty || position != _position || _color != color)
			{
				_numQuads = 0;

				using (var text = TextString.Create(_text))
				{
					// Ensure that the quads list does not have to be resized by setting its capacity to the number of
					// characters; however, this wastes some space as not all characters generate quads
					if (_quads == null || text.Length > _quads.Length)
						_quads = new Quad[text.Length];

					for (var i = 0; i < text.Length; ++i)
					{
						var area = _characterAreas[i].Offset(position);
						Quad quad;

						if (_font.CreateGlyphQuad(text, i, ref area, color, out quad))
							_quads[_numQuads++] = quad;
					}
				}
			}

			_dirty = false;
			_position = position;
			_color = color;

			spriteBatch.Draw(_quads, _numQuads, _font.Texture);
		}

		/// <summary>
		///     Creates the character areas and lines for the text.
		/// </summary>
		private void ComputeCharacterAreasAndLines()
		{
			// The offset that is applied to all character positions
			var offset = Vector2i.Zero;

			// The current line of text; the first line starts at the top left corner of the desired area
			var line = TextLine.Create();

			// Initialize the token stream and get the first token
			using (var text = TextString.Create(_text))
			{
				if (_characterAreas == null || text.Length > _characterAreas.Length)
					_characterAreas = new Rectangle[text.Length];

				var stream = new TextTokenStream(_font, text, _desiredSize.Width);
				var token = stream.GetNextToken();

				while (token.Type != TextTokenType.EndOfText)
				{
					switch (token.Type)
					{
						case TextTokenType.Space:
						case TextTokenType.Word:
							// Compute the areas of the characters referenced by the word token and append them to the current line
							ComputeCharacterAreas(text, token.Sequence, ref offset);
							line.Append(token.Sequence, offset.X);

							break;
						case TextTokenType.WrappedSpace:
							// Compute the areas of the characters referenced by the word token
							ComputeCharacterAreas(text, token.Sequence, ref offset);

							// The width of the line shouldn't change as the space is actually wrapped to the next line;
							// however, we still want to know (for instance, for the calculation of the caret position) that
							// the space conceptually belongs to this line
							line.Append(token.Sequence, line.Width);
							StartNewLine(ref line, ref offset);

							break;
						case TextTokenType.Wrap:
						case TextTokenType.NewLine:
							StartNewLine(ref line, ref offset);
							break;
						default:
							Assert.That(false, "Unexpected token type.");
							break;
					}

					// Advance to the next token in the stream
					token = stream.GetNextToken();
				}
			}

			// Store the last line in the lines array
			AddLine(line);
		}

		/// <summary>
		///     Add the given line to the lines array.
		/// </summary>
		/// <param name="line">The line that should be added.</param>
		private void AddLine(TextLine line)
		{
			// Most texts fit in just one line
			if (_lines == null)
				_lines = new TextLine[1];

			// Check if we have to allocate more lines and if so, copy the old ones
			if (_lineCount + 1 >= _lines.Length)
			{
				// Assume that there will be two more lines
				var lines = new TextLine[_lines.Length + 2];
				Array.Copy(_lines, lines, _lines.Length);
				_lines = lines;
			}

			_lines[_lineCount++] = line;
		}

		/// <summary>
		///     Starts a new line.
		/// </summary>
		/// <param name="line">The predecessor of the new line.</param>
		/// <param name="offset">The position offset.</param>
		private void StartNewLine(ref TextLine line, ref Vector2i offset)
		{
			// Store the current line in the lines array and create a new one
			AddLine(line);
			line = TextLine.Create();

			// Update the offsets
			offset.X = 0;
			offset.Y += _font.LineHeight + _lineSpacing;
		}

		/// <summary>
		///     Computes the character areas of the characters in the given sequence.
		/// </summary>
		/// <param name="text">The text that is layouted.</param>
		/// <param name="sequence">The sequence whose character areas should be computed.</param>
		/// <param name="offset">The offset of the character position.</param>
		private void ComputeCharacterAreas(TextString text, TextSequence sequence, ref Vector2i offset)
		{
			for (var i = sequence.FirstCharacter; i < sequence.LastCharacter; ++i)
				_characterAreas[i] = _font.GetGlyphArea(text, sequence.FirstCharacter, i, ref offset);
		}

		/// <summary>
		///     Aligns the lines.
		/// </summary>
		private void AlignLines()
		{
			if (_alignment == TextAlignment.Left)
				return;

			for (var i = 0; i < _lineCount; ++i)
			{
				// Move each quad of the line by the given deltas
				var delta = Vector2i.Zero;

				if (_alignment == TextAlignment.Right)
					delta.X = _desiredSize.Width - _lines[i].Width;
				else if (_alignment == TextAlignment.Center)
					delta.X = (_desiredSize.Width - _lines[i].Width) / 2;

				// Move the line, if necessary
				if (delta != Vector2i.Zero)
				{
					for (var j = _lines[i].FirstCharacter; j < _lines[i].LastCharacter; ++j)
						_characterAreas[j] = _characterAreas[j].Offset(delta);
				}
			}
		}

		/// <summary>
		///     Computes the actual text rendering size. Usually, the actual size is smaller than the desired size.
		///     If any words overlap, however, the actual size is bigger.
		/// </summary>
		private void ComputeActualSize()
		{
			var width = 0;

			for (var i = 0; i < _lineCount; ++i)
			{
				if (_lines[i].Width > width)
					width = _lines[i].Width;
			}

			Size = new Size(width, ComputeHeight());
		}

		/// <summary>
		///     Computes the actual height of the text rendering area.
		/// </summary>
		private int ComputeHeight()
		{
			// We know that each line has a height of _font.LineHeight pixels. Additionally, after each line (except
			// the last one), we have a spacing of _lineSpacing pixels.
			return _lineCount * _font.LineHeight + Math.Max(0, _lineCount - 1) * _lineSpacing;
		}
	}
}