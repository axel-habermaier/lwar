using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using Platform;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///   Represents a text that may optionally contain color specifiers or text-representations of emoticons.
	/// </summary>
	public class Text : PooledObject<Text>
	{
		/// <summary>
		///   Maps characters to colors. The character plus the color marker comprise a color specifier. For instance, 'w'
		///   is mapped to the color white, so a text containing "\wA" prints a white 'A'.
		/// </summary>
		private static readonly ColorSpecifier[] Colors = new[]
		{
			new ColorSpecifier("\\w", new Color(255, 255, 255, 255)),
			new ColorSpecifier("\\b", new Color(0, 0, 0, 255)),
			new ColorSpecifier("\\r", new Color(255, 0, 0, 255)),
			new ColorSpecifier("\\g", new Color(0, 255, 0, 255)),
			new ColorSpecifier("\\b", new Color(0, 0, 255, 255))
		};

		/// <summary>
		///   Maps text-representations of emoticons to their single-character encoding.
		/// </summary>
		private static readonly Emoticon[] Emoticons = new[]
		{
			new Emoticon(":)", (char)277),
			new Emoticon(":(", (char)278),
			new Emoticon(";)", (char)279),
			new Emoticon(":P", (char)280),
			new Emoticon(":D", (char)281)
		};

		/// <summary>
		///   A cached string builder instance that is used to construct the string representation of text instances.
		/// </summary>
		private static readonly StringBuilder Builder = new StringBuilder();

		/// <summary>
		///   The color ranges defined by the text.
		/// </summary>
		private readonly List<ColorRange> _colorRanges = new List<ColorRange>(2);

		/// <summary>
		///   The text with the color specifiers removed and the emoticons replaced by their single-character encoding.
		/// </summary>
		private readonly StringBuilder _text = new StringBuilder();

		/// <summary>
		///   Gets the source string that might contain color specifiers and text-representations of emotions.
		/// </summary>
		public string SourceString { get; private set; }

		/// <summary>
		///   Gets the length of the text, excluding all color specifiers and counting all single-character encodings of
		///   emoticons.
		/// </summary>
		public int Length
		{
			get { return _text.Length; }
		}

		/// <summary>
		///   Gets the character at the specified index. Color specifier are not returned and do not increase the index
		///   count, whereas the single-character encodings of emoticons are returned and increase the index count by one.
		/// </summary>
		/// <param name="index">The index of the character that should be returned.</param>
		public char this[int index]
		{
			get
			{
				Assert.InRange(index, 0, _text.Length - 1);
				return _text[index];
			}
		}

		/// <summary>
		///   Gets a value indicating whether the text consists of white space only.
		/// </summary>
		public bool IsWhitespaceOnly
		{
			get
			{
				for (var i = 0; i < _text.Length; ++i)
					if (!Char.IsWhiteSpace(_text[i]))
						return false;

				return true;
			}
		}

		/// <summary>
		///   Creates a new text instance.
		/// </summary>
		/// <param name="textString">
		///   The string, possibly containing color specifiers or text-representations of emoticons,
		///   that is the source for the text.
		/// </param>
		public static Text Create(string textString)
		{
			Assert.ArgumentNotNull(textString);

			var text = GetInstance();
			text.SourceString = textString;
			text._text.Clear();
			text._colorRanges.Clear();
			text.ProcessSourceText();
			return text;
		}

		/// <summary>
		///   Processes the source text: Removes all color specifiers, using them to build up the color range list and
		///   replaces all text-representations of emoticons by their single-character encoding.
		/// </summary>
		private void ProcessSourceText()
		{
			var colorRange = new ColorRange();
			for (var i = 0; i < SourceString.Length; ++i)
			{
				ColorSpecifier color;
				Emoticon emoticon;

				if (TryMatch(SourceString, i, out color))
				{
					colorRange.End = _text.Length;
					_colorRanges.Add(colorRange);

					colorRange = new ColorRange(color.Color, _text.Length);
					i += color.Specifier.Length - 1;
				}
				else if (TryMatch(SourceString, i, out emoticon))
				{
					_text.Append(emoticon.CharacterEncoding);
					i += emoticon.TextRepresentation.Length - 1;
				}
				else
					_text.Append(SourceString[i]);
			}

			colorRange.End = _text.Length;
			_colorRanges.Add(colorRange);
		}

		/// <summary>
		///   Tries to match all color specifiers at the current input position and returns the first match. Returns false to
		///   indicate that no match has been found.
		/// </summary>
		/// <param name="source">The source string on which the matching should be performed.</param>
		/// <param name="index">The index of the first character that should be used for the match.</param>
		/// <param name="matchedColor">Returns the matched color specifier.</param>
		private static bool TryMatch(string source, int index, out ColorSpecifier matchedColor)
		{
			for (var i = 0; i < Colors.Length; ++i)
			{
				if (Colors[i].Specifier.Length > source.Length - index)
					continue;

				var matches = true;
				for (var j = 0; j < Colors[i].Specifier.Length && j + index < source.Length; ++j)
				{
					if (source[j + index] != Colors[i].Specifier[j])
					{
						matches = false;
						break;
					}
				}

				if (matches)
				{
					matchedColor = Colors[i];
					return true;
				}
			}

			matchedColor = new ColorSpecifier();
			return false;
		}

		/// <summary>
		///   Tries to match all emoticon text representations at the current input position and returns the first match. Returns
		///   false to indicate that no match has been found.
		/// </summary>
		/// <param name="source">The source string on which the matching should be performed.</param>
		/// <param name="index">The index of the first character that should be used for the match.</param>
		/// <param name="matchedEmoticon">Returns the matched emoticon single-character encoding.</param>
		private static bool TryMatch(string source, int index, out Emoticon matchedEmoticon)
		{
			for (var i = 0; i < Emoticons.Length; ++i)
			{
				if (Emoticons[i].TextRepresentation.Length > source.Length - index)
					continue;

				var matches = true;
				for (var j = 0; j < Emoticons[i].TextRepresentation.Length && j + index < source.Length; ++j)
				{
					if (source[j + index] != Emoticons[i].TextRepresentation[j])
					{
						matches = false;
						break;
					}
				}

				if (matches)
				{
					matchedEmoticon = Emoticons[i];
					return true;
				}
			}

			matchedEmoticon = new Emoticon();
			return false;
		}

		/// <summary>
		///   Maps the logical text index to the corresponding source index.
		/// </summary>
		/// <param name="index">The index that should be mapped.</param>
		private int MapToSource(int index)
		{
			return MapToSource(SourceString, index);
		}

		/// <summary>
		///   Maps the logical text index to the corresponding source index.
		/// </summary>
		/// <param name="source">The source string for which the index should be mapped.</param>
		/// <param name="index">The index that should be mapped.</param>
		private static int MapToSource(string source, int index)
		{
			var sourceIndex = 0;
			var logicalIndex = 0;

			for (; sourceIndex < source.Length && logicalIndex < index; ++sourceIndex)
			{
				ColorSpecifier color;
				Emoticon emoticon;

				if (TryMatch(source, sourceIndex, out color))
				{
					sourceIndex += color.Specifier.Length - 1;
				}
				else if (TryMatch(source, sourceIndex, out emoticon))
				{
					++logicalIndex;
					sourceIndex += emoticon.TextRepresentation.Length - 1;
				}
				else
					++logicalIndex;
			}

			return sourceIndex;
		}

		/// <summary>
		///   Gets the text color at the given index.
		/// </summary>
		/// <param name="index">The index for which the color should be returned.</param>
		/// <param name="color">The returned color.</param>
		public void GetColor(int index, out Color? color)
		{
			foreach (var range in _colorRanges)
			{
				if (range.Begin <= index && range.End > index)
				{
					color = range.Color;
					return;
				}
			}

			color = null;
		}

		/// <summary>
		///   Retrieves a substring from the text's source string and returns the result.
		/// </summary>
		/// <param name="startIndex">The zero-based index position of the substring.</param>
		public string Substring(int startIndex)
		{
			Assert.ArgumentInRange(startIndex, 0, _text.Length);
			return SourceString.Substring(MapToSource(startIndex));
		}

		/// <summary>
		///   Retrieves a substring from the text's source string and returns the result.
		/// </summary>
		/// <param name="startIndex">The zero-based index position of the substring.</param>
		/// <param name="length">The number of characters in the substring.</param>
		public string Substring(int startIndex, int length)
		{
			Assert.ArgumentInRange(startIndex, 0, _text.Length);
			Assert.ArgumentInRange(length, 0, Int32.MaxValue);
			Assert.ArgumentInRange(startIndex + length, 0, _text.Length);

			var endIndex = MapToSource(startIndex + length);
			startIndex = MapToSource(startIndex);

			return SourceString.Substring(startIndex, endIndex - startIndex);
		}

		/// <summary>
		///   Inserts the given value at the given start index into the text's source string and returns the result. Returns the
		///   effect of the insert operation on the given start index. This number is 0 if a printable character is inserted or a
		///   negative number if the new character becomes part of a new color specifier or emoticon.
		/// </summary>
		/// <param name="startIndex">The zero-based index position of the insertion.</param>
		/// <param name="character">The character that should be inserted.</param>
		/// <param name="offset">Returns the effect the insert operation had on the given start index.</param>
		[Pure]
		public string Insert(int startIndex, char character, out int offset)
		{
			Assert.ArgumentInRange(startIndex, 0, _text.Length);

			var index = MapToSource(startIndex);
			var sourceString = SourceString.Insert(index, character.ToString(CultureInfo.InvariantCulture));

			if (startIndex == 0)
				offset = 0;
			else
			{
				var delta = index - startIndex;
				var now = MapToSource(sourceString, startIndex) - startIndex - delta;

				offset = -now;
			}

			return sourceString;
		}

		/// <summary>
		///   Removes the given number of characters at the specified index from the text's source string and returns the result.
		/// </summary>
		/// <param name="startIndex">The zero-based index position to begin deletion.</param>
		/// <param name="count">The number of characters to delete.</param>
		[Pure]
		public string Remove(int startIndex, int count)
		{
			Assert.ArgumentInRange(startIndex, 0, _text.Length);
			Assert.ArgumentInRange(count, 0, Int32.MaxValue);
			Assert.ArgumentInRange(startIndex + count, 0, _text.Length);

			var endIndex = MapToSource(startIndex + count);
			startIndex = MapToSource(startIndex);

			return SourceString.Remove(startIndex, endIndex - startIndex);
		}

		/// <summary>
		///   Concatenates the source string of the given text with the given character.
		/// </summary>
		/// <param name="text">The text that should be concatenated.</param>
		/// <param name="character">The character that should be concatenated.</param>
		public static string operator +(Text text, char character)
		{
			return text.SourceString + character;
		}

		/// <summary>
		///   Concatenates the source string of the given text with the given string.
		/// </summary>
		/// <param name="text">The text that should be concatenated.</param>
		/// <param name="str">The string that should be concatenated.</param>
		public static string operator +(Text text, string str)
		{
			return text.SourceString + str;
		}

		/// <summary>
		///   Converts the text into a regular .NET string with text-representations of emoticons and without any color specifiers.
		/// </summary>
		public override string ToString()
		{
			// TODO: EMOTICONS!
			return _text.ToString();
		}

		/// <summary>
		///   Provides color information for a range of characters.
		/// </summary>
		private struct ColorRange
		{
			/// <summary>
			///   The index of the first character that belongs to the range.
			/// </summary>
			public readonly int Begin;

			/// <summary>
			///   The color of the range, if any.
			/// </summary>
			public readonly Color? Color;

			/// <summary>
			///   The index of the first character that does not belong to the range anymore.
			/// </summary>
			public int End;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="color">The color of the range, if any.</param>
			/// <param name="begin">The index of the first character that belongs to the range.</param>
			public ColorRange(Color? color, int begin)
				: this()
			{
				Color = color;
				Begin = begin;
			}
		}

		/// <summary>
		///   Represents a color specifier, mapping a character to a color.
		/// </summary>
		private struct ColorSpecifier
		{
			/// <summary>
			///   The color that the specifier represents.
			/// </summary>
			public readonly Color Color;

			/// <summary>
			///   The specifier that indicates which color should be used.
			/// </summary>
			public readonly string Specifier;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="specifier">The specifier that indicates which color should be used.</param>
			/// <param name="color">The color that the specifier represents.</param>
			public ColorSpecifier(string specifier, Color color)
			{
				Specifier = specifier;
				Color = color;
			}
		}

		/// <summary>
		///   Represents an emoticon, defining its text representation and single-character encoding.
		/// </summary>
		private struct Emoticon
		{
			/// <summary>
			///   The single-character encoding of the emoticon.
			/// </summary>
			public readonly char CharacterEncoding;

			/// <summary>
			///   The text representation of the emoticon.
			/// </summary>
			public readonly string TextRepresentation;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="textRepresentation">The text representation of the emoticon.</param>
			/// <param name="characterEncoding">The single-character encoding of the emoticon.</param>
			public Emoticon(string textRepresentation, char characterEncoding)
			{
				Assert.ArgumentNotNullOrWhitespace(textRepresentation);

				TextRepresentation = textRepresentation;
				CharacterEncoding = characterEncoding;
			}
		}
	}
}