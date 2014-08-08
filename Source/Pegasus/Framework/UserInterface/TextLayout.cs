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
		///     Caches the computed size during the arrange phase.
		/// </summary>
		private LayoutInfo _arranged;

		/// <summary>
		///     The layouted areas of the individual characters of the text.
		/// </summary>
		private Rectangle[] _characterAreas;

		/// <summary>
		///     The default text color.
		/// </summary>
		private Color _color;

		/// <summary>
		///     A value indicating whether the cached text quads are dirty.
		/// </summary>
		private bool _dirty;

		/// <summary>
		///     The number of lines that are currently used.
		/// </summary>
		private int _lineCount;

		/// <summary>
		///     The individual lines of the text.
		/// </summary>
		private TextLine[] _lines;

		/// <summary>
		///     Caches the computed size during the measure phase.
		/// </summary>
		private LayoutInfo _measured;

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
		///     Measures the size of the text.
		/// </summary>
		/// <param name="font">The font that should be used to draw the size of the individual characters.</param>
		/// <param name="text">The text that should be layouted and measured.</param>
		/// <param name="desiredSize">The size of the desired drawing area of the text. If the text doesn't fit, it overlaps vertically.</param>
		/// <param name="lineSpacing">The amount of spacing between consecutive lines.</param>
		/// <param name="alignment">The alignment of the text within the desired drawing area.</param>
		/// <param name="wrapping">Indicates whether the text should be wrapped.</param>
		public Size Measure(Font font, string text, Size desiredSize, int lineSpacing, TextAlignment alignment, TextWrapping wrapping)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(text);
			Assert.ArgumentInRange(alignment);
			Assert.ArgumentInRange(wrapping);

			if (text == String.Empty)
				return new Size(0, font.LineHeight);

			if (_measured.SizeOutdated(font, text, desiredSize, lineSpacing, alignment, wrapping))
			{
				_lineCount = 0;
				_position = Vector2i.Zero;

				if (wrapping == TextWrapping.NoWrap)
					_measured.ActualSize = new Size(font.MeasureWidth(text), font.LineHeight + lineSpacing);
				else
				{
					ComputeCharacterAreasAndLines(ref _measured);
					_measured.ActualSize = ComputeActualSize(ref _measured);
				}
			}

			return _measured.ActualSize;
		}

		/// <summary>
		///     Arranges the layouted text.
		/// </summary>
		/// <param name="font">The font that should be used to draw the size of the individual characters.</param>
		/// <param name="text">The text that should be layouted and measured.</param>
		/// <param name="desiredSize">The size of the desired drawing area of the text. If the text doesn't fit, it overlaps vertically.</param>
		/// <param name="lineSpacing">The amount of spacing between consecutive lines.</param>
		/// <param name="alignment">The alignment of the text within the desired drawing area.</param>
		/// <param name="wrapping">Indicates whether the text should be wrapped.</param>
		public Size Arrange(Font font, string text, Size desiredSize, int lineSpacing, TextAlignment alignment, TextWrapping wrapping)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(text);
			Assert.ArgumentInRange(alignment);
			Assert.ArgumentInRange(wrapping);

			if (text == String.Empty)
			{
				_arranged.Text = String.Empty;
				return new Size(0, font.LineHeight);
			}

			if (_arranged.SizeOutdated(font, text, desiredSize, lineSpacing, alignment, wrapping))
			{
				_lineCount = 0;
				_position = Vector2i.Zero;
				_dirty = true;

				ComputeCharacterAreasAndLines(ref _arranged);
				_arranged.ActualSize = ComputeActualSize(ref _arranged);
				AlignLines(ref _arranged);
			}

			return _arranged.ActualSize;
		}

		/// <summary>
		///     Computes the physical position of the caret at the given logical caret position.
		/// </summary>
		/// <param name="position">The logical position of the caret.</param>
		public Vector2i ComputeCaretPosition(int position)
		{
			// The caret 'origin' is at the top left corner of the desired area; 
			// non-left/top aligned layouts are not supported
			if (position == 0 || String.IsNullOrWhiteSpace(_arranged.Text))
				return Vector2i.Zero;

			// Find the line that contains the caret
			var lineIndex = 0;
			while (lineIndex < _lineCount && _lines[lineIndex].LastCharacter <= position)
				++lineIndex;

			Assert.That(lineIndex >= 0, "Could not find line of caret.");

			// If the caret does not belong to the last line, place it at the end of the last line anyway
			if (lineIndex == _lineCount)
				--lineIndex;

			// The caret position is relative to the line 'origin'
			var lineY = lineIndex * (_arranged.Font.LineHeight + _arranged.LineSpacing);
			var result = new Vector2i(0, lineY);

			// Calculate the caret's offset from the line's left edge
			if (!_lines[lineIndex].IsInvalid)
				result.X += _arranged.Font.MeasureWidth(_arranged.Text, Math.Max(0, _lines[lineIndex].FirstCharacter), position);
			
			return result;
		}

		/// <summary>
		///     Gets the index of the character closest to the given position.
		/// </summary>
		/// <param name="position">The position the closest character should be returned for.</param>
		internal int GetCharacterIndexAt(Vector2i position)
		{
			if (String.IsNullOrWhiteSpace(_arranged.Text))
				return 0;

			// Search for the correct line
			var lineIndex = 0;
			var lineHeight = _arranged.Font.LineHeight + _arranged.LineSpacing;

			while (lineIndex < _lineCount - 1 && position.Y > 0)
			{
				if (position.Y < (lineIndex + 1) * lineHeight)
					break;

				++lineIndex;
			}

			// Search for the correct character on the line
			var characterIndex = _lines[lineIndex].FirstCharacter;
			var lastCharacter = lineIndex == _lineCount - 1 ? _lines[lineIndex].LastCharacter : _lines[lineIndex].LastCharacter - 1;
			while (characterIndex < lastCharacter && position.X > 0)
			{
				var start = _arranged.Font.MeasureWidth(_arranged.Text, _lines[lineIndex].FirstCharacter, characterIndex);
				var end = _arranged.Font.MeasureWidth(_arranged.Text, _lines[lineIndex].FirstCharacter, characterIndex + 1);

				if (position.X <= end - (end - start) / 2)
					break;

				++characterIndex;
			}

			return characterIndex;
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

			if (String.IsNullOrWhiteSpace(_arranged.Text))
				return;

			Assert.That(_arranged.Font != null, "Arrange() must be called at least once before drawing.");
			if (_dirty || position != _position || _color != color)
			{
				_numQuads = 0;

				using (var text = TextString.Create(_arranged.Text))
				{
					// Ensure that the quads list does not have to be resized by setting its capacity to the number of
					// characters; however, this wastes some space as not all characters generate quads
					if (_quads == null || text.Length > _quads.Length)
						_quads = new Quad[text.Length];

					for (var i = 0; i < text.Length; ++i)
					{
						var area = _characterAreas[i].Offset(position);
						Quad quad;

						if (_arranged.Font.CreateGlyphQuad(text, i, ref area, color, out quad))
							_quads[_numQuads++] = quad;
					}
				}
			}

			_dirty = false;
			_position = position;
			_color = color;

			spriteBatch.Draw(_quads, _numQuads, _arranged.Font.Texture);
		}

		/// <summary>
		///     Creates the character areas and lines for the text.
		/// </summary>
		/// <param name="layoutInfo">The layout info the layouting information should be obtained from.</param>
		private void ComputeCharacterAreasAndLines(ref LayoutInfo layoutInfo)
		{
			// The offset that is applied to all character positions
			var offset = Vector2i.Zero;

			// The current line of text; the first line starts at the top left corner of the desired area
			var line = TextLine.Create();

			// Initialize the token stream and get the first token
			using (var text = TextString.Create(layoutInfo.Text))
			{
				if (_characterAreas == null || text.Length > _characterAreas.Length)
					_characterAreas = new Rectangle[text.Length];

				if (layoutInfo.Wrapping == TextWrapping.NoWrap)
				{
					var sequence = new TextSequence(text);
					ComputeCharacterAreas(ref layoutInfo, text, sequence, ref offset);
					line.Append(sequence, offset.X);
				}
				else
				{
					var stream = new TextTokenStream(layoutInfo.Font, text, layoutInfo.DesiredSize.Width);
					var token = stream.GetNextToken();

					while (token.Type != TextTokenType.EndOfText)
					{
						switch (token.Type)
						{
							case TextTokenType.Space:
							case TextTokenType.Word:
								// Compute the areas of the characters referenced by the word token and append them to the current line
								ComputeCharacterAreas(ref layoutInfo, text, token.Sequence, ref offset);
								line.Append(token.Sequence, offset.X);

								break;
							case TextTokenType.WrappedSpace:
								// Compute the areas of the characters referenced by the word token
								ComputeCharacterAreas(ref layoutInfo, text, token.Sequence, ref offset);

								// The width of the line shouldn't change as the space is actually wrapped to the next line;
								// however, we still want to know (for instance, for the calculation of the caret position) that
								// the space conceptually belongs to this line
								line.Append(token.Sequence, line.Width);
								StartNewLine(ref layoutInfo, ref line, ref offset);

								break;
							case TextTokenType.Wrap:
							case TextTokenType.NewLine:
								StartNewLine(ref layoutInfo, ref line, ref offset);
								break;
							default:
								Assert.That(false, "Unexpected token type.");
								break;
						}

						// Advance to the next token in the stream
						token = stream.GetNextToken();
					}
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
		/// <param name="layoutInfo">The layout info the layouting information should be obtained from.</param>
		/// <param name="line">The predecessor of the new line.</param>
		/// <param name="offset">The position offset.</param>
		private void StartNewLine(ref LayoutInfo layoutInfo, ref TextLine line, ref Vector2i offset)
		{
			// Store the current line in the lines array and create a new one
			AddLine(line);
			line = TextLine.Create();

			// Update the offsets
			offset.X = 0;
			offset.Y += layoutInfo.Font.LineHeight + layoutInfo.LineSpacing;
		}

		/// <summary>
		///     Computes the character areas of the characters in the given sequence.
		/// </summary>
		/// <param name="layoutInfo">The layout info the layouting information should be obtained from.</param>
		/// <param name="text">The text that is layouted.</param>
		/// <param name="sequence">The sequence whose character areas should be computed.</param>
		/// <param name="offset">The offset of the character position.</param>
		private void ComputeCharacterAreas(ref LayoutInfo layoutInfo, TextString text, TextSequence sequence, ref Vector2i offset)
		{
			for (var i = sequence.FirstCharacter; i < sequence.LastCharacter; ++i)
				_characterAreas[i] = layoutInfo.Font.GetGlyphArea(text, sequence.FirstCharacter, i, ref offset);
		}

		/// <summary>
		///     Aligns the lines.
		/// </summary>
		/// <param name="layoutInfo">The layout info the layouting information should be obtained from.</param>
		private void AlignLines(ref LayoutInfo layoutInfo)
		{
			if (layoutInfo.Alignment == TextAlignment.Left ||
				(layoutInfo.Wrapping == TextWrapping.NoWrap && layoutInfo.DesiredSize.Width <= layoutInfo.ActualSize.Width))
				return;

			for (var i = 0; i < _lineCount; ++i)
			{
				// Move each quad of the line by the given deltas
				var delta = Vector2i.Zero;

				if (layoutInfo.Alignment == TextAlignment.Right)
					delta.X = layoutInfo.DesiredSize.Width - _lines[i].Width;
				else if (layoutInfo.Alignment == TextAlignment.Center)
					delta.X = (layoutInfo.DesiredSize.Width - _lines[i].Width) / 2;

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
		/// <param name="layoutInfo">The layout info the layouting information should be obtained from.</param>
		private Size ComputeActualSize(ref LayoutInfo layoutInfo)
		{
			var width = 0;

			for (var i = 0; i < _lineCount; ++i)
			{
				if (_lines[i].Width > width)
					width = _lines[i].Width;
			}

			return new Size(width, ComputeHeight(ref layoutInfo));
		}

		/// <summary>
		///     Computes the actual height of the text rendering area.
		/// </summary>
		/// <param name="layoutInfo">The layout info the layouting information should be obtained from.</param>
		private int ComputeHeight(ref LayoutInfo layoutInfo)
		{
			// We know that each line has a height of _font.LineHeight pixels. Additionally, after each line (except
			// the last one), we have a spacing of _lineSpacing pixels.
			return _lineCount * layoutInfo.Font.LineHeight + Math.Max(0, _lineCount - 1) * layoutInfo.LineSpacing;
		}

		/// <summary>
		///     Performs hit test for the given position.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		internal bool HitTest(Vector2d position)
		{
			// Check if the position lies within the text's bounding box. If not, there can be no hit.
			var horizontalHit = position.X >= _position.X && position.X <= _position.X + _arranged.ActualSize.Width;
			var verticalHit = position.Y >= _position.Y && position.Y <= _position.Y + _arranged.ActualSize.Height;

			return horizontalHit && verticalHit;
		}

		/// <summary>
		///     Caches the sizes computed during the measure and arrange phases.
		/// </summary>
		private struct LayoutInfo
		{
			/// <summary>
			///     The actual result text drawing size. Usually, the actual size is smaller than the desired size.
			///     If any words overlap, however, the actual size is bigger.
			/// </summary>
			public Size ActualSize;

			/// <summary>
			///     The alignment of the text within the desired drawing area.
			/// </summary>
			public TextAlignment Alignment;

			/// <summary>
			///     The desired drawing size of the text. If the text doesn't fit, it overlaps vertically.
			/// </summary>
			public Size DesiredSize;

			/// <summary>
			///     The font that is used to determine the size of the individual characters.
			/// </summary>
			public Font Font;

			/// <summary>
			///     The amount of spacing between consecutive lines.
			/// </summary>
			public int LineSpacing;

			/// <summary>
			///     The layouted text.
			/// </summary>
			public string Text;

			/// <summary>
			///     Indicates whether the text should be wrapped.
			/// </summary>
			public TextWrapping Wrapping;

			/// <summary>
			///     Indicates whether the actual size is outdated and needs to be recomputed with the given values.
			/// </summary>
			/// <param name="font">The font that should be used to draw the size of the individual characters.</param>
			/// <param name="text">The text that should be layouted and measured.</param>
			/// <param name="desiredSize">The size of the desired drawing area of the text. If the text doesn't fit, it overlaps vertically.</param>
			/// <param name="lineSpacing">The amount of spacing between consecutive lines.</param>
			/// <param name="alignment">The alignment of the text within the desired drawing area.</param>
			/// <param name="wrapping">Indicates whether the text should be wrapped.</param>
			public bool SizeOutdated(Font font, string text, Size desiredSize, int lineSpacing, TextAlignment alignment, TextWrapping wrapping)
			{
				Assert.ArgumentNotNull(font);
				Assert.ArgumentNotNull(text);
				Assert.ArgumentInRange(alignment);
				Assert.ArgumentInRange(wrapping);

				if (Font == font && Text == text && DesiredSize == desiredSize && LineSpacing == lineSpacing &&
					Alignment == alignment && Wrapping == wrapping)
					return false;

				Font = font;
				Text = text;
				DesiredSize = desiredSize;
				LineSpacing = lineSpacing;
				Alignment = alignment;
				Wrapping = wrapping;

				return true;
			}
		}
	}
}