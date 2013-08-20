using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///   Represents a text that may optionally contain color specifiers.
	/// </summary>
	public class Text : PooledObject<Text>
	{
		/// <summary>
		///   The marker that introduces a color specifier.
		/// </summary>
		private const char ColorMarker = '\\';

		/// <summary>
		///   Maps characters to colors. The character plus the color marker comprise a color specifier. For instance, 'w'
		///   is mapped to the color white, so a text containing "\wA" prints a white 'A'.
		/// </summary>
		private static readonly ColorSpecifier[] Colors =
		{
			// A special color specifier that restores the original color; can only be used by the application
			new ColorSpecifier(ColorMarker + "\0", null),

			// Predefined colors
			new ColorSpecifier(ColorMarker + "white", new Color(255, 255, 255, 255)),
			new ColorSpecifier(ColorMarker + "black", new Color(0, 0, 0, 255)),
			new ColorSpecifier(ColorMarker + "red", new Color(255, 0, 0, 255)),
			new ColorSpecifier(ColorMarker + "green", new Color(0, 255, 0, 255)),
			new ColorSpecifier(ColorMarker + "blue", new Color(0, 0, 255, 255)),
			new ColorSpecifier(ColorMarker + "yellow", new Color(255, 255, 0, 255)),
			new ColorSpecifier(ColorMarker + "magenta", new Color(255, 0, 255, 255)),
			new ColorSpecifier(ColorMarker + "grey", new Color(128, 128, 128, 255)),
			new ColorSpecifier(ColorMarker + "cyan", new Color(0, 255, 255, 255))
		};

		/// <summary>
		///   The color ranges defined by the text.
		/// </summary>
		private readonly List<ColorRange> _colorRanges = new List<ColorRange>(2);

		/// <summary>
		///   The text with the color specifiers removed.
		/// </summary>
		private readonly StringBuilder _text = new StringBuilder();

		/// <summary>
		///   Gets the source string that might contain color specifiers.
		/// </summary>
		public string SourceString { get; private set; }

		/// <summary>
		///   Gets the length of the text, excluding all color specifiers.
		/// </summary>
		public int Length
		{
			get
			{
				Assert.NotPooled(this);
				return _text.Length;
			}
		}

		/// <summary>
		///   Gets the length of the text's source string.
		/// </summary>
		public int SourceLength
		{
			get
			{
				Assert.NotPooled(this);
				return SourceString.Length;
			}
		}

		/// <summary>
		///   Gets the character at the specified index. Color specifier are not returned and do not increase the index count.
		/// </summary>
		/// <param name="index">The index of the character that should be returned.</param>
		public char this[int index]
		{
			get
			{
				Assert.NotPooled(this);
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
				Assert.NotPooled(this);

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
		///   The string, possibly containing color specifiers, that is the source for the text.
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
		///   Processes the source text: Removes all color specifiers, using them to build up the color range list.
		/// </summary>
		private void ProcessSourceText()
		{
			var colorRange = new ColorRange();
			for (var i = 0; i < SourceString.Length; ++i)
			{
				ColorSpecifier color;

				if (TryMatch(SourceString, i, out color))
				{
					colorRange.End = _text.Length;
					_colorRanges.Add(colorRange);

					colorRange = new ColorRange(color.Color, _text.Length);
					i += color.Specifier.Length - 1;
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
			if (source[index] != ColorMarker)
			{
				matchedColor = new ColorSpecifier();
				return false;
			}

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
		///   Maps the given source index to the corresponding logical text index.
		/// </summary>
		/// <param name="sourceIndex">The source index that should be mapped.</param>
		public int MapToText(int sourceIndex)
		{
			Assert.NotPooled(this);
			Assert.ArgumentInRange(sourceIndex, 0, SourceString.Length);

			if (sourceIndex == SourceString.Length)
				return Length;

			var logicalIndex = sourceIndex;
			for (var i = 0; i < sourceIndex; ++i)
			{
				ColorSpecifier color;

				if (TryMatch(SourceString, i, out color))
				{
					i += color.Specifier.Length - 1;
					logicalIndex -= color.Specifier.Length;
				}
			}

			return logicalIndex;
		}

		/// <summary>
		///   Maps the given logical text index to the corresponding source index.
		/// </summary>
		/// <param name="logicalIndex">The index that should be mapped.</param>
		public int MapToSource(int logicalIndex)
		{
			Assert.NotPooled(this);
			Assert.ArgumentInRange(logicalIndex, 0, Length);

			if (logicalIndex == Length)
				return SourceString.Length;

			var index = -1;
			for (var i = 0; i < SourceString.Length; ++i)
			{
				ColorSpecifier color;

				if (TryMatch(SourceString, i, out color))
				{
					i += color.Specifier.Length - 1;
				}
				else
					++index;

				if (index == logicalIndex)
					return i;
			}

			Assert.That(false, "Failed to map logical index to source index.");
			return -1;
		}

		/// <summary>
		///   Gets the text color at the given index.
		/// </summary>
		/// <param name="index">The index for which the color should be returned.</param>
		/// <param name="color">The returned color.</param>
		public void GetColor(int index, out Color? color)
		{
			Assert.NotPooled(this);

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
		///   Writes the given string into the given text writer. Color specifiers are not written.
		/// </summary>
		/// <param name="writer">The text writer that the text should be written to.</param>
		/// <param name="text">The text that should be written.</param>
		public static void Write(TextWriter writer, string text)
		{
			Assert.ArgumentNotNull(writer);
			Assert.ArgumentNotNullOrWhitespace(text);

			for (var i = 0; i < text.Length; ++i)
			{
				ColorSpecifier color;

				if (TryMatch(text, i, out color))
					i += color.Specifier.Length - 1;
				else
					writer.Write(text[i]);
			}
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
			public readonly Color? Color;

			/// <summary>
			///   The specifier that indicates which color should be used.
			/// </summary>
			public readonly string Specifier;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="specifier">The specifier that indicates which color should be used.</param>
			/// <param name="color">The color that the specifier represents.</param>
			public ColorSpecifier(string specifier, Color? color)
			{
				Specifier = specifier;
				Color = color;
			}
		}
	}
}