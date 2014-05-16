namespace Pegasus.Rendering.UserInterface
{
	using System;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Determines a layout for a text based on the font, desired with, alignment, etc.
	/// </summary>
	internal class TextLayout : DisposableObject
	{
		/// <summary>
		///     The alignment of the text within the desired area.
		/// </summary>
		private TextAlignment _alignment;

		/// <summary>
		///     The areas of the individual characters of the text.
		/// </summary>
		private Rectangle[] _characterAreas;

		/// <summary>
		///     The desired drawing area of the text. If the text doesn't fit, it overlaps vertically.
		/// </summary>
		private Rectangle _desiredArea;

		/// <summary>
		///     Indicates that the metadata is out of date.
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
		///     The amount of spacing between lines.
		/// </summary>
		private int _lineSpacing;

		/// <summary>
		///     The individual lines of the text.
		/// </summary>
		private TextLine[] _lines;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that is used to determine the size of the individual characters.</param>
		/// <param name="text">The text that should be layouted.</param>
		public TextLayout(Font font, string text)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(text);

			Font = font;
			TextString = text;
		}

		/// <summary>
		///     Gets the text that is layouted.
		/// </summary>
		public TextString Text { get; private set; }

		/// <summary>
		///     Gets the layouting data for the individual characters of the layouted text.
		/// </summary>
		public Rectangle[] LayoutData
		{
			get { return _characterAreas; }
		}

		/// <summary>
		///     Gets or sets the text that should be layouted.
		/// </summary>
		public string TextString
		{
			get { return Text.SourceString; }
			set
			{
				Assert.ArgumentNotNull(value);

				if (Text != null && Text.SourceString == value)
					return;

				Text.SafeDispose();
				Text = Platform.TextString.Create(value);
				_dirty = true;

				if (_characterAreas == null || Text.Length > _characterAreas.Length)
					_characterAreas = new Rectangle[Text.Length];
			}
		}

		/// <summary>
		///     Gets or sets the amount of spacing between lines.
		/// </summary>
		public int LineSpacing
		{
			get { return _lineSpacing; }
			set
			{
				if (_lineSpacing == value)
					return;

				_lineSpacing = value;
				_dirty = true;
			}
		}

		/// <summary>
		///     Gets or sets the desired drawing area of the text. If the text doesn't fit, it overlaps vertically.
		/// </summary>
		public Rectangle DesiredArea
		{
			get { return _desiredArea; }
			set
			{
				if (_desiredArea == value)
					return;

				// Optimization: If only the position changed, we can move the lines and character areas without
				// layouting the entire text again
				var sizeChanged = _desiredArea.Size != value.Size;
				var offset = value.Position - _desiredArea.Position;
				_desiredArea = value;

				if (sizeChanged)
					_dirty = true;
				else if (!_dirty)
					OffsetPosition(offset);
			}
		}

		/// <summary>
		///     Gets or sets the font that is used to determine the size of the individual characters.
		/// </summary>
		public Font Font
		{
			get { return _font; }
			set
			{
				Assert.ArgumentNotNull(value);

				if (_font == value)
					return;

				_font = value;
				_dirty = true;
			}
		}

		/// <summary>
		///     Gets or sets the alignment of the text within the desired area.
		/// </summary>
		public TextAlignment Alignment
		{
			get { return _alignment; }
			set
			{
				if (_alignment == value)
					return;

				_alignment = value;
				_dirty = true;
			}
		}

		/// <summary>
		///     Gets the actual text rendering area. Usually, the actual area is smaller than the desired area.
		///     If any words overlap, however, the actual area is bigger.
		/// </summary>
		public Rectangle ActualArea { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Text.SafeDispose();
		}

		/// <summary>
		///     Raised when the layout has changed.
		/// </summary>
		public event Action LayoutChanged;

		/// <summary>
		///     Updates the layout if any changes have been made since the last layout update that
		///     might possibly affect the text's layout.
		/// </summary>
		public void UpdateLayout()
		{
			Assert.That(_characterAreas != null, "TextLayout is not initialized properly.");

			if (!_dirty)
				return;

			_lineCount = 0;
			_dirty = false;

			ComputeCharacterAreasAndLines();
			AlignLines();
			ComputeActualArea();

			if (LayoutChanged != null)
				LayoutChanged();
		}

		/// <summary>
		///     Draws debugging visualizations of the sequences and lines.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the debug visualizations.</param>
		public void DrawDebugVisualizations(SpriteBatch spriteBatch)
		{
			//spriteBatch.Draw(DesiredArea, Texture2D.White, new Color(0.2f, 0.2f, 0.2f, 0.3f));
			//spriteBatch.Draw(ActualArea, Texture2D.White, new Color(0, 0, 0.5f, 0.3f));

			//for (var i = 0; i < _lineCount; ++i)
			//	spriteBatch.Draw(_lines[i].Area, Texture2D.White, new Color(0, 0.5f, 0, 0.3f));
		}

		/// <summary>
		///     Relayouts the text.
		/// </summary>
		public void Relayout()
		{
			_dirty = true;
			UpdateLayout();
		}

		/// <summary>
		///     Computes the physical position of the caret at the given logical caret position.
		/// </summary>
		/// <param name="position">The logical position of the caret.</param>
		public Vector2i ComputeCaretPosition(int position)
		{
			// The caret 'origin' as at the top left corner of the desired area; 
			// non-left/top aligned layouts are not supported
			if (position == 0)
				return _desiredArea.Position;

			// Find the line that contains the caret
			var lineIndex = 0;
			while (lineIndex < _lineCount && _lines[lineIndex].LastCharacter <= position)
				++lineIndex;

			Assert.That(lineIndex >= 0, "Could not find line of caret.");

			// If the caret does not belong to the last line, place it at the end of the last line anyway
			if (lineIndex == _lineCount)
				--lineIndex;

			// The caret position is relative to the line 'origin'
			var result = _lines[lineIndex].Area.Position;

			// Calculate the caret's offset from the line's left edge
			if (!_lines[lineIndex].IsInvalid)
				result.X += _font.MeasureWidth(Text, Math.Max(0, _lines[lineIndex].FirstCharacter), position);

			return result;
		}

		/// <summary>
		///     Creates the character areas and lines for the text.
		/// </summary>
		private void ComputeCharacterAreasAndLines()
		{
			// The offset that is applied to all character positions
			var offset = _desiredArea.Position;

			// The current line of text; the first line starts at the top left corner of the desired area
			var line = new TextLine(DesiredArea.Left, DesiredArea.Top, Font.LineHeight);

			// Initialize the token stream and get the first token
			var stream = new TextTokenStream(_font, Text, _desiredArea.Width);
			var token = stream.GetNextToken();
			while (token.Type != TextTokenType.EndOfText)
			{
				switch (token.Type)
				{
					case TextTokenType.Space:
					case TextTokenType.Word:
						// Compute the areas of the characters referenced by the word token
						ComputeCharacterAreas(token.Sequence, ref offset);

						// The new width of the current line is the delta between the current offset in X direction
						// and the left edge of the desired area
						var lineWidth = offset.X - _desiredArea.Left;
						line.Append(token.Sequence, lineWidth);

						break;
					case TextTokenType.WrappedSpace:
						// Compute the areas of the characters referenced by the word token
						ComputeCharacterAreas(token.Sequence, ref offset);

						// The width of the line shouldn't change as the space is actually wrapped to the next line;
						// however, we still want to know (for instance, for the calculation of the caret position) that
						// the space conceptually belongs to this line
						line.Append(token.Sequence, line.Area.Width);
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

			// Store the last line in the lines array
			AddLine(line);
		}

		/// <summary>
		///     Aligns the lines.
		/// </summary>
		private void AlignLines()
		{
			var totalHeight = ComputeHeight();
			for (var i = 0; i < _lineCount; ++i)
			{
				// Move each quad of the line by the given deltas
				var delta = Vector2i.Zero;

				// Compute delta in X direction
				if (_alignment.IsRightAligned())
					delta.X = _desiredArea.Width - _lines[i].Area.Width;
				else if (_alignment.IsHorizontallyCentered())
					delta.X = (_desiredArea.Width - _lines[i].Area.Width) / 2;

				// Compute delta in Y direction
				if (_alignment.IsBottomAligned())
					delta.Y = _desiredArea.Height - totalHeight;
				else if (_alignment.IsVerticallyCentered())
					delta.Y = (_desiredArea.Height - totalHeight) / 2;

				// Move the line, if necessary
				if (delta != Vector2i.Zero)
				{
					_lines[i].Offset(delta);
					for (var j = _lines[i].FirstCharacter; j < _lines[i].LastCharacter; ++j)
						_characterAreas[j] = _characterAreas[j].Offset(delta);
				}
			}
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
			// Store the current line in the lines array
			AddLine(line);

			// Create a new line
			var totalLineHeight = _font.LineHeight + _lineSpacing;
			line = new TextLine(DesiredArea.Left, line.Area.Top + totalLineHeight, _font.LineHeight);

			// Update the offsets
			offset.X = _desiredArea.Left;
			offset.Y += totalLineHeight;
		}

		/// <summary>
		///     Computes the character areas of the characters in the given sequence.
		/// </summary>
		/// <param name="sequence">The sequence whose character areas should be computed.</param>
		/// <param name="offset">The offset of the character position.</param>
		private void ComputeCharacterAreas(TextSequence sequence, ref Vector2i offset)
		{
			for (var i = sequence.FirstCharacter; i < sequence.LastCharacter; ++i)
				_characterAreas[i] = _font.GetGlyphArea(Text, sequence.FirstCharacter, i, ref offset);
		}

		/// <summary>
		///     Computes the actual text rendering area. Usually, the actual area is smaller than the desired area.
		///     If any words overlap, however, the actual area is bigger.
		/// </summary>
		private void ComputeActualArea()
		{
			if (_lineCount == 0)
			{
				// FIXME: This is only correct for left/top-aligned layouts
				ActualArea = new Rectangle(_desiredArea.Left, _desiredArea.Top, 0, 0);
				return;
			}

			// The maximum line width
			var width = 0;
			// The minimum line position in X direction
			var x = Int32.MaxValue;

			for (var i = 0; i < _lineCount; ++i)
			{
				var area = _lines[i].Area;

				if (area.Width > width)
					width = area.Width;

				if (area.Left < x)
					x = area.Left;
			}

			var y = _lines[0].Area.Top;
			ActualArea = new Rectangle(x, y, width, ComputeHeight());
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

		/// <summary>
		///     Offsets the position of the lines and character areas without performing a full layout pass.
		/// </summary>
		/// <param name="offset">The position offset.</param>
		private void OffsetPosition(Vector2i offset)
		{
			if (_dirty || _characterAreas == null)
				return;

			for (var i = 0; i < _lineCount; ++i)
				_lines[i].Offset(offset);

			for (var i = 0; i < _lines[_lineCount - 1].LastCharacter; ++i)
				_characterAreas[i] = _characterAreas[i].Offset(offset);

			ActualArea = ActualArea.Offset(offset);

			if (LayoutChanged != null)
				LayoutChanged();
		}
	}
}