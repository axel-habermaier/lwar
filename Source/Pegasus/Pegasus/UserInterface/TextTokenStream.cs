namespace Pegasus.UserInterface
{
	using System;
	using Math;
	using Platform.Graphics;
	using Utilities;

	/// <summary>
	///     Represents a stream of text tokens for the given text. Wrap tokens are inserted when the remainder
	///     of the text should be placed on a new line.
	/// </summary>
	internal struct TextTokenStream
	{
		/// <summary>
		///     The font that is used to determine the size of the text's characters.
		/// </summary>
		private readonly Font _font;

		/// <summary>
		///     The maximum width of a line.
		/// </summary>
		private readonly float _maxLineWidth;

		/// <summary>
		///     The text that should be wrapped.
		/// </summary>
		private readonly TextString _text;

		/// <summary>
		///     The width of the current line.
		/// </summary>
		private float _lineWidth;

		/// <summary>
		///     The current text token.
		/// </summary>
		private TextToken _token;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that should be used to determine the width of the text's characters.</param>
		/// <param name="text">The text that should be wrapped.</param>
		/// <param name="maxLineWidth">The maximum width of a line.</param>
		public TextTokenStream(Font font, TextString text, float maxLineWidth)
			: this()
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentSatisfies(maxLineWidth > 0, "Line width too small.");

			_font = font;
			_text = text;
			_maxLineWidth = maxLineWidth;
			_lineWidth = 0;

			Advance(0);
		}

		/// <summary>
		///     Gets the next token in the stream.
		/// </summary>
		public TextToken GetNextToken()
		{
			switch (_token.Type)
			{
				case TextTokenType.EndOfText:
					// Simply return end of text tokens
					return _token;
				case TextTokenType.NewLine:
					// Simply return new line and wrap tokens and proceed to the next token
					_lineWidth = 0;
					return Advance(_token.Sequence.LastCharacter);
				case TextTokenType.Space:
					return HandleSpaceToken();
				case TextTokenType.Word:
					// Ignore invalid word sequences
					if (_token.Sequence.IsInvalid)
					{
						Advance(_token.Sequence.LastCharacter);
						return GetNextToken();
					}

					// Word tokens might be split into several tokens if they don't fit into a single line
					return HandleWordToken();
				default:
					Assert.NotReached("Unexpected token type.");
					return TextToken.EndOfText;
			}
		}

		/// <summary>
		///     Handles a word token, splitting the token over multiple lines, if necessary.
		/// </summary>
		private TextToken HandleWordToken()
		{
			Assert.That(_token.Type == TextTokenType.Word, "Wrong token type.");
			Assert.That(!_token.Sequence.IsInvalid, "Unexpected invalid sequence.");

			// Compute the token's width and check whether it fits into the current line or any line at all
			var width = _token.Sequence.ComputeWidth(_font, _text);
			var fitsIntoLine = _lineWidth + width <= _maxLineWidth;
			var fitsIntoAnyLine = width <= _maxLineWidth;

			// If it fits into the current line, simply return the token
			if (fitsIntoLine)
			{
				_lineWidth += width;
				return Advance(_token.Sequence.LastCharacter);
			}

			// If it doesn't fit into the current line, but its width doesn't exceed the maximum line width,
			// return a wrap token. Then, the next time GetNextToken() is called, the current word token will fit.
			if (fitsIntoAnyLine)
			{
				_lineWidth = 0;
				return TextToken.Wrap;
			}

			// If it doesn't fit into any line at all, we have to split the word into several tokens
			var allowedWidth = _maxLineWidth - _lineWidth;

			// If allowedWidth is zero, this means that either the line already is perfectly full or
			// we're about to add the second part of a previously split word; in any case, return a wrap token and
			// deal with the word on the next iteration
			if (MathUtils.Equals(allowedWidth, 0))
			{
				_lineWidth = 0;
				return TextToken.Wrap;
			}

			TextToken part1, part2;
			_token.Split(_font, _text, _maxLineWidth - _lineWidth, out part1, out part2);

			Assert.That(!part1.Sequence.IsInvalid && !part2.Sequence.IsInvalid, "Unexpected invalid sequence(s).");
			_token = part2;
			_lineWidth = _maxLineWidth; // Ensure that part2 is placed on the next line
			return part1;
		}

		/// <summary>
		///     Handles a space token. If the token doesn't fit into the current line or if no other tokens follow on the
		///     current line, it is replaced by a new line token.
		/// </summary>
		/// <returns></returns>
		private TextToken HandleSpaceToken()
		{
			Assert.That(_token.Type == TextTokenType.Space, "Wrong token type.");
			Assert.That(!_token.Sequence.IsInvalid, "Unexpected invalid sequence.");
			Assert.That(_token.Sequence.LastCharacter - _token.Sequence.FirstCharacter == 1,
				"Space token with more than one space.");

			// Compute the token's width and check whether it fits into the current line or any line at all
			var width = _token.Sequence.ComputeWidth(_font, _text);
			var fitsIntoLine = _lineWidth + width <= _maxLineWidth;

			Assert.That(width <= _maxLineWidth, "Line width is too small for single space characters.");

			// If it doesn't fit into the current line, return a new line token instead
			if (!fitsIntoLine)
				return ReplaceSpaceWithNewLineToken();

			// Otherwise, we have to check the next token to find out what we are supposed to do with this space
			var nextToken = TextToken.Next(_text, _token.Sequence.LastCharacter);
			switch (nextToken.Type)
			{
				case TextTokenType.NewLine:
				case TextTokenType.EndOfText:
					// Return the space if the next token is a new line or the end of the text
					_lineWidth += width;
					return Advance(_token.Sequence.LastCharacter);
				case TextTokenType.Space:
					// Return the space if the next token is another space that also fits; otherwise, 
					// replace this space by a new line token.
					var bothSpacesFit = _lineWidth + 2 * width <= _maxLineWidth;
					if (!bothSpacesFit)
						return ReplaceSpaceWithNewLineToken();

					_lineWidth += width;
					return Advance(_token.Sequence.LastCharacter);
				case TextTokenType.Word:
					// Check if the next word also fits into this line (or at least some part of it).
					// If this is the not case, the space token must be replaced by a new line token.
					var nextTokenWidth = nextToken.Sequence.ComputeWidth(_font, _text);
					var nextFitsIntoLine = _lineWidth + width + nextTokenWidth <= _maxLineWidth;
					var nextFitsIntoAnyLine = nextTokenWidth <= _maxLineWidth;

					// The next token also fits, so return the space
					if (nextFitsIntoLine)
					{
						_lineWidth += width;
						return Advance(_token.Sequence.LastCharacter);
					}

					// If the next token doesn't fit into the current line, but into the next one, return
					// a new line instead of a space
					if (nextFitsIntoAnyLine)
						return ReplaceSpaceWithNewLineToken();

					// The next token has to be split. Only return the space if the first part is valid, i.e.,
					// if there actually is at least one character of the next token that fits into the current line.
					TextToken part1, part2;
					nextToken.Split(_font, _text, _maxLineWidth - _lineWidth - width, out part1, out part2);

					if (part1.Sequence.IsInvalid)
						return ReplaceSpaceWithNewLineToken();

					_lineWidth += width;
					return Advance(_token.Sequence.LastCharacter);
				default:
					Assert.That(false, "Unexpected token type.");
					return TextToken.EndOfText;
			}
		}

		/// <summary>
		///     Replaces the current space token with a new line token and advances the token stream.
		/// </summary>
		private TextToken ReplaceSpaceWithNewLineToken()
		{
			Assert.That(_token.Type == TextTokenType.Space, "Wrong token type.");

			var newLineIndex = _token.Sequence.FirstCharacter;
			_token = TextToken.Next(_text, _token.Sequence.LastCharacter);

			_lineWidth = 0;
			return TextToken.CreateWrappedSpaceToken(newLineIndex);
		}

		/// <summary>
		///     Advances the token stream and returns the current token.
		/// </summary>
		/// <param name="offset">The offset into the text.</param>
		/// <returns></returns>
		private TextToken Advance(int offset)
		{
			var currentToken = _token;
			_token = TextToken.Next(_text, offset);

			return currentToken;
		}
	}
}